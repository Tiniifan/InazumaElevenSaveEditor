namespace InazumaElevenSaveEditor.Logic
{
    public class Gender
    {
        public string Name;

        public Gender(string _Name)
        {
            Name = _Name;
        }

        public static Gender Boy()
        {
            return new Gender("Boy");
        }

        public static Gender Girl()
        {
            return new Gender("Girl");
        }

        public static Gender Unknow()
        {
            return new Gender("Unknown");
        }
    }
}
