using System;
using System.IO;
using System.Windows.Forms;
using InazumaElevenSaveEditor.Tools;
using InazumaElevenSaveEditor.InazumaEleven.Games;
using InazumaElevenSaveEditor.InazumaEleven.Games.IEGO;
using InazumaElevenSaveEditor.InazumaEleven.Games.IEGOCS;
using InazumaElevenSaveEditor.InazumaEleven.Games.IEGOGalaxy;
using InazumaElevenSaveEditor.Level_5.Compression.XORShift;

namespace InazumaElevenSaveEditor.InazumaEleven.Saves.IE
{
    public class IE : ISave
    {
        public string Name => "Inazuma Eleven";

        public string Extension => ".ie";

        public string Description => "Inazuma Eleven GO CS/Galaxy Format by Level 5 (Go == .ie4)";

        public IGame Game { get; set; }

        public void Open(BinaryDataReader reader)
        {
            reader.Skip(4);
            ushort header = reader.ReadValue<ushort>();

            if (header != 0x2CF1 & header != 0x4CF1 & header != 0x40F1)
            {
                reader = new BinaryDataReader(new XORShift().Decompress(reader.GetSection(0, (int)reader.Length)));
                reader.Skip(4);
                header = reader.ReadValue<ushort>();
            }

            switch (header)
            {
                case 0x2CF1:
                    Game = new GO(reader.BaseStream);
                    break;
                case 0x4CF1:
                    Game = new CS(reader.BaseStream);
                    break;
                case 0x40F1:
                    Game = new Galaxy(reader.BaseStream);
                    break;
                default:
                    throw new FormatException("Save Game not supported");
            }
        }

        public void Save(OpenFileDialog initialDirectory)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.FileName = Path.GetFileName(initialDirectory.FileName);

            if (Game.Code == "IEGO")
            {
                saveFileDialog.Title = "Save IEGO save file";
                saveFileDialog.Filter = "IE files|*.ie4|IE files decrypted|*.ie4";
            } else if (Game.Code == "IEGOCS")
            {
                saveFileDialog.Title = "Save IEGOCS save file";
                saveFileDialog.Filter = "IE files|*.ie|IE files decrypted|*.ie";
            } else
            {
                saveFileDialog.Title = "Save IEGOGalaxy save file";
                saveFileDialog.Filter = "IE files|*.ie|IE files decrypted|*.ie";
            }

            saveFileDialog.InitialDirectory = initialDirectory.InitialDirectory;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string saveFileName = saveFileDialog.FileName;
                byte[] saveData = Game.Save();

                if (saveFileDialog.FilterIndex == 1)
                {
                    saveData = Level5.Encrypt(saveData);
                }

                File.WriteAllBytes(saveFileName, saveData);
                MessageBox.Show("Saved!");
            }
        }
    }
}
