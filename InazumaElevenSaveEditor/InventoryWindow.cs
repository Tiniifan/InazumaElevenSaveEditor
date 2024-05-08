using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using InazumaElevenSaveEditor.InazumaEleven.Logic;
using InazumaElevenSaveEditor.InazumaEleven.Saves;

namespace InazumaElevenSaveEditor
{
    public partial class InventoryWindow : Form
    {
        private ISave Save = null;

        private DataGridView CurrentDataGridView;

        List<DataGridView> DataGridViews;

        public InventoryWindow(ISave save)
        {
            InitializeComponent();

            DataGridViews = new List<DataGridView>
            {
                dataGridView1,
                dataGridView2,
                dataGridView3,
                dataGridView4,
                dataGridView5,
                dataGridView6,
                dataGridView7,
                dataGridView8,
                dataGridView9,
                dataGridView10,
                dataGridView11,
                dataGridView12,
                dataGridView13,
                dataGridView14,
                dataGridView15,
                dataGridView16,
                dataGridView17,
                dataGridView18,
                dataGridView19,
                dataGridView20,
                dataGridView21,
                dataGridView22,
                dataGridView23,
            };

            Save = save;
        }

        private void PopulateDataGridView(DataGridView dataGridView, int subcategory, int maximum)
        {
            var items = Save.Game.Items.Where(x => x.Value.SubCategory == subcategory).Select(x => x.Value.Name).ToList();

            ((DataGridViewComboBoxColumn)dataGridView.Columns[0]).Items.AddRange(items.ToArray());
            for (int i = 1; i < maximum; i++)
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

        private void UnlockAllItems(DataGridView dataGridView)
        {
            // Get all items
            int subCategory = Convert.ToInt32(dataGridView.Name.Replace("dataGridView", ""));
            List<string> itemNames = Save.Game.Items
            .Where(x => x.Value.SubCategory == subCategory && x.Value.Name != "Scroll of Zhuge Liang")
            .Select(x => x.Value.Name)
            .ToList();

            if (itemNames.Count() > 0)
            {
                // Remove items already owned
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (itemNames.Count() > 0)
                    {
                        if (row.Cells[0].Value != null)
                        {
                            int index = itemNames.IndexOf(row.Cells[0].Value.ToString());
                            if (index != -1)
                            {
                                itemNames.RemoveAt(index);
                            }
                        }
                    }
                }

                // add missing items
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (itemNames.Count() > 0)
                    {
                        if (row.Cells[0].Value == null)
                        {
                            row.Cells[1].Value = 1;
                            row.Cells[0].Value = itemNames[0];
                            itemNames.RemoveAt(0);
                        }
                    }
                }
            }
        }

