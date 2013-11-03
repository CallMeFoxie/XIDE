using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace XIDE.XComm
{
    class XComm
    {
        private bool Connected = false;
        private const int ChunkSize = 32; // 32 bytes per transfer

        private const int BaudRateBootloader = 1200;
        private const byte CMD_NOP = 0x00;
        private const byte CMD_GETID = 0x01;
        private const byte CMD_RESET = 0x02;
        private const byte CMD_APP = 0x03;
        private const byte CMD_PAGE = 0x04;
        private const byte CMD_WRITE = 0x05;

        private const byte CMD_BLDNVM = 0x80;
        private const byte CMD_BLWRITE = 0x81;

        private const byte CMD_NOP_REPLY = 0x5A;

        public XComm()
        {
            Comm = new SerialPort();
        }

        public bool Connect(string SerialPort)
        {
            Comm = new SerialPort(SerialPort);
            return true;
        }

        public void Disconnect()
        {
            Comm.Close();
        }

        public bool UpdateBootloader(string File)
        {
            Crc16Ccitt CRC = new Crc16Ccitt(InitialCrcValue.Zeros);

            FileInfo bl = new FileInfo(File);
            StreamReader Read = new StreamReader(File);

            if (Board == null)
                LoadBoardInfo();

            // load header of the bootloader file
            BootloaderImage img = BootloaderImage.Load(File);

            if(Board.Board != img.Board)
                return false;
            else if (img.Revision != 0 && img.Revision != Board.BoardRevision)
                return false;

            int Pages = (int)Math.Ceiling((double)bl.Length / Board.PageSize);

            for (int Page = 0; Page < Pages; Page++)
            {
                for (int Part = 0; Part < Board.PageSize / ChunkSize; Part++)
                {
                    byte[] Content = new byte[ChunkSize];

                    PacketPAGE packet = new PacketPAGE();
                    packet.Part = (byte)Part;
                    packet.PageNumber = (byte)Page;
                    for (int i = 0; i < ChunkSize; i++)
                    {
                        if(i + Part * (Board.PageSize / ChunkSize) + Page * Board.PageSize < (img.Length - 8))
                            Content[i] = img.Content[i + Part * (Board.PageSize / ChunkSize) + Page * Board.PageSize];
                        else
                            Content[i] = 0;
                    }

                    packet.CRC16 = CRC.ComputeChecksum(Content);
                    packet.Payload = Content;

                    packet.Send(Comm);
                }
            }

            return true;
        }

        public bool UploadCode(string File)
        {

            return true;
        }

        public string GetBoardName()
        {
            if (Board == null)
                LoadBoardInfo();

            string Name = "";
            switch (Board.Board)
            {
                case Boards.Coco: Name = "XBoard Coco"; break;
                case Boards.CocoDuino: Name = "XBoard Coco-Duino"; break;
                case Boards.Mini: Name = "XBoard Mini"; break;
                case Boards.MiniDuino: Name = "XBoard Mini-Duino"; break;
                case Boards.Ultra: Name = "XBoard Ultra"; break;
            }

            return Name;
        }

        public BoardInfo Board;

        private void LoadBoardInfo()
        {
            BLMode();

            Comm.Write(new byte[] { CMD_GETID }, 0, 1);

            ReplyGETID reply = new ReplyGETID();
            reply.Load(Comm);

            Board = new BoardInfo();

            Board.BLVersion = reply.BootloaderVersion;
            Board.Board = (Boards)reply.Board;
            Board.BoardRevision = reply.BoardRevision;
            Board.FlashSize = reply.FlashSize;
            Board.PageSize = reply.PageSize;

            RegularMode();
        }

        private void BLMode()
        {
            if (!Connected) throw new Exception("Have to connect to the board first!");
            BaudRate = Comm.BaudRate;
            WasOpen = false;

            if (Comm.IsOpen)
            {
                Comm.Close();
                WasOpen = true;
            }

            Comm.BaudRate = BaudRateBootloader;
            Comm.Open();
        }

        private void RegularMode()
        {
            if (!Connected) throw new Exception("Have to connect to the board first!");
            if (Comm.IsOpen)
                Comm.Close();

            Comm.BaudRate = BaudRate;
            if (WasOpen) 
                Comm.Open();
        }

        //Timer TimerDetectBoard;

        public bool DetectBoard()
        {
            string[] Ports = SerialPort.GetPortNames();

            List<string> Found = new List<string>();

            Board = null;

            foreach (string Port in Ports)
            {
                try
                {
                    SerialPort sp = new SerialPort(Port, BaudRateBootloader, Parity.None, 8, StopBits.One);
                    sp.RtsEnable = false;
                    sp.DtrEnable = false;

                    sp.Open();
                    /*TimerDetectBoard = new Timer();
                    TimerDetectBoard.Interval = 500;
                    TimerDetectBoard.Start();
                    TimerDetectBoard.Tick += new EventHandler(t_Detect_Tick);*/

                    sp.Write(new byte[] { CMD_NOP }, 0, 1);
                    int reply = -1;

                    //while (TimerDetectBoard.Enabled)
                    //reply = sp.ReadByte();
                    Thread.Sleep(500);
                    if (sp.BytesToRead > 0)
                        reply = sp.ReadByte();

                    if (reply != -1 && reply == CMD_NOP_REPLY)
                    {
                        Found.Add(Port);
                    }
                    Thread.Sleep(1000);

                    sp.Close();
                }
                catch (Exception)
                {

                }
            }

            if (Found.Count > 1)
                MessageBox.Show("Detected more XBoards! Using the first one.");

            foreach (string Port in Found)
                MessageBox.Show("Detected XBoard on port " + Port);

            if (Found.Count > 0)
            {
                Comm = new SerialPort(Found[0], BaudRateBootloader, Parity.None, 8, StopBits.One);
                Comm.RtsEnable = false;
                Comm.DtrEnable = false;
                Comm.Open();
                Connected = true;

                LoadBoardInfo();

                return true;
            }

            return false;

        }

        /*private void t_Detect_Tick(object sender, EventArgs e)
        {
            TimerDetectBoard.Enabled = false;
        }*/

        private bool WasOpen = false;
        private int BaudRate = 0;
        SerialPort Comm;
        
    }

    class BootloaderImage
    {
        public ushort Length;
        public ushort CRC16;
        public ushort Version;
        public Boards Board;
        public byte Revision;
        public byte[] Content;

        public static BootloaderImage Load(string File)
        {
            FileInfo f = new FileInfo(File);
            if(f.Length < 8) throw new Exception("Wrong file.");

            StreamReader r = new StreamReader(File);
            BootloaderImage img = new BootloaderImage();

            img.Length = (ushort)((byte)r.BaseStream.ReadByte() | ((byte)r.BaseStream.ReadByte()) << 8);
            img.CRC16 = (ushort)((byte)r.BaseStream.ReadByte() | ((byte)r.BaseStream.ReadByte()) << 8);
            img.Version = (ushort)((byte)r.BaseStream.ReadByte() | ((byte)r.BaseStream.ReadByte()) << 8);
            byte brd = (byte)r.BaseStream.ReadByte();

            switch(brd)
            {
                case 0: img.Board = Boards.Mini; break;
                case 1: img.Board = Boards.Coco; break;
                case 2: img.Board = Boards.Ultra; break;
                case 3: img.Board = Boards.CocoDuino; break;
                case 4: img.Board = Boards.MiniDuino; break;
            }
            
            img.Revision = (byte)r.BaseStream.ReadByte();
            img.Content = new byte[img.Length - 8];

            for(int i = 7; i < img.Length; i++)
            {
                img.Content[i - 7] = (byte)r.BaseStream.ReadByte();
            }

            return img;
        }
    }

    class BoardInfo
    {
        public Boards Board;
        public ushort BLVersion;
        public ushort PageSize;
        public ushort FlashSize;
        public ushort BoardRevision;
    }

    enum Boards
    {
        Mini,
        Coco,
        Ultra,
        CocoDuino,
        MiniDuino
    }
}
