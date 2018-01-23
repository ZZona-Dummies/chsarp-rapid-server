using System;
using System.Collections;
using Concurrent = System.Collections.Concurrent;
using Net = System.Net;
using Xml = System.Xml;
using IO = System.IO;
using Threading = System.Threading;
using Text = System.Text;
using static RapidServer.Globals;

namespace RapidServer.Http.Type1
{

    // '' <summary>
    // '' An http server with an async I/O model implemented via IAsyncResult (.NET 2.0+). Utilizes the event-based asynchronous pattern (EAP) and the asynchronous programming model (APM) pattern.
    // '' </summary>
    // '' <remarks></remarks>
    public class Server
    {

        public int SendBufferSize;

        public int ReceiveBufferSize;

        // Public BufferSize As Integer
        private Handlers _handlers = new Handlers();

        private int _connections;

        public Hashtable Sites = new Hashtable();

        public Hashtable MimeTypes = new Hashtable();

        public Concurrent.ConcurrentDictionary<string, Response> OutputCache = new Concurrent.ConcurrentDictionary<string, Response>();

        public Concurrent.ConcurrentDictionary<string, Request> RequestCache = new Concurrent.ConcurrentDictionary<string, Request>();

        public ArrayList DefaultDocuments = new ArrayList();

        public ArrayList ResponseHeaders = new ArrayList();

        public int ConnectedClients;

        public int ConnectedA;

        public int DisconnectedA;

        public int DisconnectedB;

        public int DisconnectedC;

        //  the server should function out-of-box by handling its own events internally, but the events can also be overridden during implementation for custom handling
        event EventHandler SiteStarted;

        event EventHandler ServerStarted;

        event EventHandler ServerShutdown;

        event EventHandler HandleRequest;

        private Request req;

        private Net.Sockets.Socket client;

        event EventHandler ProxyRequest;

        private string server_address;

        event EventHandler ClientConnecting;

        //private Request req;

        private Net.Sockets.Socket socket;

        private string head;

        event EventHandler ClientConnected;

        private Net.Sockets.Socket argClientSocket;

        event EventHandler ClientDisconnected;

        event EventHandler LogMessage;

        private string message;

        public bool EnableOutputCache;

        public bool EnableDirectoryListing;

        private Client Proxy1 = new Client(false);

        // '' <summary>
        // '' Constructs a new HTTP server using the config file.
        // '' </summary>
        // '' <remarks></remarks>
        Server()
        {
            //  we need to load the config once so Form_Load() can populate the form
            //  TODO: we need to unload and reload the config when the server is stopped and restarted via the form
            LoadConfig();
        }

