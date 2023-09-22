using System;
using System.Windows.Forms;
using System.Collections.Generic;
using InazumaElevenSaveEditor.InazumaEleven.Logic;
using InazumaElevenSaveEditor.InazumaEleven.Saves;

namespace InazumaElevenSaveEditor
{
    public partial class PlayRecordsWindow : Form
    {
        private ISave Save = null;

        public PlayRecordsWindow(ISave save)
        {
            InitializeComponent();

            Save = save;
        }

        private void PlayRecordsWindow_Load(object sender, EventArgs e)
        {
            Save.Game.OpenPlayRecords();

            foreach (KeyValuePair<int, List<PlayRecord>> playRecord in Save.Game.PlayRecords)
            {
                for (int i = 0; i < playRecord.Value.Count; i++)
                {
                    dataGridView1.Rows.Add(playRecord.Value[i].Name, playRecord.Value[i].Unlocked);
                }
            }

            dataGridView1.AllowUserToAddRows = false;
            resetAllButton.Enabled = true;
            unlockAllButton.Enabled = true;
        }

        private void ResetAllButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Cells[1].Value = false;
            }
        }

        private void UnlockAllButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Cells[1].Value = true;
            }
        }

        private void PlayRecordsWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            int key = 0;
            int count = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                Save.Game.PlayRecords[key][count].Unlocked = Convert.ToBoolean(row.Cells[1].Value);
                count++;

                if (count >= Save.Game.PlayRecords[key].Count)
                {
                    key++;
                    count = 0;
                }
            }
        }
    }
}
