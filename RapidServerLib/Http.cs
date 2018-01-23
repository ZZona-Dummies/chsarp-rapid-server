using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using Net = System.Net;
using Text = System.Text;

namespace RapidServer.Http
{

    // '' <summary>
    // '' Global variables used throughout the library.
    // '' TODO: since the server and client apps use their own instance of the library, when the server sets these values,
    // '' the client does not see them but tries to use them anyway, which breaks the client functionality, move this into
    // '' the server and create another for the client
    // '' </summary>
    // '' <remarks></remarks>
    public class Module1
    {
    }

    // '' <summary>
    // '' A site is simply a website defined in the config file.
    // '' </summary>
    // '' <remarks></remarks>
    public class Site
    {

        public string Title;

        public string Path;

        public string Host;

        public int Port;

        public string RootPath;

        public string RootUrl;

        public Net.Sockets.Socket Socket;

        public string Role;

        public string Upstream;
    }

    // '' <summary>
    // '' A simple http header is just an object which contains the key and value of the header. Used for cookies, since there might be multiple cookies and we can't add multiple items with the same key to a hashtable.
    // '' </summary>
    // '' <remarks></remarks>
    public class SimpleHttpHeader
    {

        public string Key;

        public string Value;
    }

    // '' <summary>
    // '' A simple object for parsing raw http data into strings, inherited by Request and Response classes
    // '' </summary>
    // '' <remarks></remarks>
    public class SimpleRequestResponse
    {

        public Hashtable Headers = new Hashtable();

        public ArrayList Cookies = new ArrayList();

        public byte[] Content;

        public string ErrorMessage = "";

        public string HeaderString = "";

        public string ContentString = "";

        public string ContentStringLength = "";

        public bool CacheAllowed = true;

        public void Parse(string payload)
        {
            //  parse the responseString into header/content parts
            // Dim headerPart As String = ""
            Text.StringBuilder headerPart = new Text.StringBuilder();
            string contentPart = "";
            try
            {
                int i = payload.IndexOf(Environment.NewLine + Environment.NewLine, StringComparison.Ordinal);
                //  TODO: this is very slow, use a string builder's substring instead
                // headerPart = payload.Substring(0, i).Trim
                headerPart.Append(payload, 0, i);
                contentPart = payload.Substring((i + 4), (payload.Length
                                - (i - 4)));
                HeaderString = headerPart.ToString();
                ContentString = contentPart;
                //  TODO: also parse headers from Request Body (for certain POST requests)
                ContentStringLength = contentPart.Length.ToString();
                Content = Text.Encoding.ASCII.GetBytes(ContentString);
            }
            catch (Exception ex)
            {
                //  couldn't parse the responseString as expected, probably an error
                // ErrorMessage = "Could not parse responseString for new SimpleResponse. responseString: " & responseString
                ErrorMessage = payload;
                Content = Text.Encoding.ASCII.GetBytes(ErrorMessage);
            }

            //  parse the header string
            string[] headerStringParts = HeaderString.Split('\n');
            foreach (string header in headerStringParts)
            {
                string headerClean = header.Trim();
                if ((headerClean != ""))
                {
                    if (headerClean.Contains(":"))
                    {
                        int pos = headerClean.IndexOf(":");
                        string[] headerParts = SplitFirst(headerClean, ":", true);
                        //  there can be multiple cookies, can't store them raw in our hashtable so use an arraylist and merge both before sending the response
                        if ((headerParts[0].ToLower() == "set-cookie"))
                        {
                            SimpleHttpHeader h = new SimpleHttpHeader();
                            h.Key = "Set-Cookie";
                            h.Value = headerParts[1];
                            Cookies.Add(h);
                        }

                        if ((headerParts[0].ToLower() == "cache-control"))
                        {
                            if ((headerParts[1].ToLower() == "no-cache"))
                            {
                                CacheAllowed = false;
                            }

                        }

                        //  only set the same header once
                        if ((Headers.ContainsKey(headerParts[0]) == false))
                        {
                            Headers.Add(headerParts[0], headerParts[1]);
                        }

                    }

                }

            }

        }
    }

    // '' <summary>
    // '' Handlers are external processes or API calls that can be automated to do work or return results. Such handlers include PHP, ASP.NET, Windows Shell, etc. Interpreters are made available through custom definitions specified in the interpreters.xml file.
    // '' </summary>
    // '' <remarks></remarks>
    public class Handlers
    {

        private Hashtable _handlers = new Hashtable();

        private string _webRoot;

        void Add(Handler h)
        {
            _handlers.Add(h.Name, h);
        }

        Handler this[int index]
        {
            get => ((Handler)(from DictionaryEntry entry in _handlers.Values select entry.Key).Skip(index).FirstOrDefault());//(_handlers.Values.ElementAt(index)));
            set
            {

            }
        }

