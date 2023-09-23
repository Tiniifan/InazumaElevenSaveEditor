using System;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using InazumaElevenSaveEditor.Tools;
using System.Text.RegularExpressions;
using System.Windows.Forms.DataVisualization.Charting;
using InazumaElevenSaveEditor.InazumaEleven.Logic;
using InazumaElevenSaveEditor.InazumaEleven.Saves;

namespace InazumaElevenSaveEditor
{
    public partial class TeamWindow : Form
    {
        private ISave Save = null;

        private List<string> FormationTextContent = new List<string>();

        public TeamWindow(ISave save)
        {
            InitializeComponent();

            Save = save;
        }

        private void BindRessource()
        {
            teamTextBox.Name = null;
            teamNumericUpDown.Value = 0;
            formationBox.Items.Clear();
            coachBox.Items.Clear();
            kitBox.Items.Clear();
            emblemBox.Items.Clear();

            // Fills Emblem/Kit/Formation/Coach box
            foreach (KeyValuePair<int, Item> entry in Save.Game.Inventory)
            {
                if (entry.Value.SubCategory == 16)
                {
                    if (entry.Value.Name.StartsWith("B-") == false)
                    {
                        formationBox.Items.Add(entry.Value);
                    }
                }
                else if (entry.Value.SubCategory == 17)
                {
                    coachBox.Items.Add(entry.Value);
                }
                else if (entry.Value.SubCategory == 19)
                {
                    kitBox.Items.Add(entry.Value);
                }
                else if (entry.Value.SubCategory == 20)
                {
                    emblemBox.Items.Add(entry.Value);
                }
            }

            // Create empty player
            Player emptyPlayer = new Player();
            emptyPlayer.Name = " ";
            emptyPlayer.Index = 0x0;
            List<Player> players = new List<Player>();
            players.AddRange(Save.Game.Reserve);
            players.Add(emptyPlayer);

            // Fills playerComBoBox with Player Names from the save
            for (int i = 0; i < players.Count; i++)
            {
                playerComboBox1.Items.Add(players[i]);
            }
            for (int i = 1; i < 16; i++)
            {
                ComboBox playerComboBox = this.Controls.Find("playerComboBox" + (i + 1), true).First() as ComboBox;
                playerComboBox.Items.AddRange(playerComboBox1.Items.Cast<Object>().ToArray());
            }

            // Create Background Template
            chart1.Images.Add(new NamedImage("FormationTemplate.png", Properties.Resources.FormationTemplate));
            chart1.ChartAreas[0].BackImage = "FormationTemplate.png";
        }

