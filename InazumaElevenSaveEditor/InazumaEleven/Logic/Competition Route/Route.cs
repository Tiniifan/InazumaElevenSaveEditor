using System.Collections.Generic;

namespace InazumaElevenSaveEditor.InazumaEleven.Logic.Competition_Route
{
    public class Route
    {
        public string Name { get; set; }

        public List<Cell> Cells { get; set; }

        public Route(string name, List<Cell> cells)
        {
            Name = name;
            Cells = cells;
        }
    }
}
