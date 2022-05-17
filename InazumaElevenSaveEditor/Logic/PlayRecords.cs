using System.Collections.Generic;

namespace InazumaElevenSaveEditor.Logic
{
    public class PlayRecords
    {
        public string Name;

        public bool Unlocked;

        public PlayRecords(string _Name)
        {
            Name = _Name;
        }
    }

    public class GroupPlayRecords
    {
        public List<PlayRecords> PlayRecordsList;

        public int CategoryNumber;

        public GroupPlayRecords(int _CategoryNumber)
        {
            PlayRecordsList = new List<PlayRecords>();
            CategoryNumber = _CategoryNumber;
        }

        public GroupPlayRecords(List<PlayRecords> _PlayRecordsList, int _CategoryNumber, byte[] binary)
        {
            PlayRecordsList = _PlayRecordsList;
            CategoryNumber = _CategoryNumber;

            for (int i = 0; i < binary.Length; i++)
            {
                if (binary[i] == 0x00)
                {
                    PlayRecordsList[PlayRecordsList.Count - 1 - i].Unlocked = false;
                }
                else
                {
                    PlayRecordsList[PlayRecordsList.Count - 1 - i].Unlocked = true;
                }
            }
        }

        public void CalcBinary(byte[] binary)
        {
            for (int i = 0; i < binary.Length; i++)
            {
                if (binary[i] == 0x00)
                {
                    PlayRecordsList[PlayRecordsList.Count - 1 - i].Unlocked = false;
                }
                else
                {
                    PlayRecordsList[PlayRecordsList.Count - 1 - i].Unlocked = true;
                }
            }
        }

        public byte[] NewBinary()
        {
            byte[] binary = new byte[PlayRecordsList.Count];

            for (int i = 0; i < PlayRecordsList.Count; i++)
            {
                if (PlayRecordsList[i].Unlocked == false)
                {
                    binary[i] = 0x00;
                }
                else
                {
                    binary[i] = 0x01;
                }
            }

            return binary;
        }
    }
}
