using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace XIDE
{
    public partial class XIDE
    {

        List<Hint> Hints;
        List<SmartWord> Suggestions;

        private void LoadHints()
        {
            Hints = new List<Hint>();
            Suggestions = new List<SmartWord>();

            foreach (FileInfo File in new DirectoryInfo(Path.GetFullPath("Libs")).GetFiles("*.h"))
            {
                ParseHintFile(File.FullName);
            }

        }

        private void ParseHintFile(string FileName)
        {
            StreamReader rd = new StreamReader(FileName);

            Position Pos = Position.Nothing;

            Hint nHint = new Hint();
            SmartWord SW = new SmartWord();

            string LastParentClass = null;

            while (rd.Peek() > 0)
            {
                string Line = rd.ReadLine().Trim();

                if (Line.Length > 5)
                {
                    if (Line.Substring(0, 3) == "///")
                    {
                        string[] Parts;
                        switch (Pos)
                        {
                            case Position.Nothing: // first ///
                                nHint = new Hint();
                                SW = new SmartWord();
                                nHint.Params = new Dictionary<string, string>();
                                SW.Params = new Dictionary<string, string>();

                                Parts = Line.Substring(3).Trim().Split(new char[] { ':' }, 2);
                                Color HighColor;

                                nHint.Text = Parts[1].Trim();
                                Pos = Position.Params;
                                // add to highliting list since we have got function name now
                                
                                SW.Description = nHint.Text;

                                if (Parts[0].Length > "const".Length && Parts[0].Trim().Substring(0, "const".Length) == "const")
                                {
                                    nHint.Word = Parts[0].Substring("const".Length).Trim();
                                    HighColor = Color.HotPink;
                                    SW.Type = SmartWord.SWType.Constant;
                                }
                                else if (Parts[0].Length > "class".Length && Parts[0].Trim().Substring(0, "class".Length) == "class")
                                {
                                    nHint.Word = Parts[0].Substring("class".Length).Trim();
                                    HighColor = Color.Orange;
                                    SW.Type = SmartWord.SWType.Class;
                                    LastParentClass = nHint.Word;
                                }
                                else if (Parts[0].Length > "static".Length && Parts[0].Trim().Substring(0, "static".Length) == "static")
                                {
                                    nHint.Word = Parts[0].Substring(Parts[0].LastIndexOf(" ")).Trim();
                                    HighColor = Color.Brown;
                                    SW.Type = SmartWord.SWType.StaticMethod;
                                    SW.ParentClass = Parts[0].Substring("static".Length, Parts[0].Length - "static".Length - nHint.Word.Length).Trim();
                                    for (int i = 0; i < Suggestions.Count; i++)
                                    {
                                        if (Suggestions[i].Word == LastParentClass && Suggestions[i].Type == SmartWord.SWType.Class)
                                            Suggestions[i].HasStatic = true;
                                    }
                                }
                                else if (Parts[0].Trim().LastIndexOf(" ") > -1 )
                                {
                                    HighColor = Color.Red;
                                }
                                else
                                {
                                    nHint.Word = Parts[0].Trim();
                                    HighColor = Color.BlueViolet;
                                    SW.Type = SmartWord.SWType.Function;
                                }
                                SW.Word = nHint.Word;


                                Highlights.Add(new SmartSyntax() { Bold = false, Highlight = HighColor, Word = nHint.Word });
                                
                               
                                break;
                            case Position.Params:
                                Parts = Line.Substring(3).Trim().Split(new char[] { ':' }, 2);
                                nHint.Params.Add(Parts[0].Trim(), Parts[1].Trim());
                                SW.Params.Add(Parts[0].Trim(), Parts[1].Trim());
                                break;
                        }
                    }
                    else
                    {
                        if (Pos != Position.Nothing)
                        {
                            Hints.Add(nHint);
                            Pos = Position.Nothing;
                            Suggestions.Add(SW);
                        }
                    }
                }
            }

            
        }

        enum Position
        {
            Nothing,
            Params,
            Function
        };
    }

    class Hint
    {
        public string Word;
        public string Text;
        public Dictionary<string, string> Params;
    }
}