        private void PrintTeam(Team team)
        {
            teamTextBox.Text = team.Name;
            formationBox.SelectedIndex = (team.Formation == null) ? -1 : formationBox.Items.IndexOf(team.Formation);
            coachBox.SelectedIndex = (team.Coach == null) ? -1 : coachBox.Items.IndexOf(team.Coach);
            kitBox.SelectedIndex = (team.Kit == null) ? -1 : kitBox.Items.IndexOf(team.Kit);
            emblemBox.SelectedIndex = (team.Emblem == null) ? -1 : emblemBox.Items.IndexOf(team.Emblem);

            // Fills playerIndexComBoBox with Player Names from the Team
            playerIndexComboBox1.Items.Clear();
            for (int i = 0; i < 16; i++)
            {
                if (i < team.Players.Count)
                {
                    playerIndexComboBox1.Items.Add(team.Players[i]);
                }
                else
                {
                    playerIndexComboBox1.Items.Add(playerComboBox1.Items[playerComboBox1.Items.Count - 1]);
                }
            }
            for (int i = 1; i < 16; i++)
            {
                ComboBox playerIndexComboBox = this.Controls.Find("playerIndexComboBox" + (i + 1), true).First() as ComboBox;
                playerIndexComboBox.Items.Clear();
                playerIndexComboBox.Items.AddRange(playerIndexComboBox1.Items.Cast<Object>().ToArray());
            }

            // Print Player Name of the team and Player Number Kit
            int playerNumber = 1;
            for (int i = 0; i < 16; i++)
            {
                ComboBox playerComboBox = this.Controls.Find("playerComboBox" + playerNumber, true).First() as ComboBox;
                ComboBox playerIndexComboBox = this.Controls.Find("playerIndexComboBox" + team.PlayersFormationIndex[playerNumber - 1], true).First() as ComboBox;
                NumericUpDown playerNumericUpDown = this.Controls.Find("playerNumericUpDown" + (playerNumber), true).First() as NumericUpDown;

                if (i < team.Players.Count)
                {
                    playerComboBox.SelectedItem = team.Players[i];
                } else
                {
                    playerComboBox.SelectedIndex = playerComboBox.Items.Count - 1;
                }

                playerIndexComboBox.SelectedIndex = playerNumber - 1;
                playerNumericUpDown.Value = team.PlayersKitNumber[playerNumber - 1];

                playerNumber++;
            }

            // Calc The Maximum Of Some Values
            double totalPlayer = team.Players.SumIf(x => x != null, x => 1.0);
            double totalKick = team.Players.SumIf(x => x != null, x => x.Stat[2] + x.InvestedPoint[0] + x.Equipments[0].Stat[2]);
            double totalDribble = team.Players.SumIf(x => x != null, x => x.Stat[3] + x.InvestedPoint[1] + x.Equipments[2].Stat[3]);
            double totalBlock = team.Players.SumIf(x => x != null, x => x.Stat[5] + x.InvestedPoint[3] + x.Equipments[2].Stat[5]);
            double totalCatch = team.Players.SumIf(x => x != null, x => x.Stat[8] + x.InvestedPoint[6] + x.Equipments[3].Stat[8]);
            double totalTechnique = team.Players.SumIf(x => x != null, x => x.Stat[4] + x.InvestedPoint[2] + x.Equipments[3].Stat[4]);
            double totalSpeed = team.Players.SumIf(x => x != null, x => x.Stat[6] + x.InvestedPoint[4] + x.Equipments[0].Stat[6]);
            double totalStamina = team.Players.SumIf(x => x != null, x => x.Stat[7] + x.InvestedPoint[5] + x.Equipments[1].Stat[7]);
            double totalLuck = team.Players.SumIf(x => x != null, x => x.Stat[9] + x.InvestedPoint[7] + x.Equipments[1].Stat[9]);

            // Draw Point (Graphic Stat)
            chart2.Series[0].Points.Clear();
            chart2.Series[0].Points.AddY(totalKick);
            chart2.Series[0].Points.AddY(totalDribble);
            chart2.Series[0].Points.AddY(totalBlock);
            chart2.Series[0].Points.AddY(totalCatch);
            chart2.Series[0].Points.AddY(totalTechnique);
            chart2.Series[0].Points.AddY(totalSpeed);
            chart2.Series[0].Points.AddY(totalStamina);
            chart2.Series[0].Points.AddY(totalLuck);

            // Draw Label and Team Level
            kickLabel.Text = "Kick: " + totalKick;
            dribbleLabel.Text = "Dribble: " + totalDribble;
            blockLabel.Text = "Block: " + totalBlock;
            catchLabel.Text = "Catch: " + totalCatch;
            techniqueLabel.Text = "Tech. " + totalTechnique;
            speedLabel.Text = "Speed: " + totalSpeed;
            staminaLabel.Text = "Stamina: " + totalStamina;
            luckLabel.Text = "Luck: " + totalLuck;
            if (totalPlayer != 0)
            {
                windLabel.Text = "Wind: " + Convert.ToInt32(team.Players.SumIf(x => x != null && x.Element.Name == "Wind", x => 1.0) / totalPlayer * 100) + "%";
                woodLabel.Text = "Wood: " + Convert.ToInt32(team.Players.SumIf(x => x != null && x.Element.Name == "Wood", x => 1.0) / totalPlayer * 100) + "%";
                fireLabel.Text = "Fire: " + Convert.ToInt32(team.Players.SumIf(x => x != null && x.Element.Name == "Fire", x => 1.0) / totalPlayer * 100) + "%";
                earthLabel.Text = "Earth: " + Convert.ToInt32(team.Players.SumIf(x => x != null && x.Element.Name == "Earth", x => 1.0) / totalPlayer * 100) + "%";
                boyLabel.Text = "Boy: " + Convert.ToInt32(team.Players.SumIf(x => x != null && x.Gender.Name == "Boy", x => 1.0) / totalPlayer * 100) + "%";
                girlLabel.Text = "Girl: " + Convert.ToInt32(team.Players.SumIf(x => x != null && x.Gender.Name == "Girl", x => 1.0) / totalPlayer * 100) + "%";
                unknownLabel.Text = "Unknown: " + Convert.ToInt32(team.Players.SumIf(x => x != null && x.Gender.Name == "Unknown", x => 1.0) / totalPlayer * 100) + "%";
                teamNumericUpDown.Value = Convert.ToInt32(team.Players.SumIf(x => x != null, x => x.Level / totalPlayer));
            }
            else
            {
                windLabel.Text = "Wind: 0";
                woodLabel.Text = "Wood: 0";
                fireLabel.Text = "Fire: 0";
                earthLabel.Text = "Earth: 0";
                boyLabel.Text = "Boy: 0";
                girlLabel.Text = "Girl: 0";
                unknownLabel.Text = "Unknown: 0";
                teamNumericUpDown.Value = 0;
            }

            // Update Formation
            FormationTextContent = new ResourceReader("InazumaElevenSaveEditor.InazumaEleven.Common.GO.Formations.F-Gihl.txt").Content;
            strategyComboBox.SelectedIndex = 0;
            StrategyComboBox_SelectedIndexChanged(strategyComboBox, EventArgs.Empty);
        }

