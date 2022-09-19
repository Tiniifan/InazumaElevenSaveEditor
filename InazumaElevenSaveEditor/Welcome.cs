using System;
using System.IO;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using InazumaElevenSaveEditor;
using System.Collections.Generic;
using InazumaElevenSaveEditor.Logic;
using InazumaElevenSaveEditor.Tools;
using InazumaElevenSaveEditor.Formats;
using InazumaElevenSaveEditor.Formats.Saves;
using InazumaElevenSaveEditor.Formats.Player_Files;

namespace NoFarmForMeOpenSource
{
    public partial class Welcome : Form
    {
        private DataReader FileData = null;

        private IFormat Format = null;

        private IGame Game = null;

        private List<int> SelectedPlayers = null;

        public Welcome(string path)
        {
            InitializeComponent();

            if (path != string.Empty)
            {
                openFileDialog1.FileName = path;
                LoadFile(openFileDialog1.FileName);
            }
        }

        private void InitializeRessource()
        {
            // Print Name Code Of The Game
            this.Text = Game.GameNameCode + " Save Editor";

            // Clear All ComboBoxs
            nameBox.Items.Clear();
            avatarNameBox.Items.Clear();
            moveBox1.Items.Clear();
            moveBox2.Items.Clear();
            moveBox3.Items.Clear();
            moveBox4.Items.Clear();
            moveBox5.Items.Clear();
            moveBox6.Items.Clear();
            miximaxAvatarNameBox.Items.Clear();
            bootsBox.Items.Clear();
            glovesBox.Items.Clear();
            braceletBox.Items.Clear();
            pendantBox.Items.Clear();

            // Remove All Extra PlayerTabPages
            while (tabControl4.TabPages.Count > 1)
            {
                RemoveToolStripMenuItem_Click(tabControl4, System.EventArgs.Empty);
            }

            // Fill All ComboBoxs
            // Player Name
            nameBox.Items.AddRange(Game.Players.Select(x => x.Value.Name).ToArray());
            nameBox.Items.Remove("");

            // Avatars Name
            avatarNameBox.Items.AddRange(Game.Avatars.Select(x => x.Value.Name).ToArray());
            miximaxAvatarNameBox.Items.AddRange(avatarNameBox.Items.Cast<Object>().ToArray());

            // Special Moves Name
            moveBox1.Items.AddRange(Game.Moves.Select(x => x.Value.Name).ToArray());
            moveBox2.Items.AddRange(moveBox1.Items.Cast<Object>().ToArray());
            moveBox3.Items.AddRange(moveBox1.Items.Cast<Object>().ToArray());
            moveBox4.Items.AddRange(moveBox1.Items.Cast<Object>().ToArray());
            moveBox5.Items.AddRange(moveBox1.Items.Cast<Object>().ToArray());
            moveBox6.Items.AddRange(moveBox1.Items.Cast<Object>().ToArray());

            // Equipment Name
            bootsBox.Items.AddRange(Game.Equipments.Where(x => x.Value.Type.Name == "Boots").Select(x => x.Value.Name).ToArray());
            glovesBox.Items.AddRange(Game.Equipments.Where(x => x.Value.Type.Name == "Gloves").Select(x => x.Value.Name).ToArray());
            braceletBox.Items.AddRange(Game.Equipments.Where(x => x.Value.Type.Name == "Bracelet").Select(x => x.Value.Name).ToArray());
            pendantBox.Items.AddRange(Game.Equipments.Where(x => x.Value.Type.Name == "Pendant").Select(x => x.Value.Name).ToArray());
        }

        private void CreatePage(int maximum)
        {
            pageComboBox.Items.Clear();
            pageComboBox.Items.Add("Main Players");

            for (int page = 1; page < maximum ; page++)
            {
                pageComboBox.Items.Add("Reserves " + page +  "/" + (maximum-1));
            }
        }

        private void PrintPage(int page)
        {
            for (int i = 0; i < 16; i++)
            {
                int index = page * 16 + i;
                PictureBox playerPictureBox = this.Controls.Find("playerPictureBox" + (i + 1), true).First() as PictureBox;

                if (index < Game.PlayersInSave.Count)
                {
                    playerPictureBox.Image = Draw.DrawString(InazumaElevenSaveEditor.Properties.Resources.PlayerRectangleBox, Game.GetPlayer(index).Name, 3, 2);
                } 
                else
                {
                    playerPictureBox.Image = InazumaElevenSaveEditor.Properties.Resources.PlayerRectangleBox;
                }
            }
        }

