using System;
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

        public List<int> Coins = new List<int>(new int[5]);

        public bool UnlockAllData;

        public Dictionary<int, List<PlayRecord>> PlayRecords;

        public Dictionary<UInt32, Item> Inventory = new Dictionary<UInt32, Item>();

        public List<Team> Teams;

        public Save()
        {

        }
    }
}
