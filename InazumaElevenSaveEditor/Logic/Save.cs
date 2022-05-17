using System.Collections.Generic;

namespace InazumaElevenSaveEditor.Logic
{
    public class Save
    {
        public string Name;

        public string TeamName;

        public int Chapter;

        public int Hours;

        public int Min;

        public int Prestige;

        public int Friendship;

        public int SecretLinkLevel;

        public bool UnlockAllData;

        public List<GroupPlayRecords> PlayRecords;

        public Dictionary<string, Item> Inventory;

        public Save()
        {

        }
    }
}
