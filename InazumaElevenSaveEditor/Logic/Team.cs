using System.Collections.Generic;

namespace InazumaElevenSaveEditor.Logic
{
    public class Team
    {
        public string Name;

        public int Level;

        public Item Emblem;

        public Item Kit;

        public Item Formation;

        public Item Coach;

        public List<Player> Players;

        public Team()
        {
        }

        public Team(string _Name, int _Level, Item _Emblem, Item _Kit, Item _Formation, Item _Coach)
        {
            Name = _Name;
            Level = _Level;
            Emblem = _Emblem;
            Kit = _Kit;
            Formation = _Formation;
            Coach = _Coach;
        }
    }
}
