using RapidSever.Enums;
using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Text;
using static RapidServer.Globals;

namespace RapidServer.Http.Type2
{
    // '' <summary>
    // '' An http response, normally sent from the server back to the client who made the initial request.
    // '' </summary>
    // '' The server will respond to clients using and http response with the following signature:
    // '' ---------------------------
    // '' HTTP/1.1 200 OK\r\n
    // '' Content-Type: text/plain\r\n
    // '' Content-Length: 13\r\n
    // '' Connection: close\r\n
    // '' \r\n
    // '' hello world
    // '' ---------------------------
    // '' The first line in the response code which is mandatory. Subsequent lines are header-lines. The second and third lines (header-lines) describe the content, which are mandatory. Every line in the response must be terminated with \r\n. Two line terminators delimit the content from the headers, which is mandatory.
    // '' <remarks></remarks>
    public class Response
    {
        private Server _server;

        //  a reference to the server instance
        private byte[] _content;

        //  contains the content bytes for the response
        private Hashtable _headers = new Hashtable();

        //  contains the header strings for the response
        public MimeType MimeType;

        //  the requested uri's mimetype
        public TransferMethod TransferMethod;

        //  the transfer method to be used (store and forward, chunked encoding)
        public int StatusCode;

        //  the http status code (e.g. 200 = OK, 404 = Page not found)
        public string ContentType;

        //  the requested uri's content type, which is pulled from the mimetype
        public string ContentLength;

        private Response()
        { }

        //  the number of bytes representing the content
        public Response(Server server, Request req, Net.Sockets.Socket client)
        {
            if ((req.Headers.ContainsKey("Connection") == true))
            {
                if ((req.Headers["Connection"].ToString().ToLower() == "keep-alive"))
                {
                    SetHeader("Connection", "keep-alive");
                }
            }

            //  set any headers required by the requested resource's mimetype
            MimeType = req.MimeType;
            if ((MimeType.Compress != CompressionMethod.None))
            {
                SetHeader("Content-Encoding", Enum.GetName(typeof(CompressionMethod), MimeType.Compress).ToLower());
            }

            //  set any custom request headers defined in the config file
            foreach (string s in server.ResponseHeaders)
            {
                // Dim spl() As String = s.Split(": ")
                // SetHeader(spl(0), spl(1))
                int delimIndex = s.IndexOf(": ");
                if ((delimIndex > 0))
                {
                    string key = s.Substring(0, delimIndex);
                    string value = s.Substring((delimIndex + 2), (s.Length
                                    - (delimIndex - 2)));
                    SetHeader(key, value);
                }
            }
        }

        public object Content
        {
            get
            {
                return _content;
            }
        }

        public object Headers
        {
            get
            {
                return _headers;
            }
        }

        public void SetContent(byte[] contentBytes)
        {
            byte[] cbuf = null;
            //  create a buffer exactly the size of the memorystream length (not its buffer length)
            byte[] mbuf = null;

            //  TODO: conditionally set Content-Length if needed - the header is not always necessary (e.g. when TransferMethod = ChunkedEncoding)
            using (MemoryStream ms = new MemoryStream())
            {
                if (contentBytes != null)
                {
                    if ((MimeType.Compress == CompressionMethod.Gzip))
                    {
                        using (GZipStream gZip = new GZipStream(ms, CompressionMode.Compress, true))
                        {
                            gZip.Write(contentBytes, 0, contentBytes.Length);
                            //  make sure we close the compression stream or else it won't flush the full buffer! see: http://stackoverflow.com/questions/6334463/gzipstream-compression-problem-lost-byte
                            gZip.Close();
                        }
                    }
                    else if ((MimeType.Compress == CompressionMethod.Deflate))
                    {
                        using (DeflateStream deflate = new DeflateStream(ms, CompressionMode.Compress, true))
                        {
                            deflate.Write(contentBytes, 0, contentBytes.Length);
                            //  make sure we close the compression stream or else it won't flush the full buffer! see: http://stackoverflow.com/questions/6334463/gzipstream-compression-problem-lost-byte
                            deflate.Close();
                        }
                    }
                    else
                    {
                        //  no compression should be used on this resource, write the data as-is (uncompressed or already-compressed)
                        ms.Write(contentBytes, 0, contentBytes.Length);
                    }
                }

                mbuf = ms.GetBuffer();
                ms.Close();
            }
            Buffer.BlockCopy(mbuf, 0, cbuf, 0, cbuf.Length);
            ContentLength = cbuf.GetLength(0).ToString(); //???
            _content = cbuf;
        }

        private void SetContent(string contentString)
        {
            //  just pass the string as bytes to the primary SetContent method
            SetContent(Encoding.ASCII.GetBytes(contentString));
        }

        private void SetHeader(string key, string value)
        {
            //  TODO: this seems to be trying to set the Reponse Headers several times per page load... e.g. _headers.Add(key, key & value) says item already exists
            // _Headers[key) = key & ": " & value
            _headers[key] = value;
        }

        private string GetHeaderString()
        {
            string s = "";
            s += ("HTTP/1.1 "
                        + (StatusCode + (" "
                        + (StatusCodeMessage() + '\n')))) +
            ("Content-Length: "
                        + (ContentLength + '\n')) +
            ("Content-Type: "
                        + (ContentType + '\n')) +
            ("Date: "
                        + (DateTime.Now.ToString("r") + '\n'));
            //  append the headers that have been dynamically or conditionally set (request headers, compression, etc)
            foreach (string h in _headers.Keys)
                s += h + ": " + _headers[h] + '\n';

            //  one extra cr/lf is required for delimiting the header from the content, per http specs
            s += '\n';
            return s;
        }

        // '' <summary>
        // '' Gets the bytes that represent the final response including the headers and content.
        // '' </summary>
        // '' <returns></returns>
        // '' <remarks></remarks>
        public byte[] GetResponseBytes()
        {
            MemoryStream ms = new MemoryStream();
            //  get the header bytes and add it to the response
            byte[] headerBytes = Encoding.ASCII.GetBytes(GetHeaderString());
            ms.Write(headerBytes, 0, headerBytes.Length);
            //  if there is content, add it to the response
            if (_content != null)
            {
                ms.Write(_content, 0, _content.Length);
            }

            byte[] rbuf = null;
            byte[] mbuf = ms.GetBuffer();
            Buffer.BlockCopy(mbuf, 0, rbuf, 0, rbuf.Length);
            return rbuf;
        }

        // '' <summary>
        // '' Gets a standard message for an http status code.
        // '' </summary>
        // '' <returns></returns>
        // '' <remarks></remarks>
        private string StatusCodeMessage()
        {
            string msg = "";
            switch (StatusCode)
            {
                case 200:
                    msg = "OK";
                    break;

                case 404:
                    msg = "Page not found.";
                    break;
            }
            return msg;
        }

        private void WriteHeader(object statusCode)
        {
            //  TODO: write the full header to the client stream
        }

        private void Send(int statusCode, string message)
        {
        }

        private void SendFile(int statusCode, byte[] file)
        {
        }

        private void Finish()
        {
            //  TODO: finish the response cycle by flushing buffered data in the client stream
        }
    }
}