using System;
using static RapidServer.Globals;
using Net = System.Net;

namespace RapidServer.Http.Type1
{

    // '' <summary>
    // '' An HTTP client.
    // '' </summary>
    // '' <remarks></remarks>
    public class Client
    {

        public int SendBufferSize = 4096;

        public int ReceiveBufferSize = 4096;

        private Net.Sockets.Socket _clientSocket;

        private string _request;

        private UriBuilder _req;

        private bool _keepAlive;

        event EventHandler HandleResponse;

        private string res;

        private object state;

        event EventHandler ConnectSucceeded;

        event EventHandler ConnectFailed;

        event EventHandler LogMessage;

        private string message;

        Client(bool keepAlive)
        {
            _keepAlive = keepAlive;
        }

        // '' <summary>
        // '' Issues an HTTP GET request to the URL specified in the textbox.
        // '' </summary>
        // '' <param name="url"></param>
        // '' <remarks></remarks>
        void Go(string url, object state)
        {
            //  use uribuilder to format the url:
            Connect(new UriBuilder(url), state);
        }

        string GetHostIP(UriBuilder uri)
        {
            string hostAddress = "";
            System.Net.IPAddress ipExists = null;
            if (System.Net.IPAddress.TryParse(uri.Host, out ipExists))
            {
                //  localhost
                hostAddress = uri.Host;
                _request = uri.Path;
            }
            else
            {
                //  TODO: this could halt with an error if the host doesn't exist (we should return name_not_resolved)
                System.Net.IPHostEntry hostEntry;
                hostEntry = System.Net.Dns.GetHostEntry(uri.Host);
                foreach (System.Net.IPAddress ip in hostEntry.AddressList)
                {
                    hostAddress = ip.ToString();
                }

            }

            return hostAddress;
        }

        void Connect(UriBuilder req, object state)
        {
            //  store the request in a global so we can use it during async callbacks
            _req = req;
            //  extract ip address from _req or _req Url/Host
            string ip = GetHostIP(req);
            int port = req.Port;
            //  create endpoint
            Net.IPEndPoint endPoint = AddressToEndpoint(ip, port);
            _clientSocket = new Net.Sockets.Socket(Net.Sockets.AddressFamily.InterNetwork, Net.Sockets.SocketType.Stream, Net.Sockets.ProtocolType.Tcp);
            //  connect to server async
            try
            {
                _clientSocket.BeginConnect(endPoint, new AsyncCallback(new System.EventHandler(this.AsyncClientConnected)), new AsyncSendState(_clientSocket, SendBufferSize, state));
            }
            catch (Exception ex)
            {
                DebugMessage("Could not connect to server.", DebugMessageType.ErrorMessage, "Connect", ex.Message);
                ConnectFailed(state, null);
            }

        }

        void AsyncClientConnected(IAsyncResult ar)
        {
            //  get the async state object returned by the callback
            AsyncSendState asyncState = ((AsyncSendState)(ar.AsyncState));
            object state = asyncState.State;
            //  end the async connection request so we can check if we are connected to the server
            bool connectSuccessful = false;
            try
            {
                //  call the EndConnect method which will succeed or throw an error depending on the result of the connection
                asyncState.Socket.EndConnect(ar);
                //  at this point, the EndConnect succeeded and we are connected to the server! handle the success outside this Try block.
                connectSuccessful = true;
                ConnectSucceeded(state, null);
            }
            catch (Exception ex)
            {
                //  at this point, the EndConnect failed and we are NOT connected to the server!
                DebugMessage("Could not connect to the server.", DebugMessageType.ErrorMessage, "Connect", ex.Message);
                ConnectFailed(state, null);
            }

            //  if the client has connected, proceed
            if ((connectSuccessful == true))
            {
                //  start waiting for messages from the server
                AsyncReceiveState receiveState = new AsyncReceiveState(ReceiveBufferSize, state);
                receiveState.Socket = asyncState.Socket;
                receiveState.Socket.BeginReceive(receiveState.Buffer, 0, ReceiveBufferSize, Net.Sockets.SocketFlags.None, new AsyncCallback(new System.EventHandler(this.DataReceived)), receiveState);
                //  make a request to the server
                AsyncSendState sendState = new AsyncSendState(asyncState.Socket, SendBufferSize, state);
                //  if the path is a directory, ensure it has a trailing /
                // If IO.Path.GetExtension(_request) = "" Then
                //     If _request.EndsWith("/") = False Then
                //         _request &= "/"
                //     End If
                // End If
                //  construct the GET request string and byte array:
                //  NOTE: node.js requires two vbCrLf terminator where other servers only require one. IIS 7.5 requires HTTP/1.1 and Host 
                //    header or will not return headers with the response.
                string reqString = "";
                byte[] reqBytes = null;
                reqString = ("GET "
                            + (_req.Path + (" HTTP/1.1" + ("\r\n" + ("Host: "
                            + (_req.Host + ("\r\n" + "\r\n")))))));
                reqBytes = System.Text.Encoding.ASCII.GetBytes(reqString);
                //  send the reqBytes data to the server
                LogMessage(reqString);
                sendState.Socket.BeginSend(reqBytes, 0, reqBytes.Length, Net.Sockets.SocketFlags.None, new AsyncCallback(new System.EventHandler(this.DataSent)), sendState);
            }

        }

