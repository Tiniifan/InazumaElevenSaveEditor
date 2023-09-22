using System;
using System.Collections.Generic;

namespace InazumaElevenSaveEditor.InazumaEleven.Logic
{
    public class Player
    {
        public string Name;

        public uint ID;

        public int Index;

        public Element Element;

        public Position Position;

        public Gender Gender;

        public List<int> Stat;

        public int Freedom;

        public List<Move> Moves;

        public List<UInt32> UInt32Moves;

        public int Level;

        public bool Invoke;

        public bool Armed;

        public int Style;

        public List<int> InvestedPoint;

        public List<int> InvestedFreedom;

        public Avatar Avatar;

        public List<Equipment> Equipments;

        public MixiMax MixiMax;

        public bool IsAura;

        public bool IsTrained;

        public int Score;

        public int Participation;

        public int FP;

        public int TP;

        public Player()
        {

        }

        public Player(Player player)
        {
            Name = player.Name;
            ID = player.ID;
            Index = player.Index;
            Element = player.Element;
            Position = player.Position;
            Gender = player.Gender;
            Stat = player.Stat;
            Freedom = player.Freedom;
            Moves = player.Moves;
            UInt32Moves = player.UInt32Moves;
            Level = player.Level;
            Invoke = player.Invoke;
            Armed = player.Armed;
            Style = player.Style;
            InvestedPoint = player.InvestedPoint;
            InvestedFreedom = player.InvestedFreedom;
            Avatar = player.Avatar;
            Equipments = player.Equipments;
            MixiMax = player.MixiMax;
            IsAura = player.IsAura;
            IsTrained = player.IsTrained;
            Score = player.Score;
            Participation = player.Participation;
            FP = player.FP;
            TP = player.TP;
        }

        public Player(Player player, uint id) : this(player)
        {
            ID = id;
        }

        public Player(string _Name, Position _Position, Element _Element, Gender _Gender, List<UInt32> _Moves, List<int> _Stat, int _Freedom)
        {
            Name = _Name;
            Position = _Position;
            Element = _Element;
            Gender = _Gender;
            UInt32Moves = _Moves;
            Stat = _Stat;
            Freedom = _Freedom;
            InvestedFreedom = new List<int>(new int[8]);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
