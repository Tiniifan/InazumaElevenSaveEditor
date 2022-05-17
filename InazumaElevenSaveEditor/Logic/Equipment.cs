using System.Collections.Generic;

namespace InazumaElevenSaveEditor.Logic
{
    public class Equipment
    {
        public string Name;

        public EquipmentType Type;

        public List<int> Stat;

        public Equipment(string _Name, EquipmentType _Type, List<int> _Stat)
        {
            Name = _Name;
            Type = _Type;
            Stat = _Stat;
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
