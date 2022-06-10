using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using InazumaElevenSaveEditor.Logic;
using InazumaElevenSaveEditor.Formats;

namespace InazumaElevenSaveEditor
{
    public partial class InventoryWindow : Form
    {
        public ContainerGames Game = null;

        public InventoryWindow(ContainerGames _Game)
        {
            InitializeComponent();
            Game = _Game;
        }

        private void PopulateDataGridView(DataGridView dataGridView, int subcategory, int maximum)
        {
            var items = Game.Items.Where(x => x.Value.SubCategory == subcategory).Select(x => x.Value.Name).ToList();

            ((DataGridViewComboBoxColumn)dataGridView.Columns[0]).Items.AddRange(items.ToArray());
            for (int i = 0; i < maximum; i++)
            {
                dataGridView.Rows.Add();
            }
        }

        private int GetFirstEmptyRow(DataGridView dataGridView)
        {
            int index = 0;

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells[0].Value == null) return index;
                index++;
            }

            return index;
        }

        private void SaveDataGridView(DataGridView dataGridView, int category, int subCategory)
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells[0].Value == null) continue;

                string itemName = row.Cells[0].Value.ToString();

                int quantity = 1;
                if (row.Cells[1].Value != null)
                {
                    int.TryParse(row.Cells[1].Value.ToString(), out quantity);
                }
                if (quantity < 0) quantity = 1;
                if (quantity > 255) quantity = 255;

                int itemIndex = 1;
                if (row.Cells[2].Value != null)
                {
                    int.TryParse(row.Cells[2].Value.ToString(), out itemIndex);
                }

                if (row.Index < Game.SaveInfo.Inventory.Count(x => x.Value.SubCategory == subCategory))
                {
                    var itemKeyValuePair = Game.SaveInfo.Inventory.ElementAt(itemIndex);
                    itemKeyValuePair.Value.Name = itemName;
                    itemKeyValuePair.Value.Quantity = quantity;
                }
                else
                {
                    UInt32 lastItemCategory = Game.SaveInfo.Inventory.LastOrDefault(x => x.Value.Category == category).Key;
                    UInt16 itemCategoryCount = Convert.ToUInt16(lastItemCategory / 0x10000);
                    itemCategoryCount = (UInt16)((itemCategoryCount & 0xFFU) << 8 | (itemCategoryCount & 0xFF00U) >> 8);

                    int x1 = itemCategoryCount;
                    x1 += x1 / 256;
                    int x2 = x1;
                    x2 += x1 / 256;

                    byte[] rightByte = BitConverter.GetBytes(x1);
                    byte[] leftByte = BitConverter.GetBytes(x2);
                    UInt32 newIndex = BitConverter.ToUInt32(new byte[4] { leftByte[1], leftByte[0], rightByte[1], rightByte[0] }, 0);
                    Console.WriteLine(Game.SaveInfo.Inventory.ContainsKey(newIndex)) ;

                    Item item = Game.Items.FirstOrDefault(x => x.Value.Name == itemName).Value;
                    item.Quantity = quantity;

                    Game.SaveInfo.Inventory.Add(newIndex, item);
                }
            }
        }

        private void InventoryWindow_Load(object sender, EventArgs e)
        {
            // Ugly Bulk Code
            PopulateDataGridView(dataGridView1, 1, Game.Items.Count(x => x.Value.SubCategory == 1));
            PopulateDataGridView(dataGridView2, 2, Game.Items.Count(x => x.Value.SubCategory == 2));
            PopulateDataGridView(dataGridView3, 3, Game.Items.Count(x => x.Value.SubCategory == 3));
            PopulateDataGridView(dataGridView4, 4, Game.Items.Count(x => x.Value.SubCategory == 4));
            PopulateDataGridView(dataGridView5, 5, Game.Items.Count(x => x.Value.SubCategory == 5));
            PopulateDataGridView(dataGridView6, 6, Game.Items.Count(x => x.Value.SubCategory == 6));
            PopulateDataGridView(dataGridView7, 7, Game.Items.Count(x => x.Value.SubCategory == 7));
            PopulateDataGridView(dataGridView8, 8, Game.Items.Count(x => x.Value.SubCategory == 8));
            PopulateDataGridView(dataGridView9, 9, Game.Items.Count(x => x.Value.SubCategory == 9));
            PopulateDataGridView(dataGridView10, 10, Game.Items.Count(x => x.Value.SubCategory == 10));
            PopulateDataGridView(dataGridView11, 11, Game.Items.Count(x => x.Value.SubCategory == 11));
            PopulateDataGridView(dataGridView12, 12, Game.Items.Count(x => x.Value.SubCategory == 12));
            PopulateDataGridView(dataGridView13, 13, Game.Items.Count(x => x.Value.SubCategory == 13));
            PopulateDataGridView(dataGridView14, 14, Game.Items.Count(x => x.Value.SubCategory == 14));
            PopulateDataGridView(dataGridView15, 15, Game.Items.Count(x => x.Value.SubCategory == 15));
            PopulateDataGridView(dataGridView16, 16, Game.Items.Count(x => x.Value.SubCategory == 16));
            PopulateDataGridView(dataGridView17, 17, Game.Items.Count(x => x.Value.SubCategory == 17));
            PopulateDataGridView(dataGridView18, 18, Game.Items.Count(x => x.Value.SubCategory == 18));
            PopulateDataGridView(dataGridView19, 19, Game.Items.Count(x => x.Value.SubCategory == 19));
            PopulateDataGridView(dataGridView20, 20, Game.Items.Count(x => x.Value.SubCategory == 20));
            PopulateDataGridView(dataGridView21, 21, Game.Items.Count(x => x.Value.SubCategory == 21));
            PopulateDataGridView(dataGridView23, 23, 120);

            int count = 0;
            foreach (KeyValuePair<UInt32, Item> item in Game.SaveInfo.Inventory)
            {
                DataGridView dataGridView = this.Controls.Find("dataGridView" + item.Value.SubCategory, true).First() as DataGridView;

                DataGridViewComboBoxCell comboBox = (dataGridView.Rows[0].Cells[0] as DataGridViewComboBoxCell);
                dataGridView.Rows[GetFirstEmptyRow(dataGridView)].Cells[1].Value = item.Value.Quantity;
                dataGridView.Rows[GetFirstEmptyRow(dataGridView)].Cells[2].Value = count;
                dataGridView.Rows[GetFirstEmptyRow(dataGridView)].Cells[0].Value = comboBox.Items[comboBox.Items.IndexOf(item.Value.Name)];
                count++;
            }

            tabControl2.TabPages.RemoveAt(6);
            tabControl1.Enabled = true;
        }

        private void InventoryWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Ugly Bulk code
            SaveDataGridView(dataGridView1, 1, 1);
            SaveDataGridView(dataGridView2, 2, 2);
            SaveDataGridView(dataGridView3, 2, 3);
            SaveDataGridView(dataGridView4, 2, 4);
            SaveDataGridView(dataGridView5, 2, 5);
            SaveDataGridView(dataGridView6, 1, 6);
            SaveDataGridView(dataGridView7, 1, 7);
            SaveDataGridView(dataGridView8, 1, 8);
            SaveDataGridView(dataGridView9, 1, 9);
            SaveDataGridView(dataGridView10, 1, 10);
            SaveDataGridView(dataGridView11, 1, 11);
            SaveDataGridView(dataGridView12, 3, 12);
            SaveDataGridView(dataGridView13, 3, 13);
            SaveDataGridView(dataGridView14, 3, 14);
            SaveDataGridView(dataGridView15, 3, 15);
            SaveDataGridView(dataGridView16, 3, 16);
            SaveDataGridView(dataGridView17, 3, 17);
            SaveDataGridView(dataGridView18, 3, 18);
            SaveDataGridView(dataGridView19, 3, 19);
            SaveDataGridView(dataGridView20, 3, 20);
            SaveDataGridView(dataGridView21, 3, 21);
            SaveDataGridView(dataGridView23, 3, 23);

            // Reset Aura Item and Equipment
            Game.UpdateResource();
            NoFarmForMeOpenSource.Welcome welcome = (NoFarmForMeOpenSource.Welcome)Application.OpenForms["Welcome"];
            welcome.bootsBox.Items.Clear();
            welcome.glovesBox.Items.Clear();
            welcome.braceletBox.Items.Clear();
            welcome.pendantBox.Items.Clear();
            foreach (KeyValuePair<UInt32, Equipment> entry in Game.Equipments)
            {
                if (entry.Value.Type.Name == "Boots")
                    welcome.bootsBox.Items.Add(entry.Value.Name);
                else if (entry.Value.Type.Name == "Gloves")
                    welcome.glovesBox.Items.Add(entry.Value.Name);
                else if (entry.Value.Type.Name == "Bracelet")
                    welcome.braceletBox.Items.Add(entry.Value.Name);
                else if (entry.Value.Type.Name == "Pendant")
                    welcome.pendantBox.Items.Add(entry.Value.Name);
            }
        }
    }
}
