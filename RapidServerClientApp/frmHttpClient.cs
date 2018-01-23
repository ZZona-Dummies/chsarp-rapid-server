using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Xml = System.Xml;
using IO = System.IO;
using Text = System.Text;
using Management = System.Management;
using RapidServer.Http.Type1;
using System.Management;
using System.Collections.Generic;

namespace RapidServerClientApp
{
    public partial class frmHttpClient : Form
    {
        public frmHttpClient()
        {
            InitializeComponent();
        }

        private Client client = new Client(false);

        public Hashtable Sites = new Hashtable();

        public Hashtable Tools = new Hashtable();

        public string stdout;

        public delegate void HandleResponseDelegate(string res);

        public delegate void ConnectFailedDelegate();

        public delegate void LogMessageDelegate(string message);

        private void client_ConnectFailed()
        {
            Invoke(new ConnectFailedDelegate(ConnectFailed));
        }

        private void client_HandleResponse(string res, object state)
        {
            Invoke(new HandleResponseDelegate(HandleResponse), res);
        }

        private void client_LogMessage(string message)
        {
            Invoke(new LogMessageDelegate(LogMessage), message);
        }

        //  form load
        private void frmHttpClient_Load(object sender, EventArgs e)
        {
            //  load the config
            LoadConfig();
            //  add the sites
            foreach (Site s in Sites.Values)
            {
                cboUrl.Items.Add(s.Url);
            }

            cboUrl.SelectedItem = cboUrl.Items[0];
            //  add the tools
            foreach (Tool t in Tools.Values)
            {
                cboBenchmarkTool.Items.Add(t.Name);
                cboBenchmarkTool2.Items.Add(t.Name);
                cboBenchmarkTool3.Items.Add(t.Name);
            }

            cboBenchmarkTool.SelectedItem = cboBenchmarkTool.Items[0];
            cboBenchmarkTool2.SelectedItem = cboBenchmarkTool2.Items[0];
            cboBenchmarkTool3.SelectedItem = cboBenchmarkTool3.Items[0];
        }

        // '' <summary>
        // '' Loads the server config file http.xml from disk and configures the server to operate as defined by the config.
        // '' </summary>
        // '' <remarks></remarks>
        void LoadConfig()
        {
            //  TODO: Xml functions are very picky after load, if we try to access a key that doesn't exist it will throw a 
            //    vague error that does not stop the debugger on the error line, and the innerexception states 'object reference 
            //    not set to an instance of an object'. a custom function GetValue() helps avoid nulls but not this. default values should
            //    be assumed by the server for cases when the value can't be loaded from the config, or the server should regenerate the config 
            //    per its known format and then load it.
            if ((IO.File.Exists("client.xml") == false))
            {
                CreateConfig();
            }

            Xml.XmlDocument cfg = new Xml.XmlDocument();
            try
            {
                cfg.Load("client.xml");
            }
            catch (Exception ex)
            {
                //  TODO: we need to notify the user that the config couldn't be loaded instead of just dying...
                Console.WriteLine(ex.Message);
                return;
            }

            Xml.XmlNode root = cfg["Settings"];
            //  parse the sites:
            foreach (Xml.XmlNode n in root["Sites"])
            {
                Site s = new Site();
                s.Name = n["Name"].Value;
                s.Description = n["Description"].Value;
                s.Url = n["Url"].Value;
                Sites.Add(s.Name, s);
            }

            //  parse the tools:
            foreach (Xml.XmlNode n in root["Tools"])
            {
                Tool t = new Tool();
                t.Name = n["Name"].Value;
                t.Path = n["Path"].Value;
                t.Speed = n["Speed"].Value;
                t.Time = n["Time"].Value;
                foreach (Xml.XmlNode nn in n["Data"])
                {
                    if ((nn.Name == "RPS"))
                    {
                        t.Data.RPS = nn.InnerText;
                    }
                    else if ((nn.Name == "CompletedRequests"))
                    {
                        t.Data.CompletedRequests = nn.InnerText;
                    }
                    else if ((nn.Name == "ResponseTime"))
                    {
                        t.Data.ResponseTime = nn.InnerText;
                    }

                }

                Tools.Add(t.Name, t);
            }

        }

        void CreateConfig()
        {
        }

