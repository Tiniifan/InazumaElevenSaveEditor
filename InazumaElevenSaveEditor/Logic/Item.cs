namespace InazumaElevenSaveEditor.Logic
{
    public class Item
    {
        public string Name;

        public int Quantity;

        public string IndexSorted;

        public int Category;

        public int SubCategory;

        public Item()
        {

        }

        public Item(string _Name, int _Category, int _SubCategory)
        {
            Name = _Name;
            Category = _Category;
            SubCategory = _SubCategory;
        }

        public Item(string _IndexSorted, int _Quantity)
        {
            IndexSorted = _IndexSorted;
            Quantity = _Quantity;
        }
    }
}