        // '' <summary>
        // '' Loads the server config file http.xml from disk and configures the server to operate as defined by the config.
        // '' </summary>
        // '' <remarks></remarks>
        void LoadConfig()
        {
            //  TODO: Xml functions are very picky after load, if we try to access a key that doesn't exist it will throw a 
            //    vague error that does not stop the debugger on the error line, and the innerexception states 'object reference 
            //    not set to an instance of an object'. a custom function GetValue() helps avoid nulls but not this. default values should
            //    be assumed by the server for cases when the value can't be loaded from the config, or the server should regenerate the config 
            //    per its known format and then load it.
            if ((IO.File.Exists("http.xml") == false))
            {
                CreateConfig();
            }

            Xml.XmlDocument cfg = new Xml.XmlDocument();
            try
            {
                cfg.Load("http.xml");
            }
            catch (Exception ex)
            {
                DebugMessage("Could not parse config file - malformed xml detected.", DebugMessageType.ErrorMessage, "LoadConfig", ex.Message);
                //  TODO: we need to notify the user that the config couldn't be loaded instead of just dying...
                return;
            }

            Xml.XmlNode root = cfg["Settings"];
            //  parse the sites (aka virtual hosts):
            foreach (Xml.XmlNode n in root["Sites"])
            {
                Site s = new Site();
                s.Title = n["Title"].Value;
                s.Path = n["Path"].Value;
                s.Host = n["Host"].Value;
                s.Port = n["Port"].Value;
                s.RootPath = s.Path;
                //  TODO: convert relpath to abspath
                s.RootUrl = ("http://"
                            + (s.Host + (":" + s.Port)));
                s.Upstream = n["Upstream"].Value;
                if ((s.Upstream != ""))
                {
                    s.Role = "Load Balancer";
                }
                else
                {
                    s.Role = "Standard";
                }

                Sites.Add(s.Title, s);
            }

            //  parse the basic options:
            SendBufferSize = root["Options"]["SendBufferSize"].Value.Length;
            ReceiveBufferSize = root["Options"]["ReceiveBufferSize"].Value.Length;
            root["Options"]["DirectoryListing"].Attributes["Enabled"].InnerText = bool.TrueString;
            EnableDirectoryListing = true;
            root["Options"]["OutputCache"].Attributes["Enabled"].InnerText = bool.TrueString;
            EnableOutputCache = true;
            foreach (Xml.XmlNode n in root["MimeTypes"])
            {
                string[] fileExtensions = n.Attributes["FileExtension"].Value.Split(((char)(",")));
                foreach (string ext in fileExtensions)
                {
                    MimeType m = new MimeType();
                    m.Name = n.Value;
                    m.FileExtension = ext;
                    m.Compress = ((CompressionMethod)(Enum.Parse(typeof(CompressionMethod), n.Attributes["Compress"].Value, true)));
                    m.Expires = n.Attributes["Expires"].Value;
                    m.Handler = n.Attributes["Handler"].Value;
                    MimeTypes.Add(m.FileExtension, m);
                }

            }

            //  parse the default documents, which are used when the request uri is a directory instead of a document:
            foreach (Xml.XmlNode n in root["DefaultDocuments"])
            {
                DefaultDocuments.Add(n.InnerText);
            }

            //  parse the response headers, which let us include certain headers in the http response by default:
            if (bool.Parse(root["ResponseHeaders"].Attributes["Enabled"].InnerText) == true)
            {
                foreach (Xml.XmlNode n in root["ResponseHeaders"])
                {
                    ResponseHeaders.Add(n.InnerText);
                }

            }

            //  parse the handlers, which let us use external programs and api calls, such as a php script parser or ldap query:
            foreach (Xml.XmlNode n in root["Handlers"])
            {
                if ((bool.Parse(n.Attributes["Enabled"].InnerText) == true))
                {
                    string handlerName = n["Name"].InnerText;
                    string handlerPath = n["ExecutablePath"].InnerText;
                    //  parse the handler name and create a matching handler object if one exists
                    if ((handlerName == "PhpCgi"))
                    {
                        PhpCgiHandler h = new PhpCgiHandler();
                        h.Name = handlerName;
                        h.ExecutablePath = handlerPath;
                        _handlers.Add(h);
                    }

                }

            }

        }

