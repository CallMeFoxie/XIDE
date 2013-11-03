using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Text.RegularExpressions;
using System.Xml;
namespace XIDE
{
    enum Optimalize { Size, Speed };
    enum Chip { a4u, a3u, a1u };
    enum Flash { F32, F128, F256 };
    enum Board { Ultra, MiniDuino, Mini, cocoDuino, coco };

    struct CompilerSettings
    {
        public Optimalize Optimalize;
        public Chip Chip;
        public Flash Flash;
        public bool OutputDisassembly;
        public Board Board;
        public string BoardDefine;
    }
    class Compiler
    {
        static string ToolsBase = Path.GetFullPath(Path.Combine(new DirectoryInfo(".").FullName, "Tools"));
        static string ToolsDir = Path.Combine(ToolsBase, "avr");
        static string Libs = Path.Combine(ToolsDir, "Libs");
        static string Gcc = Path.Combine(Path.Combine(ToolsDir, "bin"), "avr-g++");
        static string Objcopy = Path.Combine(Path.Combine(ToolsDir, "bin"), "avr-objcopy");
        static string Objdump = Path.Combine(Path.Combine(ToolsDir, "bin"), "avr-objdump");
        static string Ar = Path.Combine(Path.Combine(ToolsDir, "bin"), "avr-ar");
        //const string[] CheckToolsPrograms = { "gcc", "objcopy" };


        public static string Compile(string CompileFile, CompilerSettings Options)
        {
            //CheckTools();

            string Output = "";

            if (!File.Exists(Path.Combine("Tools", "libxboard.a")))
            {
                Output += "Precompiled XBoard library not found! Compiling it first... ";
                Precompile(Options.Board);
                Output += "done\r\n";
            }

            Output += "Compiling...\r\n";

            if(!Directory.Exists("Temp"))
                Directory.CreateDirectory("Temp");

            ClearDirectory("Temp");

            CopyFiles(CompileFile);

            SetCompiler(Options.Board, ref Options);

            string mmcu = GenerateMmcu(Options);

            string gcc = InvokeGcc("-funsigned-char -funsigned-bitfields " + (Options.Optimalize == Optimalize.Size ? "-Os" : "-O3") +
                " -fpack-struct -fshort-enums -ffunction-sections -mshort-calls -g0 -Wall -o \"" + Path.GetFullPath(Path.Combine("Temp", "Output.o")) + "\" -Wl,-lm " +
                " -I \"" + Path.Combine(Path.GetFullPath("."), "Libs") + "\" -D_XBOARD_" + Options.BoardDefine + "_ " +
                "-Wl,--gc-sections -mmcu=" + mmcu + " \"" + Path.GetFullPath(Path.Combine("Temp", "Code.c")) + "\" " +
                "\"" + Path.GetFullPath(Path.Combine("Temp", "Main.c")) + "\" -L \"" + Path.GetFullPath("Tools") + "\" -lxboard" );

            if (File.Exists(Path.Combine("Temp", "Output.o")))
            {
                Output += "Compilation OK. Getting binary content...";

                string objcopy = InvokeObjcopy("-O binary -R .eeprom -R .fuse -R .lock -R .signature \"" + Path.GetFullPath(Path.Combine("Temp", "Output.o")) + "\" \"" + Path.GetFullPath(Path.Combine("Temp", "Flash.bin")) + "\"");

                if (File.Exists(Path.Combine("Temp", "Flash.bin")))
                {
                    if (Options.OutputDisassembly)
                        InvokeObjdump("-h -S \"" + Path.GetFullPath(Path.Combine("Temp", "Output.o")) + "\"");

                    Output += "\r\nBinary extracted. File size: " + new FileInfo(Path.Combine("Temp", "Flash.bin")).Length + "B";
                }
                else
                {
                    Output += "\r\nFailed getting binary image!\r\n\r\n" + objcopy;
                }
            }
            else
            {
                Output += "Compilation failed!\r\n";

                // remove file path from the gcc output and move it by lines(Header.c) up

                gcc = gcc.Replace(Path.GetFullPath(Path.Combine("Temp", "Code.c")) + ":", "> ");
                gcc = Regex.Replace(gcc, @"\d+:\d+: error", m => { return "Line " + (Convert.ToInt32(m.Groups[0].Value.ToString().Split(':')[0]) - 3).ToString()/* + ":" + (m.Groups[0].Value.ToString().Split(':')[1]).ToString()*/; });
                Output += gcc;
            }

            return Output;
        }

        public static void DeleteCache()
        {
            if (File.Exists(Path.Combine("Tools", "libxboard.a")))
                File.Delete(Path.Combine("Tools", "libxboard.a"));
        }

        private static string GenerateMmcu(CompilerSettings Options)
        {
            string mmcu = "atxmega";
            switch (Options.Flash)
            {
                case Flash.F128: mmcu += "128"; break;
                case Flash.F256: mmcu += "256"; break;
                case Flash.F32: mmcu += "32"; break;
            }

            switch (Options.Chip)
            {
                case Chip.a1u: mmcu += "a1u"; break;
                case Chip.a3u: mmcu += "a3u"; break;
                case Chip.a4u: mmcu += "a4u"; break;
            }

            return mmcu;
        }

        private static string InvokeObjdump(string p)
        {
            StreamWriter list = new StreamWriter(Path.GetFullPath(Path.Combine("Temp", "Dis.lss")));

            string x = InvokeProcess(Objdump, p, list);
            list.Close();

            return x;
        }

        private static string InvokeObjcopy(string p)
        {
            
            return InvokeProcess(Objcopy, p);

            
        }

