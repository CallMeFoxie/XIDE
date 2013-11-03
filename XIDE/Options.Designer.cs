namespace XIDE
{
    partial class frmOptions
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
            this.checkRememberLast = new System.Windows.Forms.CheckBox();
            this.checkCompile = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.btnClose = new System.Windows.Forms.Button();
            this.selectPrefer = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // checkRememberLast
            // 
            this.checkRememberLast.AutoSize = true;
            this.checkRememberLast.Location = new System.Drawing.Point(12, 12);
            this.checkRememberLast.Name = "checkRememberLast";
            this.checkRememberLast.Size = new System.Drawing.Size(170, 17);
            this.checkRememberLast.TabIndex = 0;
            this.checkRememberLast.Text = "Remember last opened project";
            this.checkRememberLast.UseVisualStyleBackColor = true;
            // 
            // checkCompile
            // 
            this.checkCompile.AutoSize = true;
            this.checkCompile.Location = new System.Drawing.Point(12, 35);
            this.checkCompile.Name = "checkCompile";
            this.checkCompile.Size = new System.Drawing.Size(209, 17);
            this.checkCompile.TabIndex = 1;
            this.checkCompile.Text = "Automatically compile before uploading";
            this.checkCompile.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 167);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "XIDE v0.1, build 001";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(12, 188);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(135, 13);
            this.linkLabel1.TabIndex = 3;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "http://MyXBoard.net/XIDE";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(12, 127);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(105, 23);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Save && close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // selectPrefer
            // 
            this.selectPrefer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectPrefer.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.selectPrefer.FormattingEnabled = true;
            this.selectPrefer.Items.AddRange(new object[] {
            "Size",
            "Speed"});
            this.selectPrefer.Location = new System.Drawing.Point(53, 76);
            this.selectPrefer.Name = "selectPrefer";
            this.selectPrefer.Size = new System.Drawing.Size(187, 21);
            this.selectPrefer.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Prefer:";
            // 
            // frmOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 209);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.selectPrefer);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkCompile);
            this.Controls.Add(this.checkRememberLast);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmOptions";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.SizeChanged += new System.EventHandler(this.frmOptions_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkLabel1;
        public System.Windows.Forms.Button btnClose;
        public System.Windows.Forms.CheckBox checkRememberLast;
        public System.Windows.Forms.CheckBox checkCompile;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.ComboBox selectPrefer;
    }
}