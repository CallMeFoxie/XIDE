using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace XIDE
{
    public partial class XIDE : Form
    {
        bool UseLastFile = false;
        bool CompileBeforeUpload = false;
        frmOptions Options;
        Optimalize Optimalize;
        Board Board;
        bool HasEdited = false;

        XComm.XComm Comm;


        public XIDE()
        {
            InitializeComponent();
            Comm = new XComm.XComm();
        }

        string OpenedFile = "";

        private void btnOptions_Click(object sender, EventArgs e)
        {
            Options = new frmOptions();
            Options.btnClose.Click += Options_btnClose;
            Options.checkCompile.Checked = CompileBeforeUpload;
            Options.checkRememberLast.Checked = UseLastFile;
            Options.selectPrefer.SelectedIndex = (int)Optimalize;
            Options.ShowDialog();
        }

        void Options_btnClose(object sender, EventArgs e)
        {
            CompileBeforeUpload = Options.checkCompile.Checked;
            UseLastFile = Options.checkRememberLast.Checked;
            Optimalize = (Optimalize)Options.selectPrefer.SelectedIndex;

            //Compiler.Precompile(Board);
            if (File.Exists(Path.Combine("Tools", "libxboard.a")))
                File.Delete(Path.Combine("Tools", "libxboard.a"));
        }

        void btnSample_Click(object sender, EventArgs e)
        {
            OpenNewTab(((sender as ToolStripMenuItem).Tag.ToString()));
        }

        private void XIDE_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (HasEdited)
            {
                DialogResult res = MessageBox.Show("Do you want to save your code before closing XIDE?", "", MessageBoxButtons.YesNoCancel);
                if (res == System.Windows.Forms.DialogResult.Yes)
                    SaveFile(0);
                else if (res == System.Windows.Forms.DialogResult.Cancel)
                    e.Cancel = true;

            }

            SaveSettings(UseLastFile, CompileBeforeUpload, OpenedFile);
        }
        
        private void XIDE_Load(object sender, EventArgs e)
        {
            //

            // load possible samples
            if (!Directory.Exists("Examples"))
            {
                Directory.CreateDirectory("Examples");
                MessageBox.Show("It seems that your Examples folder is missing.\r\nCheck XIDE's website for new set of examples!");
            }


            foreach (DirectoryInfo dir in new DirectoryInfo("Examples").GetDirectories())
            {
                if (dir.GetFiles().Length != 0)
                {
                    ToolStripMenuItem item = new ToolStripMenuItem();
                    item.Text = dir.Name;
                    item.Text = item.Text.Replace('_', ' ');
                    foreach (FileInfo file in new DirectoryInfo(Path.Combine("Examples", dir.Name)).GetFiles("*.cpp"))
                    {
                        ToolStripMenuItem subitem = new ToolStripMenuItem(Path.GetFileNameWithoutExtension(file.Name));
                        subitem.Tag = file.FullName;
                        subitem.Click += btnSample_Click;
                        subitem.Text = subitem.Text.Replace('_', ' ');
                        item.DropDownItems.Add(subitem);
                    }

                    toolSamples.DropDownItems.Add(item);
                }
            }

            if (new DirectoryInfo("Examples").GetFiles().Length != 0)
            {
                ToolStripSeparator sp = new ToolStripSeparator();
                toolSamples.DropDownItems.Add(sp);
            }

            foreach (FileInfo file in new DirectoryInfo("Examples").GetFiles("*.cpp"))
            {
                ToolStripMenuItem item = new ToolStripMenuItem(Path.GetFileNameWithoutExtension(file.Name));
                item.Tag = file.FullName;
                item.Text = item.Text.Replace('_', ' ');
                item.Click += btnSample_Click;
                toolSamples.DropDownItems.Add(item);
            }

            if (!Directory.Exists("Tools"))
            {
                MessageBox.Show("It seems that your tools folder is missing.\r\nCheck XIDE's website for new set of tools!");
                MessageBox.Show(new DirectoryInfo(".").FullName);
            }

            if (!Directory.Exists("Projects"))
                Directory.CreateDirectory("Projects");

            LoadHints();

            LoadSettings(ref UseLastFile, ref CompileBeforeUpload, ref OpenedFile);
            LoadHighlighters();
            

            if (!UseLastFile)
            {
                //txtCode.Text = "";
            }
            else
            {
                if (!File.Exists(OpenedFile))
                {
                    MessageBox.Show("Couldn't find previous project source!\r\n Creating new one.");
                    File.Create(OpenedFile).Close();
                    //txtCode.Text = "";
                }
                else
                {
                    /*StreamReader rd = new StreamReader(OpenedFile);
                    //txtCode.Text = rd.ReadToEnd();
                    rd.Close();*/
                    //OpenNewTab(OpenedFile);
                    HasEdited = false;
                }

            }

            UpdateTitle();

            
        }

        List<SmartSyntax> Highlights = new List<SmartSyntax>();

        private void LoadHighlighters()
        {
            if (File.Exists(Path.Combine("Tools", "Highlight.xml")))
            {
                XmlTextReader rd = new XmlTextReader(Path.Combine("Tools", "Highlight.xml"));

                rd.ReadStartElement("Highlights");

                while (rd.Read())
                {
                    if(rd.NodeType == XmlNodeType.Element)
                        Highlights.Add(new SmartSyntax() { Word = rd.GetAttribute("Word"), Highlight = Color.FromArgb(
                        Convert.ToInt32(rd.GetAttribute("Red")), Convert.ToInt32(rd.GetAttribute("Green")), 
                        Convert.ToInt32(rd.GetAttribute("Blue"))), Bold = (rd.GetAttribute("Bold") == "True" ? true : false) });
                }
            }
        }

        private void LoadSettings(ref bool UseLastFile, ref bool CompileBeforeUpload, ref string OpenedFile)
        {


            if(File.Exists(Path.Combine("Tools", "Settings.xml")))
            {
                XmlReader reader = null;
                try
                {
                    reader = XmlReader.Create(Path.Combine("Tools", "Settings.xml"));
                    reader.ReadStartElement("Settings");
                    string open = reader.ReadElementString("OpenLastFile");
                    string compile = reader.ReadElementString("CompileBeforeUpload");
                    string LastFile = reader.ReadElementString("LastFile");
                    string Optim = reader.ReadElementString("Optimalize");
                    string Brd = reader.ReadElementString("Board");
                    
                    reader.Close();

                    if(open == "1")
                        UseLastFile = true;
                    else
                        UseLastFile = false;

                    if(compile == "1")
                        CompileBeforeUpload = true;
                    else
                        CompileBeforeUpload = false;

                    if (Optim == "Size")
                        Optimalize = Optimalize.Size;
                    else
                        Optimalize = Optimalize.Speed;

                    Board = (Board)Convert.ToInt32(Brd);


                    OpenedFile = LastFile;

                    if (LastFile == "" || !UseLastFile)
                    {
                        OpenedFile = FindNewNumber();
                        
                    }

                    OpenNewTab(OpenedFile);

                }
                catch(Exception)
                {
                    if (reader != null)
                        reader.Close();
                    File.Delete(Path.Combine("Tools", "Settings.xml"));
                }
            }

            if (!File.Exists(Path.Combine("Tools", "Settings.xml")))
            {
                
                UseLastFile = false;
                CompileBeforeUpload = true;
                string max = "0";

                foreach (FileInfo file in new DirectoryInfo("Projects").GetFiles("Project_*.cpp"))
                {
                    max = file.Name.Substring(8, file.Name.Length - 8 - 4);
                }
                OpenedFile = Path.Combine("Projects", "Project_" + (Convert.ToInt32(max) + 1) + ".cpp");
                SaveSettings(UseLastFile, CompileBeforeUpload, OpenedFile);

                OpenNewTab(OpenedFile);
            }
        }

        private void OpenNewTab(string OpenedFile)
        {
            TabInfo ti = new TabInfo();
            ti.File = OpenedFile;

            TabPage tp = new TabPage(new FileInfo(OpenedFile).Name);
            tabTabs.TabPages.Add(tp);

            SmartTextBox rtb = new SmartTextBox();
            rtb.AcceptsTab = true;
            rtb.Dock = DockStyle.Fill;
            rtb.LinkClicked += rtb_LinkClicked;
            rtb.Highlights = this.Highlights;
            rtb.Hints = Hints;
            rtb.Suggestions = Suggestions;

            Panel pnlParent = new Panel();
            pnlParent.Padding = new Padding(0, 0, 0, 0);
            pnlParent.Controls.Add(rtb);
            pnlParent.Dock = DockStyle.Fill;

            Panel pnlNumbers = new Panel();
            pnlNumbers.Dock = DockStyle.Left;
            pnlNumbers.Width = 35;
            pnlNumbers.Height = tabTabs.Height;

            LineLabel lLbl = new LineLabel();
            lLbl.Width = pnlNumbers.Size.Width - 3;
            lLbl.Height = pnlNumbers.Height;
            lLbl.Location = new Point(0, 1);
            lLbl.Font = new Font(FontFamily.GenericMonospace, 8);
            lLbl.MouseDown += lLbl_Click;

            if (Helper.IsUnix())
                lLbl.LineSpacing = 13f;

            pnlNumbers.Controls.Add(lLbl);
            pnlParent.Controls.Add(pnlNumbers);

            ti.Liner = lLbl;
            ti.Rtb = rtb;

            rtb.KeyDown += txtCode_KeyDown;
            rtb.SelectionTabs = new int[] { 12, 25, 37, 50 };
            rtb.Font = txtLog.Font = new System.Drawing.Font(FontFamily.GenericMonospace, 8);
            rtb.TextChanged += txtCode_TextChanged;
            rtb.VScroll += txtCode_VScroll;
            rtb.KeyPress += txtCode_KeyPress;

            rtb.Tag = ti;

            tp.Controls.Add(pnlParent);

            tp.Tag = ti;

            if (File.Exists(OpenedFile))
            {
                StreamReader rd = new StreamReader(OpenedFile);
                rtb.Text = rd.ReadToEnd();
                rd.Close();
            }

            tabTabs.SelectedTab = tp;
            ti.TitleFileName = new FileInfo(OpenedFile).Name;

            RebuildTitle(tabTabs.SelectedIndex);

        }

        void lLbl_Click(object sender, MouseEventArgs e)
        {
            //MessageBox.Show("X: " + e.X + ", Y: " + e.Y);
            // get the # of the line where is the cursor
            LineLabel lbl = (sender as LineLabel);
            int Line = lbl.minNumber;
            Line += (int)(e.Y / lbl.LineSpacing);
            //MessageBox.Show(Line.ToString());

            // TODO: Breakpoint manager!
        }

        void rtb_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }

        private string FindNewNumber()
        {
            string max = "0", NewFile;

            foreach (FileInfo file in new DirectoryInfo("Projects").GetFiles("Project_*.cpp"))
            {
                max = file.Name.Substring(8, file.Name.Length - 8 - 4);
            }

            NewFile = Path.Combine("Projects", "Project_" + (Convert.ToInt32(max) + 1) + ".cpp");
            return NewFile;
        }

        private void SaveSettings(bool UseLastFile, bool CompileBeforeUpload, string OpenedFile)
        {
            File.Delete(Path.Combine("Tools", "Settings.xml"));

            XmlWriter writer = XmlWriter.Create(Path.Combine("Tools", "Settings.xml"));
            writer.WriteStartDocument();
            writer.WriteStartElement("Settings");
            writer.WriteElementString("OpenLastFile", (UseLastFile ? "1" : "0"));
            writer.WriteElementString("CompileBeforeUpload", (CompileBeforeUpload ? "1" : "0"));
            writer.WriteElementString("LastFile", OpenedFile);
            writer.WriteElementString("Optimalize", Optimalize == Optimalize.Size ? "Size" : "Speed");
            writer.WriteElementString("Board", ((int)Board).ToString());
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }

        private void UpdateTitle()
        {
            //this.Text = "XIDE - " + OpenedFile;
            this.Text = "XIDE";

            //UpdateNumberLabels();
        }

        private void XIDE_Resize(object sender, EventArgs e)
        {
            
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.InitialDirectory = "Projects";
            ofd.Filter = "C++ files|*.cpp";

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                OpenedFile = ofd.FileName;

                OpenNewTab(OpenedFile);

                UpdateTitle();
            }
            HasEdited = false;
        }

        private void btnCompile_Click(object sender, EventArgs e)
        {
            if((File.GetAttributes((tabTabs.SelectedTab.Tag as TabInfo).File) & FileAttributes.ReadOnly) != FileAttributes.ReadOnly)
                SaveFile(tabTabs.SelectedIndex);

            txtLog.Text = Compiler.Compile((tabTabs.SelectedTab.Tag as TabInfo).File, new CompilerSettings { Optimalize = Optimalize, OutputDisassembly = false, Board = Board });
            txtLog.SelectionStart = txtLog.Text.Length - 1;
            txtLog.ScrollToCaret();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFile(tabTabs.SelectedIndex);
        }

        private void SaveFile(int p)
        {
            string OpenedFile = (tabTabs.TabPages[p].Tag as TabInfo).File;
            if (File.Exists(OpenedFile))
                File.Delete(OpenedFile);
            else
            { // let user pick his own file name
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "XIDE";
                sfd.DefaultExt = ".cpp";
                sfd.AddExtension = true;
                sfd.InitialDirectory = Path.GetFullPath("Projects");
                sfd.Filter = "C++ Files|*.cpp";
                

                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    OpenedFile = sfd.FileName;
                    (tabTabs.TabPages[p].Tag as TabInfo).File = sfd.FileName;
                    tabTabs.TabPages[p].Text = new FileInfo(sfd.FileName).Name;
                    (tabTabs.TabPages[p].Tag as TabInfo).TitleFileName = new FileInfo(sfd.FileName).Name;
                }
            }

            StreamWriter wr = new StreamWriter(OpenedFile);
            wr.Write((tabTabs.TabPages[p].Tag as TabInfo).Rtb.Text);
            wr.Flush();
            wr.Close();

            RebuildTitle(p);

            (tabTabs.TabPages[p].Tag as TabInfo).MadeChanges = false;
            RebuildTitle(p);
        }

        private void RebuildTitle(int p)
        {
            string OpenedFile = (tabTabs.TabPages[p].Tag as TabInfo).File;

            tabTabs.TabPages[p].Text = (tabTabs.TabPages[p].Tag as TabInfo).TitleFileName;

            if (File.Exists(OpenedFile))
            {
                if ((File.GetAttributes(OpenedFile) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    tabTabs.TabPages[p].Text += " [RO]";

                if (Directory.Exists("Examples"))
                    if (Path.GetFullPath(OpenedFile).Substring(0, Path.GetFullPath("Examples").Length) == Path.GetFullPath("Examples"))
                    {
                        string FileName = new FileInfo(OpenedFile).Name;
                        tabTabs.TabPages[p].Text = "Example: " + FileName.Replace('_', ' ').Substring(0, FileName.Length - 4);

                    }
            }

        }

        #region XIDE Line numbering
        private void txtCode_TextChanged(object sender, EventArgs e)
        {
            UpdateNumberLabels(sender as SmartTextBox);
            /*if (!HasEdited)
                this.Text += " *";-*/

            TabInfo ti = tabTabs.SelectedTab.Tag as TabInfo;

            if (!ti.MadeChanges)
                tabTabs.SelectedTab.Text += " *";

            ti.MadeChanges = true;
        }

        private void txtCode_VScroll(object sender, EventArgs e)
        {
            UpdateNumberLabels(sender as SmartTextBox);
            //txtLineNumbers.AutoScrollOffset = txtCode.AutoScrollOffset;
            
        }

        private void UpdateNumberLabels(SmartTextBox rtb)
        {
            LineLabel lLbl = (rtb.Tag as TabInfo).Liner;

            int Start = rtb.GetLineFromCharIndex(rtb.GetCharIndexFromPosition(new Point(0, 0)));
            int Stop = rtb.GetLineFromCharIndex(rtb.GetCharIndexFromPosition(new Point(0, rtb.ClientRectangle.Height)));

            if (lLbl != null)
            {
                lLbl.minNumber = Start + 1;
                lLbl.maxNumber = Stop + 1;

                lLbl.Invalidate();

                if (Helper.IsUnix())
                {
                    lLbl.Location = new Point(0, (int)Math.Ceiling((decimal)rtb.GetPositionFromCharIndex(0).Y % (rtb.Font.Height)));
                }
                else
                {
                    int d = rtb.GetPositionFromCharIndex(0).Y % (rtb.Font.Height);
                    lLbl.Location = new Point(0, (int)Math.Ceiling((decimal)rtb.GetPositionFromCharIndex(0).Y % (rtb.Font.Height + 1)) + 2);
                }
            }
        }

        #endregion

        private void txtCode_KeyDown(object sender, KeyEventArgs e)
        {
            SmartTextBox rtb = (sender as SmartTextBox);
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            OpenNewTab(FindNewNumber());
            SaveFile(tabTabs.SelectedIndex);
         
        }

        private void XIDE_KeyDown(object sender, KeyEventArgs e)
        {
            /*if (e.KeyCode == Keys.N)
                MessageBox.Show(e.KeyCode);*/

            if (e.KeyCode == Keys.F5)
            { // Build
                btnUpload_Click(null, EventArgs.Empty);
            }
            if (e.KeyCode == Keys.F6)
            {
                btnCompile_Click(null, EventArgs.Empty);
            }
        }

        private void txtCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(File.Exists((tabTabs.SelectedTab.Tag as TabInfo).File))
            {
                if ((File.GetAttributes((tabTabs.SelectedTab.Tag as TabInfo).File) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    e.Handled = true;
                    return;
                }
            }

            if (!((tabTabs.SelectedTab.Tag as TabInfo).MadeChanges))
                tabTabs.SelectedTab.Text += " *";

            (tabTabs.SelectedTab.Tag as TabInfo).MadeChanges = true;

        }


        private void tabTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabTabs.SelectedTab == null)
                return;

            if (tabTabs.SelectedTab.Tag != null)
            {
                if (File.Exists((tabTabs.SelectedTab.Tag as TabInfo).File))
                {
                    FileAttributes attr = File.GetAttributes((tabTabs.SelectedTab.Tag as TabInfo).File);

                    if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        btnSave.Enabled = false;
                    else
                        btnSave.Enabled = true;
                }
                else
                { // new file
                    btnSave.Enabled = true;
                }

                (tabTabs.SelectedTab.Tag as TabInfo).Rtb.Focus();
            }
        }

        private void tabTabs_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                if (CloseTab(tabTabs.SelectedIndex))
                {
                    
                }
            }
        }

        private bool CloseTab(int p)
        {
            if ((tabTabs.TabPages[p].Tag as TabInfo).MadeChanges)
            {
                switch (MessageBox.Show("Do you want to save changes before closing the file first?", "Save", MessageBoxButtons.YesNoCancel))
                {
                    case System.Windows.Forms.DialogResult.Cancel:
                        return false;
                    case System.Windows.Forms.DialogResult.Yes:
                        SaveFile(p);
                        tabTabs.TabPages.RemoveAt(p);
                        return true;
                    case System.Windows.Forms.DialogResult.No:
                        tabTabs.TabPages.RemoveAt(p);
                        return true;
                }
            }
            else
            {
                tabTabs.TabPages.RemoveAt(p);
                return true;
            }

            return false;
        }

        private void btnClearCache_Click(object sender, EventArgs e)
        {
            Compiler.DeleteCache();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {

        }

        private void btnConsole_Click(object sender, EventArgs e)
        {

        }

        private void btnDetect_Click(object sender, EventArgs e)
        {
            Comm.DetectBoard();
            if (Comm != null)
            {
                lblBoard.Text = "Connected to board: " + Comm.Board.Board.ToString() + " (" + Comm.Board.BLVersion + ")";
            }
            else
            {
                lblBoard.Text = "No boards found";
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {

        }

    }

    class TabInfo
    {
        public string File;
        public LineLabel Liner;
        public SmartTextBox Rtb;
        public bool MadeChanges = false;
        public string TitleFileName;
    }
}