        private void TeamWindow_Load(object sender, EventArgs e)
        {
            BindRessource();

            if (Save.Game.Teams == null)
            {
                Save.Game.OpenTactics();
            }

            for (int i = 0; i < Save.Game.Teams.Count; i++)
            {
                teamListBox.Items.Add(Save.Game.Teams[i].Name);
            }
        }

        private void TeamListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (teamListBox.SelectedIndex == -1) return;

            PrintTeam(Save.Game.Teams[teamListBox.SelectedIndex]);
            tabControl1.Enabled = true;
        }

        private void StrategyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (strategyComboBox.SelectedIndex == -1) return;

            for (int i = 0; i < 11; i++)
            {
                ComboBox playerIndexComboBox = this.Controls.Find("playerIndexComboBox" + (i + 1), true).First() as ComboBox;
                NumericUpDown numericUpDown = this.Controls.Find("playerNumericUpDown" + (i + 1), true).First() as NumericUpDown;

                List<string> playerInformation = Regex.Split(Regex.Replace(FormationTextContent[strategyComboBox.SelectedIndex * 17 + 13 - i].Trim(), @"(\[|\],|\"")", ""), @",\s").ToList();

                switch (playerInformation[1])
                {
                    case "GK":
                        chart1.Series["player" + i].LabelForeColor = Color.Orange;
                        break;
                    case "DF":
                        chart1.Series["player" + i].LabelForeColor = Color.Green;
                        break;
                    case "MF":
                        chart1.Series["player" + i].LabelForeColor = Color.Blue;
                        break;
                    case "FW":
                        chart1.Series["player" + i].LabelForeColor = Color.Red;
                        break;
                    default:
                        chart1.Series["player" + i].LabelForeColor = Color.Orange;
                        break;
                }

                chart1.Series["player" + i].Label = playerIndexComboBox.Text + " (" + numericUpDown.Value + ")";
                chart1.Series["player" + i].Points.Clear();
                chart1.Series["player" + i].Points.AddXY(Convert.ToDecimal(playerInformation[2]), Convert.ToDecimal(playerInformation[3]));
            }

            chart1.Enabled = true;
            chart1.ChartAreas[0].Visible = true;
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
            {
                this.MinimumSize = new Size(657, 447);
                this.MaximumSize = new Size(0, 0);
            }
            else if (tabControl1.SelectedIndex == 2)
            {
                TabControl2_SelectedIndexChanged(sender, e);
            } else 
            {
                this.MinimumSize = new Size(657, 447);
                this.MaximumSize = new Size(657, 447);
                this.Size = new Size(657, 447);
            }
        }

