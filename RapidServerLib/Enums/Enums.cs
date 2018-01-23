namespace RapidSever.Enums
{
    // '' <summary>
    // '' The type of event to be logged. Some events will be enabled/disabled in the LogEvent method, depending on our current debugging goals.
    // '' </summary>
    // '' <remarks></remarks>
    public enum DebugMessageType
    {
        InfoMessage = 0,

        WarningMessage = 1,

        ErrorMessage = 2,

        UsageMessage = 3,

        UnhandledMessage = 4,
    }

    // '' <summary>
    // '' Types of compression methods.
    // '' </summary>
    // '' <remarks></remarks>
    public enum CompressionMethod
    {
        None = 0,

        Gzip = 1,

        Deflate = 2,
    }

    // '' <summary>
    // '' The transfer method used for sending data to a client.
    // '' </summary>
    // '' <remarks></remarks>
    public enum TransferMethod
    {
        StoreAndForward = 0,

        //  HTTP/1.0 - offers the "Connection: close" header, but generally HTTP/1.0 does not support persistent connections so we should always close the socket whether or not this header is present. Content-Length can be omitted as long as the socket is always closed after the response has been sent.
        ChunkedEncoding = 1,
    }
}