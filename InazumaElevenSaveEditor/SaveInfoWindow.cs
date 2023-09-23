using System;
using System.Windows.Forms;
using InazumaElevenSaveEditor.InazumaEleven.Saves;

namespace InazumaElevenSaveEditor
{
    public partial class SaveInfoWindow : Form
    {
        private ISave Save = null;

        public SaveInfoWindow(ISave save)
        {
            InitializeComponent();

            Save = save;
        }

        private void SaveInfoWindow_Load(object sender, EventArgs e)
        {
            // Change Text Depending Of The Game Loaded
            switch (Save.Game.Code)
            {
                case "IEGO":
                    label2.Text = "Team Name";
                    label9.Visible = true;
                    numericUpDown6.Visible = true;
                    button1.Text = "Unlock Data Download + Password + Link with Go Content";
                    break;
                case "IEGOCS":
                    label2.Text = "Chrono Stone Name";
                    label9.Visible = true;
                    numericUpDown6.Visible = true;
                    button1.Text = "Unlock Data Download + Password + Link with Go Content";
                    break;
                case "IEGOGALAXY":
                    label2.Text = "Team Name";
                    textBox2.Enabled = false;
                    label9.Visible = false;
                    numericUpDown6.Visible = false;
                    button1.Text = "Unlock Data Download + QRcode + Link with GO/CS Content";
                    break;
            }

            Save.Game.OpenSaveInfo();

            textBox1.Text = Save.Game.SaveInfo.Name;
            textBox2.Text = Save.Game.SaveInfo.TeamName;

            if (Save.Game.Code == "IEGO")
            {
                label5.Visible = false;
                numericUpDown1.Visible = false;
                button2.Enabled = true;
            } else
            {
                label5.Visible = true;
                numericUpDown1.Visible = true;
                button2.Enabled = false;
                numericUpDown1.Value = Save.Game.SaveInfo.SecretLinkLevel;
            }

            numericUpDown3.Value = Save.Game.SaveInfo.Hours;
            numericUpDown4.Value = Save.Game.SaveInfo.Min;

            if (Save.Game.SaveInfo.Chapter != -1)
            {
                numericUpDown12.Value = Save.Game.SaveInfo.Chapter;
            } else
            {
                numericUpDown12.Enabled = false;
            }

            if (numericUpDown12.Value < 2)
            {
                button1.Enabled = false;
            }
            else
            {
                button1.Enabled = true;
            }

            if (numericUpDown12.Value < 10)
            {
                numericUpDown1.Maximum = 2;
            }
            else
            {
                numericUpDown1.Maximum = 3;
            }

            numericUpDown5.Value = Save.Game.SaveInfo.Prestige;

            if (Save.Game.Code == "IEGOGALAXY")
            {
                for (int i = 0; i < 5; i++)
                {
                    PictureBox coinPictureBox = (PictureBox)tabPage2.Controls.Find("coinPictureBox" + (i + 1), false)[0];
                    NumericUpDown coinNumericUpDown = (NumericUpDown)tabPage2.Controls.Find("coinNumericUpDown" + (i + 1), false)[0];
                    coinPictureBox.Visible = true;
                    coinNumericUpDown.Visible = true;
                    coinNumericUpDown.Value = Save.Game.SaveInfo.Coins[i];
                }
            }
            else
            {
                numericUpDown6.Value = Save.Game.SaveInfo.Friendship;
            }
        }

        private void SaveInfoWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Save.Game.SaveInfo.Name = textBox1.Text;
            Save.Game.SaveInfo.TeamName = textBox2.Text;
            Save.Game.SaveInfo.Hours = Convert.ToInt32(numericUpDown3.Value);
            Save.Game.SaveInfo.Min = Convert.ToInt32(numericUpDown4.Value);
            Save.Game.SaveInfo.Prestige = Convert.ToInt32(numericUpDown5.Value);
            Save.Game.SaveInfo.Friendship = Convert.ToInt32(numericUpDown6.Value);


            for (int i = 0; i < 5; i++)
            {
                NumericUpDown coinNumericUpDown = (NumericUpDown)tabPage2.Controls.Find("coinNumericUpDown" + (i + 1), false)[0];
                Save.Game.SaveInfo.Coins[i] = Convert.ToInt32(coinNumericUpDown.Value);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Save.Game.SaveInfo.UnlockAllData = true;
            MessageBox.Show("All additional content has been added, check the Inalink for more details!");
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (!numericUpDown1.Focused) return;

            if (numericUpDown12.Value == 10 & numericUpDown1.Value == 3)
            {
                DialogResult dialogResult = MessageBox.Show("You must defeat the exclusive team in your version before activating the level 3 secret.\nIf you activate the level 3 secret without defeating the exclusive team, you will get a glitched save file.\n\nHave you defeated the exclusive team? If you don't know, please answer no.", "Warning", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                {
                    numericUpDown1.Value = 2;
                }
            }

            Save.Game.SaveInfo.SecretLinkLevel = Convert.ToInt32(numericUpDown1.Value);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("You must defeat the exclusive team in your version before activating the level 3 secret.\nIf you activate the level 3 secret without defeating the exclusive team, you will get a glitched save file.\n\nHave you defeated the exclusive team? If you don't know, please answer no.", "Warning", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                MessageBox.Show("Link Level 3 Enabled!");
                Save.Game.SaveInfo.SecretLinkLevel = 3;
            }
        }
    }
}
