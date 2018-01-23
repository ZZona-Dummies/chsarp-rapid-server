using System.Collections;
using System.IO;
using System.Text;
using static RapidServer.Globals;

namespace RapidServer.Http.Type2
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
    public class Request
    {
        private Server _server;

        public string Method;

        //  the request method (GET or POST)
        public string Uri;

        //  the Uri requested by the client
        public Hashtable Headers = new Hashtable();

        //  the request Headers (key:value pairs)
        public string FileType;

        //  the requested Uri's file type
        public string QueryString;

        //  the Query String which sometimes appears as a subcomponent of the Uri (e.g. "?user=Perry")
        public string AbsoluteUrl;

        public MimeType MimeType;

        private Request()
        { }

        public Request(string requestString, Server server)
        {
            _server = server;
            ParseRequestString(requestString);
        }

        public Request(byte[] buffer, Server server)
        {
            _server = server;
            object requestString = Encoding.ASCII.GetString(buffer);
            ParseRequestString((string)requestString);
        }

        // '' <summary>
        // '' Parses the raw request string received from the client socket
        // '' </summary>
        // '' <param name="requestString"></param>
        // '' <remarks></remarks>
        private void ParseRequestString(string requestString)
        {
            string[] requestStringParts = requestString.Split('\n');
            //  parse the request-line which is the first line in the request string (e.g. "GET /file.html HTTP/1.1")
            object httpRequestLine = requestStringParts[0];
            // requestString.Split(vbNewLine)(0)
            //  parse the uri (including query string) from the request-line
            //  TODO: during a few refreshes, an exception is thrown here because the requestString is fragmented - "36 Accept(-Encoding) : gzip, deflate, sdch Accept-Language: en-US,en;q=0.8
            Uri = ((string)httpRequestLine).Substring(4, (((string)httpRequestLine).Length - 13)).Replace("/", "\\");
            //  split the uri and query string into their separate components
            int qsIndex = Uri.IndexOf("?");
            if ((qsIndex != -1))
            {
                QueryString = Uri.Substring(qsIndex);
                Uri = Uri.Substring(0, qsIndex);
            }

            //  determine the absolute path for the requested resource
            AbsoluteUrl = (_server.WebRoot + Uri);
            //  if the client requested a directory, provide a directory listing or prepare to serve up the default document
            if ((Directory.Exists(AbsoluteUrl) == true))
            {
                foreach (string d in _server.DefaultDocuments)
                {
                    if (File.Exists((_server.WebRoot + ("/" + d))))
                    {
                        Uri += d;
                        AbsoluteUrl = (_server.WebRoot + Uri);
                        break;
                    }
                }
            }

            //  parse the requested resource's file type (extension) and path
            FileType = Path.GetExtension(Uri).TrimStart('.');
            //  parse the requested resource's mime type
            MimeType m = (MimeType)_server.MimeTypes[FileType];
            if ((m == null))
                m = (MimeType)_server.MimeTypes[""];

            MimeType = m;
            //  parse the remaining request headers
            for (int i = 1; i <= requestStringParts.Length - 2; i++)
            {
                int delimIndex = requestStringParts[i].IndexOf(": ");
                if ((delimIndex > 0))
                {
                    string key = requestStringParts[i].Substring(1, (delimIndex - 1));
                    string value = requestStringParts[i].Substring((delimIndex + 2), (requestStringParts[i].Length
                                    - (delimIndex - 2)));
                    Headers.Add(key, value);
                }
            }
        }
    }
}