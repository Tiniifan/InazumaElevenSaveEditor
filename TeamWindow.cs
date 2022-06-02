using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InazumaElevenSaveEditor.Logic;
using InazumaElevenSaveEditor.Tools;
using InazumaElevenSaveEditor.Formats;
using InazumaElevenSaveEditor.Common;
using InazumaElevenSaveEditor.Formats.Saves;
using System.Text.RegularExpressions;
using System.Windows.Forms.DataVisualization.Charting;

namespace InazumaElevenSaveEditor
{
    public partial class TeamWindow : Form
    {
        public ContainerGames Game = null;
        private List<string> FormationTextContent = new List<string>();

        public TeamWindow(ContainerGames _Game)
        {
            InitializeComponent();
            Game = _Game;
        }

        private void InitializeRessource()
        {
            teamTextBox.Name = null;
            teamNumericUpDown.Value = 0;
            formationBox.Items.Clear();
            coachBox.Items.Clear();
            kitBox.Items.Clear();
            emblemBox.Items.Clear();

            // Fills Emblem/Kit/Formation/Coach box
            foreach (KeyValuePair<UInt32, Item> entry in Game.SaveInfo.Inventory)
            {
                if (entry.Value.SubCategory == 16)
                {
                    formationBox.Items.Add(entry.Value.Name);
                }
                else if (entry.Value.SubCategory == 17)
                {
                    coachBox.Items.Add(entry.Value.Name);
                }
                else if (entry.Value.SubCategory == 19)
                {
                    kitBox.Items.Add(entry.Value.Name);
                }
                else if (entry.Value.SubCategory == 20)
                {
                    emblemBox.Items.Add(entry.Value.Name);
                }
            }

            // Fills playerComBoBox with Player Names from the save
            for (int i = 0; i < Game.PlayersInSaveSort.Count; i++)
            {
                playerComboBox1.Items.Add(Game.GetPlayer(i).Name);
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
            formationBox.SelectedIndex = formationBox.Items.IndexOf(team.Formation.Name);
            coachBox.SelectedIndex = coachBox.Items.IndexOf(team.Coach.Name);
            kitBox.SelectedIndex = kitBox.Items.IndexOf(team.Kit.Name);
            emblemBox.SelectedIndex = emblemBox.Items.IndexOf(team.Emblem.Name);

            // Fills playerIndexComBoBox with Player Names from the Team
            playerIndexComboBox1.Items.Clear();
            foreach (KeyValuePair<UInt32, Player> entry in team.Players)
            {
                if (entry.Value != null)
                {
                    playerIndexComboBox1.Items.Add(entry.Value.Name);
                } else
                {
                    playerIndexComboBox1.Items.Add(" ");
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
            foreach (KeyValuePair<UInt32, Player> player in team.Players)
            {
                ComboBox playerComboBox = this.Controls.Find("playerComboBox" + playerNumber, true).First() as ComboBox;
                ComboBox playerIndexComboBox = this.Controls.Find("playerIndexComboBox" + team.PlayersFormationIndex[playerNumber-1], true).First() as ComboBox;
                NumericUpDown playerNumericUpDown = this.Controls.Find("playerNumericUpDown" + (playerNumber), true).First() as NumericUpDown;

                playerComboBox.SelectedIndex = Game.PlayersInSaveSort.IndexOf(player.Key);
                playerIndexComboBox.SelectedIndex = playerNumber - 1;
                playerNumericUpDown.Value = team.PlayersKitNumber[playerNumber-1];
                playerNumber++;
            };

            // Calc The Maximum Of Some Values
            double totalPlayer = team.Players.SumIf(x => x.Value != null, x => 1.0);
            double totalKick = team.Players.SumIf(x => x.Value != null, x => x.Value.Stat[2] + x.Value.InvestedPoint[0] + x.Value.Equipments[0].Stat[2]);
            double totalDribble = team.Players.SumIf(x => x.Value != null, x => x.Value.Stat[3] + x.Value.InvestedPoint[1] + x.Value.Equipments[2].Stat[3]);
            double totalBlock = team.Players.SumIf(x => x.Value != null, x => x.Value.Stat[5] + x.Value.InvestedPoint[3] + x.Value.Equipments[2].Stat[5]);
            double totalCatch = team.Players.SumIf(x => x.Value != null, x => x.Value.Stat[8] + x.Value.InvestedPoint[6] + x.Value.Equipments[3].Stat[8]);
            double totalTechnique = team.Players.SumIf(x => x.Value != null, x => x.Value.Stat[4] + x.Value.InvestedPoint[2] + x.Value.Equipments[3].Stat[4]);
            double totalSpeed = team.Players.SumIf(x => x.Value != null, x => x.Value.Stat[6] + x.Value.InvestedPoint[4] + x.Value.Equipments[0].Stat[6]);
            double totalStamina = team.Players.SumIf(x => x.Value != null, x => x.Value.Stat[7] + x.Value.InvestedPoint[5] + x.Value.Equipments[1].Stat[7]);
            double totalLuck = team.Players.SumIf(x => x.Value != null, x => x.Value.Stat[9] + x.Value.InvestedPoint[7] + x.Value.Equipments[1].Stat[9]);

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
            dribbleLabel.Text = "Dribble: "  + totalDribble;
            blockLabel.Text = "Block: "  + totalBlock;
            catchLabel.Text = "Catch: "  + totalCatch;
            techniqueLabel.Text = "Tech. "  + totalTechnique;
            speedLabel.Text = "Speed: "  + totalSpeed;
            staminaLabel.Text = "Stamina: " + totalStamina;
            luckLabel.Text = "Luck: "  + totalLuck;
            if (totalPlayer != 0)
            {
                windLabel.Text = "Wind: " + Convert.ToInt32(team.Players.SumIf(x => x.Value != null && x.Value.Element.Name == "Wind", x => 1.0) / totalPlayer * 100) + "%";
                woodLabel.Text = "Wood: " + Convert.ToInt32(team.Players.SumIf(x => x.Value != null && x.Value.Element.Name == "Wood", x => 1.0) / totalPlayer * 100) + "%";
                fireLabel.Text = "Fire: " + Convert.ToInt32(team.Players.SumIf(x => x.Value != null && x.Value.Element.Name == "Fire", x => 1.0) / totalPlayer * 100) + "%";
                earthLabel.Text = "Earth: " + Convert.ToInt32(team.Players.SumIf(x => x.Value != null && x.Value.Element.Name == "Earth", x => 1.0) / totalPlayer * 100) + "%";
                boyLabel.Text = "Boy: " + Convert.ToInt32(team.Players.SumIf(x => x.Value != null && x.Value.Gender.Name == "Boy", x => 1.0) / totalPlayer * 100) + "%";
                girlLabel.Text = "Girl: " + Convert.ToInt32(team.Players.SumIf(x => x.Value != null && x.Value.Gender.Name == "Girl", x => 1.0) / totalPlayer * 100) + "%";
                unknownLabel.Text = "Unknown: " + Convert.ToInt32(team.Players.SumIf(x => x.Value != null && x.Value.Gender.Name == "Unknown", x => 1.0) / totalPlayer * 100) + "%";
                teamNumericUpDown.Value = Convert.ToInt32(team.Players.SumIf(x => x.Value != null, x => x.Value.Level / totalPlayer));
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
            FormationTextContent = Properties.Resources.ResourceManager.GetObject(team.Formation.Name).ToString().Split('\n').ToList();
            strategyComboBox.SelectedIndex = 0;
        }

        private void TeamWindow_Load(object sender, EventArgs e)
        {
            InitializeRessource();

            if (Game.SaveInfo.Teams == null)
            {
                Game.OpenTactics();
            }

            for (int i = 0; i < Game.SaveInfo.Teams.Count; i++)
            {
                teamListBox.Items.Add(Game.SaveInfo.Teams[i].Name);
            }
        }

        private void TeamListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (teamListBox.SelectedIndex == -1) return;

            PrintTeam(Game.SaveInfo.Teams[teamListBox.SelectedIndex]);
            tabControl1.Enabled = true;
        }

        private void StrategyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (strategyComboBox.SelectedIndex == -1) return;

            for (int i = 0; i < 11; i++)
            {
                ComboBox playerIndexComboBox = this.Controls.Find("playerIndexComboBox" + (i+1), true).First() as ComboBox;

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

                chart1.Series["player" + i].Label = playerIndexComboBox.Text;
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
    }

    public static class Extended
    {
        public static double SumIf<T>(this IEnumerable<T> source, Func<T, bool> predicate, Func<T, double> valueSelector)
        {
            return source.Where(predicate)
                         .Sum(valueSelector);
        }
    }
}