        void DetectSystemInfo()
        {
            if ((Chart1.Titles.Count == 1))
            {
                //  get os
                ManagementObjectSearcher wmios = new ManagementObjectSearcher("SELECT * FROM  Win32_OperatingSystem");
                ManagementObject os = wmios.Get().OfType<ManagementObject>().First();
                string osName = (string)os["Name"];
                if (osName.Contains("Windows 7"))
                    osName = "Win7";

                osName = osName + " " + ((string)os["OSArchitecture"]).Trim();
                //  get cpu
                ManagementObjectSearcher wmicpu = new ManagementObjectSearcher("SELECT * FROM  Win32_Processor");
                ManagementObject cpu = wmicpu.Get().OfType<ManagementObject>().First();
                string cpuName = (string)cpu["Name"];
                cpuName = cpuName.Replace("(R)", "").Replace("(r)", "").Replace("(TM)", "").Replace("(tm)", "").Replace("CPU ", "").Trim();
                //  get ram
                ManagementObjectSearcher wmiram = new ManagementObjectSearcher("SELECT * FROM  Win32_ComputerSystem");
                ManagementObject ram = wmiram.Get().OfType<ManagementObject>().First();
                int totalRam = ((int)ram["TotalPhysicalMemory"] / (1024 / (1024 / 1024)));
                //  print results to chart title
                Chart1.Titles.Add((osName + (" - "
                                + (cpuName + (" - "
                                + (totalRam + "GB"))))));
                Chart3.Titles.Add((osName + (" - "
                                + (cpuName + (" - "
                                + (totalRam + "GB"))))));
                Chart2.Titles.Add((osName + (" - "
                                + (cpuName + (" - "
                                + (totalRam + "GB"))))));
            }

        }

        //  runs the benchmark tool with selected parameters
        void RunBenchmark()
        {
            if ((TabControl3.SelectedTab.Text == "Speed"))
            {
                SpeedBenchmark();
            }
            else if ((TabControl3.SelectedTab.Text == "Time"))
            {
                TimeBenchmark();
            }
            else
            {

            }

        }

        //  TODO: this gets an unhandled exception when it tries to parse data that doesn't exist, we shouldn't assume 
        //    we'll always have the data and use a try...catch here with error reporting
        private string SubstringBetween(string s, string startTag, string endTag)
        {
            try
            {
                string ss = "";
                int i;
                if ((startTag.ToLower() == Environment.NewLine))
                {
                    startTag = Environment.NewLine;
                }

                if ((endTag.ToLower() == Environment.NewLine))
                {
                    endTag = Environment.NewLine;
                }

                i = s.IndexOf(startTag);
                ss = s.Substring((i + startTag.Length));
                i = ss.IndexOf(endTag);
                ss = ss.Substring(0, i);
                return ss.Trim();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }

        }

        private string[] ParseAny(string s)
        {
            if ((s == ""))
            {
                return new string[] {
                    "FAIL WHALE!"};
            }

            string[] results = null;
            string[] spl = s.Split(',');
            if ((spl[0] == "stdout"))
            {
                //  data is in stdout
                if ((spl[1] == "between"))
                    results[0] = SubstringBetween(stdout, spl[2], spl[3]);
            }
            else
            {
                //  data is in a file
                IO.StreamReader f = new IO.StreamReader(spl[0]);
                char delim = Convert.ToChar(0);
                if ((spl[1] == "tabs"))
                    delim = '\t';
                else
                    delim = ',';

                //  determine if the tool data contains a formula
                string[] formula = null;
                bool useFormula = false;
                if (spl[3].Contains("+"))
                {
                    //  the data we want requires a formula rather than a single value
                    formula = spl[3].Split('+');
                    useFormula = true;
                }

                //  TODO: check if we should read the file as rows or as summary...
                //  read the file as rows
                List<string> lines = new List<string>();
                while ((f.Peek() != -1))
                    lines.Add(f.ReadLine());

                f.Close();
                f.Dispose();
                //  filter the rows
                if ((spl[2] == "first"))
                {
                    //  grab the first row only
                    for (int i = 0; (i
                                <= (lines.Count - 1)); i++)
                    {
                        lines.RemoveAt(1);
                    }

                    string line = lines[0];
                    string[] fields = line.Split(delim);
                    if ((useFormula == true))
                    {
                        int val = 0;
                        for (int i = 0; (i
                                    <= (formula.Length - 1)); i++)
                        {
                            val = val + int.Parse(fields[int.Parse(formula[i])]);
                        }

                        results[0] = val.ToString();
                    }
                    else
                    {
                        results[0] = fields[int.Parse(spl[3])].Trim();
                    }

                }
                else if ((spl[2] == "last"))
                {
                    //  grab the last row only
                    for (int i = 0; (i
                                <= (lines.Count - 2)); i++)
                    {
                        lines.RemoveAt(0);
                    }

                    string line = lines[0];
                    string[] fields = line.Split(delim);
                    if ((useFormula == true))
                    {
                        int val = 0;
                        for (int i = 0; (i
                                    <= (formula.Length - 1)); i++)
                        {
                            val = val + int.Parse(fields[int.Parse(formula[i])]);
                        }

                        results[0] = val.ToString();
                    }
                    else
                    {
                        results[0] = fields[int.Parse(spl[3])].Trim();
                    }

                }
                else
                {
                    //  grab all the rows after a specific row index
                    for (int i = 0; i <= int.Parse(spl[2]) - 1; i++)
                    {
                        lines.RemoveAt(i);
                    }

                    for (int i = 0; (i
                                <= (lines.Count - 1)); i++)
                    {
                        string line = lines[i];
                        string[] fields = line.Split(delim);
                        if ((useFormula == true))
                        {
                            int val = 0;
                            for (int ii = 0; (ii
                                        <= (formula.Length - 1)); ii++)
                            {
                                val = val + int.Parse(fields[int.Parse(formula[ii])]);
                            }

                            results[(results.Length - 1)] = val.ToString();
                        }
                        else
                            results[(results.Length - 1)] = fields[int.Parse(spl[3])].Trim();

                        Array.Resize(ref results, results.Length);
                    }

                }

            }

            return results;
        }

