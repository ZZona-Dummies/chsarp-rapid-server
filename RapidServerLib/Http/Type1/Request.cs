using System.IO;
using System.Net.Sockets;
using System.Text;
using static RapidServer.Globals;

namespace RapidServer.Http.Type1
{
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

        public Request(string requestString, Server server, Socket client, Site site)
        {
            _server = server;
            RequestString = requestString;
            Site = site;
            ClientAddress = client.RemoteEndPoint.ToString();
            //  convert the raw request data into a string and parse it
            // Me.RequestString = System.Encoding.Default.GetString(buffer).Replace(Chr(0), "")
            //  parse raw data into strings
            Parse(RequestString);
            //  parse strings into strongly typed properties
            ParseRequestString(RequestString);
        }

        // Sub New(ByVal buffer() As Byte, ByVal server As Server, ByVal client As Socket, ByVal site As Site)
        //     MyBase.new()
        //     _server = server
        //     Me.Site = site
        //     Me.ClientAddress = client.RemoteEndPoint.ToString
        //     ' convert the raw request data into a string and parse it
        //     'Me.RequestString = System.Encoding.Default.GetString(buffer).Replace(Chr(0), "")
        //     Me.RequestString = System.Encoding.ASCII.GetString(buffer).Trim(Chr(0))
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
            if (headerStringParts[0].StartsWith("HEAD") || headerStringParts[0].StartsWith("GET") || headerStringParts[0].StartsWith("POST"))
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
                if (Directory.Exists(AbsPath))
                {
                    //  if the requested path was a directory, but the Uri was missing a trailing slash, we need to 301 redirect to the correct Uri
                    if (Uri.EndsWith("/"))
                    {
                        FixPath301 = true;
                    }

                    //  build the paths to the requested resource
                    //  TODO: file.exists is one of the slowest operations here, cache it...
                    //    however, we are already caching responses and we should skip this function for cached responses which we are not currently doing since we parse before handling cache...
                    foreach (string doc in _server.DefaultDocuments)
                    {
                        if (File.Exists((Site.RootPath
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
                    //  not a directory, get filename from abspath
                    FileName = Path.GetFileName(AbsPath);

                //  TODO: if the directory was empty, Me.FileName will be empty here and we can't proceed with it; serve directory listing or return a 40X status code...
                if (FileName != null)
                {
                    //  build the scriptname
                    if (Uri.Contains(FileName))
                        ScriptName = Uri;
                    else
                        ScriptName = Uri + FileName + QueryString;

                    //  strip the querystring from the scriptname, causes problems with WP customizer
                    if (QueryString != "")
                        if (ScriptName.Contains(QueryString))
                            ScriptName = ScriptName.Replace(QueryString, "");

                    //  parse the requested resource's file type (extension) for use determining the mime type
                    FileType = Path.GetExtension(AbsPath).TrimStart('.');
                    //  parse the requested resource's mime type
                    MimeType m = ((MimeType)(_server.MimeTypes[FileType]));
                    if (m == null)
                        m = (MimeType)_server.MimeTypes[""];

                    MimeType = m;
                    //  set content length
                    ContentLength = ContentStringLength;
                    //  set the content bytes
                    Content = Encoding.ASCII.GetBytes(ContentString);
                }
            }
        }
    }
}