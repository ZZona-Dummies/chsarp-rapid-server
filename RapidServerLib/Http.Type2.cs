using Net = System.Net;
using Xml = System.Xml;
using IO = System.IO;
using Threading = System.Threading;
using Text = System.Text;
using System;
using static RapidServer.Globals;
using System.Collections;
using System.Collections.Generic;

namespace RapidServer.Http.Type2
{

    // '' <summary>
    // '' An http server with an async I/O model implemented via SocketAsyncEventArgs (.NET 3.5+). Utilizes a special async model designed for the Socket class which consists of a shared buffer and pre-allocated object pool for async state objects to avoid object instantiation and memory thrashing/fragmentation during every http request.
    // '' The MSDN code example for this pattern is very poor. The issues (and solution) are explained in the Background section in this tutorial: http://www.codeproject.com/Articles/83102/C-SocketAsyncEventArgs-High-Performance-Socket-Cod?fid=1573061
    // '' </summary>
    // '' <remarks></remarks>
    public class Server
    {

        private Net.Sockets.Socket _serverSocket;

        private Interops _interops = new Interops();

        private int _numConnections;

        private int _maxConnections;

        private BufferManager _bufferManager;

        private SocketAsyncEventArgsPool _readWritePool;

        private int _totalBytesRead;

        private Threading.Semaphore _maxNumberAcceptedClients;

        public Hashtable MimeTypes = new Hashtable();

        public Hashtable OutputCache = new Hashtable();

        public string WebRoot;

        public ArrayList DefaultDocuments = new ArrayList();

        public ArrayList ResponseHeaders = new ArrayList();

        //  events - the server should function out-of-box by calling its own events, but these events can also be overridden during implementation for custom handling if desired.
        event EventHandler ServerStarted;

        event EventHandler ServerShutdown;

        event EventHandler HandleRequest;

        private Request req;

        private Response res;

        private Net.Sockets.SocketAsyncEventArgs client;

        event EventHandler ClientConnecting;

        private Net.Sockets.Socket socket;

        private string head;

        event EventHandler ClientConnected;

        private Net.Sockets.Socket argClientSocket;

        event EventHandler ClientDisconnected;

        // '' <summary>
        // '' Constructs a new HTTP server given a desired web root path.
        // '' </summary>
        // '' <param name="rootPath"></param>
        // '' <remarks></remarks>
        Server(string rootPath)
        {
            WebRoot = rootPath;
            LoadConfig();
            const int maxConnections = 10000;
            //  pull this from the config, but for now we just make them usable
            const int receiveBufferSize = 4096;
            //  pull this from the config, but for now we just make them usable
            _maxConnections = maxConnections;
            _bufferManager = new BufferManager((receiveBufferSize
                            * (_maxConnections * 2)), receiveBufferSize);
            _readWritePool = new SocketAsyncEventArgsPool(_maxConnections);
            _maxNumberAcceptedClients = new Threading.Semaphore(_maxConnections, _maxConnections);
        }

