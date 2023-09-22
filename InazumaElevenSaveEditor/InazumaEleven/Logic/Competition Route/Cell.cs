using System;
using System.Collections.Generic;

namespace InazumaElevenSaveEditor.InazumaEleven.Logic.Competition_Route
{
    public class Cell
    {
        public string Name { get; set; }

        public int Level { get; set; }

        public List<UInt32> Drop { get; set; }

        public Cell(string name, int level, List<UInt32> drop)
        {
            Name = name;
            Level = level;
            Drop = drop;
        }
    }
}
