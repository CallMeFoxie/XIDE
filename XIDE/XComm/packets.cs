using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Text;

namespace XIDE.XComm
{
    struct PacketNOP
    {
        public const byte Command = 0;
    }

    struct PacketGETID
    {
        public const byte Command = 1;
    }

    struct PacketRESET
    {
        public const byte Command = 2;
    }

    struct PacketAPP
    {
        public const byte Command = 3;
    }

    struct PacketPAGE
    {
        public const byte Command = 4;
        public ushort PageNumber;
        public byte Part;
        public ushort CRC16;
        public byte[] Payload;

        public void Send(SerialPort SP)
        {
            SP.Write(new byte[] { Command }, 0, 1);
            SP.Write(new byte[] { (byte)(PageNumber >> 8), (byte)(PageNumber & 0xFF) }, 0, 2);
            SP.Write(new byte[] { Part }, 0, 1);
            SP.Write(new byte[] { (byte)(CRC16 >> 8), (byte)(CRC16 & 0xFF) }, 0, 2);

            SP.Write(Payload, 0, Payload.Length); // should be 32 bytes in length
        }
    }

    struct PacketWRITE
    {
        public const byte Command = 5;
    }

    struct PacketBLDNVM
    {
        public const byte Command = 7;
    }

    class PacketBLWRITE
    {
        public const byte Command = 8;
        public short Magic = 0x5F81;
    }

    
    class ReplyGETID
    {
        private const int Size = 8;

        public byte Board;
        public ushort BootloaderVersion;
        public ushort PageSize;
        public ushort FlashSize;
        public byte BoardRevision;

        public void Load(SerialPort SP)
        {
            Board = (byte)SP.ReadByte();
            BootloaderVersion = (ushort)((((byte)SP.ReadByte()) << 8) | (byte)SP.ReadByte());
            PageSize = (ushort)((((byte)SP.ReadByte()) << 8) | (byte)SP.ReadByte());
            FlashSize = (ushort)((((byte)SP.ReadByte()) << 8) | (byte)SP.ReadByte());
            BoardRevision = (byte)SP.ReadByte();
        }
    }
}