        // '' <summary>
        // '' Loads the server config file http.xml from disk and configures the server to operate as defined by the config.
        // '' </summary>
        // '' <remarks></remarks>
        void LoadConfig()
        {
            Xml.XmlDocument cfg = new Xml.XmlDocument();
            try
            {
                cfg.Load("http.xml");
            }
            catch (Exception ex)
            {
                DebugMessage("Could not parse config file - malformed xml detected.", DebugMessageType.ErrorMessage, "LoadConfig", ex.Message);
                //  TODO: should we terminate the server now, or allow it to startup without a config file?
                return;
            }

            Xml.XmlNode root = cfg["Settings"];
            //  parse the MIME types, letting us know what compression and expiration settings to use when serving them to clients:
            foreach (Xml.XmlNode n in root["MimeTypes"])
            {
                string[] fileExtensions = n.Attributes["FileExtension"].Value.Split(',');
                foreach (string ext in fileExtensions)
                {
                    MimeType m = new MimeType();
                    m.Name = n.Value;
                    m.FileExtension = ext;
                    m.Compress = (CompressionMethod)Enum.Parse(typeof(CompressionMethod), n.Attributes["Compress"].Value, true);
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

            //  parse the virtual hosts, which let us define more than one functional site from various paths on disk:
            foreach (Xml.XmlNode n in root["VirtualHosts"])
            {
                //  TODO: implement support for virtual hosts here...
            }

            // ' parse the interops, which let us use external programs and api calls, such as a php script parser or ldap query:
            // For Each n As Xml.XmlNode In root("Interops")
            //     If n("Enabled").InnerText = True Then
            //         Dim i As New Interop
            //         i.Name = n("Name").InnerText
            //         i.ExecutablePath = n("ExecutablePath").InnerText
            //         i.MaxInstances = n("MaxInstances").InnerText
            //         i.UsesPerInstance = n("UsesPerInstance").InnerText
            //         i.LifetimePerInstance = n("LifetimePerInstance").InnerText
            //         _interops.AddInterop(i)
            //     End If
            // Next
        }

        // '' <summary>
        // '' Matches the requested file to a MimeType based on the FileType (extension). MimeTypes are defined in the server config file and include attributes that determine how the server will handle compression and expiration for the resource.
        // '' </summary>
        // '' <param name="path"></param>
        // '' <returns></returns>
        // '' <remarks></remarks>
        public string GetContentType(string path)
        {
            string ext = IO.Path.GetExtension(path).TrimStart('.');
            MimeType m = (MimeType)MimeTypes[ext];
            string contentType;
            if (m != null)
                contentType = m.Name;
            else
                contentType = "text/plain";

            return contentType;
        }

        // '' <summary>
        // '' Starts the server, allowing clients to connect and make requests.
        // '' </summary>
        // '' <param name="Ip"></param>
        // '' <param name="Port"></param>
        // '' <remarks></remarks>
        void StartServer(string Ip, int Port)
        {
            //  new async pattern
            _bufferManager.InitBuffer();
            Net.Sockets.SocketAsyncEventArgs readWriteEventArg = new Net.Sockets.SocketAsyncEventArgs();
            //  allocate enough memory in the shared buffer, and enough SocketAsyncEventArgs objects, for the max connections that we wish to support
            for (int i = 0; i <= _maxConnections - 1; ++i)
            {
                readWriteEventArg = new Net.Sockets.SocketAsyncEventArgs();
                readWriteEventArg.Completed += IoCompleted;
                readWriteEventArg.UserToken = new AsyncUserToken();
                _bufferManager.SetBuffer(readWriteEventArg);
                _readWritePool.Push(readWriteEventArg);
            }

            Net.IPEndPoint endPoint = AddressToEndpoint(Ip, Port);
            _serverSocket = new Net.Sockets.Socket(endPoint.AddressFamily, Net.Sockets.SocketType.Stream, Net.Sockets.ProtocolType.Tcp);
            _serverSocket.Bind(endPoint);
            _serverSocket.Listen(20000);
            StartAccept(null);
            ServerStarted(null, null);
        }

        void StartAccept(Net.Sockets.SocketAsyncEventArgs acceptEventArg)
        {
            if ((acceptEventArg == null))
            {
                acceptEventArg = new Net.Sockets.SocketAsyncEventArgs();
                acceptEventArg.Completed += AcceptEventArgCompleted;
            }
            else
            {
                acceptEventArg.AcceptSocket = null;
            }

            _maxNumberAcceptedClients.WaitOne();
            bool willRaiseEvent = _serverSocket.AcceptAsync(acceptEventArg);
            if ((willRaiseEvent == false))
            {
                ProcessAccept(acceptEventArg);
            }

        }

        void AcceptEventArgCompleted(object sender, Net.Sockets.SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        // '' <summary>
        // '' Process the accepted client connection, start receiving data from that client, and start accepting more client connections.
        // '' </summary>
        // '' <param name="e"></param>
        // '' <remarks></remarks>
        void ProcessAccept(Net.Sockets.SocketAsyncEventArgs e)
        {
            Threading.Interlocked.Increment(ref _numConnections);
            Net.Sockets.SocketAsyncEventArgs readEventArgs = _readWritePool.Pop;
            ((AsyncUserToken)readEventArgs.UserToken).Socket = e.AcceptSocket;
            bool willRaiseEvent = e.AcceptSocket.ReceiveAsync(readEventArgs);
            if (!willRaiseEvent)
            {
                ProcessReceive(readEventArgs);
                //  NEVER GETS CALLED...due to keep-alive?
            }

            //  accept the next incoming client connection
            StartAccept(e);
            //  maybe move this up higher in the method
        }

        void IoCompleted(object sender, Net.Sockets.SocketAsyncEventArgs e)
        {
            string s = Text.Encoding.UTF8.GetString(e.Buffer, e.Offset, e.BytesTransferred);
            switch (e.LastOperation)
            {
                case Net.Sockets.SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case Net.Sockets.SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                default:
                    Beep();
                    break;
            }
        }

        void ProcessReceive(Net.Sockets.SocketAsyncEventArgs e)
        {
            //  get the client who we are receiving data from
            AsyncUserToken token = (AsyncUserToken)e.UserToken;
            //  handle the received data by parsing it into a request and sending a response in return
            if (((e.BytesTransferred > 0)
                        && (e.SocketError == Net.Sockets.SocketError.Success)))
            {
                Threading.Interlocked.Add(ref _totalBytesRead, e.BytesTransferred);
                //  place the received data into the shared buffer to avoid frequent memory allocations as we process it
                // e.SetBuffer(e.Offset, e.BytesTransferred) ' if bytesTransferred is very small (40 bytes) it will set the buffer size to 40 as well..and it will be 40 the next time receive...BAD NEWS
                // e.SetBuffer(e.Offset, 4096)
                //  get the data
                // Dim b(e.BytesTransferred - 1) As Byte
                // Buffer.BlockCopy(e.Buffer, e.Offset, b, 0, e.BytesTransferred)
                string s = Text.Encoding.UTF8.GetString(e.Buffer, e.Offset, e.BytesTransferred);
                //  TODO: process the data (below methods use old async, fix this)
                //  construct the req/res and pass it to the event handler for final handling
                //  probably pass the raw request to a threadpool thread for processing, where we can construct the req/res
                Request req = new Request(s, this);
                Response res = new Response(this, req, token.Socket);
                string rs = ("HTTP/1.1 200 OK" + ('\n' + ("Content-Length: 2" + ('\n' + ('\n' + "hi")))));
                byte[] rb = Text.Encoding.ASCII.GetBytes(rs);
                // rb = res.GetResponseBytes
                // e.SetBuffer(resb, 0, resb.Length)
                // e.SetBuffer(e.Offset, 4096)
                e.SetBuffer(rb, 0, rb.Length);
                // Buffer.BlockCopy(rb, 0, e.Buffer, e.Offset, rb.Length)
                bool willRaiseEvent = token.Socket.SendAsync(e);
                // Dim willRaiseEvent As Boolean = token.Socket.ReceiveAsync(e)
                // Dim o As New ProcessRequestObject
                // o.requestBytes = b
                // o.server = Me
                // o.clientSocket = token.Socket
                // o.e = e
                // Threading.ThreadPool.QueueUserWorkItem(AddressOf PrepareRequest, o)
                // Dim pr As New ProcessRequestObject()
                // pr.requestBytes = b
                // Dim test As String = Text.Encoding.ASCII.GetString(pr.requestBytes)
                // pr.server = Me
                // pr.clientSocket = token.Socket
                // pr.e = e
                // Threading.ThreadPool.QueueUserWorkItem(AddressOf ProcessRequest, pr)
                // Dim s As String = System.Text.Encoding.ASCII.GetString(e.Buffer, e.Offset, e.BytesTransferred) ' TODO: contains the http request
                // Dim req As New HttpRequest(s, Me)
                // Dim res As New HttpResponse(Me, req, token.Socket)
                // RaiseEvent HandleRequest(req, res, token.Socket)
                //  TODO: what to do with s? what is the point of willRaiseEvent?
                // Dim willRaiseEvent As Boolean = token.Socket.SendAsync(e)
                // If Not willRaiseEvent Then
                //     ProcessSend(e)
                // End If
            }
            else
            {
                CloseClientSocket(e);
            }

        }

        void ProcessSend(Net.Sockets.SocketAsyncEventArgs e)
        {
            if ((e.SocketError == Net.Sockets.SocketError.Success))
            {
                AsyncUserToken token = (AsyncUserToken)e.UserToken;
                // ' TODO: LEFT OFF HERE
                // Dim x As Net.Sockets.SocketAsyncEventArgs = _readWritePool.Pop
                // x.UserToken = token
                // Dim willRaiseEvent As Boolean = token.Socket.ReceiveAsync(e)
                CloseClientSocket(e);
                // If Not willRaiseEvent Then
                //     ProcessReceive(e)
                // End If
            }
            else
            {
                CloseClientSocket(e);
            }

        }

        // '' <summary>
        // '' This prepares the request/response. Should be called from an IOCP thread with Threadpool.QueueUserWorkItem() to free up the IOCP thread as soon as possible and do the processing on a threadpool thread. Finally, this passes the request/response to the HandleRequest event for default HTTP handling, which can be overridden during implementation for custom handling.
        // '' </summary>
        // '' <remarks></remarks>
        private void PrepareRequest(ProcessRequestObject o)
        {
            //  convert the raw request bytes/string into an HttpRequest object for ease of use
            Request req = new Request(o.requestBytes, o.server);
            //  prepare the response that will be sent back to the client (e.g. load and send a static page e.g. txt or html, load and send a resource e.g. jpg or xml, parse a dynamic script e.g. php or asp.net)
            //  TODO: here we construct a response, but when serving a response from the output cache we shouldn't construct one at all!
            Response res = new Response(this, req, o.clientSocket);
            //  raise an event to handle the request/response cycle (this can be overridden during implementation to allow for custom handling)
            _HandleRequest(req, res, o.e); //Handles ... ???
        }

        // '' <summary>
        // '' Handles the client request/response cycle, replying to a request with a response by default. May be overridden during implementation for custom response handling.
        // '' </summary>
        // '' <param name="req"></param>
        // '' <param name="res"></param>
        // '' <param name="client"></param>
        // '' <remarks></remarks>
        private void _HandleRequest(Request req, Response res, Net.Sockets.SocketAsyncEventArgs client)
        {
            //  serve the requested resource from the output cache or from disk; better yet, store the entire response and serve that up to save some time
            if ((OutputCache.ContainsKey(req.AbsoluteUrl) == true))
            {
                Response cachedResponse = (Response)OutputCache[req.AbsoluteUrl];
                res = cachedResponse;
                DebugMessage(("Serving resource from cache: "
                                + (req.AbsoluteUrl + ".")), DebugMessageType.UsageMessage, "ClientRequest event");
                //  TODO: we might want to move the res.GetAllBytes call into the Else condition here so it doesn't get called again for an already cached response
            }
            else
            {
                //  serve the file from disk
                //  TODO: depending on TransferMethod requested by the client, we should implement StoreAndForward or ChunkedEncoding, but for now we will just use StoreAndForward
                if ((IO.File.Exists(req.AbsoluteUrl) == true))
                {
                    //  determine how to the process the client's request based on the requested uri's file type
                    //  this is where we might load a resourince, maybe using Interops to parse dynamic scripts (e.g. PHP and ASP.NET)
                    if ((req.MimeType.Handler == ""))
                    {
                        //  no custom handler for this mimetype, serve as a static file
                        res.ContentType = req.MimeType.Name;
                        // GetContentType(req.AbsoluteUrl)
                        res.SetContent(IO.File.ReadAllBytes(req.AbsoluteUrl));
                        res.StatusCode = 200;
                    }
                    else
                    {
                        //  TODO: there is a custom handler assigned to this filetype, but handlers are not implemented yet so we just serve it as a static file
                        res.ContentType = req.MimeType.Name;
                        // GetContentType(req.AbsoluteUrl)
                        res.SetContent(IO.File.ReadAllBytes(req.AbsoluteUrl));
                        res.StatusCode = 200;
                    }

                    //  cache the response for future use to improve performance, avoiding the need to process the same response frequently
                    if ((OutputCache.ContainsKey(req.AbsoluteUrl) == false))
                    {
                        OutputCache.Add(req.AbsoluteUrl, res);
                    }
                    else
                    {
                        //  page not found, return 404 status code
                        res.StatusCode = 404;
                    }

                }

                //  send the response to the client who made the initial request
                byte[] responseBytes = res.GetResponseBytes();
                // Dim sendEventArg As Net.Sockets.SocketAsyncEventArgs = _readWritePool.Pop
                // Dim ar As AsyncUserToken = client
                // sendEventArg.UserToken = ar
                // sendEventArg.SetBuffer(responseBytes, 0, responseBytes.Length)
                client.SetBuffer(responseBytes, 0, responseBytes.Length);
                AsyncUserToken token = (AsyncUserToken)client.UserToken;
                bool willRaiseEvent = token.Socket.SendAsync(client);
                if (!willRaiseEvent)
                {
                    //  TODO: this never fires!! why not?
                    Beep();
                }

                //  handle keep-alive or disconnect
                if (res.Headers["Connection"] == "keep-alive")
                {
                    //  receive more from the client
                    // Dim readEventArgs As Net.Sockets.SocketAsyncEventArgs = _readWritePool.Pop
                    // token.Socket.ReceiveAsync(readEventArgs)
                }
                else
                {
                    // token.Socket.Disconnect(False)
                    // token.Socket.Close()
                    //  TODO: maybe move this into the CompleteReceive where we call ReceiveAsync...
                    CloseClientSocket(client);
                }

                // Dim sendState As New AsyncSendState(client)
                // sendState.BytesToSend = responseBytes
                // sendState.Tag = req.AbsoluteUrl
                // If res.Headers["Connection") = "keep-alive" Then
                //     sendState.Persistent = True
                // End If
                // Try
                //     client.BeginSend(responseBytes, 0, responseBytes.Length, Net.Sockets.SocketFlags.None, AddressOf CompleteSend, sendState)
                // Catch ex As Exception
                //     LogEvent("Could not send data to this client. An unhandled exception occurred.", LogEventType.UsageMessage, "ClientRequest", ex.Message)
                // End Try
                // ' call BeginReceive again, so we can receive more data from this client socket (either not needed for http server or only for persistent connections...reimplement for binary rpc server)
                // If sendState.Persistent = True Then
                //     Dim receiveState As New AsyncReceiveState
                //     receiveState.Socket = client
                //     Try
                //         receiveState.Socket.BeginReceive(receiveState.Buffer, 0, gBufferSize, Net.Sockets.SocketFlags.None, New AsyncCallback(AddressOf CompleteRequest), receiveState)
                //     Catch ex As Exception
                //         LogEvent("Could not receive more data from this client. An unhandled exception occurred.", LogEventType.UsageMessage, "ClientRequest", ex.Message)
                //     End Try
                // End If
            }

        }

        void CloseClientSocket(Net.Sockets.SocketAsyncEventArgs e)
        {
            AsyncUserToken token = (AsyncUserToken)e.UserToken;
            try
            {
                token.Socket.Shutdown(Net.Sockets.SocketShutdown.Send);
            }
            catch (Exception ex)
            {
            }

            token.Socket.Close();
            Threading.Interlocked.Decrement(ref _numConnections);
            _maxNumberAcceptedClients.Release();
            _readWritePool.Push(e);
            Console.WriteLine(_readWritePool.Count);
        }

        // '' <summary>
        // '' Stops the server, disconnecting any current clients and terminating any pending requests/responses.
        // '' </summary>
        // '' <remarks></remarks>
        void StopServer()
        {
            try
            {
                //  try to safely shutdown first, allowing pending transmissions to finish which helps prevent prevent data loss
                _serverSocket.Shutdown(Net.Sockets.SocketShutdown.Both);
            }
            catch (Exception ex)
            {
                DebugMessage("Server put up a fight while shutting down.", DebugMessageType.WarningMessage, "StopServer", ex.Message);
            }

            //  kill the server socket, terminating any clients abrubtly (data loss may occur if Shutdown is unsuccessful)
            _serverSocket.Close();
        }

        class ProcessRequestObject
        {

            public byte[] requestBytes;

            public Server server;

            public Net.Sockets.Socket clientSocket;

            public Net.Sockets.SocketAsyncEventArgs e;
        }
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
            object requestString = Text.Encoding.ASCII.GetString(buffer);
            ParseRequestString((string)requestString);
        }

        // '' <summary>
        // '' Parses the raw request string received from the client socket
        // '' </summary>
        // '' <param name="requestString"></param>
        // '' <remarks></remarks>
        void ParseRequestString(string requestString)
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
            if ((IO.Directory.Exists(AbsoluteUrl) == true))
            {
                foreach (string d in _server.DefaultDocuments)
                {
                    if (IO.File.Exists((_server.WebRoot + ("/" + d))))
                    {
                        Uri += d;
                        AbsoluteUrl = (_server.WebRoot + Uri);
                        break;
                    }

                }

            }

            //  parse the requested resource's file type (extension) and path
            FileType = IO.Path.GetExtension(Uri).TrimStart('.');
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
            //  TODO: conditionally set Content-Length if needed - the header is not always necessary (e.g. when TransferMethod = ChunkedEncoding)
            IO.MemoryStream ms = new IO.MemoryStream();
            if (contentBytes != null)
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
                    //  no compression should be used on this resource, write the data as-is (uncompressed or already-compressed)
                    ms.Write(contentBytes, 0, contentBytes.Length);
                }

            }

