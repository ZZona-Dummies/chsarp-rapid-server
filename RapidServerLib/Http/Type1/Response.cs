using RapidServerLib.Enums;
using System;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using System.Text;
using static RapidServer.Globals;

namespace RapidServer.Http.Type1
{
    // '' <summary>
    // '' An http response, normally sent from the server back to the client who made the initial request.
    // '' </summary>
    // '' <remarks></remarks>
    public class Response : SimpleRequestResponse
    {
        private Server _server;

        //  a reference to the server instance
        private byte[] _content;

        //  content payload
        public MimeType MimeType;

        //  requested uri's mimetype
        // Public TransferMethod As TransferMethod    ' TODO: the transfer method to be used (store and forward, chunked encoding)
        public string ContentType;

        //  requested uri's content type, which is pulled from the mimetype
        public string ContentLength;

        //  number of bytes representing the content
        public string StatusCode;

        //  status code of the response (e.g. 200, 302, 404)
        public string ScriptName;

        public Request Request;

        public byte[] ResponseBytes;

        private Response()
        { }

        public Response(Server server, Request req, Socket client)
        {
            ScriptName = req.ScriptName;
            Request = req;
            //  if the request includes a Connection: keep-alive header, we need to add it to the response:
            if ((req.Headers.ContainsKey("Connection") == true))
            {
                if ((req.Headers["Connection"].ToString().ToLower() == "keep-alive"))
                {
                    Headers["Connection"] = "Keep-Alive";
                }
            }

            //  set the Content-Encoding header to properly represent the requested resource's mimetype:
            if (req.MimeType != null)
            {
                MimeType = req.MimeType;
                if ((MimeType.Compress != CompressionMethod.None))
                {
                    Headers["Content-Encoding"] = Enum.GetName(typeof(CompressionMethod), MimeType.Compress).ToLower();
                }
            }

            //  set any custom response headers defined in the config file:
            //  UNDONE: conflicts with load balancer mode...
            // For Each s As String In server.ResponseHeaders
            //     Dim delimIndex As Integer = s.IndexOf(": ")
            //     If delimIndex > 0 Then
            //         Dim key As String = s.Substring(0, delimIndex)
            //         Dim value As String = s.Substring(delimIndex + 2, s.Length - delimIndex - 2)
            //         Me.Headers[key) = value
            //     End If
            // Next
        }

        // '' <summary>
        // '' The primary method for setting the response content. Any other methods which also set the content should ultimately route through this method.
        // '' </summary>
        // '' <param name="contentBytes"></param>
        // '' <remarks></remarks>
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
                    if (MimeType != null)
                    {
                        if ((MimeType.Compress == CompressionMethod.Gzip))
                        {
                            GZipStream gZip = new GZipStream(ms, CompressionMode.Compress, true);
                            gZip.Write(contentBytes, 0, contentBytes.Length);
                            //  make sure we close the compression stream or else it won't flush the full buffer! see: http://stackoverflow.com/questions/6334463/gzipstream-compression-problem-lost-byte
                            gZip.Close();
                            gZip.Dispose();
                        }
                        else if ((MimeType.Compress == CompressionMethod.Deflate))
                        {
                            DeflateStream deflate = new DeflateStream(ms, CompressionMode.Compress, true);
                            deflate.Write(contentBytes, 0, contentBytes.Length);
                            //  make sure we close the compression stream or else it won't flush the full buffer! see: http://stackoverflow.com/questions/6334463/gzipstream-compression-problem-lost-byte
                            deflate.Close();
                            deflate.Dispose();
                        }
                        else
                        {
                            //  no compression should be used on this resource, just write the data as-is:
                            ms.Write(contentBytes, 0, contentBytes.Length);
                        }
                    }
                    else
                    {
                        //  no mimetype, just write the data as is:
                        ms.Write(contentBytes, 0, contentBytes.Length);
                    }
                }

