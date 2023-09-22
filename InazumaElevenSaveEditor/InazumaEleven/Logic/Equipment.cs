using System;
using System.Collections.Generic;

namespace InazumaElevenSaveEditor.InazumaEleven.Logic
{
    public class Equipment
    {
        public string Name;

        public UInt32 ID;

        public EquipmentType Type;

        public List<int> Stat;

        public Equipment(string _Name, EquipmentType _Type, List<int> _Stat)
        {
            Name = _Name;
            Type = _Type;
            Stat = _Stat;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class EquipmentType
    {
        public string Name;

        public EquipmentType(string _Name)
        {
            Name = _Name;
        }
    }
}
