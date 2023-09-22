namespace InazumaElevenSaveEditor.InazumaEleven.Logic
{
    public class Streetpass
    {
        public string Name;

        public string Message;

        public string GameVersion;

        public bool HasWon;

        public int PlayTime;

        public int ChallengesReceived;

        public int ChallengesWon;

        public int PlayersScouted;

        public Team Team;

        public Streetpass()
        {

        }

        public Streetpass(string _Name, string _Message, string _GameVersion, bool _HasWon, int _PlayTime, int _ChallengesReceived, int _ChallengesWon, int _PlayersScouted, Team _Team)
        {
            Name = _Name;
            Message = _Message;
            GameVersion = _GameVersion;
            HasWon = _HasWon;
            PlayTime = _PlayTime;
            ChallengesReceived = _ChallengesReceived;
            ChallengesWon = _ChallengesWon;
            PlayersScouted = _PlayersScouted;
            Team = _Team;
        }
    }
}
