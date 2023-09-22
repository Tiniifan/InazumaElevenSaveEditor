namespace InazumaElevenSaveEditor.InazumaEleven.Logic
{
    public class Item
    {
        public string Name;

        public int Quantity;

        public int QuantityEquiped;

        public string IndexSorted;

        public int Category;

        public int SubCategory;

        public uint ID;

        public Item()
        {

        }

        public Item(uint id, Item item)
        {
            ID = id;
            Name = item.Name;
            Quantity = item.Quantity;
            QuantityEquiped = item.QuantityEquiped;
            IndexSorted = item.IndexSorted;
            SubCategory = item.SubCategory;
            Category = item.Category;
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

        public Item(uint id, Item item, int quantity)
        {
            ID = id;
            Name = item.Name;
            Category = item.Category;
            SubCategory = item.SubCategory;
            Quantity = quantity;
        }

        public Item(uint id, Item item, int quantity, int quantityEquiped)
        {
            ID = id;
            Name = item.Name;
            Category = item.Category;
            SubCategory = item.SubCategory;
            Quantity = quantity;
            QuantityEquiped = quantityEquiped;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
