using System;
using System.Collections.Generic;
namespace InazumaElevenSaveEditor.InazumaEleven.Logic
{
    public class Team
    {
        public string Name;

        public int Level;

        public Item Emblem;

        public Item Kit;

        public Item Formation;

        public Item Coach;

        public List<Player> Players = new List<Player>();

        public List<int> PlayersFormationIndex = new List<int>();

        public List<int> PlayersKitNumber = new List<int>();

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

        public Team(string _Name, Item _Emblem, Item _Kit, Item _Formation, Item _Coach, List<Player> _Players, List<int> _PlayersFormationIndex, List<int> _PlayersKitNumber)
        {
            Name = _Name;
            Emblem = _Emblem;
            Kit = _Kit;
            Formation = _Formation;
            Coach = _Coach;
            Players = _Players;
            PlayersFormationIndex = _PlayersFormationIndex;
            PlayersKitNumber = _PlayersKitNumber;
        }
    }
}

