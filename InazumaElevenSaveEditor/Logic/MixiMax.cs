using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InazumaElevenSaveEditor.Tools;

namespace InazumaElevenSaveEditor.Logic
{
    public class MixiMax
    {
        public Player AuraPlayer;

        public bool AuraData;

        public (bool, Move) BestMatch;

        public List<int> MixiMaxMoveNumber = new List<int>();

        public MixiMax()
        {
        }

        public MixiMax(Player _BasePlayer, Player _AuraPlayer, (int, int) _MixiMaxMove)
        {

        }

        public List<int> GetStat(Player _BasePlayer)
        {
            List<int> miximaxStat = new List<int>(new int[10]);
            return miximaxStat;
        }
    }
}