        void Disconnect()
        {
            _clientSocket.Disconnect(false);
        }

        void DataSent(IAsyncResult ar)
        {
            //  get the async state object returned by the callback
            AsyncSendState asyncState = ((AsyncSendState)(ar.AsyncState));
            try
            {
                asyncState.Socket.EndSend(ar);
            }
            catch (Exception ex)
            {
                Console.WriteLine(("DataSent exception: " + ex.Message));
            }

        }

        // '' <summary>
        // '' This callback fires when the client socket receives data (an HTTP response) asynchronously from the server.
        // '' </summary>
        // '' NOTE:
        // ''     Since HTTP is by nature a stream, a response will be sent by the server which is broken down into pieces per the
        // ''     server's configured SendBufferSize, so we must continue issuing BeginReceive's on the socket until the end of the stream. 
        // ''     We can properly detect the end of stream per the HTTP spec which states that the Content-Length header should be used to stop 
        // ''     issuiing BeginReceive's when the total bytes received equals the Header + Content length, or in the case of a 
        // ''     "Transfer-Encoding: chunked" header we look for a null character which signals termination of the chunked stream.
        // '' <param name="ar"></param>
        // '' <remarks></remarks>
        void DataReceived(IAsyncResult ar)
        {
            //  get the async state object returned by the callback
            AsyncReceiveState asyncState = ((AsyncReceiveState)(ar.AsyncState));
            string responseChunk = System.Text.Encoding.ASCII.GetString(asyncState.Buffer).TrimEnd(vbNullChar);
            string responseString = (asyncState.Packet + responseChunk);
            //  if we haven't determined the Content-Length yet, try doing so now by attempting to extract it from the responseChunk:
            //  TODO: this halts on an error when we try a random URL.
            //  TODO: we need to handle the various transfer types here...check for chunked encoding and parse the size etc...
            if ((asyncState.ReceiveSize == 0))
            {
                string contentLength = "";
                string transferEncoding = "";
                contentLength = responseChunk.SubstringEx("Content-Length: ", "\r\n");
                asyncState.ReceiveSize = contentLength;
            }

            //  if we haven't determined the Content offset yet, try doing so now. content is located after the header and two newlines (crlf) which is 4 bytes.
            if ((asyncState.ContentOffset == 0))
            {
                int contentOffset;
                contentOffset = (responseChunk.IndexOf(("\r\n" + "\r\n")) + 4);
                asyncState.ContentOffset = contentOffset;
            }

            //  add the responseChunk's length to the total received bytes count:
            asyncState.TotalBytesReceived = (asyncState.TotalBytesReceived + responseChunk.Length);
            //  if we haven't received all the bytes yet, issue another BeginReceive, otherwise we have all the data so we handle the response
            if (((asyncState.TotalBytesReceived - asyncState.ContentOffset)
                        < asyncState.ReceiveSize))
            {
                AsyncReceiveState receiveState = new AsyncReceiveState(ReceiveBufferSize, asyncState.State);
                receiveState.Socket = asyncState.Socket;
                receiveState.Packet = responseString;
                receiveState.ReceiveSize = asyncState.ReceiveSize;
                receiveState.TotalBytesReceived = asyncState.TotalBytesReceived;
                receiveState.Socket.BeginReceive(receiveState.Buffer, 0, ReceiveBufferSize, Net.Sockets.SocketFlags.None, new AsyncCallback(new System.EventHandler(this.DataReceived)), receiveState);
            }
            else
            {
                HandleResponse(responseString, asyncState.State);
            }

        }
    }
}