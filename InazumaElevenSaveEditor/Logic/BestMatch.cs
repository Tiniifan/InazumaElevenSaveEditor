using System;

namespace InazumaElevenSaveEditor.Logic
{
    public class BestMatch
    {
        private UInt32 PlayerID;

        private UInt32 AuraID;

        public UInt32 MixiMaxMove;

        public BestMatch(UInt32 _PlayerID, UInt32 _AuraID, UInt32 _MixiMaxMove)
        {
            PlayerID = _PlayerID;
            AuraID = _AuraID;
            MixiMaxMove = _MixiMaxMove;
        }

        public bool IsBestMatch(UInt32 TryPlayerID, UInt32 TryAuraID)
        {
            return PlayerID == TryPlayerID && AuraID == TryAuraID;
        }
    }
}