                mbuf = ms.GetBuffer();
                ms.Close();
            }
            Buffer.BlockCopy(mbuf, 0, cbuf, 0, cbuf.Length);
            ContentLength = cbuf.Length.ToString();
            _content = cbuf;
        }

        private void SetContent(string contentString)
        {
            //  just pass the string as bytes to the primary SetContent method
            SetContent(Encoding.UTF8.GetBytes(contentString));
        }

        public string BuildHeaderString()
        {
            string s =
            ("HTTP/1.1 "
                        + (StatusCode + (" "
                        + (StatusCodeMessage() + '\n')))) +
            ("Content-Length: "
                        + (ContentLength + '\n')) +
            ("Content-Type: "
                        + (ContentType + '\n'));
            // s &= "Date: " & DateTime.Now.ToString("r") & vbCrLf ' TODO: high cost detected in profiler...reimplement using a faster date method
            //  append the headers that have been dynamically or conditionally set (request headers, compression, etc)
            foreach (string h in Headers.Keys)
            {
                s += h + ": " + Headers[h].ToString() + '\n';
            }

            return s;
        }

        //  TODO: merge this into BuildHeaderString(), doesn't need it's own func...
        private string BuildCookieString()
        {
            string s = "";
            foreach (SimpleHttpHeader h in Cookies)
            {
                s += (h.Key + (": "
                            + (h.Value + '\n')));
            }

            return s;
        }

        // '' <summary>
        // '' Gets the bytes that represent the final response including the headers and content.
        // '' </summary>
        // '' <returns></returns>
        // '' <remarks></remarks>
        public byte[] BuildResponseBytes()
        {
            MemoryStream ms = new MemoryStream();
            string fullHeaderString = BuildHeaderString() + BuildCookieString() + '\n';
            //  one extra cr/lf separates header from content
            byte[] fullHeaderBytes = Encoding.ASCII.GetBytes(fullHeaderString);
            ms.Write(fullHeaderBytes, 0, fullHeaderBytes.Length);
            // ' get the header bytes and add it to the response
            // Dim headerBytes() As Byte = System.Encoding.ASCII.GetBytes(Me.BuildHeaderString)
            // ms.Write(headerBytes, 0, headerBytes.Length)
            // ' TODO: get the cookies and add it to the response
            // Dim cookieBytes() As Byte = System.Encoding.ASCII.GetBytes(Me.BuildCookieString)
            // ms.Write(cookieBytes, 0, cookieBytes.Length)
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
            switch (int.Parse(StatusCode))
            {
                case 100:
                    msg = "Continue";
                    break;

                case 101:
                    msg = "Switching Protocols";
                    break;

                case 200:
                    msg = "OK";
                    break;

                case 301:
                    msg = "Moved Permanently";
                    break;

                case 302:
                    msg = "Found";
                    break;

                case 303:
                    msg = "See Other";
                    break;

                case 304:
                    msg = "Not Modified";
                    break;

                case 305:
                    msg = "Use Proxy";
                    break;

                case 306:
                    msg = "Unused StatusCode (Deprecated)";
                    break;

                case 307:
                    msg = "Temporary Redirect";
                    break;

                case 400:
                    msg = "Bad Request";
                    break;

                case 401:
                    msg = "Unauthorized";
                    break;

                case 402:
                    msg = "Payment Required";
                    break;

                case 403:
                    msg = "Forbidden";
                    break;

                case 404:
                    msg = "Page Not Found";
                    break;

                case 405:
                    msg = "Method Not Allowed";
                    break;

                case 406:
                    msg = "Not Acceptable";
                    break;

                case 407:
                    msg = "Proxy Authentication Required";
                    break;

                case 408:
                    msg = "Request Timeout";
                    break;

                case 409:
                    msg = "Conflict";
                    break;

                case 410:
                    msg = "Gone";
                    break;

                case 411:
                    msg = "Length Required";
                    break;

                case 412:
                    msg = "Precondition Failed";
                    break;

                case 413:
                    msg = "Request Entity Too Large";
                    break;

                case 414:
                    msg = "Request-URI Too Long";
                    break;

                case 415:
                    msg = "Unsupported Media Type";
                    break;

                case 416:
                    msg = "Requested Range Not Satisfiable";
                    break;

                case 417:
                    msg = "Expectation Failed";
                    break;

                case 500:
                    msg = "Internal Server Error";
                    break;

                case 501:
                    msg = "Not Implemented";
                    break;

                case 502:
                    msg = "Bad Gateway";
                    break;

                case 503:
                    msg = "Service Unavailable";
                    break;

                case 504:
                    msg = "Gateway Timeout";
                    break;

                case 505:
                    msg = "HTTP Version Not Supported";
                    break;
            }
            return msg;
        }
    }
}