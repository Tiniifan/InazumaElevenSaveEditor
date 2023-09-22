using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace InazumaElevenSaveEditor.Tools
{
    public class BinaryDataWriter : IDisposable
    {
        private Stream _stream;

        public bool BigEndian { get; set; } = false;

        public long Length => _stream.Length;

        public Stream BaseStream => _stream;

        public long Position => _stream.Position;

        public BinaryDataWriter(byte[] data)
        {
            _stream = new MemoryStream(data);
        }

        public BinaryDataWriter(Stream stream)
        {
            _stream = stream;
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        public void Skip(uint size)
        {
            _stream.Seek(size, SeekOrigin.Current);
        }

        public void Seek(uint position)
        {
            _stream.Seek(position, SeekOrigin.Begin);
        }

        public void PrintPosition()
        {
            Console.WriteLine(_stream.Position.ToString("X"));
        }

        public void Write(byte[] data)
        {
            _stream.Write(data, 0, data.Length);
        }

        public void Write(byte value)
        {
            _stream.WriteByte(value);
        }

        public void Write(short value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void Write(int value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void Write(long value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void Write(ushort value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void Write(uint value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void Write(ulong value)
        {
            Write(BitConverter.GetBytes(value));
        }
        public void Write(float value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void WriteAlignment(int alignment = 16, byte alignmentByte = 0x0)
        {
            var remainder = BaseStream.Position % alignment;
            if (remainder <= 0) return;
            for (var i = 0; i < alignment - remainder; i++)
                Write(alignmentByte);
        }

        public void WriteAlignment()
        {
            Write((byte)0x00);
            WriteAlignment(16, 0xFF);
        }

        public void WriteStruct<T>(T structure)
        {
            byte[] bytes = new byte[Marshal.SizeOf(typeof(T))];

            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            Marshal.StructureToPtr(structure, handle.AddrOfPinnedObject(), false);
            handle.Free();

            Write(bytes);
        }

        public void WriteMultipleStruct<T>(IEnumerable<T> structures)
        {
            foreach (T structure in structures)
            {
                WriteStruct(structure);
            }
        }
    }
}
