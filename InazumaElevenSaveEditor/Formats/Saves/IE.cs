using System;
using System.Windows.Forms;
using InazumaElevenSaveEditor.Tools;
using InazumaElevenSaveEditor.Formats.Games;

namespace InazumaElevenSaveEditor.Formats.Saves
{
    public class IE : IFormat
    {
        public string Name => "Inazuma Eleven";

        public string Extension => ".ie";

        public string Description => "Inazuma Eleven GO CS/Galaxy Format by Level 5 (Go == .ie4)";

        public bool CanOpen => true;

        public bool CanSave => true;

        public bool CanBeCompressed => true;

        public IGame Open(DataReader file)
        {
            file.Seek(4);
            ushort header = file.ReadUInt16();

            if (header != 0x2CF1 & header != 0x4CF1 & header != 0x40F1)
            {
                file.Seek(0);
                file = new DataReader(Level5.Decrypt(file.ReadBytes(Convert.ToInt32(file.Length))));
                file.Seek(4);
                header = file.ReadUInt16();
            }

            switch (header)
            {
                case 0x2CF1:
                    MessageBox.Show("IEGO save support isn't available, wait update");
                    throw new FormatException("IEGO save support isn't available");
                case 0x4CF1:
                    return new CS(file);
                case 0x40F1:
                    return new Galaxy(file);
                default:
                    throw new FormatException("Save Game not supported");
            }
        }
    }
}