        private void TabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl2.SelectedIndex == 0)
            {
                this.MinimumSize = new Size(657, 447);
                this.MaximumSize = new Size(0, 0);
                this.Size = new Size(744, 529);
            } else
            {
                this.MinimumSize = new Size(596, 356);
                this.MaximumSize = new Size(596, 356);
                this.Size = new Size(596, 356);
            }
        }

        private void EmblemBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!emblemBox.Focused) return;

            Save.Game.Teams[teamListBox.SelectedIndex].Emblem = emblemBox.SelectedItem as Item;
        }

        private void KitBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!kitBox.Focused) return;

            Save.Game.Teams[teamListBox.SelectedIndex].Kit = kitBox.SelectedItem as Item;
        }

        private void CoachBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!coachBox.Focused) return;

            Save.Game.Teams[teamListBox.SelectedIndex].Coach = coachBox.SelectedItem as Item;
        }

        private void FormationBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!formationBox.Focused) return;

            Save.Game.Teams[teamListBox.SelectedIndex].Formation = formationBox.SelectedItem as Item;
            FormationTextContent = new ResourceReader("InazumaElevenSaveEditor.InazumaEleven.Common.GO.Formations.F-Gihl.txt").Content;
            StrategyComboBox_SelectedIndexChanged(sender, e);
        }

        private void PlayerNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown numericUpDown = sender as NumericUpDown;
            if (!numericUpDown.Focused) return;

            int index = Convert.ToInt32(numericUpDown.Name.Replace("playerNumericUpDown", "")) - 1;
            Save.Game.Teams[teamListBox.SelectedIndex].PlayersKitNumber[index] = Convert.ToInt32(numericUpDown.Value);
            PrintTeam(Save.Game.Teams[teamListBox.SelectedIndex]);
        }

        private void PlayerComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (!comboBox.Focused) return;

            int index = Convert.ToInt32(comboBox.Name.Replace("playerComboBox", "")) - 1;
            Save.Game.Teams[teamListBox.SelectedIndex].Players[index] = comboBox.SelectedItem as Player;
            PrintTeam(Save.Game.Teams[teamListBox.SelectedIndex]);
        }

        private void PlayerIndexComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (!comboBox.Focused) return;

            // Get player index
            int index = Convert.ToInt32(comboBox.Name.Replace("playerIndexComboBox", ""));
            index = Save.Game.Teams[teamListBox.SelectedIndex].PlayersFormationIndex.FindIndex(x => x == index);
            int newIndex = Save.Game.Teams[teamListBox.SelectedIndex].Players.FindIndex(x => x == comboBox.SelectedItem);

            // Get player formation index
            int formationIndex = Save.Game.Teams[teamListBox.SelectedIndex].PlayersFormationIndex[index];
            int formationNewIndex = Save.Game.Teams[teamListBox.SelectedIndex].PlayersFormationIndex[newIndex];

            // Swap formation Index
            Save.Game.Teams[teamListBox.SelectedIndex].PlayersFormationIndex[index] = formationNewIndex;
            Save.Game.Teams[teamListBox.SelectedIndex].PlayersFormationIndex[newIndex] = formationIndex;

            // Update form
            ComboBox playerIndexComboBox = this.Controls.Find("playerIndexComboBox" + (formationNewIndex), true).First() as ComboBox;
            playerIndexComboBox.SelectedItem = Save.Game.Teams[teamListBox.SelectedIndex].Players[index];
            StrategyComboBox_SelectedIndexChanged(sender, e);
        }

        private void PreviousButton_Click(object sender, EventArgs e)
        {
            if (strategyComboBox.SelectedIndex == 0)
            {
                strategyComboBox.SelectedIndex = strategyComboBox.Items.Count - 1;
            }
            else
            {
                strategyComboBox.SelectedIndex--;
            }
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            if (strategyComboBox.SelectedIndex < strategyComboBox.Items.Count - 1)
            {
                strategyComboBox.SelectedIndex++;
            }
            else
            {
                strategyComboBox.SelectedIndex = 0;
            }
        }
    }
}