        void ParseResults(string results)
        {
            stdout = results;
            Tool currentTool = (Tool)Tools[cboBenchmarkTool.Text];
            //  parse it
            string[] requestsPerSecond = ParseAny(currentTool.Data.RPS);
            string[] completedRequests = ParseAny(currentTool.Data.CompletedRequests);
            // Dim time() As String = ParseAny(currentTool.Data.ResponseTime)
            //  chart it
            bool failedParse = false;
            if (((requestsPerSecond[0] == "")
                        || (completedRequests[0] == "")))
            {
                failedParse = true;
            }

            if ((failedParse == false))
            {
                Site currentSite = null;
                foreach (Site s in Sites.Values)
                {
                    if ((s.Url == cboUrl.Text))
                    {
                        currentSite = s;
                    }

                }

                string seriesName = cboUrl.Text;
                if (currentSite != null)
                {
                    seriesName = currentSite.Name;
                    //  update the rps log
                    //  TODO: remove sitename and match text color to legend color
                    TextBox1.AppendText((seriesName + (" - "
                                    + (requestsPerSecond[0] + '\n'))));
                    //  plot the requests completed value to the bar chart
                    if ((Chart1.Series.IndexOf(seriesName) == -1))
                    {
                        Chart1.Series.Add(seriesName);
                    }

                    Chart1.Series[seriesName].Points.AddXY(0, requestsPerSecond[0]);
                    //  plot the requests completed value to the bar chart
                    if ((Chart2.Series.IndexOf(seriesName) == -1))
                    {
                        Chart2.Series.Add(seriesName);
                    }

                    Chart2.Series[seriesName].Points.AddXY(0, completedRequests[0]);
                    //  plot the gnuplot data to the line graph
                    if ((Chart3.Series.IndexOf(seriesName) == -1))
                    {
                        //  create the series for this url
                        Chart3.Series.Add(seriesName);
                        Chart3.Series[seriesName].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
                    }
                    else
                    {
                        //  series was already plotted, clear the series and replot it
                        Chart3.Series[seriesName].Points.Clear();
                    }

                    // For Each s As String In time
                    //     Chart3.Series[seriesName).Points.AddXY(0, s)
                    // Next
                }
                else
                {
                    //  update the rps log
                    TextBox1.AppendText(("FAIL WHALE!" + '\n'));
                }

            }

        }

        void TimeBenchmark()
        {
            Tool t = (Tool)Tools[cboBenchmarkTool.Text];
            string cmd = t.Time;
            cmd = cmd.Replace("%time", txtBenchmarkDuration.Text);
            cmd = cmd.Replace("%url", (cboUrl.Text.TrimEnd('/') + "/"));
            LogMessage((t.Path + cmd));
            ManagedProcess p = new ManagedProcess(t.Path, cmd);
            txtRaw.Text = p.Output.ToString();
            ParseResults(p.Output.ToString());
        }

        void SpeedBenchmark()
        {
            Tool t = (Tool)Tools[cboBenchmarkTool.Text];
            string cmd = t.Speed;
            cmd = cmd.Replace("%num", txtBenchmarkNumber.Text);
            cmd = cmd.Replace("%conc", txtBenchmarkConcurrency.Text);
            cmd = cmd.Replace("%url", (cboUrl.Text.TrimEnd('/') + "/"));
            LogMessage((t.Path + cmd));
            ManagedProcess p = new ManagedProcess(t.Path, cmd);
            //  TODO: this throws an exception due to multithreading...
            //    https://stackoverflow.com/questions/24181910/stringbuilder-thread-safety?rq=1
            txtRaw.Text = p.Output.ToString();
            ParseResults(p.Output.ToString());
        }