        private void PrintPlayer(Player player)
        {
            tabPage3.Focus();
            tabControl4.TabPages[tabControl4.SelectedIndex].Text = player.Name + " (Lv." + player.Level + ")";

            // Print General Player Information
            nameBox.SelectedIndex = nameBox.Items.IndexOf(player.Name);
            positionBox.Text = player.Position.ToString();
            elementBox.Text = player.Element.ToString();
            genderBox.Text = player.Gender.ToString();
            levelNumericUpDown.Value = player.Level;
            styleBox.SelectedIndex = player.Style;
            scoreNumericUpDown.Value = player.Score;
            participationNumericUpDown.Value = player.Participation;

            // Print Player Stat Level 99 and Freedom
            for (int i = 0; i < player.Stat.Count; i++)
            {
                TextBox statBox = this.Controls.Find("statBox" + (i + 1), true).First() as TextBox;
                statBox.Text = player.Stat[i].ToString();
            }
            freedomBox.Text = player.Freedom.ToString();

            // Print Invested Point
            for (int i = 0; i < player.InvestedPoint.Count; i++)
            {
                NumericUpDown investedNumericUpDown = this.Controls.Find("investedNumericUpDown" + (i + 3), true).First() as NumericUpDown;
                investedNumericUpDown.Maximum = 65535;
                investedNumericUpDown.Minimum = -65535;
                TextBox statBox = this.Controls.Find("statBox" + (i + 3), true).First() as TextBox;
                investedNumericUpDown.Value = player.InvestedPoint[i];
                statBox.Text = (Convert.ToInt32(statBox.Text) + investedNumericUpDown.Value).ToString();
            }

            // Check if the player is trained
            resetButton.Enabled = player.InvestedPoint.Sum() != 0;
            for (int i = 0; i < player.InvestedPoint.Count; i++)
            {
                NumericUpDown investedNumericUpDown = this.Controls.Find("investedNumericUpDown" + (i + 3), true).First() as NumericUpDown;
                investedNumericUpDown.Enabled = player.InvestedPoint.Sum() == 0;
            }

            // Print Figthing Spirit
            avatarNameBox.SelectedIndex = avatarNameBox.Items.IndexOf(player.Avatar.Name);
            avatarNumericUpDown.Value = player.Avatar.Level;
            invokeBox.Checked = player.Invoke;
            avatarNameBox.Enabled = player.Invoke;
            avatarNumericUpDown.Enabled = player.Invoke & player.Avatar.IsFightingSpirit;
            armedBox.Checked = player.Armed;
            armedBox.Enabled = player.Invoke;

            // Print Moves
            for (int i = 0; i < player.Moves.Count; i++)
            {
                ComboBox moveBox = this.Controls.Find("moveBox" + (i + 1), true).First() as ComboBox;
                NumericUpDown moveNumericUpDown = this.Controls.Find("moveNumericUpDown" + (i + 1), true).First() as NumericUpDown;
                CheckBox moveCheckBox = this.Controls.Find("moveCheckBox" + (i + 1), true).First() as CheckBox;
                moveBox.SelectedIndex = moveBox.Items.IndexOf(player.Moves[i].Name);
                moveBox.Enabled = player.Moves[i].Unlock;
                moveNumericUpDown.Maximum = player.Moves[i].EvolutionCount;
                if (player.Moves[i].Level > player.Moves[i].EvolutionCount)
                {
                    player.Moves[i].Level = player.Moves[i].EvolutionCount;
                }
                moveNumericUpDown.Value = player.Moves[i].Level;
                moveNumericUpDown.Enabled = player.Moves[i].Unlock;
                moveCheckBox.Checked = player.Moves[i].Unlock;
            }

            // Print Equipment
            for (int i = 0; i < player.Equipments.Count; i++)
            {
                ComboBox equipmentBox = this.Controls.Find(player.Equipments[i].Type.Name.ToLower() + "Box", true).First() as ComboBox;
                equipmentBox.SelectedIndex = equipmentBox.Items.IndexOf(player.Equipments[i].Name);
            }

            // Print MixiMax
            if (player.MixiMax != null)
            {
                // Print Aura Name
                label24.Text = "Miximax";
                auraButton.Text = player.MixiMax.AuraPlayer.Name;
                auraButton.Visible = true;
                auraComboBox.Visible = false;
                removeMiximaxButton.Enabled = true;

                // Check If The Aura Is a Player
                if (player.MixiMax.AuraData == true)
                {
                    auraButton.Enabled = false;
                }
                else
                {
                    auraButton.Enabled = true;
                }

                // Get Miximax Moves
                moveBox7.Items.Clear();
                moveBox8.Items.Clear();
                for (int t = 0; t < player.MixiMax.AuraPlayer.Moves.Count; t++)
                {
                    moveBox7.Items.Add(player.MixiMax.AuraPlayer.Moves[t].Name);
                    moveBox8.Items.Add(player.MixiMax.AuraPlayer.Moves[t].Name);
                }
                if (player.MixiMax.BestMatch != null)
                {
                    moveBox7.Items.Add(Game.Moves[player.MixiMax.BestMatch.MixiMaxMove].Name);
                    moveBox8.Items.Add(Game.Moves[player.MixiMax.BestMatch.MixiMaxMove].Name);
                }

                // Print Miximax Moves Selected
                for (int i = 0; i < 2; i++)
                {
                    ComboBox moveBox = this.Controls.Find("moveBox" + (i + 7), true).First() as ComboBox;
                    NumericUpDown moveNumericUpDown = this.Controls.Find("moveNumericUpDown" + (i + 7), true).First() as NumericUpDown;
                    Label moveLabel = this.Controls.Find("moveLabel" + (i + 7), true).First() as Label;

                    if (player.MixiMax.MixiMaxMoveNumber[i] == 6)
                    {
                        moveBox.SelectedIndex = moveBox.Items.Count - 1;
                        moveNumericUpDown.Maximum = 1;
                        moveNumericUpDown.Value = 1;
                        moveNumericUpDown.Enabled = false;
                    }
                    else
                    {
                        if (player.MixiMax.MixiMaxMoveNumber[i] == 255)
                        {
                            moveBox.SelectedIndex = -1;
                            moveNumericUpDown.Value = 1;
                            moveNumericUpDown.Enabled = false;
                        } else
                        {
                            moveBox.SelectedIndex = player.MixiMax.MixiMaxMoveNumber[i];
                            moveNumericUpDown.Maximum = player.MixiMax.AuraPlayer.Moves[player.MixiMax.MixiMaxMoveNumber[i]].EvolutionCount;
                            if (player.MixiMax.AuraPlayer.Moves[player.MixiMax.MixiMaxMoveNumber[i]].Level > Convert.ToInt32(moveNumericUpDown.Maximum))
                            {
                                player.MixiMax.AuraPlayer.Moves[player.MixiMax.MixiMaxMoveNumber[i]].Level = Convert.ToInt32(moveNumericUpDown.Maximum);
                            }
                            moveNumericUpDown.Value = player.MixiMax.AuraPlayer.Moves[player.MixiMax.MixiMaxMoveNumber[i]].Level;
                            moveNumericUpDown.Enabled = true;
                        }
                    }

                    if (player.MixiMax.AuraData == true)
                    {
                        moveNumericUpDown.Enabled = false;
                    }

                    moveBox.Visible = true;
                    moveBox.Enabled = true;
                    moveNumericUpDown.Visible = true;
                    moveLabel.Visible = true;
                    moveLabel.Enabled = true;
                }

                // Print MixiMax Avatar
                if (player.MixiMax.AuraPlayer.Invoke == true)
                {
                    miximaxAvatarNameBox.SelectedIndex = miximaxAvatarNameBox.Items.IndexOf(player.MixiMax.AuraPlayer.Avatar.Name);
                    miximaxAvatarNumericUpDown.Value = player.MixiMax.AuraPlayer.Avatar.Level;
                    miximaxAvatarNameBox.Visible = true;
                    miximaxAvatarNameBox.Enabled = true;
                    miximaxAvatarNumericUpDown.Visible = true;
                    miximaxAvatarNumericUpDown.Enabled = player.MixiMax.AuraPlayer.Avatar.IsFightingSpirit;
                    miximaxAvatarLabel.Visible = true;
                    miximaxAvatarLabel.Enabled = true;
                } else
                {
                    miximaxAvatarNameBox.SelectedIndex = -1;
                    miximaxAvatarNameBox.Visible = false;
                    miximaxAvatarNumericUpDown.Visible = false;
                    miximaxAvatarLabel.Visible = false;
                }
            }
            else
            {
                if (player.IsAura == true)
                {
                    label24.Text = "Aura of";
                    auraButton.Text = Game.PlayersInSave.First(x => x.Value.MixiMax != null && x.Value.MixiMax.AuraPlayer == player).Value.Name;
                    auraButton.Visible = true;
                    auraComboBox.Visible = false;
                } 
                else
                {
                    // Fill Aura ComboBox
                    List<string> playersWhoCanBeAura = new List<string>();
                    foreach (KeyValuePair<UInt32, Player> auraKeyValue in Game.AuraInSave)
                    {
                        if (auraKeyValue.Value.IsAura == false)
                        {
                            playersWhoCanBeAura.Add(auraKeyValue.Value.Name);
                        }
                    }
                    foreach (KeyValuePair<UInt32, Player> playerKeyValue in Game.PlayersInSave)
                    {
                        if (playerKeyValue.Value.MixiMax == null & playerKeyValue.Value != player & playerKeyValue.Value.IsAura == false)
                        {
                            playersWhoCanBeAura.Add(playerKeyValue.Value.Name);
                        }
                    }

                    label24.Text = "Miximax";
                    auraComboBox.Text = "";
                    auraComboBox.Items.Clear();
                    auraComboBox.Items.AddRange(playersWhoCanBeAura.ToArray());
                    auraButton.Visible = false;
                    auraComboBox.Visible = true;
                }

                removeMiximaxButton.Enabled = false;

                for (int i = 0; i < 2; i++)
                {
                    ComboBox moveBox = this.Controls.Find("moveBox" + (i + 7), true).First() as ComboBox;
                    NumericUpDown moveNumericUpDown = this.Controls.Find("moveNumericUpDown" + (i + 7), true).First() as NumericUpDown;
                    Label moveLabel = this.Controls.Find("moveLabel" + (i + 7), true).First() as Label;
                    moveBox.SelectedIndex = -1;
                    moveBox.Visible = false;
                    moveBox.Enabled = false;
                    moveNumericUpDown.Visible = false;
                    moveNumericUpDown.Enabled = false;
                    moveLabel.Visible = false;
                    moveLabel.Enabled = false;
                }
                miximaxAvatarNameBox.SelectedIndex = -1;
                miximaxAvatarNumericUpDown.Value = 1;
                miximaxAvatarNameBox.Visible = false;
                miximaxAvatarNameBox.Enabled = false;
                miximaxAvatarNumericUpDown.Visible = false;
                miximaxAvatarNumericUpDown.Enabled = false;
                miximaxAvatarLabel.Visible = false;
                miximaxAvatarLabel.Enabled = false;
            }

            tabControl1.Enabled = true;
        }

