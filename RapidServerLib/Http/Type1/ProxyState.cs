using System.Net.Sockets;

namespace RapidServer.Http.Type1
{
    internal class ProxyState
    {
        public Request req;

        public Socket client;
    }
}