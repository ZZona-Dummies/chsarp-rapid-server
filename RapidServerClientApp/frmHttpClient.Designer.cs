namespace RapidServerClientApp
{
    public partial class frmHttpClient : System.Windows.Forms.Form
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
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Title title2 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Title title3 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.Label1 = new System.Windows.Forms.Label();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.cboUrl = new System.Windows.Forms.ComboBox();
            this.btnGo = new System.Windows.Forms.Button();
            this.Panel2 = new System.Windows.Forms.Panel();
            this.TabControl1 = new System.Windows.Forms.TabControl();
            this.tabRaw = new System.Windows.Forms.TabPage();
            this.txtRaw = new System.Windows.Forms.TextBox();
            this.tabBenchmark = new System.Windows.Forms.TabPage();
            this.btnClear = new System.Windows.Forms.Button();
            this.TabControl3 = new System.Windows.Forms.TabControl();
            this.TabPage1 = new System.Windows.Forms.TabPage();
            this.cboBenchmarkTool = new System.Windows.Forms.ComboBox();
            this.Label6 = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.txtBenchmarkNumber = new System.Windows.Forms.TextBox();
            this.Label5 = new System.Windows.Forms.Label();
            this.txtBenchmarkConcurrency = new System.Windows.Forms.TextBox();
            this.TabPage2 = new System.Windows.Forms.TabPage();
            this.cboBenchmarkTool2 = new System.Windows.Forms.ComboBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.txtBenchmarkDuration = new System.Windows.Forms.TextBox();
            this.TabPage3 = new System.Windows.Forms.TabPage();
            this.cboBenchmarkTool3 = new System.Windows.Forms.ComboBox();
            this.Label7 = new System.Windows.Forms.Label();
            this.Label8 = new System.Windows.Forms.Label();
            this.txtBenchmarkRampNumber = new System.Windows.Forms.TextBox();
            this.Label9 = new System.Windows.Forms.Label();
            this.txtBenchmarkRampConcurrency = new System.Windows.Forms.TextBox();
            this.TabControl2 = new System.Windows.Forms.TabControl();
            this.tabBenchmarkBarChart = new System.Windows.Forms.TabPage();
            this.Chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabBenchmarkCompletedRequests = new System.Windows.Forms.TabPage();
            this.Chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabBenchmarkLineChart = new System.Windows.Forms.TabPage();
            this.Chart3 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btnDetectSystemInfo = new System.Windows.Forms.Button();
            this.TextBox1 = new System.Windows.Forms.TextBox();
            this.tabLog = new System.Windows.Forms.TabPage();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.Panel3 = new System.Windows.Forms.Panel();
            this.chkEnableLog = new System.Windows.Forms.CheckBox();
            this.chkWrapLog = new System.Windows.Forms.CheckBox();
            this.tabOptions = new System.Windows.Forms.TabPage();
            this.chkFetchSubResources = new System.Windows.Forms.CheckBox();
            this.Timer1 = new System.Windows.Forms.Timer(this.components);
            this.Panel1.SuspendLayout();
            this.Panel2.SuspendLayout();
            this.TabControl1.SuspendLayout();
            this.tabRaw.SuspendLayout();
            this.tabBenchmark.SuspendLayout();
            this.TabControl3.SuspendLayout();
            this.TabPage1.SuspendLayout();
            this.TabPage2.SuspendLayout();
            this.TabPage3.SuspendLayout();
            this.TabControl2.SuspendLayout();
            this.tabBenchmarkBarChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Chart1)).BeginInit();
            this.tabBenchmarkCompletedRequests.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Chart2)).BeginInit();
            this.tabBenchmarkLineChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Chart3)).BeginInit();
            this.tabLog.SuspendLayout();
            this.Panel3.SuspendLayout();
            this.tabOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // Label1
            // 
            this.Label1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.Label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.Label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.Label1.Location = new System.Drawing.Point(0, 0);
            this.Label1.Name = "Label1";
            this.Label1.Padding = new System.Windows.Forms.Padding(2);
            this.Label1.Size = new System.Drawing.Size(615, 33);
            this.Label1.TabIndex = 4;
            this.Label1.Text = "An HTTP client for testing request/response cycles and benchmarking web server pe" +
    "rformance under various heavy load scenarios.";
            // 
            // Panel1
            // 
            this.Panel1.Controls.Add(this.cboUrl);
            this.Panel1.Controls.Add(this.btnGo);
            this.Panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel1.Location = new System.Drawing.Point(0, 33);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(615, 29);
            this.Panel1.TabIndex = 5;
            // 
            // cboUrl
            // 
            this.cboUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboUrl.FormattingEnabled = true;
            this.cboUrl.Location = new System.Drawing.Point(4, 4);
            this.cboUrl.Name = "cboUrl";
            this.cboUrl.Size = new System.Drawing.Size(565, 21);
            this.cboUrl.Sorted = true;
            this.cboUrl.TabIndex = 5;
            // 
            // btnGo
            // 
            this.btnGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGo.Location = new System.Drawing.Point(575, 3);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(37, 23);
            this.btnGo.TabIndex = 3;
            this.btnGo.Text = "Go";
            this.btnGo.UseVisualStyleBackColor = true;
            // 
            // Panel2
            // 
            this.Panel2.Controls.Add(this.TabControl1);
            this.Panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Panel2.Location = new System.Drawing.Point(0, 62);
            this.Panel2.Name = "Panel2";
            this.Panel2.Size = new System.Drawing.Size(615, 389);
            this.Panel2.TabIndex = 6;
            // 
            // TabControl1
            // 
            this.TabControl1.Controls.Add(this.tabRaw);
            this.TabControl1.Controls.Add(this.tabBenchmark);
            this.TabControl1.Controls.Add(this.tabLog);
            this.TabControl1.Controls.Add(this.tabOptions);
            this.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl1.Location = new System.Drawing.Point(0, 0);
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            this.TabControl1.Size = new System.Drawing.Size(615, 389);
            this.TabControl1.TabIndex = 2;
            // 
            // tabRaw
            // 
            this.tabRaw.Controls.Add(this.txtRaw);
            this.tabRaw.Location = new System.Drawing.Point(4, 22);
            this.tabRaw.Name = "tabRaw";
            this.tabRaw.Padding = new System.Windows.Forms.Padding(3);
            this.tabRaw.Size = new System.Drawing.Size(607, 363);
            this.tabRaw.TabIndex = 0;
            this.tabRaw.Text = "Raw";
            this.tabRaw.UseVisualStyleBackColor = true;
            // 
            // txtRaw
            // 
            this.txtRaw.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRaw.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRaw.Location = new System.Drawing.Point(3, 3);
            this.txtRaw.Multiline = true;
            this.txtRaw.Name = "txtRaw";
            this.txtRaw.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtRaw.Size = new System.Drawing.Size(601, 357);
            this.txtRaw.TabIndex = 2;
            // 
            // tabBenchmark
            // 
            this.tabBenchmark.Controls.Add(this.btnClear);
            this.tabBenchmark.Controls.Add(this.TabControl3);
            this.tabBenchmark.Controls.Add(this.TabControl2);
            this.tabBenchmark.Controls.Add(this.btnDetectSystemInfo);
            this.tabBenchmark.Controls.Add(this.TextBox1);
            this.tabBenchmark.Location = new System.Drawing.Point(4, 22);
            this.tabBenchmark.Name = "tabBenchmark";
            this.tabBenchmark.Padding = new System.Windows.Forms.Padding(3);
            this.tabBenchmark.Size = new System.Drawing.Size(607, 363);
            this.tabBenchmark.TabIndex = 3;
            this.tabBenchmark.Text = "Benchmark";
            this.tabBenchmark.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(3, 145);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(154, 23);
            this.btnClear.TabIndex = 19;
            this.btnClear.Text = "Clear Results";
            this.btnClear.UseVisualStyleBackColor = true;
            // 
            // TabControl3
            // 
            this.TabControl3.Controls.Add(this.TabPage1);
            this.TabControl3.Controls.Add(this.TabPage2);
            this.TabControl3.Controls.Add(this.TabPage3);
            this.TabControl3.Location = new System.Drawing.Point(3, 6);
            this.TabControl3.Name = "TabControl3";
            this.TabControl3.SelectedIndex = 0;
            this.TabControl3.Size = new System.Drawing.Size(154, 108);
            this.TabControl3.TabIndex = 18;
            // 
            // TabPage1
            // 
            this.TabPage1.Controls.Add(this.cboBenchmarkTool);
            this.TabPage1.Controls.Add(this.Label6);
            this.TabPage1.Controls.Add(this.Label4);
            this.TabPage1.Controls.Add(this.txtBenchmarkNumber);
            this.TabPage1.Controls.Add(this.Label5);
            this.TabPage1.Controls.Add(this.txtBenchmarkConcurrency);
            this.TabPage1.Location = new System.Drawing.Point(4, 22);
            this.TabPage1.Name = "TabPage1";
            this.TabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage1.Size = new System.Drawing.Size(146, 82);
            this.TabPage1.TabIndex = 0;
            this.TabPage1.Text = "Speed";
            this.TabPage1.UseVisualStyleBackColor = true;
            // 
            // cboBenchmarkTool
            // 
            this.cboBenchmarkTool.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBenchmarkTool.FormattingEnabled = true;
            this.cboBenchmarkTool.Location = new System.Drawing.Point(41, 6);
            this.cboBenchmarkTool.Name = "cboBenchmarkTool";
            this.cboBenchmarkTool.Size = new System.Drawing.Size(99, 21);
            this.cboBenchmarkTool.Sorted = true;
            this.cboBenchmarkTool.TabIndex = 23;
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.Location = new System.Drawing.Point(4, 9);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(31, 13);
            this.Label6.TabIndex = 22;
            this.Label6.Text = "Tool:";
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Location = new System.Drawing.Point(6, 36);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(47, 13);
            this.Label4.TabIndex = 0;
            this.Label4.Text = "Number:";
            // 
            // txtBenchmarkNumber
            // 
            this.txtBenchmarkNumber.Location = new System.Drawing.Point(79, 33);
            this.txtBenchmarkNumber.Name = "txtBenchmarkNumber";
            this.txtBenchmarkNumber.Size = new System.Drawing.Size(61, 20);
            this.txtBenchmarkNumber.TabIndex = 1;
            this.txtBenchmarkNumber.Text = "1000";
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Location = new System.Drawing.Point(6, 62);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(70, 13);
            this.Label5.TabIndex = 2;
            this.Label5.Text = "Concurrency:";
            // 
            // txtBenchmarkConcurrency
            // 
            this.txtBenchmarkConcurrency.Location = new System.Drawing.Point(79, 59);
            this.txtBenchmarkConcurrency.Name = "txtBenchmarkConcurrency";
            this.txtBenchmarkConcurrency.Size = new System.Drawing.Size(61, 20);
            this.txtBenchmarkConcurrency.TabIndex = 3;
            this.txtBenchmarkConcurrency.Text = "100";
            // 
            // TabPage2
            // 
            this.TabPage2.Controls.Add(this.cboBenchmarkTool2);
            this.TabPage2.Controls.Add(this.Label3);
            this.TabPage2.Controls.Add(this.Label2);
            this.TabPage2.Controls.Add(this.txtBenchmarkDuration);
            this.TabPage2.Location = new System.Drawing.Point(4, 22);
            this.TabPage2.Name = "TabPage2";
            this.TabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage2.Size = new System.Drawing.Size(146, 82);
            this.TabPage2.TabIndex = 1;
            this.TabPage2.Text = "Time";
            this.TabPage2.UseVisualStyleBackColor = true;
            // 
            // cboBenchmarkTool2
            // 
            this.cboBenchmarkTool2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBenchmarkTool2.FormattingEnabled = true;
            this.cboBenchmarkTool2.Location = new System.Drawing.Point(41, 6);
            this.cboBenchmarkTool2.Name = "cboBenchmarkTool2";
            this.cboBenchmarkTool2.Size = new System.Drawing.Size(99, 21);
            this.cboBenchmarkTool2.Sorted = true;
            this.cboBenchmarkTool2.TabIndex = 25;
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(4, 9);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(31, 13);
            this.Label3.TabIndex = 24;
            this.Label3.Text = "Tool:";
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(6, 36);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(50, 13);
            this.Label2.TabIndex = 2;
            this.Label2.Text = "Duration:";
            // 
            // txtBenchmarkDuration
            // 
            this.txtBenchmarkDuration.Location = new System.Drawing.Point(79, 33);
            this.txtBenchmarkDuration.Name = "txtBenchmarkDuration";
            this.txtBenchmarkDuration.Size = new System.Drawing.Size(61, 20);
            this.txtBenchmarkDuration.TabIndex = 3;
            this.txtBenchmarkDuration.Text = "60";
            // 
            // TabPage3
            // 
            this.TabPage3.Controls.Add(this.cboBenchmarkTool3);
            this.TabPage3.Controls.Add(this.Label7);
            this.TabPage3.Controls.Add(this.Label8);
            this.TabPage3.Controls.Add(this.txtBenchmarkRampNumber);
            this.TabPage3.Controls.Add(this.Label9);
            this.TabPage3.Controls.Add(this.txtBenchmarkRampConcurrency);
            this.TabPage3.Location = new System.Drawing.Point(4, 22);
            this.TabPage3.Name = "TabPage3";
            this.TabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage3.Size = new System.Drawing.Size(146, 82);
            this.TabPage3.TabIndex = 2;
            this.TabPage3.Text = "Ramp";
            this.TabPage3.UseVisualStyleBackColor = true;
            // 
            // cboBenchmarkTool3
            // 
            this.cboBenchmarkTool3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBenchmarkTool3.FormattingEnabled = true;
            this.cboBenchmarkTool3.Location = new System.Drawing.Point(41, 6);
            this.cboBenchmarkTool3.Name = "cboBenchmarkTool3";
            this.cboBenchmarkTool3.Size = new System.Drawing.Size(99, 21);
            this.cboBenchmarkTool3.Sorted = true;
            this.cboBenchmarkTool3.TabIndex = 29;
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.Location = new System.Drawing.Point(4, 9);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(31, 13);
            this.Label7.TabIndex = 28;
            this.Label7.Text = "Tool:";
            // 
            // Label8
            // 
            this.Label8.AutoSize = true;
            this.Label8.Location = new System.Drawing.Point(6, 36);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(47, 13);
            this.Label8.TabIndex = 24;
            this.Label8.Text = "Number:";
            // 
            // txtBenchmarkRampNumber
            // 
            this.txtBenchmarkRampNumber.Location = new System.Drawing.Point(79, 33);
            this.txtBenchmarkRampNumber.Name = "txtBenchmarkRampNumber";
            this.txtBenchmarkRampNumber.Size = new System.Drawing.Size(61, 20);
            this.txtBenchmarkRampNumber.TabIndex = 25;
            this.txtBenchmarkRampNumber.Text = "1000";
            // 
            // Label9
            // 
            this.Label9.AutoSize = true;
            this.Label9.Location = new System.Drawing.Point(6, 62);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(70, 13);
            this.Label9.TabIndex = 26;
            this.Label9.Text = "Concurrency:";
            // 
            // txtBenchmarkRampConcurrency
            // 
            this.txtBenchmarkRampConcurrency.Location = new System.Drawing.Point(79, 59);
            this.txtBenchmarkRampConcurrency.Name = "txtBenchmarkRampConcurrency";
            this.txtBenchmarkRampConcurrency.Size = new System.Drawing.Size(61, 20);
            this.txtBenchmarkRampConcurrency.TabIndex = 27;
            this.txtBenchmarkRampConcurrency.Text = "100";
            // 
            // TabControl2
            // 
            this.TabControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TabControl2.Controls.Add(this.tabBenchmarkBarChart);
            this.TabControl2.Controls.Add(this.tabBenchmarkCompletedRequests);
            this.TabControl2.Controls.Add(this.tabBenchmarkLineChart);
            this.TabControl2.Location = new System.Drawing.Point(159, 6);
            this.TabControl2.Name = "TabControl2";
            this.TabControl2.SelectedIndex = 0;
            this.TabControl2.Size = new System.Drawing.Size(445, 354);
            this.TabControl2.TabIndex = 17;
            // 
            // tabBenchmarkBarChart
            // 
            this.tabBenchmarkBarChart.Controls.Add(this.Chart1);
            this.tabBenchmarkBarChart.Location = new System.Drawing.Point(4, 22);
            this.tabBenchmarkBarChart.Name = "tabBenchmarkBarChart";
            this.tabBenchmarkBarChart.Padding = new System.Windows.Forms.Padding(3);
            this.tabBenchmarkBarChart.Size = new System.Drawing.Size(437, 328);
            this.tabBenchmarkBarChart.TabIndex = 0;
            this.tabBenchmarkBarChart.Text = "RPS";
            this.tabBenchmarkBarChart.UseVisualStyleBackColor = true;
            // 
            // Chart1
            // 
            chartArea1.AxisX.Title = "run";
            chartArea1.AxisY.Title = "requests";
            chartArea1.Name = "ChartArea1";
            this.Chart1.ChartAreas.Add(chartArea1);
            this.Chart1.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            legend1.Name = "Legend1";
            this.Chart1.Legends.Add(legend1);
            this.Chart1.Location = new System.Drawing.Point(3, 3);
            this.Chart1.Name = "Chart1";
            this.Chart1.Size = new System.Drawing.Size(431, 322);
            this.Chart1.TabIndex = 15;
            this.Chart1.Text = "Chart1";
            title1.Name = "Title1";
            title1.Text = "Requests Per Second (RPS)";
            this.Chart1.Titles.Add(title1);
            // 
            // tabBenchmarkCompletedRequests
            // 
            this.tabBenchmarkCompletedRequests.Controls.Add(this.Chart2);
            this.tabBenchmarkCompletedRequests.Location = new System.Drawing.Point(4, 22);
            this.tabBenchmarkCompletedRequests.Name = "tabBenchmarkCompletedRequests";
            this.tabBenchmarkCompletedRequests.Padding = new System.Windows.Forms.Padding(3);
            this.tabBenchmarkCompletedRequests.Size = new System.Drawing.Size(437, 328);
            this.tabBenchmarkCompletedRequests.TabIndex = 2;
            this.tabBenchmarkCompletedRequests.Text = "Completed Requests";
            this.tabBenchmarkCompletedRequests.UseVisualStyleBackColor = true;
            // 
            // Chart2
            // 
            chartArea2.AxisX.Title = "run";
            chartArea2.AxisY.Title = "requests";
            chartArea2.Name = "ChartArea1";
            this.Chart2.ChartAreas.Add(chartArea2);
            this.Chart2.Dock = System.Windows.Forms.DockStyle.Fill;
            legend2.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            legend2.Name = "Legend1";
            this.Chart2.Legends.Add(legend2);
            this.Chart2.Location = new System.Drawing.Point(3, 3);
            this.Chart2.Name = "Chart2";
            this.Chart2.Size = new System.Drawing.Size(431, 322);
            this.Chart2.TabIndex = 16;
            this.Chart2.Text = "Chart3";
            title2.Name = "Title1";
            title2.Text = "Completed Requests";
            this.Chart2.Titles.Add(title2);
            // 
            // tabBenchmarkLineChart
            // 
            this.tabBenchmarkLineChart.Controls.Add(this.Chart3);
            this.tabBenchmarkLineChart.Location = new System.Drawing.Point(4, 22);
            this.tabBenchmarkLineChart.Name = "tabBenchmarkLineChart";
            this.tabBenchmarkLineChart.Padding = new System.Windows.Forms.Padding(3);
            this.tabBenchmarkLineChart.Size = new System.Drawing.Size(437, 328);
            this.tabBenchmarkLineChart.TabIndex = 1;
            this.tabBenchmarkLineChart.Text = "Response Time";
            this.tabBenchmarkLineChart.UseVisualStyleBackColor = true;
            // 
            // Chart3
            // 
            chartArea3.AxisX.Title = "request";
            chartArea3.AxisY.Title = "milliseconds";
            chartArea3.Name = "ChartArea1";
            this.Chart3.ChartAreas.Add(chartArea3);
            this.Chart3.Dock = System.Windows.Forms.DockStyle.Fill;
            legend3.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            legend3.IsDockedInsideChartArea = false;
            legend3.Name = "Legend1";
            this.Chart3.Legends.Add(legend3);
            this.Chart3.Location = new System.Drawing.Point(3, 3);
            this.Chart3.Name = "Chart3";
            this.Chart3.Size = new System.Drawing.Size(431, 322);
            this.Chart3.TabIndex = 16;
            this.Chart3.Text = "Chart2";
            title3.Name = "titMain";
            title3.Text = "Response Time";
            this.Chart3.Titles.Add(title3);
            // 
            // btnDetectSystemInfo
            // 
            this.btnDetectSystemInfo.Location = new System.Drawing.Point(3, 118);
            this.btnDetectSystemInfo.Name = "btnDetectSystemInfo";
            this.btnDetectSystemInfo.Size = new System.Drawing.Size(154, 23);
            this.btnDetectSystemInfo.TabIndex = 12;
            this.btnDetectSystemInfo.Text = "Detect System Info";
            this.btnDetectSystemInfo.UseVisualStyleBackColor = true;
            // 
            // TextBox1
            // 
            this.TextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.TextBox1.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBox1.Location = new System.Drawing.Point(6, 174);
            this.TextBox1.Multiline = true;
            this.TextBox1.Name = "TextBox1";
            this.TextBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TextBox1.Size = new System.Drawing.Size(147, 181);
            this.TextBox1.TabIndex = 5;
            // 
            // tabLog
            // 
            this.tabLog.Controls.Add(this.txtLog);
            this.tabLog.Controls.Add(this.Panel3);
            this.tabLog.Location = new System.Drawing.Point(4, 22);
            this.tabLog.Name = "tabLog";
            this.tabLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabLog.Size = new System.Drawing.Size(607, 363);
            this.tabLog.TabIndex = 5;
            this.tabLog.Text = "Log";
            this.tabLog.UseVisualStyleBackColor = true;
            // 
            // txtLog
            // 
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Location = new System.Drawing.Point(3, 27);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(601, 333);
            this.txtLog.TabIndex = 3;
            this.txtLog.WordWrap = false;
            // 
            // Panel3
            // 
            this.Panel3.Controls.Add(this.chkEnableLog);
            this.Panel3.Controls.Add(this.chkWrapLog);
            this.Panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel3.Location = new System.Drawing.Point(3, 3);
            this.Panel3.Name = "Panel3";
            this.Panel3.Size = new System.Drawing.Size(601, 24);
            this.Panel3.TabIndex = 12;
            // 
            // chkEnableLog
            // 
            this.chkEnableLog.AutoSize = true;
            this.chkEnableLog.Checked = true;
            this.chkEnableLog.CheckState = System.Windows.Forms.CheckState.Checked;
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
            // tabOptions
            // 
            this.tabOptions.Controls.Add(this.chkFetchSubResources);
            this.tabOptions.Location = new System.Drawing.Point(4, 22);
            this.tabOptions.Name = "tabOptions";
            this.tabOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tabOptions.Size = new System.Drawing.Size(607, 363);
            this.tabOptions.TabIndex = 4;
            this.tabOptions.Text = "Options";
            this.tabOptions.UseVisualStyleBackColor = true;
            // 
            // chkFetchSubResources
            // 
            this.chkFetchSubResources.AutoSize = true;
            this.chkFetchSubResources.Location = new System.Drawing.Point(8, 8);
            this.chkFetchSubResources.Name = "chkFetchSubResources";
            this.chkFetchSubResources.Size = new System.Drawing.Size(122, 17);
            this.chkFetchSubResources.TabIndex = 4;
            this.chkFetchSubResources.Text = "Fetch sub-resources";
            this.chkFetchSubResources.UseVisualStyleBackColor = true;
            // 
            // Timer1
            // 
            this.Timer1.Enabled = true;
            // 
            // frmHttpClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 451);
            this.Controls.Add(this.Panel2);
            this.Controls.Add(this.Panel1);
            this.Controls.Add(this.Label1);
            this.Name = "frmHttpClient";
            this.Text = "Rapid Web Client";
            this.Load += new System.EventHandler(this.frmHttpClient_Load);
            this.Panel1.ResumeLayout(false);
            this.Panel2.ResumeLayout(false);
            this.TabControl1.ResumeLayout(false);
            this.tabRaw.ResumeLayout(false);
            this.tabRaw.PerformLayout();
            this.tabBenchmark.ResumeLayout(false);
            this.tabBenchmark.PerformLayout();
            this.TabControl3.ResumeLayout(false);
            this.TabPage1.ResumeLayout(false);
            this.TabPage1.PerformLayout();
            this.TabPage2.ResumeLayout(false);
            this.TabPage2.PerformLayout();
            this.TabPage3.ResumeLayout(false);
            this.TabPage3.PerformLayout();
            this.TabControl2.ResumeLayout(false);
            this.tabBenchmarkBarChart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Chart1)).EndInit();
            this.tabBenchmarkCompletedRequests.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Chart2)).EndInit();
            this.tabBenchmarkLineChart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Chart3)).EndInit();
            this.tabLog.ResumeLayout(false);
            this.tabLog.PerformLayout();
            this.Panel3.ResumeLayout(false);
            this.Panel3.PerformLayout();
            this.tabOptions.ResumeLayout(false);
            this.tabOptions.PerformLayout();
            this.ResumeLayout(false);

            //Event handlers
            
        }

        internal System.Windows.Forms.Label Label1;

        internal System.Windows.Forms.Panel Panel1;

        internal System.Windows.Forms.Button btnGo;

        internal System.Windows.Forms.Panel Panel2;

        internal System.Windows.Forms.TabControl TabControl1;

        internal System.Windows.Forms.TabPage tabRaw;

        internal System.Windows.Forms.TextBox txtRaw;

        internal System.Windows.Forms.TabPage tabBenchmark;

        internal System.Windows.Forms.TextBox txtBenchmarkConcurrency;

        internal System.Windows.Forms.Label Label5;

        internal System.Windows.Forms.TextBox txtBenchmarkNumber;

        internal System.Windows.Forms.Label Label4;

        internal System.Windows.Forms.TabPage tabOptions;

        internal System.Windows.Forms.CheckBox chkFetchSubResources;

        internal System.Windows.Forms.ComboBox cboUrl;

        internal System.Windows.Forms.TabPage tabLog;

        internal System.Windows.Forms.TextBox txtLog;

        internal System.Windows.Forms.Panel Panel3;

        internal System.Windows.Forms.CheckBox chkEnableLog;

        internal System.Windows.Forms.CheckBox chkWrapLog;

        internal System.Windows.Forms.TextBox TextBox1;

        internal System.Windows.Forms.Button btnDetectSystemInfo;

        internal System.Windows.Forms.TabControl TabControl2;

        internal System.Windows.Forms.TabPage tabBenchmarkBarChart;

        internal System.Windows.Forms.DataVisualization.Charting.Chart Chart1;

        internal System.Windows.Forms.TabPage tabBenchmarkLineChart;

        internal System.Windows.Forms.DataVisualization.Charting.Chart Chart3;

        internal System.Windows.Forms.TabControl TabControl3;

        internal System.Windows.Forms.TabPage TabPage1;

        internal System.Windows.Forms.TabPage TabPage2;

        internal System.Windows.Forms.Label Label2;

        internal System.Windows.Forms.TextBox txtBenchmarkDuration;

        internal System.Windows.Forms.TabPage tabBenchmarkCompletedRequests;

        internal System.Windows.Forms.DataVisualization.Charting.Chart Chart2;

        internal System.Windows.Forms.Button btnClear;

        internal System.Windows.Forms.ComboBox cboBenchmarkTool;

        internal System.Windows.Forms.Label Label6;

        internal System.Windows.Forms.ComboBox cboBenchmarkTool2;

        internal System.Windows.Forms.Label Label3;

        internal System.Windows.Forms.Timer Timer1;

        internal System.Windows.Forms.TabPage TabPage3;

        internal System.Windows.Forms.ComboBox cboBenchmarkTool3;

        internal System.Windows.Forms.Label Label7;

        internal System.Windows.Forms.Label Label8;

        internal System.Windows.Forms.TextBox txtBenchmarkRampNumber;

        internal System.Windows.Forms.Label Label9;

        internal System.Windows.Forms.TextBox txtBenchmarkRampConcurrency;
    }
}