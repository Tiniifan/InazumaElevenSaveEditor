using System;
using System.IO;

namespace InazumaElevenSaveEditor.Tools
{
    public class DataReader : BinaryReader
    {
        public bool BigEndian { get; set; } = false;

        public long Length { get => BaseStream.Length; }

        public DataReader(byte[] data) : base(new MemoryStream(data))
        {

        }

        public override Int16 ReadInt16()
        {
            return BitConverter.ToInt16(Reverse(base.ReadBytes(2)), 0);
        }

        public override UInt16 ReadUInt16()
        {
            return BitConverter.ToUInt16(Reverse(base.ReadBytes(2)), 0);
        }

        public override Int32 ReadInt32()
        {
            return BitConverter.ToInt32(Reverse(base.ReadBytes(4)), 0);
        }

        public override UInt32 ReadUInt32()
        {
            return BitConverter.ToUInt32(Reverse(base.ReadBytes(4)), 0);
        }

        public override Int64 ReadInt64()
        {
            return BitConverter.ToInt64(Reverse(base.ReadBytes(8)), 0);
        }

        public override UInt64 ReadUInt64()
        {
            return BitConverter.ToUInt64(Reverse(base.ReadBytes(8)), 0);
        }

        public override float ReadSingle()
        {
            return BitConverter.ToSingle(Reverse(base.ReadBytes(4)), 0);
        }

        public void Skip(uint Size)
        {
            BaseStream.Seek(Size, SeekOrigin.Current);
        }

        public void Seek(uint Position)
        {
            BaseStream.Seek(Position, SeekOrigin.Begin);
        }

        public byte[] Reverse(byte[] b)
        {
            if (BitConverter.IsLittleEndian && BigEndian)
                Array.Reverse(b);
            return b;
        }

        public UInt32 Reverse(UInt32 u)
        {
            byte[] b = BitConverter.GetBytes(u);
            Array.Reverse(b);
            return BitConverter.ToUInt32(b, 0);
        }

        public byte[] GetSection(uint Offset, int Size)
        {
            long temp = (uint)BaseStream.Position;
            Seek(Offset);
            byte[] data = ReadBytes(Size);
            Seek((uint)temp);
            return data;
        }

        public void PrintPosition()
        {
            Console.WriteLine(BaseStream.Position.ToString("X"));
        }
    }
}