        void CreateConfig()
        {
            IO.StreamWriter f = new IO.StreamWriter("http.xml");
            string str = @"<![CDATA[<?xml version=""1.0"" encoding=""utf-8"" ?>
<Settings>

<!-- any node with Enabled=""False"" should be ignored/not loaded/not supported by the server instance -->
<!-- any node group with only one type of node may include a <Default> node to imply how the server should behave for unhandled cases -->

<Sites>
	<Site>
		<Title>site1</Title> <!-- a simple title for identifying the site in the UI and logs (e.g. My First Site) -->
		<Path>c:\site1</Path> <!-- relative or absolute physical path to the site's web directory (e.g. c:\myweb1) -->
		<Host>127.0.0.1</Host> <!-- ip address or dns domain name the server will utilize (e.g. localhost / 127.0.0.1) -->
		<Port>9999</Port> <!-- port on which the server will listen for incoming connections -->
	</Site>
</Sites>

<Options>
	<SendBufferSize>4096</SendBufferSize> <!--  -->
	<ReceiveBufferSize>4096</ReceiveBufferSize>
	<KeepAlive Enabled=""True"">
		<MaxRequests>10000</MaxRequests> <!-- limits how many requests can be made by the keep-alive connection before that connection is forced closed -->
		<Timeout>10</Timeout> <!-- limits how long, in seconds, that a keep-alive connection can remain open before that connection is forced closed -->
	</KeepAlive>
	<OutputCache Enabled=""True"">
		<FileQuota>20</FileQuota> <!-- limits how many files can be in the cache -->
		<SizeQuota>50</SizeQuota> <!-- limits ram usage (in MB) -->
	</OutputCache>
	<Gzip Enabled=""True""> <!-- GzipStream is internal to the .NET framework; module does not rely on an external module -->
		<MinimumFileSize>600</MinimumFileSize> <!-- prevents compressing files smaller than this (in Bytes); compressing files smaller than 150 Bytes can increase their size and compressing many smaller files during a request can tax the cpu. -->
	</Gzip>
	<Deflate Enabled=""True""> <!-- DeflateStream is internal to the .NET framework; module does not rely on an external module -->
		<MinimumFileSize>600</MinimumFileSize> <!-- prevents compressing files smaller than this (in Bytes); compressing files smaller than 150 Bytes can increase their size and compressing many smaller files during a request can tax the cpu. -->
	</Deflate>
    <DirectoryListing Enabled=""True"">
    </DirectoryListing>
</Options>

<MimeTypes>
	<Default FileExtension="""" Compress=""none"" Expires=""access plus 1 month"" Handler="""">text/plain</Default>
	<MimeType FileExtension=""js"" Compress=""gzip"" Expires=""access plus 1 year"">application/javascript</MimeType>
	<MimeType FileExtension=""css"" Compress=""gzip"" Expires=""access plus 1 year"">text/css</MimeType>
	<MimeType FileExtension=""txt"" Compress=""gzip"" Expires=""access plus 1 year"">text/plain</MimeType>
	<MimeType FileExtension=""htm,html"" Compress=""none"" Expires=""access plus 1 year"">text/html</MimeType>
	<MimeType FileExtension=""php"" Compress=""none"" Expires=""access plus 1 year"" Handler=""PhpCgi"">text/html</MimeType>
	<MimeType FileExtension=""json"" Compress=""gzip"" Expires=""access plus 1 year"">application/json</MimeType>
	<MimeType FileExtension=""jpg,jpeg"" Compress=""none"" Expires=""access plus 1 year"">image/jpeg</MimeType>
	<MimeType FileExtension=""png"" Compress=""none"" Expires=""access plus 1 year"">image/png</MimeType>
	<MimeType FileExtension=""svg"" Compress=""none"" Expires=""access plus 1 year"">image/svg+xml</MimeType>
	<MimeType FileExtension=""gif"" Compress=""none"" Expires=""access plus 1 year"">image/gif</MimeType>
</MimeTypes>

<Handlers>
	<Handler Enabled=""True"">
		<Name>PhpCgi</Name>
		<ExecutablePath>c:\php\php-cgi.exe</ExecutablePath>
	</Handler>
	<Handler Enabled=""True"">
		<Name>PhpFastCgi</Name>
		<ExecutablePath>c:\php\php-cgi.exe</ExecutablePath>
	</Handler>
</Handlers>

<DefaultDocuments>
	<Document>index.php</Document>
	<Document>index.htm</Document>
	<Document>index.html</Document>
	<Document>default.htm</Document>
</DefaultDocuments>

<ResponseHeaders Enabled=""True"">
	<Header>Server: Rapid-Server</Header>
	<Header>X-Powered-By: Rapid-Server</Header>
</ResponseHeaders>

</Settings>]]>";
            f.Write(str);
            f.Close();
            f.Dispose();
            LoadConfig();
        }

        // '' <summary>
        // '' Matches the requested file to a MimeType based on the FileType (extension). MimeTypes are defined in the server config file and include attributes that determine how the server will handle compression and expiration for the resource.
        // '' </summary>
        // '' <param name="path"></param>
        // '' <returns></returns>
        // '' <remarks></remarks>
        string GetContentType(string path)
        {
            string ext = IO.Path.GetExtension(path).TrimStart(((char)(".")));
            MimeType m = ((MimeType)(MimeTypes[ext]));
            string contentType;
            if (m != null)
                contentType = m.Name;
            else
                contentType = "text/plain";

            return contentType;
        }

        // '' <summary>
        // '' Starts the server, allowing clients to connect and make requests to one or more of the sites specified in the config.
        // '' </summary>
        // '' <remarks></remarks>
        void StartServer()
        {
            // LoadConfig()
            //  bind each site to it's address and start listening for client connections
            foreach (Site s in Sites.Values)
            {
                try
                {
                    Net.IPEndPoint ep = AddressToEndpoint(s.Host, s.Port);
                    s.Socket = new System.Net.Sockets.Socket(Net.Sockets.AddressFamily.InterNetwork, Net.Sockets.SocketType.Stream, Net.Sockets.ProtocolType.Tcp);
                    s.Socket.Bind(ep);
                    s.Socket.Listen(20000);
                    s.Socket.BeginAccept(0, new AsyncCallback(new EventHandler(this.ClientConnectedAsync)), s);
                    DebugMessage("Site started...", DebugMessageType.InfoMessage, "StartServer");
                    SiteStarted();
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    DebugMessage(("Could not start the site \'"
                                    + (s.Title + ("\'. Make sure it\'s address ("
                                    + (s.Host + (":"
                                    + (s.Port + ") is not already in use.")))))), DebugMessageType.ErrorMessage, "StartServer", ex.Message);
                }
                catch (Exception ex2)
                {
                    DebugMessage(("Unhandled exception in StartServer: " + ex2.Message), DebugMessageType.ErrorMessage, "StartServer", ex2.Message);
                }

            }

            ServerStarted();
        }

        // '' <summary>
        // '' Stops the server, shutting down each Site.
        // '' </summary>
        // '' <remarks></remarks>
        void StopServer()
        {
            //  shutdown each site:
            //  TODO: this throws a bunch of exceptions. To test, just Start then Stop.
            foreach (Site s in Sites.Values)
            {
                try
                {
                    //  try to safely shutdown first, allowing pending transmissions to finish which helps prevent prevent data loss
                    s.Socket.Shutdown(Net.Sockets.SocketShutdown.Both);
                }
                catch (Exception ex)
                {
                    DebugMessage(("The site \'"
                                    + (s.Title + "\' failed to shut down gracefully and will now be terminated.")), DebugMessageType.WarningMessage, "StopServer", ex.Message);
                }

                //  kill the Site socket, terminating any clients abrubtly (data loss may occur if Shutdown is unsuccessful)
                s.Socket.Close();
                s.Socket.Disconnect(false);
            }

            ServerShutdown();
        }

        // '' <summary>
        // '' Accepts the client that is attempting to connect, allows another client to connect (whenever that may be), and finally begins receiving data from the client that just connected.
        // '' </summary>
        // '' <param name="ar"></param>
        // '' <remarks></remarks>
        void ClientConnectedAsync(IAsyncResult ar)
        {
            //  get the async state object from the async BeginAccept method, which contains the server's listening socket
            Site s = ((Site)(ar.AsyncState));
            System.Net.Sockets.Socket clientSocket = null;
            try
            {
                //  accept the client connection, giving us the client socket to work with:
                clientSocket = s.Socket.EndAccept(ar);
                //  update the connected client count in a thread safe way
                Threading.Interlocked.Increment(ConnectedClients);
                // RaiseEvent ClientConnected(clientSocket)
                // DebugMessage("Client connected. Total connections: " & _connections, DebugMessageType.UsageMessage, "ClientConnectedAsync")
                //  begin accepting another client connection:
                s.Socket.BeginAccept(0, new AsyncCallback(new EventHandler(this.ClientConnectedAsync)), s);
            }
            catch (ObjectDisposedException ex1)
            {
                //  if we get an ObjectDisposedException it that means the server socket terminated while this async method was still active
                DebugMessage("The server was closed before the async method could complete.", DebugMessageType.WarningMessage, "ClientConnectedAsync", ex1.Message);
                return;
            }
            catch (Exception ex2)
            {
                DebugMessage("An unhandled exception occurred in ClientConnectedAsync.", DebugMessageType.ErrorMessage, "ClientConnectedAsync", ex2.Message);
                return;
            }

            //  begin receiving data (http requests) from the client socket:
            AsyncReceiveState asyncState = new AsyncReceiveState(ReceiveBufferSize, null);
            asyncState.Site = s;
            asyncState.Socket = clientSocket;
            asyncState.Socket.BeginReceive(asyncState.Buffer, 0, ReceiveBufferSize, Net.Sockets.SocketFlags.None, new AsyncCallback(new System.EventHandler(this.RequestReceivedAsync)), asyncState);
        }

        // '' <summary>
        // '' Accepts the client request, builds the request/response objects and passes them to the event handler where the response 
        // '' object will be finalized and sent back to the client.
        // '' </summary>
        // '' <param name="ar"></param>
        // '' <remarks></remarks>
        void RequestReceivedAsync(IAsyncResult ar)
        {
            //  get the async state object:
            AsyncReceiveState asyncState = ((AsyncReceiveState)(ar.AsyncState));
            int numBytesReceived;
            try
            {
                //  call EndReceive which will give us the number of bytes received
                numBytesReceived = asyncState.Socket.EndReceive(ar);
            }
            catch (Net.Sockets.SocketException ex)
            {
                //  if we get a ConnectionReset exception, it could indicate that the client has disconnected
                if ((ex.SocketErrorCode == Net.Sockets.SocketError.ConnectionReset))
                {
                    DebugMessage("EndReceive on the client socket failed because the client has disconnected.", DebugMessageType.UsageMessage, "RequestReceivedAsync", ex.Message);
                    //  update the connected client count in a thread safe way
                    Threading.Interlocked.Decrement(ConnectedClients);
                    // RaiseEvent ClientDisconnected(asyncState.Socket)
                    return;
                }

            }

            //  if we get numBytesReceived equal to zero, it could indicate that the client has disconnected
            //  TODO: does this actually disconnect the client though, or just assume it was?
            if ((numBytesReceived == 0))
            {
                //  update the connected client count in a thread safe way
                Threading.Interlocked.Decrement(ConnectedClients);
                // RaiseEvent ClientDisconnected(asyncState.Socket)
                return;
            }

            //  if we've reached this point, we were able to parse the IAsyncResult which contains our raw request bytes, so proceed 
            //    to handle it on a separate ThreadPool thread; it is important that we free up the I/O completion port being 
            //    used for this async operation as soon as possible, therefore we don't even attempt to parse the requestBytes at 
            //    this point and just immediately pass the raw request bytes to a ThreadPool thread for further processing
            Threading.ThreadPool.QueueUserWorkItem(new System.EventHandler(this.HandleRequestAsync), asyncState);
        }

        //  handles the request on a separate ThreadPool thread.
        void HandleRequestAsync(AsyncReceiveState asyncState)
        {
            //  TODO: first try pull the request from the request cache, otherwise parse it now
            //  convert the raw request bytes/string into an HttpRequest object for ease of use
            string reqString = Text.Encoding.ASCII.GetString(asyncState.Buffer).Trim('\0');
            Request req;
            //  pull the request object from the request cache, or create it now
            if (RequestCache.ContainsKey(reqString))
            {
                req = RequestCache[reqString];
            }
            else
            {
                req = new Request(reqString, this, asyncState.Socket, asyncState.Site);
                RequestCache.TryAdd(reqString, req);
            }

            //  first try to serve the response from the output cache
            bool servedFromCache = false;
            bool cacheAllowed = true;
            if ((EnableOutputCache == true))
            {
                if ((req.CacheAllowed == true))
                {
                    if ((OutputCache.ContainsKey((req.AbsPath + req.QueryString)) == true))
                    {
                        Response res = ((Response)(OutputCache[(req.AbsPath + req.QueryString)]));
                        SendResponse(req, res, asyncState.Socket);
                        servedFromCache = true;
                        DebugMessage(("Serving resource from cache: "
                                        + (req.AbsPath + ".")), DebugMessageType.UsageMessage, "HandleRequestAsync");
                    }

                }

            }

            //  response couldn't be served from cache, handle the request according to the site role
            if ((servedFromCache == false))
            {
                //  if the current site is a reverse proxy/load balancer with upstream servers defined, forward the request to another server,
                //    otherwise handle the request by this server like normal
                if ((asyncState.Site.Role == "Standard"))
                {
                    //  raise an event to handle the request/response cycle (this can be overridden during implementation to allow for custom handling)
                    HandleRequest(req, asyncState.Socket);
                }
                else if ((asyncState.Site.Role == "Load Balancer"))
                {
                    //  parse the upstream servers and select one using the defined algorithm
                    string[] upstreams = asyncState.Site.Upstream.Split(",");
                    Random r = new Random();
                    int i = r.Next(0, upstreams.Length);
                    //  forward the request to the selected upstream server
                    ProxyRequest(req, upstreams[i], asyncState.Socket);
                }

            }

        }

        //  makes a GET request to the upstream server on behalf of the client
        private void HttpServer_ProxyRequest(Request req, string server_address, Net.Sockets.Socket client)
        {
            string newUri = (server_address + req.Uri);
            ProxyState ps = new ProxyState();
            ps.client = client;
            ps.req = req;
            Proxy1.Go(newUri, ps);
        }

        //  sends the response from the upstream server back to the original client which made the request
        private void Proxy1_HandleResponse(string responseString, object state)
        {
            ProxyState ps = state;
            Request req = ps.req;
            Net.Sockets.Socket client = ps.client;
            Response res = new Response(this, ps.req, ps.client);
            res.ResponseBytes = Text.Encoding.ASCII.GetBytes(responseString);
            TryCache(req, res);
            SendResponse(req, res, client);
        }

        //  try to cache the response, if it hasn't already been
        void TryCache(Request req, Response res)
        {
            if (((EnableOutputCache == true)
                        && ((req.CacheAllowed == true)
                        && (req.Method != "POST"))))
            {
                OutputCache.TryAdd((req.AbsPath + req.QueryString), res);
            }

        }

        // '' <summary>
        // '' Handles the client request, implements the full request/response cycle. This event is triggered by HandleRequestAsync.
        // '' Can be listened to in a windows form for custom req/res inspection and handling, similar to node.
        // '' </summary>
        // '' <param name="req"></param>
        // '' <param name="client"></param>
        // '' <remarks></remarks>
        private void HttpServer_HandleRequest(Request req, Net.Sockets.Socket client)
        {
            Response res = new Response(this, req, client);
            //  if the Uri is missing a trailing slash, 301 redirect to the correct Uri
            if ((req.FixPath301 == true))
            {
                res.Headers.Remove("Connection");
                //  don't keep-alive for a 404
                res.Headers.Add("Location", (req.Uri + "/"));
                res.StatusCode = "301";
            }

            //  serve the static or dynamic file from disk:
            if ((req.FixPath301 == false))
            {
                if ((IO.File.Exists(req.AbsPath) == true))
                {
                    //  handle the request using an appropriate handler:
                    if ((req.MimeType.Handler == "PhpCgi"))
                    {
                        string data = _handlers["PhpCgi"].HandleRequest(req);
                        //  php returned a response, parse it and continue building the final response
                        res.Parse(data);
                        res.SetContent(res.Content);
                        if (res.Headers.ContainsKey("Status"))
                        {
                            //  parse the response code from the Status header:
                            res.StatusCode = res.Headers("Status").split(" ")[0];
                        }
                        else
                        {
                            //  status code not found in the response headers, just use 200 OK:
                            //  TODO: something tells me this is not obeying RFC protocol, look into it...
                            res.StatusCode = "200";
                        }

                    }
                    else
                    {
                        //  custome handler not found, serve as static file:
                        res.ContentType = req.MimeType.Name;
                        res.SetContent(IO.File.ReadAllBytes(req.AbsPath));
                        res.StatusCode = 200;
                    }

                    //  cache the response for this request:
                    //  TODO: ab -n 100000 -c 1000 breaks this if socket reuse = True in SendAsync()
                    //  TODO: caching a POST request (such as WordPress login) breaks stuff, so we should only do it when necessary instead of always...
                    //    https://stackoverflow.com/questions/626057/is-it-possible-to-cache-post-methods-in-http
                    TryCache(req, res);
                }
                else
                {
                    //  file not found, return directory list or 404:
                    if (((EnableDirectoryListing == true)
                                && IO.Directory.Exists(req.AbsPath)))
                    {
                        string listing = BuildDirectoryListing(req);
                        res.SetContent(Text.Encoding.ASCII.GetBytes(listing));
                        res.Headers.Remove("Connection");
                        //  don't keep-alive for a 404
                        res.StatusCode = 200;
                    }
                    else
                    {
                        res.SetContent(Text.Encoding.ASCII.GetBytes("FAIL WHALE!"));
                        res.Headers.Remove("Connection");
                        //  don't keep-alive for a 404
                        res.StatusCode = 404;
                    }

                }

            }

            //  the response has been finalized, send it to the client
            res.ResponseBytes = res.BuildResponseBytes;
            SendResponse(req, res, client);
        }

        //  builds a directory listing using html for basic navigation
        void BuildDirectoryListing(Request req)
        {
            string listing = "<h1>Directory Listing</h1>";
            foreach (IO.DirectoryInfo d in (new IO.DirectoryInfo(req.AbsPath) + GetDirectories))
            {
                ("<div><a href=\'"
                            + (req.Uri.TrimEnd("/") + ("/"
                            + (d.Name + ("\'>"
                            + (d.Name + "</a></div>"))))));
            }

            foreach (IO.FileInfo f in (new IO.DirectoryInfo(req.AbsPath) + GetFiles))
            {
                // Dim ms As New IO.MemoryStream
                // Dim ico As System.Drawing.Icon
                // ico = System.Drawing.Icon.ExtractAssociatedIcon(f.FullName)
                // ico.ToBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png)
                // listing &= "<div>" & "<image style='height:16px' src='data:image/png;base64," & Convert.ToBase64String(ms.ToArray) & "' /><a href='" & f.Name & "'>" & f.Name & "</a></div>"
                ("<div>" + ("<a href=\'"
                            + (f.Name + ("\'>"
                            + (f.Name + "</a></div>")))));
            }

            return listing;
        }

        //  sends the http response to the client
        void SendResponse(Request req, Response res, Net.Sockets.Socket client)
        {
            //  convert the response into bytes and package it in an async object for callback purposes:
            // Dim responseBytes() As Byte = res.BuildResponseBytes
            AsyncSendState sendState = new AsyncSendState(client, SendBufferSize, null);
            sendState.BytesToSend = res.ResponseBytes;
            sendState.Tag = req.AbsPath;
            //  start sending the response to the client in an async fashion:
            try
            {
                client.BeginSend(res.ResponseBytes, 0, res.ResponseBytes.Length, Net.Sockets.SocketFlags.None, new System.EventHandler(this.SendResponseAsync), sendState);
            }
            catch (Exception ex)
            {
                DebugMessage("Unhandled exception in SendResponse when trying to send data to the client.", DebugMessageType.UsageMessage, "SendResponse", ex.Message);
            }

            //  determine whether or not to continue receiving more data from the client:
            //  TODO: tidy this up a bit instead of setting a property to true then checking it right afterwards...
            //  IMPORTANT: for keep-alive connections we should make a final receive call to the client and if the client does not send a Connection: keep-alive header then we know to disconnect
            if ((res.Headers.ContainsKey("Connection") == true))
            {
                if ((res.Headers("Connection").ToString.ToLower == "keep-alive"))
                {
                    sendState.Persistent = true;
                }

            }

            //  start receiving more data from the client in an async fashion
            if ((sendState.Persistent == true))
            {
                AsyncReceiveState receiveState = new AsyncReceiveState(ReceiveBufferSize, null);
                receiveState.Site = req.Site;
                receiveState.Socket = client;
                try
                {
                    receiveState.Socket.BeginReceive(receiveState.Buffer, 0, ReceiveBufferSize, Net.Sockets.SocketFlags.None, new AsyncCallback(new System.EventHandler(this.RequestReceivedAsync)), receiveState);
                }
                catch (Exception ex)
                {
                    DebugMessage(("SendResponse encountered an exception when trying to BeginReceive on the client socket. " + ex.Message), DebugMessageType.ErrorMessage, "ClientRequest", ex.Message);
                }

            }

        }

        // '' <summary>
        // '' Sends the http response to the client and closes the connection using a separate thread.
        // '' </summary>
        // '' <param name="ar"></param>
        // '' <remarks></remarks>
        void SendResponseAsync(IAsyncResult ar)
        {
            AsyncSendState asyncState = ((AsyncSendState)(ar.AsyncState));
            try
            {
                asyncState.Socket.EndSend(ar);
                //  disconnect the client if not keep-alive:
                if ((asyncState.Persistent == false))
                {
                    // asyncState.Socket.Disconnect(True)
                }

                DebugMessage(("Sent "
                                + (asyncState.Tag + (" to "
                                + (asyncState.Socket.RemoteEndPoint.ToString + ".")))), DebugMessageType.UsageMessage, "SendAsync");
            }
            catch (Exception ex)
            {
                //  UNDONE: this DebugMessage() didn't used to trigger errors, but now it does...
                // DebugMessage("Failed to send " & asyncState.Tag & " to " & asyncState.Socket.RemoteEndPoint.ToString & ". The exception was: " & ex.Message, DebugMessageType.UsageMessage, "SendAsync")
                DebugMessage("SendResponseAsync encountered an exception when trying to send data to the client.", DebugMessageType.ErrorMessage, "SendResponseAsync", ex.Message);
            }

            //  finally terminate the socket after allowing pending transmissions to complete. this eliminates ERR_CONNECTION_RESET that would happen occasionally on random resources:
            if ((asyncState.Persistent == false))
            {
                //  TODO: we check Persistent attribute twice in this method...if we put Close below Disconnect it will throw the exception that we can't access the socket because it is disposed
                // asyncState.Socket.Close()
                //  update the connected client count in a thread safe way
                // Threading.Interlocked.Decrement(Me.ConnectedClients)
                // RaiseEvent ClientDisconnected(asyncState.Socket)
            }

        }
    }

    class ProxyState
    {

        public Request req;

        public Net.Sockets.Socket client;
    }
}