            byte[] cbuf = null;
            //  create a buffer exactly the size of the memorystream length (not its buffer length)
            byte[] mbuf = ms.GetBuffer();
            ms.Close();
            ms.Dispose();
            Buffer.BlockCopy(mbuf, 0, cbuf, 0, cbuf.Length);
            ContentLength = cbuf.GetLength(0).ToString(); //???
            _content = cbuf;
        }

        void SetContent(string contentString)
        {
            //  just pass the string as bytes to the primary SetContent method
            SetContent(Text.Encoding.ASCII.GetBytes(contentString));
        }

        void SetHeader(string key, string value)
        {
            //  TODO: this seems to be trying to set the Reponse Headers several times per page load... e.g. _headers.Add(key, key & value) says item already exists
            // _Headers[key) = key & ": " & value
            _headers[key] = value;
        }

        string GetHeaderString()
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
            IO.MemoryStream ms = new IO.MemoryStream();
            //  get the header bytes and add it to the response
            byte[] headerBytes = Text.Encoding.ASCII.GetBytes(GetHeaderString());
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
        string StatusCodeMessage()
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

        void WriteHeader(object statusCode)
        {
            //  TODO: write the full header to the client stream
        }

        void Send(int statusCode, string message)
        {
        }

        void SendFile(int statusCode, byte[] file)
        {
        }

