using System;
using System.Windows.Forms;
using InazumaElevenSaveEditor.Formats;

namespace InazumaElevenSaveEditor
{
    public partial class SaveInfoWindow : Form
    {
        public IGame Game = null;

        public SaveInfoWindow(IGame _Game)
        {
            InitializeComponent();
            Game = _Game;
        }

        private void SaveInfoWindow_Load(object sender, EventArgs e)
        {
            // Change Text Depending Of The Game Loaded
            switch (Game.GameNameCode)
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

            Game.OpenSaveInfo();

            textBox1.Text = Game.SaveInfo.Name;
            textBox2.Text = Game.SaveInfo.TeamName;
            numericUpDown1.Value = Game.SaveInfo.SecretLinkLevel;
            numericUpDown3.Value = Game.SaveInfo.Hours;
            numericUpDown4.Value = Game.SaveInfo.Min;

            numericUpDown12.Value = Game.SaveInfo.Chapter;

            if (numericUpDown12.Value < 2)
            {
                button1.Enabled = false;
            } else
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

            numericUpDown5.Value = Game.SaveInfo.Prestige;

            if (Game.GameNameCode == "IEGOGALAXY")
            {
                for (int i = 0; i < 5; i++)
                {
                    PictureBox coinPictureBox = (PictureBox)tabPage2.Controls.Find("coinPictureBox" + (i + 1), false)[0];
                    NumericUpDown coinNumericUpDown = (NumericUpDown)tabPage2.Controls.Find("coinNumericUpDown" + (i + 1), false)[0];
                    coinPictureBox.Visible = true;
                    coinNumericUpDown.Visible = true;
                    coinNumericUpDown.Value = Game.SaveInfo.Coins[i];
                }
            } else
            {
                numericUpDown6.Value = Game.SaveInfo.Friendship;
            }
        }

        private void SaveInfoWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Game.SaveInfo.Name = textBox1.Text;
            Game.SaveInfo.TeamName = textBox2.Text;
            Game.SaveInfo.Hours = Convert.ToInt32(numericUpDown3.Value);
            Game.SaveInfo.Min = Convert.ToInt32(numericUpDown4.Value);
            Game.SaveInfo.Prestige = Convert.ToInt32(numericUpDown5.Value);
            Game.SaveInfo.Friendship = Convert.ToInt32(numericUpDown6.Value);

            for (int i = 0; i < 5; i++)
            {
                NumericUpDown coinNumericUpDown = (NumericUpDown)tabPage2.Controls.Find("coinNumericUpDown" + (i + 1), false)[0];
                Game.SaveInfo.Coins[i] = Convert.ToInt32(coinNumericUpDown.Value);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Game.SaveInfo.UnlockAllData = true;
            MessageBox.Show("All additional content has been added, check the Inalink for more details!");
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (!numericUpDown1.Focused) return;

            if (numericUpDown12.Value == 10 & numericUpDown1.Value == 3)
            {
                DialogResult dialogResult = MessageBox.Show("You must have beaten your exclusive team before activate the Link Level 3\nDo you want to activate the Link Level 3?", "Warning", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                {
                    numericUpDown1.Value = 2;
                }
            }

            Game.SaveInfo.SecretLinkLevel = Convert.ToInt32(numericUpDown1.Value);
        }
    }
}
