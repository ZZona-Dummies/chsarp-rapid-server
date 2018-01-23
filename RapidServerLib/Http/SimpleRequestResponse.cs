using System;
using System.Collections;
using System.Text;

namespace RapidServer.Http
{
    // '' <summary>
    // '' A simple object for parsing raw http data into strings, inherited by Request and Response classes
    // '' </summary>
    // '' <remarks></remarks>
    public class SimpleRequestResponse
    {
        public Hashtable Headers = new Hashtable();

        public ArrayList Cookies = new ArrayList();

        public byte[] Content;

        public string ErrorMessage = "";

        public string HeaderString = "";

        public string ContentString = "";

        public string ContentStringLength = "";

        public bool CacheAllowed = true;

        public void Parse(string payload)
        {
            //  parse the responseString into header/content parts
            // Dim headerPart As String = ""
            StringBuilder headerPart = new StringBuilder();
            string contentPart = "";
            try
            {
                int i = payload.IndexOf(Environment.NewLine + Environment.NewLine, StringComparison.Ordinal);
                //  TODO: this is very slow, use a string builder's substring instead
                // headerPart = payload.Substring(0, i).Trim
                headerPart.Append(payload, 0, i);
                contentPart = payload.Substring((i + 4), (payload.Length
                                - (i - 4)));
                HeaderString = headerPart.ToString();
                ContentString = contentPart;
                //  TODO: also parse headers from Request Body (for certain POST requests)
                ContentStringLength = contentPart.Length.ToString();
                Content = Encoding.ASCII.GetBytes(ContentString);
            }
            catch (Exception ex)
            {
                //  couldn't parse the responseString as expected, probably an error
                // ErrorMessage = "Could not parse responseString for new SimpleResponse. responseString: " & responseString
                ErrorMessage = payload;
                Content = Encoding.ASCII.GetBytes(ErrorMessage);
            }

            //  parse the header string
            string[] headerStringParts = HeaderString.Split('\n');
            foreach (string header in headerStringParts)
            {
                string headerClean = header.Trim();
                if ((headerClean != ""))
                {
                    if (headerClean.Contains(":"))
                    {
                        int pos = headerClean.IndexOf(":");
                        string[] headerParts = SplitFirst(headerClean, ":", true);
                        //  there can be multiple cookies, can't store them raw in our hashtable so use an arraylist and merge both before sending the response
                        if ((headerParts[0].ToLower() == "set-cookie"))
                        {
                            SimpleHttpHeader h = new SimpleHttpHeader();
                            h.Key = "Set-Cookie";
                            h.Value = headerParts[1];
                            Cookies.Add(h);
                        }

                        if ((headerParts[0].ToLower() == "cache-control"))
                        {
                            if ((headerParts[1].ToLower() == "no-cache"))
                            {
                                CacheAllowed = false;
                            }
                        }

                        //  only set the same header once
                        if ((Headers.ContainsKey(headerParts[0]) == false))
                        {
                            Headers.Add(headerParts[0], headerParts[1]);
                        }
                    }
                }
            }
        }
    }
}