        Handler this[string name]
        {
            get
            {
                return ((Handler)(_handlers[name]));
            }
            set
            {

            }
        }
    }

    public class Handler
    {
        public string Name;

        public string ExecutablePath;

        public virtual string HandleRequest(Type1.Request req)
        {
            return null;
        }
    }

    // '' <summary>
    // '' A CGI handler for .php scripts.
    // '' </summary>
    // '' <remarks></remarks>
    public class PhpCgiHandler : Handler
    {

        public PhpCgiHandler()
        {
            Name = "PhpCgi";
        }

        public override string HandleRequest(Type1.Request req)
        {
            string results;
            //  create the php process:
            Process p = new Process();
            p.StartInfo.FileName = ExecutablePath;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            string host = (string)req.Headers["Host"];
            string[] hostParts = host.Split(':');
            p.StartInfo.EnvironmentVariables.Add("SCRIPT_FILENAME", req.AbsPath);
            p.StartInfo.EnvironmentVariables.Add("QUERY_STRING", req.QueryString.TrimStart('?'));
            p.StartInfo.EnvironmentVariables.Add("REQUEST_METHOD", req.Method);
            p.StartInfo.EnvironmentVariables.Add("REQUEST_URI", req.Uri);
            p.StartInfo.EnvironmentVariables.Add("HTTP_HOST", host);
            p.StartInfo.EnvironmentVariables.Add("SERVER_SOFTWARE", "Rapid Server");
            p.StartInfo.EnvironmentVariables.Add("SERVER_ADDR", hostParts[0]);
            //  set the SERVER_PORT if it's available:
            if ((hostParts.Length > 1))
            {
                p.StartInfo.EnvironmentVariables.Add("SERVER_PORT", hostParts[1]);
            }

            //  make sure we pass cookies:
            if (req.Headers.ContainsKey("Cookie"))
            {
                p.StartInfo.EnvironmentVariables.Add("HTTP_COOKIE", (string)req.Headers["Cookie"]);
            }

            //  if its an http post, set the content_length and content_type headers:
            if ((req.Method == "POST"))
            {
                p.StartInfo.EnvironmentVariables.Add("CONTENT_LENGTH", req.ContentLength);
                p.StartInfo.EnvironmentVariables.Add("CONTENT_TYPE", "application/x-www-form-urlencoded");
            }

            //  these are a "must" according to CGI spec:
            p.StartInfo.EnvironmentVariables.Add("SCRIPT_NAME", req.ScriptName);
            // req.Uri & req.ScriptName)
            p.StartInfo.EnvironmentVariables.Add("GATEWAY_INTERFACE", "CGI/1.1");
            p.StartInfo.EnvironmentVariables.Add("SERVER_NAME", hostParts[0]);
            p.StartInfo.EnvironmentVariables.Add("SERVER_PROTOCOL", "HTTP/1.1");
            p.StartInfo.EnvironmentVariables.Add("REMOTE_ADDR", hostParts[0]);
            //  start the php process:
            p.Start();
            //  if its an http post, write the post data (aka querystring) to the input stream (aka stdin):
            if ((req.Method == "POST"))
            {
                p.StandardInput.Write(req.ContentString);
                p.StandardInput.Flush();
                p.StandardInput.Close();
            }

            //  read the output stream (aka stdout) to get the processed request from php:
            //  TODO: ReadToEnd is thread blocking, we can use the async BeginOutputReadLine() function, but we must remember that we're 
            //    spawning a separate php-cgi.exe process (thus new thread) for every php request; I benchmarked them and found no 
            //    performance difference, no need to over-optimize this yet when our real goal is implementing a FastCGI handler...
            results = p.StandardOutput.ReadToEnd();
            //  close the process (not really needed - garbage collector takes care of it)
            p.Close();
            p.Dispose();
            //  return the processed request to the calling function where we'll continue to handle it:
            return results;
        }
    }

    // '' <summary>
    // '' A FastCGI handler for .php scripts.
    // '' </summary>
    // '' <remarks></remarks>
    public class PhpFastCgiHandler
    {
    }

    // '' <summary>
    // '' An ASP.NET handler. TODO: implement this using ISAPI dll or CGI/FastCGI (see Abyss Web Server)
    // '' </summary>
    // '' <remarks></remarks>
    public class AspDotNetHandler : Handler
    {

        public override string HandleRequest(Type1.Request req)
        {
            return null;
        }
    }
}
// UNDONE: maybe use this later
// Namespace FastCgi
//     Class Record
//         Public version As Char = "1"
//         Public type As Char = "1"
//         Public requestIdB1 As Char
//         Public requestIdB0 As Char
//         Public contentLengthB1 As Char
//         Public contentLengthB0 As Char
//         Public paddingLength As Char
//         Public reserved As Char
//         Public contentData() As Char
//         Public paddingData() As Char
//     End Class
// End Namespace