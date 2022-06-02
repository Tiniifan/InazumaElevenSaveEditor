using System;

namespace InazumaElevenSaveEditor.Logic
{
    public class BestMatch
    {
        public UInt32 PlayerID;

        public UInt32 AuraID;

        public UInt32 MixiMaxMove;

        public BestMatch(UInt32 _PlayerID, UInt32 _AuraID, UInt32 _MixiMaxMove)
        {
            PlayerID = _PlayerID;
            AuraID = _AuraID;
            MixiMaxMove = _MixiMaxMove;
        }
    }
}
