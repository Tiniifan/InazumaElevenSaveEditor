namespace InazumaElevenSaveEditor.InazumaEleven.Logic
{
    public class BestMatch
    {
        public uint PlayerID;

        public uint AuraID;

        public uint MixiMaxMove;

        public BestMatch(uint _PlayerID, uint _AuraID, uint _MixiMaxMove)
        {
            PlayerID = _PlayerID;
            AuraID = _AuraID;
            MixiMaxMove = _MixiMaxMove;
        }
    }
}