        void Finish()
        {
            //  TODO: finish the response cycle by flushing buffered data in the client stream
        }
    }

    // '' <summary>
    // '' A shared buffer which the SocketAsyncEventArgsPool utilizes to read/write data without memory thrashing/fragmentation.
    // '' </summary>
    // '' <remarks></remarks>
    class BufferManager
    {

        private int m_numBytes;

        private byte[] m_buffer;

        private Stack<int> m_freeIndexPool;

        private int m_currentIndex;

        private int m_bufferSize;

        public BufferManager(int totalBytes, int bufferSize)
        {
            m_numBytes = totalBytes;
            m_currentIndex = 0;
            m_bufferSize = bufferSize;
            m_freeIndexPool = new Stack<int>();
        }

        //  Allocates buffer space used by the buffer pool 
        public void InitBuffer()
        {
            //  create one big large buffer and divide that  
            //  out to each SocketAsyncEventArg object
            m_buffer = new byte[] {
                    (byte)(m_numBytes - 1)};
        }

        //  Assigns a buffer from the buffer pool to thespecified SocketAsyncEventArgs object returns true if the buffer was successfully set, else false.
        public bool SetBuffer(Net.Sockets.SocketAsyncEventArgs args)
        {
            if ((m_freeIndexPool.Count > 0))
            {
                args.SetBuffer(m_buffer, m_freeIndexPool.Pop(), m_bufferSize);
            }
            else
            {
                if (((m_numBytes - m_bufferSize)
                            < m_currentIndex))
                {
                    return false;
                }

                args.SetBuffer(m_buffer, m_currentIndex, m_bufferSize);
                m_currentIndex = (m_currentIndex + m_bufferSize);
            }

            return true;
        }

        //  Removes the buffer from a SocketAsyncEventArg object. This frees the buffer back to the buffer pool.
        public void FreeBuffer(Net.Sockets.SocketAsyncEventArgs args)
        {
            m_freeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }
    }

    // '' <summary>
    // '' A client state object representing a single client.
    // '' </summary>
    // '' <remarks></remarks>
    public class AsyncUserToken
    {

        private Net.Sockets.Socket _socket;

        public string Content;

        public AsyncUserToken()
        {
        }

        public AsyncUserToken(Net.Sockets.Socket socket)
        {
            _socket = socket;
        }

        public Net.Sockets.Socket Socket
        {
            get
            {
                return _socket;
            }
            set
            {
                _socket = value;
            }
        }
    }

    // '' <summary>
    // '' A pool of reusable SocketAsyncEventArgs objects.
    // '' </summary>
    // '' <remarks></remarks>
    class SocketAsyncEventArgsPool
    {

        private Stack<Net.Sockets.SocketAsyncEventArgs> m_pool;

        public SocketAsyncEventArgsPool(int capacity)
        {
            m_pool = new Stack<Net.Sockets.SocketAsyncEventArgs>(capacity);
        }

        //  Add a SocketAsyncEventArg instance to the pool 
        //  
        // The "item" parameter is the SocketAsyncEventArgs instance 
        //  to add to the pool 
        public void Push(Net.Sockets.SocketAsyncEventArgs item)
        {
            if ((item == null))
            {
                throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null");
            }

            lock(m_pool)
                m_pool.Push(item);
        }


        //  The number of SocketAsyncEventArgs instances in the pool 
        public int Count
        {
            get
            {
                return m_pool.Count;
            }
        }
    }
}