        private void TrainPlayer(Player player, NumericUpDown investedNumericUpDownSelected, int startIndex)
        {
            var trainingInformation = Game.Training(player, Convert.ToInt32(investedNumericUpDownSelected.Value), startIndex);

            NumericUpDown downStatNumericUpDown = this.Controls.Find(trainingInformation.Item3, true).First() as NumericUpDown;
            investedNumericUpDownSelected.Minimum = trainingInformation.Item1;
            investedNumericUpDownSelected.Maximum = trainingInformation.Item2;
            downStatNumericUpDown.Enabled = trainingInformation.Item4;
            if (downStatNumericUpDown.Enabled == false & Game.GameNameCode == "IEGOCS")
            {
                downStatNumericUpDown.Minimum = -65535;
            }

            // Print Stat
            for (int i = 0; i < player.Stat.Count; i++)
            {
                TextBox statBox = this.Controls.Find("statBox" + (i + 1), true).First() as TextBox;
                statBox.Text = player.Stat[i].ToString();
            }

            // Print Invested Point
            for (int i = 0; i < player.InvestedPoint.Count; i++)
            {
                NumericUpDown investedNumericUpDown = this.Controls.Find("investedNumericUpDown" + (i + 3), true).First() as NumericUpDown;
                TextBox statBox = this.Controls.Find("statBox" + (i + 3), true).First() as TextBox;
                investedNumericUpDown.Value = player.InvestedPoint[i];
                statBox.Text = (Convert.ToInt32(statBox.Text) + investedNumericUpDown.Value).ToString();
            }

            resetButton.Enabled = true;
        }

        private void PrintPlayerFullStat(Player player)
        {
            // Print Full Stat (Level 99 Stat + Equipment)
            for (int i = 0; i < player.Stat.Count; i++)
            {
                TextBox statBox = this.Controls.Find("statBox" + (i + 12), true).First() as TextBox;
                TextBox statEquipmentBox = this.Controls.Find("statEquipmentBox" + (i + 1), true).First() as TextBox;

                int bonusStat = player.Equipments.Sum(x => x.Stat[i]);
                if (bonusStat > 0)
                {
                    statEquipmentBox.BackColor = Color.GreenYellow;
                }
                else
                {
                    statEquipmentBox.BackColor = SystemColors.ControlLight;
                }
                statEquipmentBox.Text = bonusStat.ToString();


                if (i < 2)
                {
                    statBox.Text = (player.Stat[i] + bonusStat).ToString();
                }
                else
                {
                    NumericUpDown investedNumericUpDown = this.Controls.Find("investedNumericUpDown" + (i + 1), true).First() as NumericUpDown;
                    statBox.Text = (player.Stat[i] + investedNumericUpDown.Value + bonusStat).ToString();
                }
            }
        }

        private void MovePictureBox(PictureBox pictureBox, object sender, EventArgs e)
        {
            // Show or Hide current Picture Box
            pictureBox.Visible = pageComboBox.SelectedIndex != SelectedPlayers[tabControl4.SelectedIndex] / 16;

            // Draw Moved Player Picture Box
            movedPlayerPictureBox.Image = Draw.DrawString(InazumaElevenSaveEditor.Properties.Resources.MovedPlayerRectangleBox, Game.GetPlayer(SelectedPlayers[tabControl4.SelectedIndex]).Name, 3, 2);
            movedPlayerPictureBox.Left = tabControl2.PointToClient(Cursor.Position).X;
            movedPlayerPictureBox.Top = tabControl2.PointToClient(Cursor.Position).Y;
            movedPlayerPictureBox.Visible = true;

            // Detect if the picture box is over a button
            foreach (Control control in tabPage3.Controls)
            {            
                if (!control.Equals(movedPlayerPictureBox) && control is Button && control.RectangleToScreen(control.ClientRectangle).IntersectsWith(movedPlayerPictureBox.RectangleToScreen(movedPlayerPictureBox.ClientRectangle)))
                {
                    if (control.Name == "previousButton")
                    {
                        PreviousButton_Click(sender, e);
                    }
                    else if (control.Name == "nextButton")
                    {
                        NextButton_Click(sender, e);
                    }

                    movedPlayerPictureBox.Location = pictureBox.Location;
                    Cursor.Position = tabControl2.PointToScreen(movedPlayerPictureBox.Location);
                }
            }
        }

