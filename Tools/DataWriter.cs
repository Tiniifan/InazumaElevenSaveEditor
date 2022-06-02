using System;
using System.IO;

namespace InazumaElevenSaveEditor.Tools
{
    public class DataWriter : BinaryWriter
    {
        public bool BigEndian { get; set; } = false;

        public long Length { get => BaseStream.Length; }

        public DataWriter(byte[] data) : base(new MemoryStream(data))
        {

        }

        public override void Write(byte[] data)
        {
            data = Reverse(data);
            BaseStream.Write(data, 0, data.Length);
        }

        public void Skip(uint Size)
        {
            BaseStream.Seek(Size, SeekOrigin.Current);
        }

        public void Seek(uint Position)
        {
            BaseStream.Seek(Position, SeekOrigin.Begin);
        }

        public void PrintPosition()
        {
            Console.WriteLine(BaseStream.Position.ToString("X"));
        }

        public byte[] Reverse(byte[] b)
        {
            if (BitConverter.IsLittleEndian && BigEndian)
                Array.Reverse(b);
            return b;
        }
    }
}