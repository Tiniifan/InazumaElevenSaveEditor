using System.Collections.Generic;

namespace InazumaElevenSaveEditor.InazumaEleven.Logic
{
    public class MixiMax
    {
        public Player AuraPlayer;

        public bool AuraData;

        public BestMatch BestMatch;

        public List<int> MixiMaxMoveNumber = new List<int>();

        public MixiMax()
        {

        }

        public MixiMax(Player _AuraPlayer, (int, int) _MixiMaxMove)
        {
            AuraPlayer = _AuraPlayer;
            MixiMaxMoveNumber.Add(_MixiMaxMove.Item1);
            MixiMaxMoveNumber.Add(_MixiMaxMove.Item2);
        }

        public MixiMax(Player _AuraPlayer, (int, int) _MixiMaxMove, BestMatch _BestMatch)
        {
            AuraPlayer = _AuraPlayer;
            if (_MixiMaxMove.Item1 == 6)
                MixiMaxMoveNumber.Add(6);
            else
                MixiMaxMoveNumber.Add(_MixiMaxMove.Item1);

            if (_MixiMaxMove.Item2 == 6)
                MixiMaxMoveNumber.Add(6);
            else
                MixiMaxMoveNumber.Add(_MixiMaxMove.Item2);
            BestMatch = _BestMatch;
        }
    }
}