        private void SwitchPlayer()
        {
            foreach (Control control in tabPage3.Controls)
            {
                if (!control.Equals(movedPlayerPictureBox) && control is PictureBox && movedPlayerPictureBox.Bounds.IntersectsWith(control.Bounds))
                {
                    int newIndex = pageComboBox.SelectedIndex * 16 + Convert.ToInt32(control.Name.Replace("playerPictureBox", "")) - 1;
                    if (newIndex != SelectedPlayers[tabControl4.SelectedIndex] && newIndex <= Game.PlayersInSave.Count)
                    {
                        UInt32 playerPositionID = Game.PlayersInSaveSort[SelectedPlayers[tabControl4.SelectedIndex]];
                        UInt32 newPlayerPositionID = Game.PlayersInSaveSort[newIndex];
                        Game.PlayersInSaveSort[SelectedPlayers[tabControl4.SelectedIndex]] = newPlayerPositionID;
                        Game.PlayersInSaveSort[newIndex] = playerPositionID;
                        break;
                    }
                }
            }
        }

        private void LoadFile(string filePath)
        {
            FileData = new DataReader(File.ReadAllBytes(filePath));

            switch (Path.GetExtension(filePath))
            {
                case ".ie":
                    Format = new IE();
                    break;
                case ".ie4":
                    Format = new IE();
                    break;
                default:
                    MessageBox.Show("Format isn't supported");
                    return;
            }

            Game = Format.Open(FileData);
            Game.Open();

            CreatePage(Convert.ToInt32(Math.Ceiling((double)Game.PlayersInSave.Count / 16.0)));
            pageComboBox.SelectedIndex = 0;

            InitializeRessource();
            SelectedPlayers = new List<int>() { -1 };

            managePlayerToolStripMenuItem.Enabled = true;
            inventoryButton.Enabled = true;
            // teamButton.Enabled = true;
            saveInformationButton.Enabled = true;
            playRecordsButton.Enabled = true;
            saveToolStripMenuItem1.Enabled = true;
            tabControl1.Enabled = false;
            managePlayerTabToolStripMenuItem.Enabled = true;
        }

        private Player LoadPlayerFile(string filePath)
        {
            Player player = null;
            IPlayerFiles playerFile = null;

            // Check If The File Is Valid
            if (BitConverter.ToString(new Crc32().ComputeHash(File.ReadAllBytes(filePath).Skip(9).ToArray())).Replace("-", string.Empty) != File.ReadLines(filePath).First().TrimEnd())
            {
                MessageBox.Show("Corrupted INZ File");
                return null;
            }

            switch (Game.GameNameCode)
            {
                case "IEGOCS":
                    playerFile = new INZ5();
                    break;
                case "IEGOGALAXY":
                    playerFile = new INZ6();
                    break;
            }

            player = playerFile.NewPlayer(filePath);

            if (player == null)
            {
                MessageBox.Show("Can't import a player that doesn't exist in the game");
                return null;
            }

            for (int i = 0; i < 4; i++)
            {
                if (Game.Equipments.Values.Contains(player.Equipments[i]) == false)
                {
                    player.Equipments[i] = Game.Equipments[(uint)i];
                }

                if (player.MixiMax != null && Game.Equipments.Values.Contains(player.MixiMax.AuraPlayer.Equipments[i]) == false)
                {
                    player.MixiMax.AuraPlayer.Equipments[i] = Game.Equipments[(uint)i];
                }
            }

            return player;
        }

