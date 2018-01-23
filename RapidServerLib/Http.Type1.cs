﻿using System;
using static RapidServer.Globals;
using IO = System.IO;
using Net = System.Net;
using Text = System.Text;

namespace RapidServer.Http.Type1
{
    // '' <summary>
    // '' An object for passing off the request from an IOCP thread to a threadpool (worker) thread for processing.
    // '' </summary>
    // '' <remarks></remarks>
    internal class HandleRequestObject
    {
        public byte[] requestBytes;

        public Server server;

        public Net.Sockets.Socket clientSocket;

        public Site site;
    }

    // '' <summary>
    // '' An http request, usually sent by a client.
    // '' </summary>
    // '' Clients will send requests to the server using an http request with the following signature:
    // '' ---------------------------
    // '' GET /file.html HTTP/1.1\r\n
    // '' User-Agent: Chrome/1.0\r\n
    // '' Host: example.com\r\n
    // '' Accept: */*
    // '' ---------------------------
    // '' The first line in the request is the Request-Line which is mandatory. All subsequent lines are Header-Lines which are optional. Every line must be terminated with \r\n.
    // '' <remarks></remarks>
    public class Request : SimpleRequestResponse
    {
        private Server _server;

        public Site Site;

        public string ClientAddress;

        public string RequestString;

        public string RequestLine;

        //  the first line of the request string which contains the method, uri and protocol
        public string Method;

        //  the request method (GET or POST)
        public string FileName;

        public string ScriptName;

        public string Uri;

        //  the uri of the resource (e.g. /index.php)
        public string RelPath;

        //  the relative path of the resource on disk (e.g. \index.php)
        public string Protocol;

        //  the http protocol (e.g. HTTP/1.1)
        public string FileType;

        //  the file type of the resource (e.g. jpg)
        public string QueryString = "";

        public string ContentLength;

        public string AbsPath;

        //  the absolute path of the resource on disk (e.g. c:\site1\index.php)
        public MimeType MimeType;

        //  the mime type of the resource (e.g. image/jpeg)
        public bool FixPath301;

        private Request()
        { }

        public Request(string requestString, Server server, Net.Sockets.Socket client, Site site)
        {
            _server = server;
            RequestString = requestString;
            Site = site;
            ClientAddress = client.RemoteEndPoint.ToString();
            //  convert the raw request data into a string and parse it
            // Me.RequestString = System.Text.Encoding.Default.GetString(buffer).Replace(Chr(0), "")
            //  parse raw data into strings
            this.Parse(RequestString);
            //  parse strings into strongly typed properties
            ParseRequestString(RequestString);
        }

