namespace InazumaElevenSaveEditor.Logic
{
    public class Avatar
    {
        public string Name;

        public bool IsFightingSpirit;

        public int Level;

        public Avatar(Avatar avatar)
        {
            Name = avatar.Name;
            IsFightingSpirit = avatar.IsFightingSpirit;
            Level = avatar.Level;
        }

        public Avatar(string _Name, bool _IsFightingSpirit)
        {
            Name = _Name;
            IsFightingSpirit = _IsFightingSpirit;
        }
    }
}