        private void OpenToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Open Inazuma Eleven save file";
            openFileDialog1.Filter = "IEGOCS/IEGOGALAXY save file (*.ie*)|*.ie*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                LoadFile(openFileDialog1.FileName);
            }
        }

        private void SaveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Game == null) return;

            Game.Save(openFileDialog1);
        }

        private void TabControl4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!tabControl4.Focused) return;

            Control[] ctrlArray = new Control[tabControl1.Controls.Count];
            tabControl1.Controls.CopyTo(ctrlArray, 0);
            tabControl4.TabPages[tabControl4.SelectedIndex].Controls.Add(tabControl1);

            if (SelectedPlayers[tabControl4.SelectedIndex] == -1)
            {
                tabControl1.Enabled = false;
            } else
            {
                PrintPlayer(Game.GetPlayer(SelectedPlayers[tabControl4.SelectedIndex]));
            }
        }

        private void AddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Game == null) return;

            bool mouseOnControl = false;
            Control playerPictureBox = null;

            foreach (Control control in tabPage3.Controls)
            {
                playerPictureBox = control;
                mouseOnControl = control.ClientRectangle.Contains(control.PointToClient(Cursor.Position));

                if (mouseOnControl == true && control.Name.StartsWith("playerPictureBox") == true) break;
                if (mouseOnControl == true && control.Name.StartsWith("playerPictureBox") == false) return;
            }

            if (mouseOnControl == true)
            {
                int newIndex = pageComboBox.SelectedIndex * 16 + Convert.ToInt32(playerPictureBox.Name.Replace("playerPictureBox", "")) - 1;

                Player player = Game.GetPlayer(newIndex);
                PrintPlayer(player);

                if (SelectedPlayers[tabControl4.SelectedIndex] == -1)
                {
                    SelectedPlayers[tabControl4.SelectedIndex] = newIndex;
                }
                else
                {
                    SelectedPlayers.Add(newIndex);
                    tabControl4.TabPages.Add(player.Name + " (Lv." + player.Level + ")");
                    tabControl4.SelectedIndex = SelectedPlayers.Count() - 1;
                }
            } 
            else
            {
                SelectedPlayers.Add(-1);
                tabControl4.TabPages.Add("Player");
                tabControl4.SelectedIndex = SelectedPlayers.Count() - 1;
                tabControl1.Enabled = false;
            }

            Control[] ctrlArray = new Control[tabControl1.Controls.Count];
            tabControl1.Controls.CopyTo(ctrlArray, 0);
            tabControl4.TabPages[tabControl4.SelectedIndex].Controls.Add(tabControl1);
        }

        private void RemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Game == null) return;

            if (tabControl4.TabPages.Count == 1) return;

            SelectedPlayers.RemoveAt(tabControl4.SelectedIndex);
            tabControl4.TabPages.RemoveAt(tabControl4.SelectedIndex);
            tabControl4.SelectedIndex = tabControl4.TabPages.Count - 1;

            TabControl4_SelectedIndexChanged(sender, e);
        }

        private void RecruitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Game == null) return;

            if (Game.PlayersInSave.Count > Game.MaximumPlayer)
            {
                MessageBox.Show("Maximum player reached");
                return;
            }

            ComboBox customNameBox = nameBox;

            MessageComboBox inviteWindow = new MessageComboBox("Invite Player", "Select the player you want to invite", "Invite", customNameBox);
            DialogResult dialogResult = inviteWindow.ShowDialog();

            if (dialogResult == DialogResult.Yes && inviteWindow.nameBox.SelectedIndex != -1)
            {
                Game.RecruitPlayer(Game.Players.FirstOrDefault(x => x.Value.Name == inviteWindow.nameBox.Text.ToString()));

                CreatePage(Convert.ToInt32(Math.Ceiling((double)Game.PlayersInSave.Count / 16.0)));
                pageComboBox.SelectedIndex = pageComboBox.Items.Count - 1;

                Player player = Game.GetPlayer(Game.PlayersInSave.Count - 1);
                PrintPlayer(player);

                SelectedPlayers[tabControl4.SelectedIndex] = Game.PlayersInSave.Count - 1;

                MessageBox.Show(inviteWindow.nameBox.Text + " has joined you");
            }
        }

        private void DismissToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Game == null) return;

            ComboBox customNameBox = new ComboBox();

            customNameBox.Items.AddRange(Game.PlayersInSave.Select(x => x.Value.Name).ToArray());
            customNameBox.Sorted = false;

            MessageComboBox inviteWindow = new MessageComboBox("Dismiss Player", "Select the player you want to dismiss", "Dismiss", customNameBox);
            DialogResult dialogResult = inviteWindow.ShowDialog();

            if (dialogResult == DialogResult.Yes && inviteWindow.nameBox.SelectedIndex != -1)
            {
                UInt32 playerIndex = Game.PlayersInSaveSort[inviteWindow.nameBox.SelectedIndex];

                // Remove Mixi Max Aura Linked
                foreach (KeyValuePair<UInt32, Player> player in Game.PlayersInSave)
                {
                    if (player.Value.MixiMax != null && player.Value.MixiMax.AuraPlayer == Game.PlayersInSave[playerIndex])
                    {
                        player.Value.MixiMax = null;
                    }
                }

                // Remove Tabpages Which contains The PLayer Data
                int selectedIndex = Game.PlayersInSaveSort.IndexOf(playerIndex);
                int selectedPlayersLength = SelectedPlayers.Count;
                for (int i = 0; i < selectedPlayersLength; i++)
                {
                    int index = SelectedPlayers.IndexOf(selectedIndex);
                    if (index != -1 && SelectedPlayers[index] == selectedIndex)
                    {
                        if (SelectedPlayers.Count == 1)
                        {
                            SelectedPlayers[0] = -1;
                            tabControl1.Enabled = false;
                            tabControl4.TabPages[tabControl4.SelectedIndex].Text = "Player";
                        }
                        else
                        {
                            tabControl4.SelectedIndex = index;
                            RemoveToolStripMenuItem_Click(tabControl4, System.EventArgs.Empty);
                        }
                    }
                }

                // Remove The Player From The Save
                Game.PlayersInSaveSort.Remove(playerIndex);
                Game.PlayersInSave.Remove(playerIndex);

                // Reset Pages
                CreatePage(Convert.ToInt32(Math.Ceiling((double)Game.PlayersInSave.Count / 16.0)));
                pageComboBox.SelectedIndex = 0;
                SelectedPlayers[tabControl4.SelectedIndex] = -1;
                tabControl1.Enabled = false;

                MessageBox.Show(inviteWindow.nameBox.Text + " left the team");
            }
        }

        private void ExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Game == null) return;

            IPlayerFiles playerFile = null;
            Player player = null;

            if (SelectedPlayers[tabControl4.SelectedIndex] == -1)
            {
                ComboBox customNameBox = new ComboBox();

                customNameBox.Items.AddRange(Game.PlayersInSave.Select(x => x.Value.Name).ToArray());
                customNameBox.Sorted = false;

                MessageComboBox inviteWindow = new MessageComboBox("Export Player", "Select the player you want to export", "Export", customNameBox);
                DialogResult dialogResult = inviteWindow.ShowDialog();

                if (dialogResult == DialogResult.Yes && inviteWindow.nameBox.SelectedIndex != -1)
                {
                    player = Game.GetPlayer(inviteWindow.nameBox.SelectedIndex);
                }
                else
                {
                    return;
                }
            }
            else
            {
                player = Game.GetPlayer(SelectedPlayers[tabControl4.SelectedIndex]);
            }

            switch (Game.GameNameCode)
            {
                case "IEGOCS":
                    playerFile = new INZ5();
                    break;
                case "IEGOGALAXY":
                    playerFile = new INZ6();
                    break;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = player.Name;
            saveFileDialog.Title = "Save player file";
            saveFileDialog.Filter = "INZ file (*"+ playerFile.Extension+ ") | *" + playerFile.Extension;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllText(saveFileDialog.FileName, playerFile.NewFile(player));
                MessageBox.Show(player.Name + " exported!");
            }
        }

        private void ImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Game == null) return;

            if (Game.PlayersInSave.Count > Game.MaximumPlayer)
            {
                MessageBox.Show("Maximum player reached");
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open player file";
            openFileDialog.Filter = "INZ1 file (*.inz1)|*.inz1|INZ2 file (*.inz2)| *.inz2|INZ3 file (*.inz3)| *.inz3|INZ4 file (*.inz4)| *.inz4|INZ5 file (*.inz5)| *.inz5|INZ6 file (*.inz6)| *.inz6|Supported files (*.inz*)| *.inz*";
            openFileDialog.FilterIndex = 7;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Player importPlayer = LoadPlayerFile(openFileDialog.FileName);

                if (importPlayer.MixiMax != null & Game.PlayersInSave.Count + 1 < Game.MaximumPlayer)
                {
                    DialogResult dialogResult = MessageBox.Show("Do you want to import Miximax?", "Import Miximax", MessageBoxButtons.YesNo);

                    if (dialogResult == DialogResult.Yes)
                    {
                        Game.RecruitPlayer(importPlayer);
                        Game.RecruitPlayer(importPlayer.MixiMax.AuraPlayer);
                    }
                    else
                    {
                        importPlayer.MixiMax = null;
                        Game.RecruitPlayer(importPlayer);
                    }
                }
                else
                {
                    importPlayer.MixiMax = null;
                    Game.RecruitPlayer(importPlayer);
                }

                CreatePage(Convert.ToInt32(Math.Ceiling((double)Game.PlayersInSave.Count / 16.0)));
                pageComboBox.SelectedIndex = pageComboBox.Items.Count - 1;

                PrintPlayer(importPlayer);

                SelectedPlayers[tabControl4.SelectedIndex] = Game.PlayersInSave.Count - 1;

                MessageBox.Show(importPlayer.Name + " imported!");
            }
        }

        private void ReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Game == null) return;

            Player player = null;

            if (SelectedPlayers[tabControl4.SelectedIndex] == -1)
            {
                ComboBox customNameBox = new ComboBox();

                customNameBox.Items.AddRange(Game.PlayersInSave.Select(x => x.Value.Name).ToArray());
                customNameBox.Sorted = false;

                MessageComboBox inviteWindow = new MessageComboBox("Replace Player", "Select the player you want to replace", "Replace", customNameBox);
                DialogResult dialogResult = inviteWindow.ShowDialog();

                if (dialogResult == DialogResult.Yes && inviteWindow.nameBox.SelectedIndex != -1)
                {
                    player = Game.GetPlayer(inviteWindow.nameBox.SelectedIndex);
                }
                else
                {
                    return;
                }
            }
            else
            {
                player = Game.GetPlayer(SelectedPlayers[tabControl4.SelectedIndex]);
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open player file";
            openFileDialog.Filter = "INZ1 file (*.inz1)|*.inz1|INZ2 file (*.inz2)| *.inz2|INZ3 file (*.inz3)| *.inz3|INZ4 file (*.inz4)| *.inz4|INZ5 file (*.inz5)| *.inz5|INZ6 file (*.inz6)| *.inz6|Supported files (*.inz*)| *.inz*";
            openFileDialog.FilterIndex = 7;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string oldPlayerName = player.Name;

                // Remove Miximax Link
                if (player.MixiMax != null)
                {
                    player.MixiMax.AuraPlayer.IsAura = false;
                } else if (player.IsAura == true)
                {
                    player.IsAura = false;
                    Game.PlayersInSave.First(x => x.Value.MixiMax != null && x.Value.MixiMax.AuraPlayer == player).Value.MixiMax = null;
                }

                Player importPlayer = LoadPlayerFile(openFileDialog.FileName);
                Game.ChangePlayer(player, importPlayer);

                if (importPlayer.MixiMax != null & Game.PlayersInSave.Count + 1 < Game.MaximumPlayer)
                {
                    DialogResult dialogResult = MessageBox.Show("Do you want to import Miximax?", "Import Miximax", MessageBoxButtons.YesNo);

                    if (dialogResult == DialogResult.Yes)
                    {
                        Game.RecruitPlayer(importPlayer.MixiMax.AuraPlayer);
                    }
                    else
                    {
                        player.MixiMax = null;
                    }
                }
                else
                {
                    player.MixiMax = null;
                }

                PrintPlayer(player);
                PageComboBox_SelectedIndexChanged(sender, e);

                MessageBox.Show(oldPlayerName + " replaced by " + player.Name + "!");
            }
        }

        private void Welcome_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string dragPath = Path.GetFullPath(files[0]);
                string dragExt = Path.GetExtension(files[0]);

                if (files.Length > 1) return;
                if (dragExt != ".ie" & dragExt != ".ie4") return;

                openFileDialog1.FileName = dragPath;
                LoadFile(openFileDialog1.FileName);
            }
        }

        private void Welcome_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void TeamButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Be careful, the team editor doesn't have save function.\nYou can just look your teams");
            TeamWindow teamWindow = new TeamWindow(Game);
            teamWindow.Show();
        }

        private void SaveInformationButton_Click(object sender, EventArgs e)
        {
            SaveInfoWindow saveInfoWindow = new SaveInfoWindow(Game);
            saveInfoWindow.ShowDialog();
        }

        private void PlayRecordsButton_Click(object sender, EventArgs e)
        {
            PlayRecordsWindow playRecordsWindow = new PlayRecordsWindow(Game);
            playRecordsWindow.ShowDialog();
        }

        private void InventoryButton_Click(object sender, EventArgs e)
        {
            InventoryWindow inventoryWindow = new InventoryWindow(Game);
            inventoryWindow.ShowDialog();
        }

        private void PageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pageComboBox.SelectedIndex == -1) return;

            PrintPage(pageComboBox.SelectedIndex);
        }

        private void PreviousButton_Click(object sender, EventArgs e)
        {
            if (pageComboBox.SelectedIndex == 0)
            {
                pageComboBox.SelectedIndex = pageComboBox.Items.Count - 1;
            }
            else
            {
                pageComboBox.SelectedIndex--;
            }
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            if (pageComboBox.SelectedIndex < pageComboBox.Items.Count - 1)
            {
                pageComboBox.SelectedIndex++;
            }
            else
            {
                pageComboBox.SelectedIndex = 0;
            }
        }

        private void PlayerPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            PictureBox playerPictureBox = (PictureBox)sender;
            int playerNumber = Convert.ToInt32(playerPictureBox.Name.Replace("playerPictureBox", "")) - 1;

            SelectedPlayers[tabControl4.SelectedIndex] = pageComboBox.SelectedIndex * 16 + playerNumber;

            if (SelectedPlayers[tabControl4.SelectedIndex] >= Game.PlayersInSave.Count)
            {
                RecruitToolStripMenuItem_Click(sender, e);
            }
            else
            {
                Player player = Game.GetPlayer(SelectedPlayers[tabControl4.SelectedIndex]);
                PrintPlayer(player);
            }
        }

        private void PlayerPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            PictureBox playerPictureBox = (PictureBox)sender;
            int playerNumber = Convert.ToInt32(playerPictureBox.Name.Replace("playerPictureBox", "")) - 1;

            if (pageComboBox.SelectedIndex * 16 + playerNumber >= Game.PlayersInSave.Count) return;

            if (movedPlayerPictureBox.Visible == false)
            {
                SelectedPlayers[tabControl4.SelectedIndex] = pageComboBox.SelectedIndex * 16 + playerNumber;
            }

            MovePictureBox(playerPictureBox, sender, e);
        }

        private void PlayerPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            PictureBox playerPictureBox = (PictureBox)sender;

            SwitchPlayer();

            SelectedPlayers[tabControl4.SelectedIndex] = -1;
            tabControl1.Enabled = false;
            playerPictureBox.Visible = true;
            movedPlayerPictureBox.Visible = false;
            PageComboBox_SelectedIndexChanged(sender, e);
        }

        private void NameBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!nameBox.Focused) return;

            // Update Player
            Player selectedPlayer = Game.GetPlayer(SelectedPlayers[tabControl4.SelectedIndex]);
            Game.ChangePlayer(selectedPlayer, Game.Players.FirstOrDefault(x => x.Value.Name == nameBox.Items[nameBox.SelectedIndex].ToString()));

            // Reset Invested Point + Print Current Page
            ResetButton_Click(sender, e);
            PrintPlayer(Game.GetPlayer(SelectedPlayers[tabControl4.SelectedIndex]));
            PageComboBox_SelectedIndexChanged(sender, e);
        }

        private void LevelNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!levelNumericUpDown.Focused) return;

            Game.GetPlayer(SelectedPlayers[tabControl4.SelectedIndex]).Level = Convert.ToInt32(levelNumericUpDown.Value);
        }

        private void StyleBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!styleBox.Focused) return;

            Game.GetPlayer(SelectedPlayers[tabControl4.SelectedIndex]).Style = styleBox.SelectedIndex;
        }

        private void ScoreNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!scoreNumericUpDown.Focused) return;

            Game.GetPlayer(SelectedPlayers[tabControl4.SelectedIndex]).Score = Convert.ToInt32(scoreNumericUpDown.Value);
        }

        private void ParticipationNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!participationNumericUpDown.Focused) return;

            Game.GetPlayer(SelectedPlayers[tabControl4.SelectedIndex]).Participation = Convert.ToInt32(participationNumericUpDown.Value);
        }

        private void InvestedNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown investedNumericUpDown = (NumericUpDown)sender;
            if (!investedNumericUpDown.Focused) return;

            Player player = Game.GetPlayer(SelectedPlayers[tabControl4.SelectedIndex]);

            if (player.Level != 99 && Game.GameNameCode == "IEGOCS")
            {
                MessageBox.Show("The player must be level 99 to be trained");
            } else
            {
                int investedNumericUpDownNumber = Convert.ToInt32(investedNumericUpDown.Name.Replace("investedNumericUpDown", "")) - 3;
                TrainPlayer(player, investedNumericUpDown, investedNumericUpDownNumber);
            }
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            Player player = Game.GetPlayer(SelectedPlayers[tabControl4.SelectedIndex]);

            for (int i = 0; i < player.InvestedPoint.Count; i++)
            {
                player.InvestedPoint[i] = 0;
                player.InvestedFreedom[i] = 0;

                NumericUpDown investedNumericUpDown = this.Controls.Find("investedNumericUpDown" + (i + 3), true).First() as NumericUpDown;
                investedNumericUpDown.Enabled = true;
                investedNumericUpDown.Maximum = 65535;
                investedNumericUpDown.Minimum = 0;
                investedNumericUpDown.Value = player.InvestedPoint[i];

                TextBox statBox = this.Controls.Find("statBox" + (i + 3), true).First() as TextBox;
                statBox.Text = (Convert.ToInt32(player.Stat[i+2]) + investedNumericUpDown.Value).ToString();
            }

            resetButton.Enabled = false;
        }

        private void AvatarNameBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!avatarNameBox.Focused) return;

            Player player = Game.GetPlayer(SelectedPlayers[tabControl4.SelectedIndex]);
            Avatar oldAvatar = player.Avatar;

            player.Invoke = true;
            player.Avatar = Game.Avatars.FirstOrDefault(x => x.Value.Name == avatarNameBox.Text).Value;
            player.Avatar.Level = oldAvatar.Level;

            avatarNumericUpDown.Enabled = player.Avatar.IsFightingSpirit;
        }

        private void AvatarNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!avatarNumericUpDown.Focused) return;

            Game.GetPlayer(SelectedPlayers[tabControl4.SelectedIndex]).Avatar.Level = Convert.ToInt32(avatarNumericUpDown.Value);
        }

        private void InvokeBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!invokeBox.Focused) return;

            Player player = Game.GetPlayer(SelectedPlayers[tabControl4.SelectedIndex]);

            if (invokeBox.Checked == false)
            {
                Avatar newAvatar = Game.Avatars[0x0];
                newAvatar.Level = 1;
                player.Avatar = newAvatar;
                player.Armed = false;
                armedBox.Checked = false;
                avatarNameBox.SelectedIndex = 0;
                avatarNumericUpDown.Value = 1;
            }

            player.Invoke = invokeBox.Checked;
            armedBox.Enabled = invokeBox.Checked;
            avatarNameBox.Enabled = invokeBox.Checked;
            avatarNumericUpDown.Enabled = invokeBox.Checked;
        }

        private void ArmedBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!armedBox.Focused) return;

            Game.GetPlayer(SelectedPlayers[tabControl4.SelectedIndex]).Armed = armedBox.Checked;
        }

        private void MoveBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox moveBox = (ComboBox)sender;
            if (!moveBox.Focused) return;

            int moveBoxNumber = Convert.ToInt32(moveBox.Name.Replace("moveBox", "")) - 1;

            Player player = Game.GetPlayer(SelectedPlayers[tabControl4.SelectedIndex]);

            Move newMove = Game.Moves.FirstOrDefault(x => x.Value.Name == moveBox.Text).Value;
            newMove.Level = 1;
            newMove.TimeLevel = newMove.EvolutionSpeed.TimeLevel[0];
            newMove.Unlock = true;
            player.Moves[moveBoxNumber] = newMove;

            NumericUpDown moveNumericUpDown = (NumericUpDown)tabPage2.Controls.Find("moveNumericUpDown" + (moveBoxNumber+1), false)[0];
            moveNumericUpDown.Value = 1;
            moveNumericUpDown.Maximum = newMove.EvolutionCount;
        }

        private void MoveNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown moveNumericUpDown = (NumericUpDown)sender;
            if (!moveNumericUpDown.Focused) return;

            int moveNumericUpDownNumber = Convert.ToInt32(moveNumericUpDown.Name.Replace("moveNumericUpDown", "")) - 1;

            Player player = Game.GetPlayer(SelectedPlayers[tabControl4.SelectedIndex]);
            player.Moves[moveNumericUpDownNumber].Level = Convert.ToInt32(moveNumericUpDown.Value);

            if (moveNumericUpDown.Value < 6)
            {
                player.Moves[moveNumericUpDownNumber].TimeLevel = player.Moves[moveNumericUpDownNumber].EvolutionSpeed.TimeLevel[Convert.ToInt32(moveNumericUpDown.Value)];
            } else
            {
                player.Moves[moveNumericUpDownNumber].TimeLevel = 0;
            }
        }

        private void MoveCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox moveCheckBox = (CheckBox)sender;
            if (!moveCheckBox.Focused) return;

            int moveCheckBoxNumber = Convert.ToInt32(moveCheckBox.Name.Replace("moveCheckBox", "")) - 1;

            Game.GetPlayer(SelectedPlayers[tabControl4.SelectedIndex]).Moves[moveCheckBoxNumber].Unlock = moveCheckBox.Checked;

            ComboBox moveBox = (ComboBox)tabPage2.Controls.Find("moveBox" + (moveCheckBoxNumber + 1), false)[0];
            moveBox.Enabled = moveCheckBox.Checked;

            NumericUpDown moveNumericUpDown = (NumericUpDown)tabPage2.Controls.Find("moveNumericUpDown" + (moveCheckBoxNumber + 1), false)[0];
            moveNumericUpDown.Enabled = moveCheckBox.Checked;
        }

        private void AuraButton_Click(object sender, EventArgs e)
        {
            Player player = Game.GetPlayer(SelectedPlayers[tabControl4.SelectedIndex]);

            if (player.IsAura == true)
            {
                player = Game.PlayersInSave.First(x => x.Value.MixiMax != null && x.Value.MixiMax.AuraPlayer == player).Value;
            } else if (player.MixiMax != null)
            {
                player = player.MixiMax.AuraPlayer;
            }

            var playerKey = Game.PlayersInSave.FirstOrDefault(x => x.Value == player).Key;
            int playerIndex = Game.PlayersInSaveSort.IndexOf(playerKey);

            pageComboBox.SelectedIndex = playerIndex / 16;
            SelectedPlayers[tabControl4.SelectedIndex] = playerIndex;

            PrintPlayer(Game.GetPlayer(SelectedPlayers[tabControl4.SelectedIndex]));
        }

        private void AuraComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!auraComboBox.Focused) return;

            Player player = Game.GetPlayer(SelectedPlayers[tabControl4.SelectedIndex]);

            // Get List Player Of Aura
            List<KeyValuePair<UInt32, Player>> playersWhoCanBeAura = new List<KeyValuePair<UInt32, Player>>();
            foreach (KeyValuePair<UInt32, Player> auraKeyValue in Game.AuraInSave)
            {
                if (auraKeyValue.Value.IsAura == false)
                {
                    playersWhoCanBeAura.Add(auraKeyValue);
                }
            }
            foreach (KeyValuePair<UInt32, Player> playerKeyValue in Game.PlayersInSave)
            {
                if (playerKeyValue.Value.MixiMax == null & playerKeyValue.Value != player & playerKeyValue.Value.IsAura == false)
                {
                    playersWhoCanBeAura.Add(playerKeyValue);
                }
            }

            playersWhoCanBeAura.Sort((x, y) => x.Value.Name.CompareTo(y.Value.Name));
            Game.NewMixiMax(player, playersWhoCanBeAura[auraComboBox.SelectedIndex].Key, 0, 1);
            PrintPlayer(player);
        }

        private void RemoveMiximaxButton_Click(object sender, EventArgs e)
        {
            Player player = Game.GetPlayer(SelectedPlayers[tabControl4.SelectedIndex]);

            if (player.MixiMax != null)
            {
                player.MixiMax.AuraPlayer.IsAura = false;
                player.MixiMax = null;
                PrintPlayer(player);
            }
        }

        private void MoveBoxMixiMax_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox moveBox = (ComboBox)sender;
            if (!moveBox.Focused) return;

            int moveBoxNumber = Convert.ToInt32(moveBox.Name.Replace("moveBox", "")) - 1;
            int mixiMaxMove = moveBoxNumber - 6;

            NumericUpDown moveNumericUpDown = (NumericUpDown)groupBox1.Controls.Find("moveNumericUpDown" + (moveBoxNumber + 1), false)[0];

            Player player = Game.GetPlayer(SelectedPlayers[tabControl4.SelectedIndex]);

            if (moveBox7.SelectedIndex != moveBox8.SelectedIndex)
            {
                if (moveBox.SelectedIndex == moveBox.Items.Count - 1 && player.MixiMax.BestMatch != null)
                {
                    player.MixiMax.MixiMaxMoveNumber[mixiMaxMove] = 6;
                    moveNumericUpDown.Maximum = 1;
                    moveNumericUpDown.Value = 1;
                    moveNumericUpDown.Enabled = false;
                }
                else
                {
                    player.MixiMax.MixiMaxMoveNumber[mixiMaxMove] = moveBox.SelectedIndex;
                    moveNumericUpDown.Maximum = player.MixiMax.AuraPlayer.Moves[moveBox.SelectedIndex].EvolutionCount;
                    moveNumericUpDown.Value = player.MixiMax.AuraPlayer.Moves[moveBox.SelectedIndex].Level;
                    moveNumericUpDown.Enabled = true;
                }
            }
            else
            {
                moveBox.SelectedIndex = player.MixiMax.MixiMaxMoveNumber[mixiMaxMove];
            }
        }

        private void MoveNumericUpDownMixiMax_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown moveNumericUpDown = (NumericUpDown)sender;
            if (!moveNumericUpDown.Focused) return;

            int moveNumericUpDownNumber = Convert.ToInt32(moveNumericUpDown.Name.Replace("moveNumericUpDown", "")) - 1;

            Player player = Game.GetPlayer(SelectedPlayers[tabControl4.SelectedIndex]).MixiMax.AuraPlayer;

            ComboBox moveBox = (ComboBox)groupBox1.Controls.Find("moveBox" + (moveNumericUpDownNumber + 1), false)[0];
            player.Moves[moveBox.SelectedIndex].Level = Convert.ToInt32(moveNumericUpDown.Value);

            if (moveNumericUpDown.Value < 6)
            {
                player.Moves[moveBox.SelectedIndex].TimeLevel = player.Moves[moveBox.SelectedIndex].EvolutionSpeed.TimeLevel[Convert.ToInt32(moveNumericUpDown.Value)];
            } else
            {
                player.Moves[moveBox.SelectedIndex].TimeLevel = 0;
            }
        }

        private void MiximaxAvatarNameBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!miximaxAvatarNameBox.Focused) return;

            Player player = Game.GetPlayer(SelectedPlayers[tabControl4.SelectedIndex]).MixiMax.AuraPlayer;
            Avatar oldAvatar = player.Avatar;

            player.Avatar = Game.Avatars.FirstOrDefault(x => x.Value.Name == miximaxAvatarNameBox.Text).Value;
            player.Avatar.Level = oldAvatar.Level;

            miximaxAvatarNumericUpDown.Enabled = player.Avatar.IsFightingSpirit;
        }

        private void MiximaxAvatarNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!miximaxAvatarNumericUpDown.Focused) return;

            Game.GetPlayer(SelectedPlayers[tabControl4.SelectedIndex]).MixiMax.AuraPlayer.Avatar.Level = Convert.ToInt32(avatarNumericUpDown.Value);
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 2)
            {
                PrintPlayerFullStat(Game.GetPlayer(SelectedPlayers[tabControl4.SelectedIndex]));
            }
        }

        private void Equipment_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox equipmentComboBox = (ComboBox)sender;
            if (!equipmentComboBox.Focused) return;

            int equipmentIndex = 0;
            switch (equipmentComboBox.Name)
            {
                case "bootsBox":
                    equipmentIndex = 0;
                    break;
                case "braceletBox":
                    equipmentIndex = 1;
                    break;
                case "pendantBox":
                    equipmentIndex = 2;
                    break;
                case "glovesBox":
                    equipmentIndex = 3;
                    break;
            }

            Player player = Game.GetPlayer(SelectedPlayers[tabControl4.SelectedIndex]);
            var newEquipment = Game.Equipments.FirstOrDefault(x => x.Value.Name == equipmentComboBox.Text);

            if (newEquipment.Key == 0x0)
            {
                player.Equipments[equipmentIndex] = Game.Equipments[(uint)equipmentIndex];
            }
            else
            {
                player.Equipments[equipmentIndex] = newEquipment.Value;
            }

            PrintPlayerFullStat(player);
        }
    }
}
