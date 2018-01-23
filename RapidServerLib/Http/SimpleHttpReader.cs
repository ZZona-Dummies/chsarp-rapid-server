namespace RapidServer.Http
{
    // '' <summary>
    // '' A simple http header is just an object which contains the key and value of the header. Used for cookies, since there might be multiple cookies and we can't add multiple items with the same key to a hashtable.
    // '' </summary>
    // '' <remarks></remarks>
    public class SimpleHttpHeader
    {
        public string Key;

        public string Value;
    }
}