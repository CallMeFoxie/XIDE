namespace XIDE
{
    partial class XIDE
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XIDE));
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnNew = new System.Windows.Forms.ToolStripButton();
            this.btnOpen = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnCompile = new System.Windows.Forms.ToolStripButton();
            this.btnUpload = new System.Windows.Forms.ToolStripButton();
            this.btnConsole = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.btnClearCache = new System.Windows.Forms.ToolStripMenuItem();
            this.toolSamples = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolCommunication = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnDetect = new System.Windows.Forms.ToolStripMenuItem();
            this.lblBoard = new System.Windows.Forms.ToolStripLabel();
            this.tabTabs = new System.Windows.Forms.TabControl();
            this.btnRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.panel2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.txtLog);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 702);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1048, 60);
            this.panel2.TabIndex = 2;
            // 
            // txtLog
            // 
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Location = new System.Drawing.Point(0, 0);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(1048, 60);
            this.txtLog.TabIndex = 0;
            this.txtLog.Text = "";
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.GripMargin = new System.Windows.Forms.Padding(0);
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(30, 30);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNew,
            this.btnOpen,
            this.btnSave,
            this.toolStripSeparator2,
            this.btnCompile,
            this.btnUpload,
            this.btnConsole,
            this.toolStripSeparator1,
            this.toolStripDropDownButton1,
            this.toolSamples,
            this.toolCommunication,
            this.lblBoard});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1048, 40);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnNew
            // 
            this.btnNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNew.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.btnNew.Image = ((System.Drawing.Image)(resources.GetObject("btnNew.Image")));
            this.btnNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(34, 37);
            this.btnNew.Text = "New";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.AutoSize = false;
            this.btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOpen.Image = ((System.Drawing.Image)(resources.GetObject("btnOpen.Image")));
            this.btnOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(35, 35);
            this.btnOpen.Text = "Open";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(34, 37);
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 40);
            // 
            // btnCompile
            // 
            this.btnCompile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCompile.Image = ((System.Drawing.Image)(resources.GetObject("btnCompile.Image")));
            this.btnCompile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCompile.Name = "btnCompile";
            this.btnCompile.Size = new System.Drawing.Size(34, 37);
            this.btnCompile.Text = "Compile";
            this.btnCompile.Click += new System.EventHandler(this.btnCompile_Click);
            // 
            // btnUpload
            // 
            this.btnUpload.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnUpload.Image = ((System.Drawing.Image)(resources.GetObject("btnUpload.Image")));
            this.btnUpload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(34, 37);
            this.btnUpload.Text = "Upload";
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // btnConsole
            // 
            this.btnConsole.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnConsole.Image = ((System.Drawing.Image)(resources.GetObject("btnConsole.Image")));
            this.btnConsole.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnConsole.Name = "btnConsole";
            this.btnConsole.Size = new System.Drawing.Size(34, 37);
            this.btnConsole.Text = "Serial Console";
            this.btnConsole.Click += new System.EventHandler(this.btnConsole_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 40);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnOptions,
            this.btnClearCache});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(62, 37);
            this.toolStripDropDownButton1.Text = "Settings";
            // 
            // btnOptions
            // 
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(135, 22);
            this.btnOptions.Text = "Options";
            this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
            // 
            // btnClearCache
            // 
            this.btnClearCache.Name = "btnClearCache";
            this.btnClearCache.Size = new System.Drawing.Size(135, 22);
            this.btnClearCache.Text = "Clear cache";
            this.btnClearCache.Click += new System.EventHandler(this.btnClearCache_Click);
            // 
            // toolSamples
            // 
            this.toolSamples.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolSamples.Image = ((System.Drawing.Image)(resources.GetObject("toolSamples.Image")));
            this.toolSamples.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolSamples.Name = "toolSamples";
            this.toolSamples.Size = new System.Drawing.Size(69, 37);
            this.toolSamples.Text = "Examples";
            // 
            // toolCommunication
            // 
            this.toolCommunication.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolCommunication.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator3,
            this.btnDetect,
            this.btnRefresh});
            this.toolCommunication.Image = ((System.Drawing.Image)(resources.GetObject("toolCommunication.Image")));
            this.toolCommunication.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolCommunication.Name = "toolCommunication";
            this.toolCommunication.Size = new System.Drawing.Size(42, 37);
            this.toolCommunication.Text = "Port";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(149, 6);
            // 
            // btnDetect
            // 
            this.btnDetect.Name = "btnDetect";
            this.btnDetect.Size = new System.Drawing.Size(152, 22);
            this.btnDetect.Text = "Detect";
            this.btnDetect.Click += new System.EventHandler(this.btnDetect_Click);
            // 
            // lblBoard
            // 
            this.lblBoard.Name = "lblBoard";
            this.lblBoard.Size = new System.Drawing.Size(148, 37);
            this.lblBoard.Text = "Connected to board: None";
            // 
            // tabTabs
            // 
            this.tabTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabTabs.Location = new System.Drawing.Point(0, 40);
            this.tabTabs.Name = "tabTabs";
            this.tabTabs.SelectedIndex = 0;
            this.tabTabs.Size = new System.Drawing.Size(1048, 662);
            this.tabTabs.TabIndex = 4;
            this.tabTabs.SelectedIndexChanged += new System.EventHandler(this.tabTabs_SelectedIndexChanged);
            this.tabTabs.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tabTabs_MouseClick);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(152, 22);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // XIDE
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1048, 762);
            this.Controls.Add(this.tabTabs);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.panel2);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(500, 400);
            this.Name = "XIDE";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "XIDE 0.1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.XIDE_FormClosing);
            this.Load += new System.EventHandler(this.XIDE_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.XIDE_KeyDown);
            this.Resize += new System.EventHandler(this.XIDE_Resize);
            this.panel2.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnOpen;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnCompile;
        private System.Windows.Forms.ToolStripButton btnUpload;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem btnOptions;
        private System.Windows.Forms.ToolStripDropDownButton toolSamples;
        private System.Windows.Forms.ToolStripButton btnNew;
        private System.Windows.Forms.TabControl tabTabs;
        private System.Windows.Forms.ToolStripMenuItem btnClearCache;
        private System.Windows.Forms.ToolStripDropDownButton toolCommunication;
        private System.Windows.Forms.ToolStripButton btnConsole;
        private System.Windows.Forms.ToolStripLabel lblBoard;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem btnDetect;
        private System.Windows.Forms.ToolStripMenuItem btnRefresh;
    }
}

