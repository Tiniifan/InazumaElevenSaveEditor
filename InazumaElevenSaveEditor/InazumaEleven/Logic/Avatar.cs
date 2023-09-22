namespace InazumaElevenSaveEditor.InazumaEleven.Logic
{
    public class Avatar
    {
        public string Name;

        public bool IsFightingSpirit;

        public int Level;

        public int Usage;

        public Avatar(Avatar avatar, int level, int usage)
        {
            Name = avatar.Name;
            IsFightingSpirit = avatar.IsFightingSpirit;
            Level = level;
            Usage = usage;
        }

        public Avatar(string _Name, bool _IsFightingSpirit)
        {
            Name = _Name;
            IsFightingSpirit = _IsFightingSpirit;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
