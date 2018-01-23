using RapidServer;
using RapidServer.Http;
using System;
using System.Collections;
using System.Diagnostics;
using System.Net.Sockets;
using System.Windows.Forms;

namespace RapidServerServerApp
{
    public partial class frmHttpServer : Form
    {
        public frmHttpServer()
        {
            InitializeComponent();
        }

        private RapidServer.Http.Type1.Server server = new RapidServer.Http.Type1.Server();

        public delegate void HandleRequestDelegate(RapidServer.Http.Type1.Request req, RapidServer.Http.Type1.Response res);

        private Process proc;

        private PerformanceCounter cpu = new PerformanceCounter();

        private ArrayList points = new ArrayList();

        private void SpawnClient()
        {
            // Dim f As New frmHttpClient
            // f.Show()
        }

        //  when a request is handled by the server, intercept it so we can log the event
        private void HandleRequest(RapidServer.Http.Type1.Request req, RapidServer.Http.Type1.Response res)
        {
            //  prepare the date
            string clrDate = "";
            DateTime.Now.ToString("dd/MMM/yyyy:hh:mm:ss zzz");
            clrDate = clrDate.Remove(clrDate.LastIndexOf(":"), 1);
            //  log access events using CLF (combined log format):
            // If TextBox1.Text <> "" Then TextBox1.AppendText(vbCrLf)
            string logString;
            logString = (req.ClientAddress + (" -" + (" -" + (" ["
                        + (clrDate + ("]" + (" \""
                        + (req.RequestLine + ("\"" + (" "
                        + (res.StatusCode + (" "
                        + (res.ContentLength + "\r\n")))))))))))));
            txtLog.AppendText(logString);
            // txtLog.AppendText(req.ClientAddress)
            // txtLog.AppendText(" -") ' remote log name - leave null for now
            // txtLog.AppendText(" -") ' client username - leave null for now
            // txtLog.AppendText(" [" & clrDate & "]")
            // ' TextBox1.AppendText(" """ & req.RequestString.Replace(vbCrLf, ""].Trim)
            // txtLog.AppendText(" """ & req.RequestLine)
            // txtLog.AppendText("""")
            // txtLog.AppendText(" " & res.StatusCode)
            // txtLog.AppendText(" " & res.ContentLength)
            // txtLog.AppendText(vbCrLf)
        }

        private void PopulateServerInfo()
        {
            //  update the info tab
            LinkLabel1.Text = ((Site)server.Sites[cboServer.Text]).RootPath;
            LinkLabel2.Text = ((Site)server.Sites[cboServer.Text]).RootUrl;
        }

        private void StartServerByName(string name)
        {
            // Dim s As RapidServer.Http.Site = server.Sites(name)
            // server.StartServer()
        }

        private void frmHttpServer_Activated(object sender, EventArgs e)
        {
        }

        private void frmServer_Load(object sender, EventArgs e)
        {
            // Me.Width = 360 '263
            // Me.Height = 103
            //  start server
            server.StartServer();
            //  put the available sites into the combobox:
            foreach (Site s in server.Sites.Values)
            {
                cboServer.Items.Add(s.Title);
                DataGridView1.Rows.Add(s.Title, s.RootUrl, s.RootPath);
            }

            cboServer.SelectedIndex = 0;
            //  update the info tab
            PopulateServerInfo();
            //  spawn a client for testing
            SpawnClient();
            //  track cpu usage for the server app
            proc = Process.GetCurrentProcess();

            // With...
            cpu.CounterName = proc.ProcessName;
            cpu.CategoryName = proc.ProcessName;
            cpu.InstanceName = proc.ProcessName;

            for (int i = 1; i <= 200; i++)
            {
                chartConnections.Series[0].Points.AddXY(0, 0);
                chartCpu.Series[0].Points.AddXY(0, 0);
                chartRam.Series[0].Points.AddXY(0, 0);
            }
        }

        private void server_HandleRequest(RapidServer.Http.Type1.Request req, Socket client)
        {
            if (chkEnableLog.Checked)
            {
                Invoke(new HandleRequestDelegate(HandleRequest), new object[] {
                        req});
            }
        }

        private void server_ServerShutdown()
        {
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            Debug.WriteLine("Server stopped!");
        }

        private void server_ServerStarted()
        {
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            Debug.WriteLine("Server started!");
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // StartServerByName(cboServer.Text)
            server.StartServer();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            server.StopServer();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            server.StopServer();
            Close();
        }

        private void SpawnClientToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SpawnClient();
        }

        private void KillPhpcgiexeProcessesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Globals.KillAll("php-cgi.exe");
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateServerInfo();
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(LinkLabel1.Text);
        }

        private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(LinkLabel2.Text);
        }

        private void chkWrapAccessLog_CheckedChanged(object sender, EventArgs e)
        {
            if (chkWrapLog.Checked)
            {
                txtLog.WordWrap = true;
            }
            else
            {
                txtLog.WordWrap = false;
            }
        }

        private void btnPurgeCache_Click(object sender, EventArgs e)
        {
            server.OutputCache.Clear();
        }

        private void timPerformance_Tick(object sender, EventArgs e)
        {
            btnPurgeCache.Text = ("Purge " + server.OutputCache.Count);
            btnStop.Text = ("Stop " + server.ConnectedClients);
            if (tabsMain.SelectedTab.Text == "Performance")
            {
                //  plot the charts
                double val;
                //  plot connections this tick
                chartConnections.Series[0].Points.RemoveAt(0);
                val = server.ConnectedClients;
                chartConnections.Refresh();
                chartConnections.Series[0].Points.AddXY(0, val);
                //  plot cpu usage % this tick
                chartCpu.Series[0].Points.RemoveAt(0);
                val = cpu.NextValue() / Environment.ProcessorCount;
                chartCpu.Refresh();
                chartCpu.Series[0].Points.AddXY(0, val);
                //  plot ram usage this tick
                chartRam.Series[0].Points.RemoveAt(0);
                val = (proc.WorkingSet64 / (1024 / 1024));
                chartRam.Refresh();
                chartRam.Series[0].Points.AddXY(0, val);
            }
        }

        private void chkEnableLog_CheckedChanged(object sender, EventArgs e)
        {
        }
    }
}