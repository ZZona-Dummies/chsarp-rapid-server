using System.Diagnostics;
using System.Text;

namespace RapidServerClientApp.Classes
{
    internal class ManagedProcess
    {
        public Process Process = new Process();

        public StringBuilder Output = new StringBuilder();

        private ManagedProcess()
        { }

        public ManagedProcess(string filename, string commandline)
        {
            //  use a process to run the benchmark tool and read its results
            //string results = "";
            Process p = Process;
            p.OutputDataReceived += ReadOutputAsync;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.FileName = filename;
            p.StartInfo.Arguments = commandline;
            try
            {
                p.Start();
                p.BeginOutputReadLine();
                //  TODO: siege -c1000 causes a hang with WaitForExit() and no timeout...
                p.WaitForExit();
                // p.Close()
                // p.Dispose()
            }
            catch //(Exception ex)
            {
                Output.Append("the tool process failed to run");
            }
        }

        private void ReadOutputAsync(object sender, DataReceivedEventArgs e)
        {
            Output.AppendLine(e.Data);
        }
    }
}