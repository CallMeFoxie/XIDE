using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace XIDE
{
    class SmartTextBox : RichTextBox
    {
        int PreviousLine;
        public List<SmartSyntax> Highlights;
        int PreviousLines;
        public List<Hint> Hints;
        Panel HintPanel;
        Label HintLabel;
        Panel SuggestionPanel;
        ComboBox SuggestionList;
        public List<SmartWord> Suggestions;

        public SmartTextBox()
        {
            PreviousLine = 0;
            Highlights = new List<SmartSyntax>();
            Hints = new List<Hint>();
            PreviousLines = 0;
            HintPanel = new Panel();
            HintPanel.BackColor = Color.Black;
            HintLabel = new Label();
            HintPanel.Controls.Add(HintLabel);
            HintPanel.Visible = false;
            HintLabel.ForeColor = Color.White;
            HintLabel.Location = new Point(2, 2);

            SuggestionPanel = new Panel();
            SuggestionList = new ComboBox();

            SuggestionPanel.Controls.Add(SuggestionList);
            SuggestionPanel.Size = new Size(200, 400);
            SuggestionList.Size = SuggestionPanel.Size;
            SuggestionList.Location = new Point(0, 0);

            SuggestionList.FlatStyle = FlatStyle.Flat;

            SuggestionPanel.Visible = false;
            SuggestionList.DropDownStyle = ComboBoxStyle.DropDownList;
            SuggestionList.Enabled = false;

            this.Controls.Add(HintPanel);
            this.Controls.Add(SuggestionPanel);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if(EnablePaint)
                base.OnPaint(e);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            if (this.Lines.Length != 0)
            {
                int line = GetLineFromCharIndex(GetFirstCharIndexOfCurrentLine());

                if (line > PreviousLine)
                {
                    for (int i = PreviousLine; i <= line; i++)
                        ProcessLine(i);
                }
                else// if(line < PreviousLine)
                {
                    ProcessLine(line);
                }

                if (PreviousLines != Lines.Length)
                {
                    for (int i = line; i < Lines.Length; i++)
                        ProcessLine(i);
                }

                PreviousLine = line;
            }

            PreviousLines = Lines.Length;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0F) // WM_PAINT
                if (EnablePaint)
                    base.WndProc(ref m);
                else
                    m.Result = IntPtr.Zero;
            else
                base.WndProc(ref m);
        }

        bool EnablePaint = true;

        private void ProcessLine(int i)
        {
            int selection = SelectionStart;
            Color col = SelectionColor;

            EnablePaint = false;

            // First check for variables..
            SelectionStart = GetLineOffset(i);
            SelectionLength = Lines[i].Length;
            SelectionColor = Color.Black;
            SelectionFont = this.Font;

            foreach (SmartSyntax high in Highlights)
            {
                Regex regKw = new Regex(high.Word, RegexOptions.IgnoreCase);
                Match regMatch = regKw.Match(this.Lines[i]);

                for (; regMatch.Success; regMatch = regMatch.NextMatch())
                {
                    int nStart = regMatch.Index + GetLineOffset(i);
                    int Stop = high.Word.Length;
                    SelectionStart = nStart;
                    SelectionLength = Stop;
                    SelectionColor = high.Highlight;
                    if(high.Bold)
                        SelectionFont = new Font(this.Font, FontStyle.Bold);
                }
            }

            SelectionStart = selection;
            SelectionLength = 0;
            SelectionColor = col;
            EnablePaint = true;

            // check for params offer. TODO?
            // line end
            int lineEnd = GetLineOffset(i) + Lines[i].Length;
            int lineStart = GetLineOffset(i);

            if (lineEnd == SelectionStart)
            { // we are on the end of the line.
                // find last ( 
                int foundAt = -1;
                for (int j = (SelectionStart - lineStart) - 1; j > 0; j--)
                {
                    if (Lines[i][j] == ')')
                        break;

                    if (Lines[i][j] == '(')
                    {
                        foundAt = j;
                        break;
                    }
                }

                if (foundAt > 0)
                {
                    int startAt = 0;

                    for (int j = /*(SelectionStart - foundAt - lineStart - 1)*/ foundAt - 1; j >= 1; j--)
                    {

                        if (Lines[i][j] == '=' || Lines[i][j] == ' ' || Lines[i][j] == '|' || Lines[i][j] == '&' || Lines[i][j-1] == '(') // read till the previous =| 
                        {
                            startAt = j;
                            break;
                        }
                    }

                    if (startAt >= 0)
                    {
                        string function = Lines[i].Substring(startAt, foundAt - startAt).Trim();
                        //foreach (Hint hint in Hints)
                        foreach(SmartWord Word in Suggestions)
                        {
                            string SearchFor = Word.Word;
                            if (Word.ParentClass != null)
                                SearchFor = Word.ParentClass + "::" + SearchFor;
                            //if (hint.Word == function)
                            if(SearchFor == function)
                            {
                                Graphics g = HintLabel.CreateGraphics();
                                //HintLabel.Text = hint.Text;
                                HintLabel.Text = Word.Description;
                                //foreach (KeyValuePair<string, string> param in hint.Params)
                                foreach(KeyValuePair<string, string> param in Word.Params)
                                {
                                    HintLabel.Text += "\r\n > " + param.Key + ": " + param.Value;
                                }

                                HintLabel.Size = new Size(Convert.ToInt32(g.MeasureString(Word.Description, HintLabel.Font).Width *1.1f), 
                                    /*Convert.ToInt32(g.MeasureString(hint.Text, HintLabel.Font).Height * hint.Params.Count + 15f)*/
                                    MeasureLabelSize(HintLabel.Text, HintLabel.Font, 200));

                                
                                HintPanel.Size = new Size(HintLabel.Width + 4, HintLabel.Height + 4);

                                int left = GetPositionFromCharIndex(SelectionStart).X - HintPanel.Width / 2;
                                if(left < 0) 
                                    left = 0;

                                HintPanel.Location = new Point(left, 
                                    (int)(GetPositionFromCharIndex(SelectionStart).Y + Font.SizeInPoints * 2f));
                                HintPanel.Visible = true;
                            }
                        }
                    }
                    else
                    {
                        HintPanel.Visible = false;
                    }
                }
                else
                {
                    HintPanel.Visible = false;
                }


                //find previous last nonA-Z character

                //int NumChars = 0;
                string Line = Lines[i].ToUpper();
                /*
                for (int j = (SelectionStart - lineStart) - 1; j >= 0; j--)
                {
                    if (Line[j] < 'A' || Line[j] > 'Z')
                        break;
                    else
                        NumChars++;
                }
                */
                //MessageBox.Show(NumChars.ToString());

                bool Found = false;

                string LookFor = FindBaseWord(Line, SelectionStart - lineStart).Trim();

                SuggestionList.Items.Clear();

                if (LookFor != "")
                {
                    //string LookFor = Line.Substring(SelectionStart - lineStart - NumChars, NumChars);

                    //MessageBox.Show("LookFor: " + LookFor + "\r\nFindBase: " + FindBaseWord(Line, SelectionStart - lineStart));
                    foreach (SmartWord Word in Suggestions)
                    {
                        if (Word.Word.Length >= LookFor.Length)
                        {
                            if (Word.Word.ToUpper().Substring(0, LookFor.Length) == LookFor)
                            {
                                string Class = FindLastClass(Text, SelectionStart - lineStart).Trim();
                                if (Class == "")
                                {

                                    //MessageBox.Show("Suggest: " + Word.Word);
                                    if (Word.Type == SmartWord.SWType.Method) // for future when parsing user's .cpp file for instances of objects and other variables
                                    {

                                    }
                                    else if (Word.Type == SmartWord.SWType.StaticMethod)
                                    {

                                    }
                                    else
                                    {
                                        Found = true;

                                        SuggestionList.Items.Add(Word.Word);
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        //if (Line.Substring(SelectionStart - lineStart - NumChars - 2, 2) == "::")
                                        //if(Line.Substring(SelectionStart - lineStart - 

                                        {
                                            //string BaseClass = Line.Substring(
                                            //MessageBox.Show(FindLastClass(Line, SelectionStart - lineStart));

                                            if (Word.ParentClass.ToUpper() == Class.ToUpper())
                                            {
                                                SuggestionList.Items.Add(Word.Word);
                                                Found = true;
                                            }
                                        }
                                    }
                                    catch (Exception) { }
                                }
                            }
                        }
                    }
                }

                if (SelectionStart - lineStart > 3)
                {
                    string Class = FindLastClass(Line, SelectionStart - lineStart).Trim();
                    //MessageBox.Show(Class);
                    if (Class != "")
                    {
                        foreach (SmartWord wrd in Suggestions)
                        {
                            if (wrd.ParentClass != null && wrd.ParentClass.ToUpper() == Class.ToUpper() && wrd.Type == SmartWord.SWType.StaticMethod)
                            {
                                SuggestionList.Items.Add(wrd.Word);
                                Found = true;
                            }
                        }
                    }
                }

                if (!Found)
                {
                    SuggestionPanel.Visible = false;
                    SuggestionList.DroppedDown = false;
                }
                else
                {
                    SuggestionPanel.Visible = true;
                    SuggestionList.SelectedIndex = 0;
                    if(SuggestionList.Items.Count > 1)
                        SuggestionList.DroppedDown = true;


                    if (HintPanel.Visible)
                    {
                        SuggestionPanel.Location = new Point(HintPanel.Location.X, HintPanel.Location.Y + HintPanel.Height);
                    }
                    else
                    {
                        int left = GetPositionFromCharIndex(SelectionStart).X - HintPanel.Width / 2;
                        if (left < 0)
                            left = 0;

                        SuggestionPanel.Location = new Point(left, (int)(GetPositionFromCharIndex(SelectionStart).Y + Font.SizeInPoints * 2f));
                        
                    }

                }

            }

        }

        int PreviousPosition;

        private string FindBaseWord(string Text, int MaxPosition)
        {
            Text = Text.Substring(0, MaxPosition);
            int index = Text.LastIndexOfAny(new char[] { ' ', '}', '(', ')', '\n', ':' });
            if (index > 0)
                index++;

            if (index == -1) index = 0;

            return Text.Substring(index);
        }

        private string FindLastClass(string text, int MaxPosition)
        {
            int index = text.LastIndexOf("::");
            int FunctionStart = text.LastIndexOfAny(new char[] { ' ', '}', '{', '(', ')', '\n' });
            if (FunctionStart > index)
                return "";
            //index = MaxPosition;

            if (index == -1)
                //    return "";
                index = text.Length;

            string Word = FindBaseWord(text, index);
            //return Word.Substring(0, index);
            return Word;
        }

        protected override void OnSelectionChanged(EventArgs e)
        {
            //if (PreviousPosition != null)
            {
                if (PreviousPosition != SelectionStart)
                    HintPanel.Visible = false;
            }

            PreviousPosition = SelectionStart;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            EnablePaint = false;

            if (e.KeyCode == Keys.Enter)
            {
                if (SuggestionPanel.Visible)
                {
                    string Word = SuggestionList.SelectedItem.ToString();

                    int lineStart = GetLineOffset(GetLineFromCharIndex(GetFirstCharIndexOfCurrentLine()));

                    string Line = this.Lines[GetLineFromCharIndex(GetFirstCharIndexOfCurrentLine())].ToUpper();

                    int NumChars = 0;

                    for (int j = (SelectionStart - lineStart) - 1; j >= 0; j--)
                    {
                        if (Line[j] < 'A' || Line[j] > 'Z')
                            break;
                        else
                            NumChars++;
                    }

                    SelectedText = Word.Substring(NumChars);
                    


                    this.SuggestionPanel.Visible = false;
                    this.SuggestionList.DroppedDown = false;

                    foreach (SmartWord w in Suggestions)
                    {
                        if (w.Word == Word)
                            switch (w.Type)
                            {
                                case SmartWord.SWType.Class:
                                    if (!w.HasStatic)
                                    {
                                        int pos = SelectionStart;
                                        SelectedText = "  = new " + w.Word + "();\n";
                                        SelectionStart = pos + 1;
                                    }
                                    break;
                                case SmartWord.SWType.Constant:
                                    
                                    break;
                                case SmartWord.SWType.Function:
                                    SelectedText = "(";
                                    if (w.Params.Count == 0)
                                        SelectedText = ");";
                                    e.Handled = true;
                                    e.SuppressKeyPress = true;
                                    break;
                                case SmartWord.SWType.Method:

                                    break;
                                case SmartWord.SWType.StaticMethod:

                                    break;
                                case SmartWord.SWType.Datatype:

                                    break;
                                case SmartWord.SWType.Variable:

                                    break;
                            }

                    }

                    e.Handled = true;
                }
            }
            else if ((e.KeyCode == Keys.Up || e.KeyCode == Keys.Down) && SuggestionPanel.Visible)
            {
                if (e.KeyCode == Keys.Up && SuggestionList.SelectedIndex > 0)
                    SuggestionList.SelectedIndex--;
                else if (e.KeyCode == Keys.Down && SuggestionList.SelectedIndex < SuggestionList.Items.Count - 1)
                    SuggestionList.SelectedIndex++;

                e.Handled = true;
            }

            if (!e.Handled)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.SelectionStart != 0)
                    {
                        int index = this.GetFirstCharIndexOfCurrentLine();
                        int line = this.GetLineFromCharIndex(index);
                        int tabs = 0;
                        if (this.Lines[line].Length > 0)
                        {
                            for (int i = 0; i < this.Lines[line].Length; i++)
                            {
                                if (this.Lines[line].Substring(i, 1) == "\t")
                                    tabs++;
                                else
                                    break;
                            }

                            if (this.Lines[line].Trim().Length > 0)
                            {
                                if (this.Lines[line].Trim().Substring(this.Lines[line].Trim().Length - 1) == "{")
                                    tabs++;
                            }
                        }

                        this.SelectedText = "\n";
                        for (int i = 0; i < tabs; i++)
                        {
                            this.SelectedText = "\t";
                        }
                    }
                    e.SuppressKeyPress = true;
                }
                else
                {

                    if (e.KeyCode == Keys.N && e.Modifiers == (Keys.Control | Keys.Alt)) // }
                    {
                        ProcessEndBracket();
                    }
                }
            }

            EnablePaint = true;

            if(!e.Handled)
                base.OnKeyDown(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
        }

        private void ProcessEndBracket()
        {
            int index = this.GetFirstCharIndexOfCurrentLine();
            int line = this.GetLineFromCharIndex(index);
            if (this.Lines[line].Trim() == "" && this.Lines[line].Length > 0)
            {
                string newLine = this.Lines[line].Substring(1);
                //MessageBox.Show(txtCode.Lines[line].Length + " => " + newLine.Length);
                string[] lines = this.Lines;
                lines[line] = newLine;
                this.Lines = lines;
                this.SelectionStart = index + newLine.Length;
                //MessageBox.Show("index: " + index.ToString() + "newLine.Length: " + newLine.Length.ToString());

            }
        }

        private static int MeasureLabelSize(string Text, Font Font, int MaxWidth)
        {
            Form frm = new Form();
            Graphics g = frm.CreateGraphics();
            StringFormat sf = new StringFormat();
            RectangleF rf = new RectangleF(0, 0, MaxWidth, 10000);
            CharacterRange[] ranges = { new CharacterRange(0, Text.Length) };
            Region[] regions = new Region[1];

            sf.SetMeasurableCharacterRanges(ranges);
            regions = g.MeasureCharacterRanges(Text, Font, rf, sf);

            return (int)(regions[0].GetBounds(g).Bottom + 1.0f);
        }

        private int GetLineOffset(int l)
        {
            int ret = 0;
            for (int i = 0; i < l; i++)
            {
                ret += Lines[i].Length + 1;
            }
            return ret;
        }
    }

    class SmartWord
    {
        public enum SWType
        {
            Function,
            Constant,
            Class,
            StaticMethod,
            Method,
            Variable,
            Datatype
        }

        public string Word;
        public string Description;
        public SWType Type;
        //public short Params;
        public Dictionary<string, string> Params;
        public string ParentClass;
        public int DefinedAt = -1;
        public bool HasStatic = false;
    }

    class SmartSyntax
    {
        public string Word;
        public Color Highlight;
        public bool Bold;
    }
}
