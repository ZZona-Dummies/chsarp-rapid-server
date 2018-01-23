namespace RapidServer.Http
{
    // '' <summary>
    // '' Global variables used throughout the library.
    // '' TODO: since the server and client apps use their own instance of the library, when the server sets these values,
    // '' the client does not see them but tries to use them anyway, which breaks the client functionality, move this into
    // '' the server and create another for the client
    // '' </summary>
    // '' <remarks></remarks>
    /*public class Module1
    {
    }*/
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