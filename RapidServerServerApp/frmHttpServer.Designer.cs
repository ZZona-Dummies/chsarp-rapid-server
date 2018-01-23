using RapidServer.Http.Type1;
using System;
using System.Net.Sockets;

namespace RapidServerServerApp
{
    public partial class frmHttpServer : System.Windows.Forms.Form
    {

        // Form overrides dispose to clean up the component list.
        [System.Diagnostics.DebuggerNonUserCode()]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components != null)
                    components.Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }

        }

        // Required by the Windows Form Designer
        private System.ComponentModel.IContainer components;

        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.  
        // Do not modify it using the code editor.
        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title2 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title3 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.Label1 = new System.Windows.Forms.Label();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.tabsMain = new System.Windows.Forms.TabControl();
            this.tabInfo = new System.Windows.Forms.TabPage();
            this.LinkLabel2 = new System.Windows.Forms.LinkLabel();
            this.LinkLabel1 = new System.Windows.Forms.LinkLabel();
            this.Label3 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.tabLog = new System.Windows.Forms.TabPage();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.Panel3 = new System.Windows.Forms.Panel();
            this.chkEnableLog = new System.Windows.Forms.CheckBox();
            this.chkWrapLog = new System.Windows.Forms.CheckBox();
            this.tabPerformance = new System.Windows.Forms.TabPage();
            this.tabsPerformance = new System.Windows.Forms.TabControl();
            this.tabConnections = new System.Windows.Forms.TabPage();
            this.chartConnections = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabCpu = new System.Windows.Forms.TabPage();
            this.chartCpu = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabRam = new System.Windows.Forms.TabPage();
            this.chartRam = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.TabPage1 = new System.Windows.Forms.TabPage();
            this.DataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Panel4 = new System.Windows.Forms.Panel();
            this.Panel2 = new System.Windows.Forms.Panel();
            this.ctxMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SpawnClientToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.KillPhpcgiexeProcessesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.btnPurgeCache = new System.Windows.Forms.Button();
            this.cboServer = new System.Windows.Forms.ComboBox();
            this.CheckBox1 = new System.Windows.Forms.CheckBox();
            this.Button1 = new System.Windows.Forms.Button();
            this.Button2 = new System.Windows.Forms.Button();
            this.timPerformance = new System.Windows.Forms.Timer(this.components);
            this.pnlMain.SuspendLayout();
            this.tabsMain.SuspendLayout();
            this.tabInfo.SuspendLayout();
            this.tabLog.SuspendLayout();
            this.Panel3.SuspendLayout();
            this.tabPerformance.SuspendLayout();
            this.tabsPerformance.SuspendLayout();
            this.tabConnections.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartConnections)).BeginInit();
            this.tabCpu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartCpu)).BeginInit();
            this.tabRam.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartRam)).BeginInit();
            this.TabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView1)).BeginInit();
            this.Panel2.SuspendLayout();
            this.ctxMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(4, 4);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(52, 23);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(60, 4);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(71, 23);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            // 
            // Label1
            // 
            this.Label1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.Label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.Label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.Label1.Location = new System.Drawing.Point(0, 0);
            this.Label1.Name = "Label1";
            this.Label1.Padding = new System.Windows.Forms.Padding(2);
            this.Label1.Size = new System.Drawing.Size(590, 33);
            this.Label1.TabIndex = 3;
            this.Label1.Text = "A very high performance web server utilizing .NET sockets and async I/O.";
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.tabsMain);
            this.pnlMain.Controls.Add(this.Panel4);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 65);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(590, 294);
            this.pnlMain.TabIndex = 9;
            // 
            // tabsMain
            // 
            this.tabsMain.Controls.Add(this.tabInfo);
            this.tabsMain.Controls.Add(this.tabLog);
            this.tabsMain.Controls.Add(this.tabPerformance);
            this.tabsMain.Controls.Add(this.TabPage1);
            this.tabsMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabsMain.Location = new System.Drawing.Point(0, 8);
            this.tabsMain.Name = "tabsMain";
            this.tabsMain.SelectedIndex = 0;
            this.tabsMain.Size = new System.Drawing.Size(590, 286);
            this.tabsMain.TabIndex = 9;
            // 
            // tabInfo
            // 
            this.tabInfo.Controls.Add(this.LinkLabel2);
            this.tabInfo.Controls.Add(this.LinkLabel1);
            this.tabInfo.Controls.Add(this.Label3);
            this.tabInfo.Controls.Add(this.Label2);
            this.tabInfo.Location = new System.Drawing.Point(4, 22);
            this.tabInfo.Name = "tabInfo";
            this.tabInfo.Padding = new System.Windows.Forms.Padding(3);
            this.tabInfo.Size = new System.Drawing.Size(582, 260);
            this.tabInfo.TabIndex = 2;
            this.tabInfo.Text = "Info";
            this.tabInfo.UseVisualStyleBackColor = true;
            // 
            // LinkLabel2
            // 
            this.LinkLabel2.AutoSize = true;
            this.LinkLabel2.Location = new System.Drawing.Point(68, 24);
            this.LinkLabel2.Name = "LinkLabel2";
            this.LinkLabel2.Size = new System.Drawing.Size(59, 13);
            this.LinkLabel2.TabIndex = 13;
            this.LinkLabel2.TabStop = true;
            this.LinkLabel2.Text = "LinkLabel2";
            // 
            // LinkLabel1
            // 
            this.LinkLabel1.AutoSize = true;
            this.LinkLabel1.Location = new System.Drawing.Point(68, 8);
            this.LinkLabel1.Name = "LinkLabel1";
            this.LinkLabel1.Size = new System.Drawing.Size(59, 13);
            this.LinkLabel1.TabIndex = 12;
            this.LinkLabel1.TabStop = true;
            this.LinkLabel1.Text = "LinkLabel1";
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(8, 8);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(61, 13);
            this.Label3.TabIndex = 11;
            this.Label3.Text = "Root Path: ";
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(8, 24);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(49, 13);
            this.Label2.TabIndex = 10;
            this.Label2.Text = "Root Url:";
            // 
            // tabLog
            // 
            this.tabLog.Controls.Add(this.txtLog);
            this.tabLog.Controls.Add(this.Panel3);
            this.tabLog.Location = new System.Drawing.Point(4, 22);
            this.tabLog.Name = "tabLog";
            this.tabLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabLog.Size = new System.Drawing.Size(582, 260);
            this.tabLog.TabIndex = 0;
            this.tabLog.Text = "Log";
            this.tabLog.UseVisualStyleBackColor = true;
            // 
            // txtLog
            // 
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Location = new System.Drawing.Point(3, 27);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(576, 230);
            this.txtLog.TabIndex = 8;
            this.txtLog.WordWrap = false;
            // 
            // Panel3
            // 
            this.Panel3.Controls.Add(this.chkEnableLog);
            this.Panel3.Controls.Add(this.chkWrapLog);
            this.Panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel3.Location = new System.Drawing.Point(3, 3);
            this.Panel3.Name = "Panel3";
            this.Panel3.Size = new System.Drawing.Size(576, 24);
            this.Panel3.TabIndex = 11;
            // 
            // chkEnableLog
            // 
            this.chkEnableLog.AutoSize = true;
            this.chkEnableLog.Location = new System.Drawing.Point(4, 4);
            this.chkEnableLog.Name = "chkEnableLog";
            this.chkEnableLog.Size = new System.Drawing.Size(59, 17);
            this.chkEnableLog.TabIndex = 10;
            this.chkEnableLog.Text = "Enable";
            this.chkEnableLog.UseVisualStyleBackColor = true;
            // 
            // chkWrapLog
            // 
            this.chkWrapLog.AutoSize = true;
            this.chkWrapLog.Location = new System.Drawing.Point(68, 4);
            this.chkWrapLog.Name = "chkWrapLog";
            this.chkWrapLog.Size = new System.Drawing.Size(52, 17);
            this.chkWrapLog.TabIndex = 9;
            this.chkWrapLog.Text = "Wrap";
            this.chkWrapLog.UseVisualStyleBackColor = true;
            // 
            // tabPerformance
            // 
            this.tabPerformance.Controls.Add(this.tabsPerformance);
            this.tabPerformance.Location = new System.Drawing.Point(4, 22);
            this.tabPerformance.Name = "tabPerformance";
            this.tabPerformance.Padding = new System.Windows.Forms.Padding(3);
            this.tabPerformance.Size = new System.Drawing.Size(582, 260);
            this.tabPerformance.TabIndex = 3;
            this.tabPerformance.Text = "Performance";
            this.tabPerformance.UseVisualStyleBackColor = true;
            // 
            // tabsPerformance
            // 
            this.tabsPerformance.Controls.Add(this.tabConnections);
            this.tabsPerformance.Controls.Add(this.tabCpu);
            this.tabsPerformance.Controls.Add(this.tabRam);
            this.tabsPerformance.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabsPerformance.Location = new System.Drawing.Point(3, 3);
            this.tabsPerformance.Name = "tabsPerformance";
            this.tabsPerformance.SelectedIndex = 0;
            this.tabsPerformance.Size = new System.Drawing.Size(576, 254);
            this.tabsPerformance.TabIndex = 1;
            // 
            // tabConnections
            // 
            this.tabConnections.Controls.Add(this.chartConnections);
            this.tabConnections.Location = new System.Drawing.Point(4, 22);
            this.tabConnections.Name = "tabConnections";
            this.tabConnections.Padding = new System.Windows.Forms.Padding(3);
            this.tabConnections.Size = new System.Drawing.Size(568, 228);
            this.tabConnections.TabIndex = 0;
            this.tabConnections.Text = "Connections";
            this.tabConnections.UseVisualStyleBackColor = true;
            // 
            // chartConnections
            // 
            chartArea1.AxisY.Title = "connection count";
            chartArea1.Name = "ChartArea1";
            this.chartConnections.ChartAreas.Add(chartArea1);
            this.chartConnections.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartConnections.Location = new System.Drawing.Point(3, 3);
            this.chartConnections.Name = "chartConnections";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series1.Name = "Series1";
            this.chartConnections.Series.Add(series1);
            this.chartConnections.Size = new System.Drawing.Size(562, 222);
            this.chartConnections.TabIndex = 0;
            this.chartConnections.Text = "Chart1";
            title1.Name = "Title1";
            title1.Text = "Connections";
            this.chartConnections.Titles.Add(title1);
            // 
            // tabCpu
            // 
            this.tabCpu.Controls.Add(this.chartCpu);
            this.tabCpu.Location = new System.Drawing.Point(4, 22);
            this.tabCpu.Name = "tabCpu";
            this.tabCpu.Padding = new System.Windows.Forms.Padding(3);
            this.tabCpu.Size = new System.Drawing.Size(568, 228);
            this.tabCpu.TabIndex = 1;
            this.tabCpu.Text = "CPU";
            this.tabCpu.UseVisualStyleBackColor = true;
            // 
            // chartCpu
            // 
            chartArea2.AxisY.Maximum = 100D;
            chartArea2.AxisY.Minimum = 0D;
            chartArea2.AxisY.Title = "cpu usage %";
            chartArea2.Name = "ChartArea1";
            this.chartCpu.ChartAreas.Add(chartArea2);
            this.chartCpu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartCpu.Location = new System.Drawing.Point(3, 3);
            this.chartCpu.Name = "chartCpu";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series2.Name = "Series1";
            this.chartCpu.Series.Add(series2);
            this.chartCpu.Size = new System.Drawing.Size(562, 222);
            this.chartCpu.TabIndex = 19;
            this.chartCpu.Text = "Chart2";
            title2.Name = "titCpuMain";
            title2.Text = "CPU";
            this.chartCpu.Titles.Add(title2);
            // 
            // tabRam
            // 
            this.tabRam.Controls.Add(this.chartRam);
            this.tabRam.Location = new System.Drawing.Point(4, 22);
            this.tabRam.Name = "tabRam";
            this.tabRam.Padding = new System.Windows.Forms.Padding(3);
            this.tabRam.Size = new System.Drawing.Size(568, 228);
            this.tabRam.TabIndex = 2;
            this.tabRam.Text = "RAM";
            this.tabRam.UseVisualStyleBackColor = true;
            // 
            // chartRam
            // 
            chartArea3.AxisY.Title = "megabytes";
            chartArea3.Name = "ChartArea1";
            this.chartRam.ChartAreas.Add(chartArea3);
            this.chartRam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartRam.Location = new System.Drawing.Point(3, 3);
            this.chartRam.Name = "chartRam";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series3.Name = "Series1";
            this.chartRam.Series.Add(series3);
            this.chartRam.Size = new System.Drawing.Size(562, 222);
            this.chartRam.TabIndex = 20;
            this.chartRam.Text = "Chart2";
            title3.Name = "Title1";
            title3.Text = "RAM";
            this.chartRam.Titles.Add(title3);
            // 
            // TabPage1
            // 
            this.TabPage1.Controls.Add(this.DataGridView1);
            this.TabPage1.Location = new System.Drawing.Point(4, 22);
            this.TabPage1.Name = "TabPage1";
            this.TabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage1.Size = new System.Drawing.Size(582, 260);
            this.TabPage1.TabIndex = 4;
            this.TabPage1.Text = "Sites";
            this.TabPage1.UseVisualStyleBackColor = true;
            // 
            // DataGridView1
            // 
            this.DataGridView1.AllowUserToAddRows = false;
            this.DataGridView1.AllowUserToDeleteRows = false;
            this.DataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3});
            this.DataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataGridView1.Location = new System.Drawing.Point(3, 3);
            this.DataGridView1.Name = "DataGridView1";
            this.DataGridView1.ReadOnly = true;
            this.DataGridView1.Size = new System.Drawing.Size(576, 254);
            this.DataGridView1.TabIndex = 0;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Name";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 60;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "URL";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 54;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Path";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Width = 54;
            // 
            // Panel4
            // 
            this.Panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel4.Location = new System.Drawing.Point(0, 0);
            this.Panel4.Name = "Panel4";
            this.Panel4.Size = new System.Drawing.Size(590, 8);
            this.Panel4.TabIndex = 13;
            // 
            // Panel2
            // 
            this.Panel2.ContextMenuStrip = this.ctxMain;
            this.Panel2.Controls.Add(this.btnPurgeCache);
            this.Panel2.Controls.Add(this.cboServer);
            this.Panel2.Controls.Add(this.btnStart);
            this.Panel2.Controls.Add(this.btnStop);
            this.Panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel2.Location = new System.Drawing.Point(0, 33);
            this.Panel2.Name = "Panel2";
            this.Panel2.Size = new System.Drawing.Size(590, 32);
            this.Panel2.TabIndex = 10;
            // 
            // ctxMain
            // 
            this.ctxMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SpawnClientToolStripMenuItem1,
            this.KillPhpcgiexeProcessesToolStripMenuItem1});
            this.ctxMain.Name = "ContextMenuStrip1";
            this.ctxMain.Size = new System.Drawing.Size(210, 48);
            // 
            // SpawnClientToolStripMenuItem1
            // 
            this.SpawnClientToolStripMenuItem1.Name = "SpawnClientToolStripMenuItem1";
            this.SpawnClientToolStripMenuItem1.Size = new System.Drawing.Size(209, 22);
            this.SpawnClientToolStripMenuItem1.Text = "Spawn Client";
            // 
            // KillPhpcgiexeProcessesToolStripMenuItem1
            // 
            this.KillPhpcgiexeProcessesToolStripMenuItem1.Name = "KillPhpcgiexeProcessesToolStripMenuItem1";
            this.KillPhpcgiexeProcessesToolStripMenuItem1.Size = new System.Drawing.Size(209, 22);
            this.KillPhpcgiexeProcessesToolStripMenuItem1.Text = "Kill php-cgi.exe processes";
            // 
            // btnPurgeCache
            // 
            this.btnPurgeCache.Location = new System.Drawing.Point(137, 4);
            this.btnPurgeCache.Name = "btnPurgeCache";
            this.btnPurgeCache.Size = new System.Drawing.Size(72, 23);
            this.btnPurgeCache.TabIndex = 11;
            this.btnPurgeCache.Text = "Purge";
            this.btnPurgeCache.UseVisualStyleBackColor = true;
            // 
            // cboServer
            // 
            this.cboServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboServer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboServer.FormattingEnabled = true;
            this.cboServer.Location = new System.Drawing.Point(471, 5);
            this.cboServer.Name = "cboServer";
            this.cboServer.Size = new System.Drawing.Size(112, 21);
            this.cboServer.Sorted = true;
            this.cboServer.TabIndex = 10;
            // 
            // CheckBox1
            // 
            this.CheckBox1.AutoSize = true;
            this.CheckBox1.Location = new System.Drawing.Point(4, 4);
            this.CheckBox1.Name = "CheckBox1";
            this.CheckBox1.Size = new System.Drawing.Size(52, 17);
            this.CheckBox1.TabIndex = 9;
            this.CheckBox1.Text = "Wrap";
            this.CheckBox1.UseVisualStyleBackColor = true;
            // 
            // Button1
            // 
            this.Button1.Location = new System.Drawing.Point(145, 1);
            this.Button1.Name = "Button1";
            this.Button1.Size = new System.Drawing.Size(80, 23);
            this.Button1.TabIndex = 9;
            this.Button1.Text = "Open Site";
            this.Button1.UseVisualStyleBackColor = true;
            // 
            // Button2
            // 
            this.Button2.Location = new System.Drawing.Point(57, 1);
            this.Button2.Name = "Button2";
            this.Button2.Size = new System.Drawing.Size(84, 23);
            this.Button2.TabIndex = 8;
            this.Button2.Text = "Open Folder";
            this.Button2.UseVisualStyleBackColor = true;
            // 
            // timPerformance
            // 
            this.timPerformance.Enabled = true;
            this.timPerformance.Interval = 500;
            // 
            // frmHttpServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(590, 359);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.Panel2);
            this.Controls.Add(this.Label1);
            this.MinimumSize = new System.Drawing.Size(263, 103);
            this.Name = "frmHttpServer";
            this.Text = "Rapid Web Server";
            this.pnlMain.ResumeLayout(false);
            this.tabsMain.ResumeLayout(false);
            this.tabInfo.ResumeLayout(false);
            this.tabInfo.PerformLayout();
            this.tabLog.ResumeLayout(false);
            this.tabLog.PerformLayout();
            this.Panel3.ResumeLayout(false);
            this.Panel3.PerformLayout();
            this.tabPerformance.ResumeLayout(false);
            this.tabsPerformance.ResumeLayout(false);
            this.tabConnections.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartConnections)).EndInit();
            this.tabCpu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartCpu)).EndInit();
            this.tabRam.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartRam)).EndInit();
            this.TabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView1)).EndInit();
            this.Panel2.ResumeLayout(false);
            this.ctxMain.ResumeLayout(false);
            this.ResumeLayout(false);

            //Event handlers
            Activated += new System.EventHandler(frmHttpServer_Activated);
            Load += new System.EventHandler(frmServer_Load);

            btnStart.Click += btnStart_Click;
            btnStop.Click += btnStop_Click;
            SpawnClientToolStripMenuItem1.Click += SpawnClientToolStripMenuItem1_Click;
            KillPhpcgiexeProcessesToolStripMenuItem1.Click += KillPhpcgiexeProcessesToolStripMenuItem1_Click;
            cboServer.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            LinkLabel1.LinkClicked += LinkLabel1_LinkClicked;
            LinkLabel2.LinkClicked += LinkLabel2_LinkClicked;
            chkWrapLog.CheckedChanged += chkWrapAccessLog_CheckedChanged;
            btnPurgeCache.Click += btnPurgeCache_Click;
            timPerformance.Tick += timPerformance_Tick;
            chkEnableLog.CheckedChanged += chkEnableLog_CheckedChanged;

            server.HandleRequest += (obj, b) =>
            {
                server_HandleRequest((Request)((Tuple<object, object>)obj).Item1, (Socket)((Tuple<object, object>)obj).Item2);
            };

            server.ServerShutdown += (a, b) =>
            {
                server_ServerShutdown();
            };

            server.ServerStarted += (a, b) =>
            {
                server_ServerStarted();
            };
        }

        internal System.Windows.Forms.Button btnStart;

        internal System.Windows.Forms.Button btnStop;

        internal System.Windows.Forms.Label Label1;

        internal System.Windows.Forms.ContextMenuStrip ctxMain;

        internal System.Windows.Forms.ToolStripMenuItem SpawnClientToolStripMenuItem1;

        internal System.Windows.Forms.ToolStripMenuItem KillPhpcgiexeProcessesToolStripMenuItem1;

        internal System.Windows.Forms.ComboBox cboServer;

        internal System.Windows.Forms.Panel pnlMain;

        internal System.Windows.Forms.TabControl tabsMain;

        internal System.Windows.Forms.TabPage tabLog;

        internal System.Windows.Forms.TextBox txtLog;

        internal System.Windows.Forms.Panel Panel3;

        internal System.Windows.Forms.CheckBox chkWrapLog;

        internal System.Windows.Forms.Panel Panel2;

        internal System.Windows.Forms.CheckBox CheckBox1;

        internal System.Windows.Forms.Button Button1;

        internal System.Windows.Forms.Button Button2;

        internal System.Windows.Forms.Panel Panel4;

        internal System.Windows.Forms.TabPage tabInfo;

        internal System.Windows.Forms.TabPage tabPerformance;

        internal System.Windows.Forms.LinkLabel LinkLabel2;

        internal System.Windows.Forms.LinkLabel LinkLabel1;

        internal System.Windows.Forms.Label Label3;

        internal System.Windows.Forms.Label Label2;

        internal System.Windows.Forms.CheckBox chkEnableLog;

        internal System.Windows.Forms.Button btnPurgeCache;

        internal System.Windows.Forms.Timer timPerformance;

        internal System.Windows.Forms.TabControl tabsPerformance;

        internal System.Windows.Forms.TabPage tabConnections;

        internal System.Windows.Forms.TabPage tabCpu;

        internal System.Windows.Forms.DataVisualization.Charting.Chart chartCpu;

        internal System.Windows.Forms.DataVisualization.Charting.Chart chartConnections;

        internal System.Windows.Forms.TabPage TabPage1;

        internal System.Windows.Forms.DataGridView DataGridView1;

        internal System.Windows.Forms.DataGridViewTextBoxColumn Column1;

        internal System.Windows.Forms.DataGridViewTextBoxColumn Column2;

        internal System.Windows.Forms.DataGridViewTextBoxColumn Column3;

        internal System.Windows.Forms.TabPage tabRam;

        internal System.Windows.Forms.DataVisualization.Charting.Chart chartRam;
    }
}