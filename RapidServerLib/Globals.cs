//  ==================================
//  RAPID SERVER
//  ==================================
//  A client/server communications library for building or serving high performance network applications, websites and webapps.
// TODO: Option Explicit On ... Warning!!! not translated
// TODO: Option Strict On ... Warning!!! not translated

using RapidServerLib.Enums;
using System;
using System.Diagnostics;
using IO = System.IO;
using Net = System.Net;
using Xml = System.Xml;

namespace RapidServer
{
    public class Globals
    {
        // '' <summary>
        // '' Defines an http mimetype.
        // '' </summary>
        // '' <remarks></remarks>
        public class MimeType
        {
            public string Name = "";

            public string FileExtension = "";

            public CompressionMethod Compress;

            //  e.g. none | gzip | deflate
            public string Expires = "";

            public string Handler = "";
        }

        // '' <summary>
        // '' Kills all running processes matching the procName.
        // '' </summary>
        // '' <param name="procName"></param>
        // '' <remarks></remarks>
        public static void KillAll(string procName)
        {
            // Process.Start("taskkill.exe /f /im " & procName)
            //  terminate all running php-cgi.exe FastCGI daemons
            Process TaskKill = new Process();
            TaskKill.StartInfo.CreateNoWindow = true;
            TaskKill.StartInfo.UseShellExecute = false;
            TaskKill.StartInfo.FileName = "taskkill.exe";
            TaskKill.StartInfo.Arguments = ("/F /IM " + procName);
            TaskKill.Start();
            //  wait for the process to finish, otherwise if we try to start a new instance before TaskKill has spun up/down it will kill our new instance too
            TaskKill.WaitForExit();
        }

        public static Array SplitFirst(string input, string delimiter, bool trim)
        {
            string[] spl;
            string[] parts = new string[2];
            spl = input.Split(delimiter[0]);
            if ((trim == true))
            {
                for (int i = 0; i <= spl.Length - 1; i++)
                    spl[i] = spl[i].Trim();
            }

            parts[0] = spl[0];
            parts[1] = string.Join(delimiter, spl, 1, (spl.Length - 1));
            return parts;
        }

        // '' <summary>
        // '' Converts the given ip address and port into an endpoint that can be used with a socket.
        // '' </summary>
        // '' <param name="IP"></param>
        // '' <param name="Port"></param>
        // '' <returns></returns>
        // '' <remarks></remarks>
        public static Net.IPEndPoint AddressToEndpoint(string IP, int Port)
        {
            Net.IPAddress ipAddress = Net.IPAddress.Parse(IP);
            Net.IPEndPoint endPoint = new Net.IPEndPoint(ipAddress, Port);
            return endPoint;
        }

        public class AsyncReceiveState
        {
            public RapidServer.Http.Site Site;

            public Net.Sockets.Socket Socket;

            public byte[] Buffer;

            public IO.MemoryStream PacketBufferStream = new IO.MemoryStream();

            //  a buffer for appending received data to build the packet
            public string Packet;

            public int ReceiveSize;

            //  the size (in bytes) of the Packet
            public int TotalBytesReceived;

            //  the total bytes received for the Packet so far
            public int ContentOffset;

            public object State;

            private AsyncReceiveState()
            { }

            public AsyncReceiveState(int argBufferSize, object argState)
            {
                //object Buffer;
                State = argState;
            }
        }

        public class AsyncSendState
        {
            public Net.Sockets.Socket Socket;

            public byte[] BytesToSend;

            public int Progress;

            public string Tag;

            public bool Persistent;

            public int BufferSize;

            public object State;

            private AsyncSendState()
            { }

            public AsyncSendState(Net.Sockets.Socket argSocket, int argBufferSize, object argState)
            {
                Socket = argSocket;
                BufferSize = argBufferSize;
                State = argState;
            }

            public int NextOffset()
            {
                return Progress;
            }

            public int NextLength()
            {
                if (BytesToSend.Length - Progress > BufferSize)
                {
                    return BufferSize;
                }
                else
                {
                    return (BytesToSend.Length - Progress);
                }
            }
        }

        // '' <summary>
        // '' Extends the XmlNode class to return an empty string instead of null.
        // '' </summary>
        // '' <param name="x"></param>
        // '' <returns></returns>
        // '' <remarks></remarks>
        //[System.Runtime.CompilerServices.Extension()]
        public static string GetValue(Xml.XmlNode x)
        {
            if (x == null)
                return "";
            else
                return x.InnerText;
        }

        // '' <summary>
        // '' Extends the String class.
        // '' </summary>
        // '' <param name="s"></param>
        // '' <returns></returns>
        // '' <remarks></remarks>
        //[System.Runtime.CompilerServices.Extension()]
        public static string[] Slice(string s, string firstOccurrenceOf)
        {
            if ((firstOccurrenceOf == null))
                return null;
            else
            {
                string[] spl = s.Split(firstOccurrenceOf[0]);
                string firstSlice = spl[0];
                string secondSlice = "";
                for (int i = 1; i <= spl.Length - 1; i++)
                {
                    secondSlice += "/" + spl[i];
                }

                return new string[] {
                    firstSlice,
                    secondSlice};
            }
        }

        // '' <summary>
        // '' Extends the String class.
        // '' </summary>
        // '' <param name="s"></param>
        // '' <returns></returns>
        // '' <remarks></remarks>
        //[System.Runtime.CompilerServices.Extension()]
        public static string SubstringEx(string s, string s1, string s2)
        {
            string ret = "";
            int i1;
            int i2;
            i1 = (s.IndexOf(s1) + s1.Length);
            i2 = s.IndexOf(s2, i1);
            if ((s.Contains(s1) == true))
            {
                ret = s.Substring(i1, (i2 - i1));
            }

            return ret;
        }

        // '' <summary>
        // '' Logs the event to a console or file. Rather than use Console.WriteLine throughout the library we call LogEvent instead, which can be enabled or disabled in one step. Since Console.WriteLine is useful for debugging but also performance-heavy, we need the ability to enable/disable it at will.
        // '' </summary>
        // '' <param name="message"></param>
        // '' <remarks></remarks>
        public static void DebugMessage(string message, DebugMessageType level = DebugMessageType.InfoMessage, string caller = "", string internalException = "")
        {
            //  TODO: implement the caller, for logging purposes and easy bug reporting
            // Warning!!! Optional parameters not supported
            // Warning!!! Optional parameters not supported
            // Warning!!! Optional parameters not supported
            //  TODO: this can slow down the server dramatically
            switch (level)
            {
                case DebugMessageType.InfoMessage:
                    Console.WriteLine(message);
                    break;

                case DebugMessageType.WarningMessage:
                    Console.WriteLine(message);
                    break;

                case DebugMessageType.ErrorMessage:
                    Console.WriteLine(message);
                    break;

                case DebugMessageType.UsageMessage:
                    // Console.WriteLine(message)
                    break;
            }
        }
    }
}