namespace RapidServer.Http
{
    public class Handler
    {
        public string Name;

        public string ExecutablePath;

        public virtual string HandleRequest(Type1.Request req)
        {
            return null;
        }
    }
}