namespace InazumaElevenSaveEditor.Logic
{
    public class Item
    {
        public string Name;

        public int Quantity;

        public string IndexSorted;

        public Item()
        {
        }
        public Item(string _Name)
        {
            Name = _Name;
        }
        public Item(string _IndexSorted, int _Quantity)
        {
            IndexSorted = _IndexSorted;
            Quantity = _Quantity;
        }
    }
}