        //  ramp by increasing concurrency each iteration: http://wiki.dreamhost.com/Web_Server_Performance_Comparison
        void RampBenchmark()
        {
        }

        //  append message to log
        void LogMessage(string message)
        {
            //  prepare the date
            string clrDate = "";
            clrDate = DateTime.Now.ToString("dd/MMM/yyyy:hh:mm:ss zzz");
            clrDate = clrDate.Remove(clrDate.LastIndexOf(":"), 1);
            //  log access events using CLF (combined log format):
            txtLog.AppendText("127.0.0.1");
            txtLog.AppendText(" -");
            //  remote log name - leave null for now
            txtLog.AppendText(" -");
            //  client username - leave null for now
            txtLog.AppendText((" ["
                            + (clrDate + "]")));
            txtLog.AppendText((" \"" + message.Replace('\n', ' ').TrimEnd(' ')));
            txtLog.AppendText("\"");
            txtLog.AppendText(Environment.NewLine);
        }

        //  connect to server failed (invoked server event)
        void ConnectFailed()
        {
            txtRaw.Text = "Could not connect." + '\n';
            txtLog.Text += "Could not connect." + '\n';
        }

        //  response is being handled by the server (invoked server event)
        void HandleResponse(string res)
        {
            txtRaw.Text += res;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            btnGo.Enabled = false;
            txtRaw.Text = "";
            if ((TabControl1.SelectedTab.Text == "Benchmark"))
            {
                RunBenchmark();
            }
            else
            {
                client.Go(cboUrl.Text, null);
                if (cboUrl.Items.Contains(cboUrl.Text))
                {

                }
                else
                {
                    cboUrl.Items.Add(cboUrl.Text);
                }

            }

            btnGo.Enabled = true;
        }

        private void chkWrapLog_CheckedChanged(object sender, EventArgs e)
        {
            if ((chkWrapLog.Checked == true))
            {
                txtLog.WordWrap = true;
            }
            else
            {
                txtLog.WordWrap = false;
            }

        }

        private void btnDetectSystemInfo_Click(object sender, EventArgs e)
        {
            DetectSystemInfo();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            while ((Chart1.Series.Count > 0))
            {
                Chart1.Series.RemoveAt(0);
            }

            while ((Chart3.Series.Count > 0))
            {
                Chart3.Series.RemoveAt(0);
            }

            while ((Chart2.Series.Count > 0))
            {
                Chart2.Series.RemoveAt(0);
            }

            TextBox1.Text = "";
        }

        private void cboBenchmarkTool_SelectedIndexChanged(object sender, EventArgs e)
        {
            Tool t = (Tool)Tools[cboBenchmarkTool.Text];
            if (t.Speed.Contains("%num"))
            {
                txtBenchmarkNumber.Enabled = true;
            }
            else
            {
                txtBenchmarkNumber.Enabled = false;
            }

            if (t.Speed.Contains("%conc"))
            {
                txtBenchmarkConcurrency.Enabled = true;
            }
            else
            {
                txtBenchmarkConcurrency.Enabled = false;
            }

            if (t.Time.Contains("%time"))
            {
                txtBenchmarkDuration.Enabled = true;
            }
            else
            {
                txtBenchmarkDuration.Enabled = false;
            }

            cboBenchmarkTool.SelectedIndex = ((ComboBox)(sender)).SelectedIndex;
            cboBenchmarkTool2.SelectedIndex = ((ComboBox)(sender)).SelectedIndex;
            cboBenchmarkTool3.SelectedIndex = ((ComboBox)(sender)).SelectedIndex;
        }
    }
    class DataPoint
    {

        public Hashtable Topics = new Hashtable();
    }
    class ManagedProcess
    {

        public Process Process = new Process();

        public Text.StringBuilder Output = new Text.StringBuilder();

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
            catch (Exception ex)
            {
                Output.Append("the tool process failed to run");
            }

        }

        void ReadOutputAsync(object sender, DataReceivedEventArgs e)
        {
            Output.AppendLine(e.Data);
        }
    }
    class Site
    {

        public string Name;

        public string Description;

        public string Url;
    }
    class Tool
    {

        public string Name;

        public string Path;

        public string Speed;

        public string Time;

        public ToolData Data = new ToolData();
    }
    class ToolData
    {

        public string RPS;

        public string CompletedRequests;

        public string ResponseTime;
    }
}
