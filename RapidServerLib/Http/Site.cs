using System.Net.Sockets;

namespace RapidServer.Http
{
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

        public Socket Socket;

        public string Role;

        public string Upstream;
    }
}