        private void X99AllItems(DataGridView dataGridView)
        {
            // set x99 all owned items
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells[0].Value != null)
                    row.Cells[1].Value = 99;
            }
        }

        private void SaveDataGridView(DataGridView dataGridView, int subCategory)
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                DataGridViewComboBoxCell comboBox = (row.Cells[0] as DataGridViewComboBoxCell);

                if (row.Cells[0].Value != null)
                {
                    KeyValuePair<uint, Item> itemKeyValuePair = Save.Game.Items.FirstOrDefault(x => x.Value.Name == row.Cells[0].Value.ToString() & x.Value.SubCategory == subCategory);
                    Item newItem = new Item(itemKeyValuePair.Key, itemKeyValuePair.Value);
                    newItem.Quantity = (row.Cells[1].Value == null) ? 1 : Convert.ToInt32(row.Cells[1].Value);

                    if (row.Cells[2].Value != null && Save.Game.Inventory.ContainsKey(Convert.ToInt32(row.Cells[2].Value.ToString())))
                    {
                        Save.Game.Inventory[Convert.ToInt32(row.Cells[2].Value.ToString())] = newItem;
                    } else
                    {
                        int newIndex = 0;
                        if (Save.Game.Inventory.Where(x => x.Value.Category == newItem.Category).Count() > 0)
                        {
                            newIndex = Save.Game.Inventory.Where(x => x.Value.Category == newItem.Category).Last().Key;
                        }

                        while (Save.Game.Inventory.ContainsKey(newIndex))
                        {
                            short lowInt16 = (short)(newIndex & 0xFFFF);
                            short hightInt16 = (short)((newIndex >> 16) & 0xFFFF);
                            lowInt16++;
                            hightInt16++;

                            newIndex = (int)lowInt16 | ((int)hightInt16 << 16);
                        }
                       
                        Save.Game.Inventory.Add(newIndex, newItem);
                    }
                }
            }
        }

        private void InventoryWindow_Load(object sender, EventArgs e)
        {
            // Fill all dataGridComboBox
            for (int i = 1; i < 24; i++)
            {
                if (i != 23)
                    PopulateDataGridView(DataGridViews[i - 1], i, Save.Game.Items.Count(x => x.Value.SubCategory == i));
                else
                    PopulateDataGridView(DataGridViews[i - 1], i, 120);
            }

            // Remove totems entry
            if (Save.Game.Code != "IEGOGALAXY")
            {
                tabControl2.TabPages.RemoveAt(6);
            }

            // Print items
            foreach (KeyValuePair<int, Item> item in Save.Game.Inventory)
            {
                if (item.Value != null && item.Value.SubCategory != -1)
                {
                    DataGridView dataGridView = this.Controls.Find("dataGridView" + item.Value.SubCategory, true).First() as DataGridView;
                    DataGridViewComboBoxCell comboBox = (dataGridView.Rows[0].Cells[0] as DataGridViewComboBoxCell);

                    if (GetFirstEmptyRow(dataGridView) < dataGridView.Rows.Count)
                    {
                        dataGridView.Rows[GetFirstEmptyRow(dataGridView)].Cells[1].Value = item.Value.Quantity;
                        dataGridView.Rows[GetFirstEmptyRow(dataGridView)].Cells[2].Value = item.Key;
                        dataGridView.Rows[GetFirstEmptyRow(dataGridView)].Cells[0].Value = comboBox.Items[comboBox.Items.IndexOf(item.Value.Name)];
                    }                   
                }
            }

            // Set the CurrentDataGridView focused
            tabControl1.Enabled = true;
            TabControl_SelectedIndexChanged(tabControl1, e);
        }

        private void InventoryWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Update item index
            for (int i = 1; i < 24; i++)
            {
                SaveDataGridView(DataGridViews[i - 1], i);
            }

            // Reset Aura Item and Equipment
            Save.Game.UpdateInventory();
            Welcome welcome = (Welcome)Application.OpenForms["Welcome"];
            welcome.bootsBox.Items.Clear();
            welcome.glovesBox.Items.Clear();
            welcome.braceletBox.Items.Clear();
            welcome.pendantBox.Items.Clear();

            // Equipment
            welcome.bootsBox.Items.AddRange(Save.Game.Equipments.Where(x => x.Value.Type.Name == "Boots").Select(x => x.Value).ToArray());
            welcome.glovesBox.Items.AddRange(Save.Game.Equipments.Where(x => x.Value.Type.Name == "Gloves").Select(x => x.Value).ToArray());
            welcome.braceletBox.Items.AddRange(Save.Game.Equipments.Where(x => x.Value.Type.Name == "Bracelet").Select(x => x.Value).ToArray());
            welcome.pendantBox.Items.AddRange(Save.Game.Equipments.Where(x => x.Value.Type.Name == "Pendant").Select(x => x.Value).ToArray());

            // None equipment
            welcome.bootsBox.Items.Add(Save.Game.Equipments[0x0]);
            welcome.glovesBox.Items.Add(Save.Game.Equipments[0x0]);
            welcome.braceletBox.Items.Add(Save.Game.Equipments[0x0]);
            welcome.pendantBox.Items.Add(Save.Game.Equipments[0x0]);
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabControl tabControl = (TabControl)sender;

            Control control = tabControl.TabPages[tabControl.SelectedIndex];

            while (control is DataGridView == false)
            {
                control = control.Controls[0];
            }

            CurrentDataGridView = (DataGridView)control;
            manageTabToolStripMenuItem.Text = "Manage " + CurrentDataGridView.Parent.Text;
        }

        private void UnlockAllItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnlockAllItems(CurrentDataGridView);
        }

        private void X99AllItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            X99AllItems(CurrentDataGridView);
        }

        private void UnlockAllItemsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            bool maxQuantity = false;

            DialogResult dialogResult = MessageBox.Show("Do you want to unlock all items with the max quantity (x99)?", "Max Quantity", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                maxQuantity = true;
            }

            foreach(DataGridView dataGridView in DataGridViews)
            {
                // exclude palpack card
                if (dataGridView.Name != "dataGridView23")
                {
                    UnlockAllItems(dataGridView);

                    if (maxQuantity)
                    {
                        X99AllItems(dataGridView);
                    }
                }
            }
        }
    }
}
