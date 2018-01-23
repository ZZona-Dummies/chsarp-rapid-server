using System.Diagnostics;

namespace RapidServer.Http
{
    // '' <summary>
    // '' A CGI handler for .php scripts.
    // '' </summary>
    // '' <remarks></remarks>
    public class PhpCgiHandler : Handler
    {
        public PhpCgiHandler()
        {
            Name = "PhpCgi";
        }

        public override string HandleRequest(Type1.Request req)
        {
            string results;
            //  create the php process:
            Process p = new Process();
            p.StartInfo.FileName = ExecutablePath;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            string host = (string)req.Headers["Host"];
            string[] hostParts = host.Split(':');
            p.StartInfo.EnvironmentVariables.Add("SCRIPT_FILENAME", req.AbsPath);
            p.StartInfo.EnvironmentVariables.Add("QUERY_STRING", req.QueryString.TrimStart('?'));
            p.StartInfo.EnvironmentVariables.Add("REQUEST_METHOD", req.Method);
            p.StartInfo.EnvironmentVariables.Add("REQUEST_URI", req.Uri);
            p.StartInfo.EnvironmentVariables.Add("HTTP_HOST", host);
            p.StartInfo.EnvironmentVariables.Add("SERVER_SOFTWARE", "Rapid Server");
            p.StartInfo.EnvironmentVariables.Add("SERVER_ADDR", hostParts[0]);
            //  set the SERVER_PORT if it's available:
            if (hostParts.Length > 1)
            {
                p.StartInfo.EnvironmentVariables.Add("SERVER_PORT", hostParts[1]);
            }

            //  make sure we pass cookies:
            if (req.Headers.ContainsKey("Cookie"))
            {
                p.StartInfo.EnvironmentVariables.Add("HTTP_COOKIE", (string)req.Headers["Cookie"]);
            }

            //  if its an http post, set the content_length and content_type headers:
            if (req.Method == "POST")
            {
                p.StartInfo.EnvironmentVariables.Add("CONTENT_LENGTH", req.ContentLength);
                p.StartInfo.EnvironmentVariables.Add("CONTENT_TYPE", "application/x-www-form-urlencoded");
            }

            //  these are a "must" according to CGI spec:
            p.StartInfo.EnvironmentVariables.Add("SCRIPT_NAME", req.ScriptName);
            // req.Uri & req.ScriptName)
            p.StartInfo.EnvironmentVariables.Add("GATEWAY_INTERFACE", "CGI/1.1");
            p.StartInfo.EnvironmentVariables.Add("SERVER_NAME", hostParts[0]);
            p.StartInfo.EnvironmentVariables.Add("SERVER_PROTOCOL", "HTTP/1.1");
            p.StartInfo.EnvironmentVariables.Add("REMOTE_ADDR", hostParts[0]);
            //  start the php process:
            p.Start();
            //  if its an http post, write the post data (aka querystring) to the input stream (aka stdin):
            if (req.Method == "POST")
            {
                p.StandardInput.Write(req.ContentString);
                p.StandardInput.Flush();
                p.StandardInput.Close();
            }

            //  read the output stream (aka stdout) to get the processed request from php:
            //  TODO: ReadToEnd is thread blocking, we can use the async BeginOutputReadLine() function, but we must remember that we're
            //    spawning a separate php-cgi.exe process (thus new thread) for every php request; I benchmarked them and found no
            //    performance difference, no need to over-optimize this yet when our real goal is implementing a FastCGI handler...
            results = p.StandardOutput.ReadToEnd();
            //  close the process (not really needed - garbage collector takes care of it)
            p.Close();
            p.Dispose();
            //  return the processed request to the calling function where we'll continue to handle it:
            return results;
        }
    }
}