        // Sub New(ByVal buffer() As Byte, ByVal server As Server, ByVal client As Net.Sockets.Socket, ByVal site As Site)
        //     MyBase.new()
        //     _server = server
        //     Me.Site = site
        //     Me.ClientAddress = client.RemoteEndPoint.ToString
        //     ' convert the raw request data into a string and parse it
        //     'Me.RequestString = System.Text.Encoding.Default.GetString(buffer).Replace(Chr(0), "")
        //     Me.RequestString = System.Text.Encoding.ASCII.GetString(buffer).Trim(Chr(0))
        //     ' parse raw data into strings
        //     Me.Parse(Me.RequestString)
        //     ' parse strings into strongly typed properties
        //     Me.ParseRequestString(Me.RequestString)
        // End Sub
        // '' <summary>
        // '' Parses the raw request string received from the client socket
        // '' </summary>
        // '' <param name="requestString"></param>
        // '' <remarks></remarks>
        private void ParseRequestString(string requestString)
        {
            //  parse the requestString to build up the request object
            string[] headerStringParts = HeaderString.Split('\n');
            if ((headerStringParts[0].StartsWith("HEAD")
                        || (headerStringParts[0].StartsWith("GET") || headerStringParts[0].StartsWith("POST"))))
            {
                //  parse the request line
                RequestLine = headerStringParts[0];
                string[] requestLineParts;
                requestLineParts = RequestLine.Split(' ');
                Method = requestLineParts[0];
                Uri = requestLineParts[1];
                Protocol = requestLineParts[2];
                //  build the relative and absolute path to the file
                RelPath = Uri.Replace("/", "\\");
                if (Uri.Contains("?"))
                {
                    string[] uriParts = Uri.Split('?');
                    RelPath = uriParts[0].Replace("/", "\\");
                    QueryString = ("?" + uriParts[1]);
                }

                AbsPath = (Site.RootPath + RelPath);
                //  if the requested path was a directory, use the default document
                if ((IO.Directory.Exists(AbsPath) == true))
                {
                    //  if the requested path was a directory, but the Uri was missing a trailing slash, we need to 301 redirect to the correct Uri
                    if ((Uri.EndsWith("/") == false))
                    {
                        FixPath301 = true;
                    }

                    //  build the paths to the requested resource
                    //  TODO: file.exists is one of the slowest operations here, cache it...
                    //    however, we are already caching responses and we should skip this function for cached responses which we are not currently doing since we parse before handling cache...
                    foreach (string doc in _server.DefaultDocuments)
                    {
                        if (IO.File.Exists((Site.RootPath
                                        + (RelPath + ("\\" + doc)))))
                        {
                            FileName = doc;
                            RelPath = (RelPath + ("\\" + FileName));
                            AbsPath = (Site.RootPath + RelPath);
                            break;
                        }
                    }

                    // Me.FileName = "index.html"
                    // Me.RelPath = Me.RelPath & "\" & Me.FileName
                    // Me.AbsPath = Me.Site.RootPath & Me.RelPath
                }
                else
                {
                    //  not a directory, get filename from abspath
                    FileName = IO.Path.GetFileName(AbsPath);
                }

                //  TODO: if the directory was empty, Me.FileName will be empty here and we can't proceed with it; serve directory listing or return a 40X status code...
                if ((FileName != null))
                {
                    //  build the scriptname
                    if (Uri.Contains(FileName))
                    {
                        ScriptName = Uri;
                    }
                    else
                    {
                        ScriptName = (Uri
                                    + (FileName + QueryString));
                    }

                    //  strip the querystring from the scriptname, causes problems with WP customizer
                    if ((QueryString != ""))
                    {
                        if (ScriptName.Contains(QueryString))
                        {
                            ScriptName = ScriptName.Replace(QueryString, "");
                        }
                    }

                    //  parse the requested resource's file type (extension) for use determining the mime type
                    FileType = IO.Path.GetExtension(AbsPath).TrimStart('.');
                    //  parse the requested resource's mime type
                    MimeType m = ((MimeType)(_server.MimeTypes[FileType]));
                    if ((m == null))
                    {
                        m = (MimeType)_server.MimeTypes[""];
                    }

                    MimeType = m;
                    //  set content length
                    ContentLength = ContentStringLength;
                    //  set the content bytes
                    Content = Text.Encoding.ASCII.GetBytes(ContentString);
                }
            }
        }
    }

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

        public Response(Server server, Request req, Net.Sockets.Socket client)
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
            //  TODO: conditionally set Content-Length if needed - the header is not always necessary (e.g. when TransferMethod = ChunkedEncoding)
            IO.MemoryStream ms = new IO.MemoryStream();
            if (contentBytes != null)
            {
                if (MimeType != null)
                {
                    if ((MimeType.Compress == CompressionMethod.Gzip))
                    {
                        IO.Compression.GZipStream gZip = new IO.Compression.GZipStream(ms, IO.Compression.CompressionMode.Compress, true);
                        gZip.Write(contentBytes, 0, contentBytes.Length);
                        //  make sure we close the compression stream or else it won't flush the full buffer! see: http://stackoverflow.com/questions/6334463/gzipstream-compression-problem-lost-byte
                        gZip.Close();
                        gZip.Dispose();
                    }
                    else if ((MimeType.Compress == CompressionMethod.Deflate))
                    {
                        IO.Compression.DeflateStream deflate = new IO.Compression.DeflateStream(ms, IO.Compression.CompressionMode.Compress, true);
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

            byte[] cbuf = null;
            //  create a buffer exactly the size of the memorystream length (not its buffer length)
            byte[] mbuf = ms.GetBuffer();
            ms.Close();
            ms.Dispose();
            Buffer.BlockCopy(mbuf, 0, cbuf, 0, cbuf.Length);
            ContentLength = cbuf.Length.ToString();
            _content = cbuf;
        }

        private void SetContent(string contentString)
        {
            //  just pass the string as bytes to the primary SetContent method
            SetContent(Text.Encoding.UTF8.GetBytes(contentString));
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
            IO.MemoryStream ms = new IO.MemoryStream();
            string fullHeaderString = BuildHeaderString() + BuildCookieString() + '\n';
            //  one extra cr/lf separates header from content
            byte[] fullHeaderBytes = Text.Encoding.ASCII.GetBytes(fullHeaderString);
            ms.Write(fullHeaderBytes, 0, fullHeaderBytes.Length);
            // ' get the header bytes and add it to the response
            // Dim headerBytes() As Byte = System.Text.Encoding.ASCII.GetBytes(Me.BuildHeaderString)
            // ms.Write(headerBytes, 0, headerBytes.Length)
            // ' TODO: get the cookies and add it to the response
            // Dim cookieBytes() As Byte = System.Text.Encoding.ASCII.GetBytes(Me.BuildCookieString)
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