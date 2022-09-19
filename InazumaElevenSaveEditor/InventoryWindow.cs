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
        public IGame Game = null;

        private DataGridView CurrentDataGridView;

        List<DataGridView> DataGridViews;

        public InventoryWindow(IGame _Game)
        {
            InitializeComponent();
            Game = _Game;

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
        }

        private void PopulateDataGridView(DataGridView dataGridView, int subcategory, int maximum)
        {
            var items = Game.Items.Where(x => x.Value.SubCategory == subcategory).Select(x => x.Value.Name).ToList();

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

        private void SaveDataGridView(DataGridView dataGridView, int subCategory)
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                DataGridViewComboBoxCell comboBox = (row.Cells[0] as DataGridViewComboBoxCell);

                if (row.Cells[0].Value != null)
                {
                    Item newItem = Game.Items.FirstOrDefault(x => x.Value.Name == row.Cells[0].Value.ToString() & x.Value.SubCategory == subCategory).Value;
                    newItem.Quantity = Convert.ToInt32(row.Cells[1].Value);
                    Game.SaveInfo.Inventory[Convert.ToUInt32(row.Cells[2].Value.ToString())] = newItem;
                }
            }
        }

        private void InventoryWindow_Load(object sender, EventArgs e)
        {
            // Fill all dataGridComboBox
            for (int i = 1; i < 24; i++)
            {
                if (i != 23)
                    PopulateDataGridView(DataGridViews[i-1], i, Game.Items.Count(x => x.Value.SubCategory == i));
                else
                    PopulateDataGridView(DataGridViews[i-1], i, 120);
            }

            // Remove totems entry
            if (Game.GameNameCode != "IEGOGALAXY")
            {
                tabControl2.TabPages.RemoveAt(6);        
            }

            // Print items
            foreach (KeyValuePair<UInt32, Item> item in Game.SaveInfo.Inventory)
            {
                if (item.Value != null && item.Value.SubCategory != -1)
                {
                    DataGridView dataGridView = this.Controls.Find("dataGridView" + item.Value.SubCategory, true).First() as DataGridView;
                    DataGridViewComboBoxCell comboBox = (dataGridView.Rows[0].Cells[0] as DataGridViewComboBoxCell);

                    dataGridView.Rows[GetFirstEmptyRow(dataGridView)].Cells[1].Value = item.Value.Quantity;
                    dataGridView.Rows[GetFirstEmptyRow(dataGridView)].Cells[2].Value = item.Key;
                    dataGridView.Rows[GetFirstEmptyRow(dataGridView)].Cells[0].Value = comboBox.Items[comboBox.Items.IndexOf(item.Value.Name)];
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
                SaveDataGridView(DataGridViews[i-1], i);
            }

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
            // Get all index
            List<int> myItemsIndex = Enumerable.Range(0, CurrentDataGridView.Rows.Count).ToList();
            foreach (DataGridViewRow row in CurrentDataGridView.Rows)
            {
                DataGridViewComboBoxCell comboBox = (row.Cells[0] as DataGridViewComboBoxCell);

                if (row.Cells[0].Value == null) break;

                // Removes existing items
                if (myItemsIndex.IndexOf(comboBox.Items.IndexOf(comboBox.Value)) != -1) myItemsIndex.Remove(comboBox.Items.IndexOf(comboBox.Value));      
            }

            // Unlock all items
            foreach (DataGridViewRow row in CurrentDataGridView.Rows)
            {
                DataGridViewComboBoxCell comboBox = (row.Cells[0] as DataGridViewComboBoxCell);

                if (row.Cells[0].Value == null)
                {
                    // Get sub category of the item
                    int subCategory = Convert.ToInt32(CurrentDataGridView.Name.Replace("dataGridView", ""));

                    // Update row
                    row.Cells[1].Value = 1;
                    row.Cells[0].Value = comboBox.Items[myItemsIndex[0]];

                    // Create new item from comboBox text and subCategory
                    Item newItem = Game.Items.FirstOrDefault(x => x.Value.Name == row.Cells[0].Value.ToString() & x.Value.SubCategory == subCategory).Value;
                    newItem.Quantity = 1;

                    // Update row Index and Inventory
                    row.Cells[2].Value = Game.SaveInfo.Inventory.First(x => x.Value.Name == " " & x.Value.Category == newItem.Category).Key;
                    Game.SaveInfo.Inventory[Convert.ToUInt32(row.Cells[2].Value.ToString())] = newItem;

                    myItemsIndex.Remove(myItemsIndex[0]);
                }
            }
        }

        private void X99AllItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // set x99 all owned items
            foreach (DataGridViewRow row in CurrentDataGridView.Rows)
            {
                if (row.Cells[0].Value != null)
                    row.Cells[1].Value = 99;
            }
        }
    }
}