        private static void CopyFiles(string CFile)
        {
            File.Copy(Path.Combine(Path.Combine(ToolsBase, "BaseCode"), "Main.c"), Path.Combine("Temp", "Main.c"));
            File.Copy(Path.Combine(Path.Combine(ToolsBase, "BaseCode"), "Code.h"), Path.Combine("Temp", "Code.h"));

            StreamReader Rd = new StreamReader(CFile);
            StreamReader RdHeader = new StreamReader(Path.Combine(Path.Combine("Tools", "BaseCode"), "Header.c"));
            StreamWriter Wr = new StreamWriter(Path.Combine("Temp", "Code.c"));

            Wr.Write(RdHeader.ReadToEnd());
            Wr.Write(Rd.ReadToEnd());

            Wr.Close();
            RdHeader.Close();
            Rd.Close();

        }

        private static string InvokeGcc(string p)
        {
            return InvokeProcess(Gcc, p);
        }

        private static string InvokeAr(string p)
        {
            return InvokeProcess(Ar, p);
        }

        private static string InvokeProcess(string bin, string p, StreamWriter Output = null, StreamWriter ErrOutput = null)
        {
            Process pr = new Process();
            pr.StartInfo.Arguments = p;
            pr.StartInfo.CreateNoWindow = true;
            pr.StartInfo.FileName = bin;
            pr.StartInfo.RedirectStandardError = true;
            pr.StartInfo.RedirectStandardOutput = true;
            pr.StartInfo.WorkingDirectory = Path.Combine(ToolsDir, "bin");
            pr.StartInfo.UseShellExecute = false;
            pr.Start();

            StreamReader err = pr.StandardError;

            while (!pr.HasExited) Thread.Sleep(500);

            string errM = err.ReadToEnd();
            string outM = pr.StandardOutput.ReadToEnd();

            if (Output != null)
            {
                Output.Write(outM);
                Output.Write(errM);
            }

            pr.Close();

            return errM;
        }


        /*private bool CheckTools()
        {
            foreach(string Tool in CheckToolsPrograms)
            {
                if(!File.Exists(Path.Combine(Path.Combine(Gcc, "bin"), Tool)))
                {
                    MessageBox.Show("Missing tool! Re-download them from XBoard website.");
                    return false;
                }
            }

            return true;
        }*/

        private static void ClearDirectory(string p)
        {
            foreach (DirectoryInfo dir in new DirectoryInfo(p).GetDirectories())
            {
                ClearDirectory(dir.FullName);
                dir.Delete();
            }
            foreach (FileInfo file in new DirectoryInfo(p).GetFiles())
                file.Delete();

        }

        private static void SetCompiler(Board Board, ref CompilerSettings Options)
        {
            switch (Board)
            {
                case global::XIDE.Board.coco:
                case global::XIDE.Board.cocoDuino:
                    Options.Flash = Flash.F256;
                    Options.Chip = Chip.a3u;
                    break;
                case global::XIDE.Board.Mini:
                case global::XIDE.Board.MiniDuino:
                    Options.Chip = Chip.a4u;
                    //Options.Flash = Flash.F32;
                    Options.Flash = Flash.F128;
                    break;
                case global::XIDE.Board.Ultra:
                    Options.Flash = Flash.F128;
                    Options.Chip = Chip.a1u;
                    break;
            }

            switch (Board)
            {
                case global::XIDE.Board.Ultra: Options.BoardDefine = "ULTRA"; break;
                case global::XIDE.Board.MiniDuino: Options.BoardDefine = "MINI_DUINO"; break;
                case global::XIDE.Board.Mini: Options.BoardDefine = "MINI"; break;
                case global::XIDE.Board.cocoDuino: Options.BoardDefine = "COCO_DUINO"; break;
                case global::XIDE.Board.coco: Options.BoardDefine = "COCO"; break;
            }
        }

        public static void Precompile(Board Board)
        {
            // Load XML

            XmlTextReader xml = new XmlTextReader(Path.Combine("Tools", "Libraries.xml"));
            xml.ReadStartElement("Libraries");

            CompilerSettings Options = new CompilerSettings();
            SetCompiler(Board, ref Options);
            string mmcu = GenerateMmcu(Options);

            Dictionary<string, List<string>> Libraries = new Dictionary<string, List<string>>();

            string currentLib = "";

            while (xml.Read())
            {
                switch (xml.NodeType)
                {
                    case XmlNodeType.Element:
                        if (xml.Name == "Library")
                        {
                            currentLib = xml.GetAttribute("Name");
                            Libraries.Add(currentLib, new List<string>());
                        }
                        break;
                    case XmlNodeType.Text:
                        Libraries[currentLib].Add(xml.Value);
                        break;
                }
            }

            /*while (xml.Read())
            {
                MessageBox.Show(xml.ReadContentAsString());
            }*/

            xml.Close();

            foreach (KeyValuePair<string, List<string>> Library in Libraries)
            {
                string FileList = "";
                foreach (string file in Library.Value)
                    FileList += "\"" + Path.GetFullPath(Path.Combine("Libs", file)) + "\" ";

                InvokeGcc("-ffunction-sections -mshort-calls -g0 -Wall -o \"" + Path.GetFullPath(Path.Combine("Temp", Library.Key + ".o")) + "\" -Wl,-lm -D_XBOARD_" + Options.BoardDefine + "_ -c -Wl,--gc-sections -mmcu=" + mmcu + " " + FileList + " -I \"" + Path.GetFullPath("Libs") + "\"");
            }

            string Libs = "";

            foreach (KeyValuePair<string, List<string>> Library in Libraries)
            {
                Libs += "\"" + Path.GetFullPath(Path.Combine("Temp", Library.Key + ".o")) + "\" ";
            }

            InvokeAr("rcs \"" + Path.GetFullPath(Path.Combine("Tools", "libxboard.a")) + "\" " + Libs);
        }

    }
}
