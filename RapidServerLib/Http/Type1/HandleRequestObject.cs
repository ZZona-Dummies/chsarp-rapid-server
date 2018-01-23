using System.Net.Sockets;

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

        public Socket clientSocket;

        public Site site;
    }
}