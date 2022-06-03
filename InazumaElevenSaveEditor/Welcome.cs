using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using InazumaElevenSaveEditor.Logic;
using InazumaElevenSaveEditor.Tools;
using InazumaElevenSaveEditor.Formats;
using InazumaElevenSaveEditor.Common;
using InazumaElevenSaveEditor.Formats.Saves;
using InazumaElevenSaveEditor;

namespace NoFarmForMeOpenSource
{
    public partial class Welcome : Form
    {
        public DataReader file = null;
        public SaveFormat format = null;
        public ContainerGames game = null;
        public int currentPlayer = -1;

        public Welcome()
        {
            InitializeComponent();
        }

        private void InitializeRessource()
        {
            this.Text = game.GameNameCode + " Save Editor";

            nameBox.Items.Clear();
            // inviteNameBox.Items.Clear();
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

            foreach (KeyValuePair<UInt32, Player> entry in game.Players) { nameBox.Items.Add(entry.Value.Name); }
            // inviteNameBox.Items.AddRange(nameBox.Items.Cast<Object>().ToArray());
            foreach (KeyValuePair<UInt32, Avatar> entry in game.Avatars) { avatarNameBox.Items.Add(entry.Value.Name); }
            foreach (KeyValuePair<UInt32, Move> entry in game.Moves) { moveBox1.Items.Add(entry.Value.Name); }
            moveBox2.Items.AddRange(moveBox1.Items.Cast<Object>().ToArray());
            moveBox3.Items.AddRange(moveBox1.Items.Cast<Object>().ToArray());
            moveBox4.Items.AddRange(moveBox1.Items.Cast<Object>().ToArray());
            moveBox5.Items.AddRange(moveBox1.Items.Cast<Object>().ToArray());
            moveBox6.Items.AddRange(moveBox1.Items.Cast<Object>().ToArray());
            miximaxAvatarNameBox.Items.AddRange(avatarNameBox.Items.Cast<Object>().ToArray());
            foreach (KeyValuePair<UInt32, Equipment> entry in game.Equipments)
            {
                if (entry.Value.Type.Name == "Boots")
                    bootsBox.Items.Add(entry.Value.Name);
                else if (entry.Value.Type.Name == "Gloves")
                    glovesBox.Items.Add(entry.Value.Name);
                else if (entry.Value.Type.Name == "Bracelet")
                    braceletBox.Items.Add(entry.Value.Name);
                else if (entry.Value.Type.Name == "Pendant")
                    pendantBox.Items.Add(entry.Value.Name);
            }
        }

        private void CreatePage(int maximum)
        {
            pageComboBox.Items.Clear();
            pageComboBox.Items.Add("Main Players");
            for (int page = 1; page < maximum ; page++)
            {
                pageComboBox.Items.Add("Reserves " + (page+1) +  "/" + maximum);
            }
        }

        private void PrintPage(int page)
        {
            for (int i = 0; i < 16; i++)
            {
                int index = page * 16 + i;
                PictureBox playerPictureBox = this.Controls.Find("playerPictureBox" + (i + 1), true).First() as PictureBox;

                if (index < game.PlayersInSave.Count)
                {
                    playerPictureBox.Image = Draw.DrawString(InazumaElevenSaveEditor.Properties.Resources.PlayerRectangleBox, game.GetPlayer(index).Name, 3, 2);
                } else
                {
                    playerPictureBox.Image = InazumaElevenSaveEditor.Properties.Resources.PlayerRectangleBox;
                }
            }
        }

        private void PrintPlayer(Player player)
        {
            tabPage3.Focus();

            // Print General Player Information
            nameBox.SelectedIndex = nameBox.Items.IndexOf(player.Name);
            positionBox.Text = player.Position.ToString();
            elementBox.Text = player.Element.ToString();
            genderBox.Text = player.Gender.ToString();
            levelNumericUpDown.Value = player.Level;
            styleBox.SelectedIndex = player.Style;

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
            avatarNumericUpDown.Enabled = player.Invoke;
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
                moveBox7.Items.Clear();
                moveBox8.Items.Clear();
                for (int t = 0; t < player.MixiMax.AuraPlayer.Moves.Count; t++)
                {
                    moveBox7.Items.Add(player.MixiMax.AuraPlayer.Moves[t].Name);
                    moveBox8.Items.Add(player.MixiMax.AuraPlayer.Moves[t].Name);
                }
                if (player.MixiMax.BestMatch != null)
                {
                    moveBox7.Items.Add(game.Moves[player.MixiMax.BestMatch.MixiMaxMove].Name);
                    moveBox8.Items.Add(game.Moves[player.MixiMax.BestMatch.MixiMaxMove].Name);
                }
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
                        moveBox.SelectedIndex = player.MixiMax.MixiMaxMoveNumber[i];
                        moveNumericUpDown.Maximum = player.MixiMax.AuraPlayer.Moves[player.MixiMax.MixiMaxMoveNumber[i]].EvolutionCount;
                        moveNumericUpDown.Value = player.MixiMax.AuraPlayer.Moves[player.MixiMax.MixiMaxMoveNumber[i]].Level;
                        moveNumericUpDown.Enabled = true;
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

                if (player.MixiMax.AuraPlayer.Avatar != null)
                {
                    miximaxAvatarNameBox.SelectedIndex = miximaxAvatarNameBox.Items.IndexOf(player.MixiMax.AuraPlayer.Avatar.Name);
                    miximaxAvatarNumericUpDown.Value = player.MixiMax.AuraPlayer.Avatar.Level;
                    miximaxAvatarNameBox.Visible = true;
                    miximaxAvatarNameBox.Enabled = true;
                    miximaxAvatarNumericUpDown.Visible = true;
                    miximaxAvatarNumericUpDown.Enabled = true;
                    miximaxAvatarLabel.Visible = true;
                    miximaxAvatarLabel.Enabled = true;
                }
            }
            else
            {
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
            var trainingInformation = game.Training(player, Convert.ToInt32(investedNumericUpDownSelected.Value), startIndex);

            NumericUpDown downStatNumericUpDown = this.Controls.Find(trainingInformation.Item3, true).First() as NumericUpDown;
            investedNumericUpDownSelected.Minimum = trainingInformation.Item1;
            investedNumericUpDownSelected.Maximum = trainingInformation.Item2;
            downStatNumericUpDown.Enabled = trainingInformation.Item4;

            // Print Stat
            for (int i = 0; i < player.Stat.Count; i++)
            {
                TextBox statBox = this.Controls.Find("statBox" + (i + 1), true).First() as TextBox;
                statBox.Text = game.PlayersInSave[game.PlayersInSaveSort[currentPlayer]].Stat[i].ToString();
            }

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
            if (pageComboBox.SelectedIndex == currentPlayer / 16)
            {
                pictureBox.Visible = false;
            }
            else
            {
                pictureBox.Visible = true;
            }

            // Draw Moved Player Picture Box
            movedPlayerPictureBox.Image = Draw.DrawString(InazumaElevenSaveEditor.Properties.Resources.MovedPlayerRectangleBox, game.GetPlayer(currentPlayer).Name, 3, 2);
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
                    if (newIndex != currentPlayer && newIndex <= game.PlayersInSave.Count)
                    {
                        UInt32 playerPositionID = game.PlayersInSaveSort[currentPlayer];
                        UInt32 newPlayerPositionID = game.PlayersInSaveSort[newIndex];
                        game.PlayersInSaveSort[currentPlayer] = newPlayerPositionID;
                        game.PlayersInSaveSort[newIndex] = playerPositionID;
                        break;
                    }
                }
            }
        }

        private void LoadFile(string filePath)
        {
            file = new DataReader(File.ReadAllBytes(filePath));

            switch (Path.GetExtension(filePath))
            {
                case ".ie":
                    format = new IE();
                    break;
                case ".ie4":
                    format = new IE();
                    break;
                default:
                    MessageBox.Show("Format isn't supported");
                    return;
            }

            game = format.Open(file);
            game.Open();

            CreatePage(Convert.ToInt32(Math.Ceiling((double)game.PlayersInSave.Count / 16.0)));
            pageComboBox.SelectedIndex = 0;

            InitializeRessource();

            // game.CurrentPlayer = -1;
            // managePlayerToolStripMenuItem.Enabled = true;
            // searchSortPlayerToolStripMenuItem.Enabled = true;
            // inventoryButton.Enabled = true;
            // streetpassButton.Enabled = true;
            // streetpassButton.Visible = true;
            teamButton.Enabled = true;
            // saveInformationButton.Enabled = true;
            // playRecordsButton.Enabled = true;
            saveToolStripMenuItem.Enabled = true;
            tabControl1.Enabled = false;

        }

        private void OpenToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.FileName == "openFileDialog1") openFileDialog1.FileName = null;

            openFileDialog1.Title = "Open IEGOCS save file";
            openFileDialog1.Filter = "IEGOCS save file (*.ie*)|*.ie*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                LoadFile(openFileDialog1.FileName);
            }
        }

        private void SaveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (game == null) return;

            game.Save(openFileDialog1);
        }

        private void TeamButton_Click(object sender, EventArgs e)
        {
            TeamWindow teamWindow = new TeamWindow(game);
            teamWindow.Show();
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

        private void PlayerPictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (pageComboBox.SelectedIndex * 16 + 0 >= game.PlayersInSave.Count) return;

            currentPlayer = pageComboBox.SelectedIndex * 16 + 0;
            PrintPlayer(game.GetPlayer(currentPlayer));
        }

        private void PlayerPictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (pageComboBox.SelectedIndex * 16 + 1 >= game.PlayersInSave.Count) return;

            currentPlayer = pageComboBox.SelectedIndex * 16 + 1;
            PrintPlayer(game.GetPlayer(currentPlayer));
        }

        private void PlayerPictureBox3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (pageComboBox.SelectedIndex * 16 + 2 >= game.PlayersInSave.Count) return;

            currentPlayer = pageComboBox.SelectedIndex * 16 + 2;
            PrintPlayer(game.GetPlayer(currentPlayer));
        }

        private void PlayerPictureBox4_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (pageComboBox.SelectedIndex * 16 + 3 >= game.PlayersInSave.Count) return;

            currentPlayer = pageComboBox.SelectedIndex * 16 + 3;
            PrintPlayer(game.GetPlayer(currentPlayer));
        }

        private void PlayerPictureBox5_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (pageComboBox.SelectedIndex * 16 + 4 >= game.PlayersInSave.Count) return;

            currentPlayer = pageComboBox.SelectedIndex * 16 + 4;
            PrintPlayer(game.GetPlayer(currentPlayer));
        }

        private void PlayerPictureBox6_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (pageComboBox.SelectedIndex * 16 + 5 >= game.PlayersInSave.Count) return;

            currentPlayer = pageComboBox.SelectedIndex * 16 + 5;
            PrintPlayer(game.GetPlayer(currentPlayer));
        }

        private void PlayerPictureBox7_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (pageComboBox.SelectedIndex * 16 + 6 >= game.PlayersInSave.Count) return;

            currentPlayer = pageComboBox.SelectedIndex * 16 + 6;
            PrintPlayer(game.GetPlayer(currentPlayer));
        }

        private void PlayerPictureBox8_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (pageComboBox.SelectedIndex * 16 + 7 >= game.PlayersInSave.Count) return;

            currentPlayer = pageComboBox.SelectedIndex * 16 + 7;
            PrintPlayer(game.GetPlayer(currentPlayer));
        }

        private void PlayerPictureBox9_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (pageComboBox.SelectedIndex * 16 + 8 >= game.PlayersInSave.Count) return;

            currentPlayer = pageComboBox.SelectedIndex * 16 + 8;
            PrintPlayer(game.GetPlayer(currentPlayer));
        }

        private void PlayerPictureBox10_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (pageComboBox.SelectedIndex * 16 + 9 >= game.PlayersInSave.Count) return;

            currentPlayer = pageComboBox.SelectedIndex * 16 + 9;
            PrintPlayer(game.GetPlayer(currentPlayer));
        }

        private void PlayerPictureBox11_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (pageComboBox.SelectedIndex * 16 + 10 >= game.PlayersInSave.Count) return;

            currentPlayer = pageComboBox.SelectedIndex * 16 + 10;
            PrintPlayer(game.GetPlayer(currentPlayer));
        }

        private void PlayerPictureBox12_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (pageComboBox.SelectedIndex * 16 + 11 >= game.PlayersInSave.Count) return;

            currentPlayer = pageComboBox.SelectedIndex * 16 + 11;
            PrintPlayer(game.GetPlayer(currentPlayer));
        }

        private void PlayerPictureBox13_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (pageComboBox.SelectedIndex * 16 + 12 >= game.PlayersInSave.Count) return;

            currentPlayer = pageComboBox.SelectedIndex * 16 + 12;
            PrintPlayer(game.GetPlayer(currentPlayer));
        }

        private void PlayerPictureBox14_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (pageComboBox.SelectedIndex * 16 + 13 >= game.PlayersInSave.Count) return;

            currentPlayer = pageComboBox.SelectedIndex * 16 + 13;
            PrintPlayer(game.GetPlayer(currentPlayer));
        }

        private void PlayerPictureBox15_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (pageComboBox.SelectedIndex * 16 + 14 >= game.PlayersInSave.Count) return;

            currentPlayer = pageComboBox.SelectedIndex * 16 + 14;
            PrintPlayer(game.GetPlayer(currentPlayer));
        }

        private void PlayerPictureBox16_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (pageComboBox.SelectedIndex * 16 + 15 >= game.PlayersInSave.Count) return;

            currentPlayer = pageComboBox.SelectedIndex * 16 + 15;
            PrintPlayer(game.GetPlayer(currentPlayer));
        }

        private void PlayerPictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (pageComboBox.SelectedIndex * 16 + 0 >= game.PlayersInSave.Count) return;
            if (movedPlayerPictureBox.Visible == false) currentPlayer = pageComboBox.SelectedIndex * 16 + 0;

            MovePictureBox(playerPictureBox1, sender, e);
        }

        private void PlayerPictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (pageComboBox.SelectedIndex * 16 + 1 >= game.PlayersInSave.Count) return;
            if (movedPlayerPictureBox.Visible == false) currentPlayer = pageComboBox.SelectedIndex * 16 + 1;

            MovePictureBox(playerPictureBox2, sender, e);
        }

        private void PlayerPictureBox3_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (pageComboBox.SelectedIndex * 16 + 2 >= game.PlayersInSave.Count) return;
            if (movedPlayerPictureBox.Visible == false) currentPlayer = pageComboBox.SelectedIndex * 16 + 2;

            MovePictureBox(playerPictureBox3, sender, e);
        }

        private void PlayerPictureBox4_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (pageComboBox.SelectedIndex * 16 + 3 >= game.PlayersInSave.Count) return;
            if (movedPlayerPictureBox.Visible == false) currentPlayer = pageComboBox.SelectedIndex * 16 + 3;

            MovePictureBox(playerPictureBox4, sender, e);
        }

        private void PlayerPictureBox5_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (pageComboBox.SelectedIndex * 16 + 4 >= game.PlayersInSave.Count) return;
            if (movedPlayerPictureBox.Visible == false) currentPlayer = pageComboBox.SelectedIndex * 16 + 4;

            MovePictureBox(playerPictureBox5, sender, e);
        }

        private void PlayerPictureBox6_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (pageComboBox.SelectedIndex * 16 + 5 >= game.PlayersInSave.Count) return;
            if (movedPlayerPictureBox.Visible == false) currentPlayer = pageComboBox.SelectedIndex * 16 + 5;

            MovePictureBox(playerPictureBox6, sender, e);
        }

        private void PlayerPictureBox7_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (pageComboBox.SelectedIndex * 16 + 6 >= game.PlayersInSave.Count) return;
            if (movedPlayerPictureBox.Visible == false) currentPlayer = pageComboBox.SelectedIndex * 16 + 6;

            MovePictureBox(playerPictureBox7, sender, e);
        }

        private void PlayerPictureBox8_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (pageComboBox.SelectedIndex * 16 + 7 >= game.PlayersInSave.Count) return;
            if (movedPlayerPictureBox.Visible == false) currentPlayer = pageComboBox.SelectedIndex * 16 + 7;

            MovePictureBox(playerPictureBox8, sender, e);
        }

        private void PlayerPictureBox9_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (pageComboBox.SelectedIndex * 16 + 8 >= game.PlayersInSave.Count) return;
            if (movedPlayerPictureBox.Visible == false) currentPlayer = pageComboBox.SelectedIndex * 16 + 8;

            MovePictureBox(playerPictureBox9, sender, e);
        }

        private void PlayerPictureBox10_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (pageComboBox.SelectedIndex * 16 + 9 >= game.PlayersInSave.Count) return;
            if (movedPlayerPictureBox.Visible == false) currentPlayer = pageComboBox.SelectedIndex * 16 + 9;

            MovePictureBox(playerPictureBox10, sender, e);
        }

        private void PlayerPictureBox11_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (pageComboBox.SelectedIndex * 16 + 10 >= game.PlayersInSave.Count) return;
            if (movedPlayerPictureBox.Visible == false) currentPlayer = pageComboBox.SelectedIndex * 16 + 10;

            MovePictureBox(playerPictureBox11, sender, e);
        }

        private void PlayerPictureBox12_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (pageComboBox.SelectedIndex * 16 + 11 >= game.PlayersInSave.Count) return;
            if (movedPlayerPictureBox.Visible == false) currentPlayer = pageComboBox.SelectedIndex * 16 + 11;

            MovePictureBox(playerPictureBox12, sender, e);
        }

        private void PlayerPictureBox13_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (pageComboBox.SelectedIndex * 16 + 12 >= game.PlayersInSave.Count) return;
            if (movedPlayerPictureBox.Visible == false) currentPlayer = pageComboBox.SelectedIndex * 16 + 12;

            MovePictureBox(playerPictureBox13, sender, e);
        }

        private void PlayerPictureBox14_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (pageComboBox.SelectedIndex * 16 + 13 >= game.PlayersInSave.Count) return;
            if (movedPlayerPictureBox.Visible == false) currentPlayer = pageComboBox.SelectedIndex * 16 + 13;

            MovePictureBox(playerPictureBox14, sender, e);
        }

        private void PlayerPictureBox15_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (pageComboBox.SelectedIndex * 16 + 14 >= game.PlayersInSave.Count) return;
            if (movedPlayerPictureBox.Visible == false) currentPlayer = pageComboBox.SelectedIndex * 16 + 14;

            MovePictureBox(playerPictureBox15, sender, e);
        }

        private void PlayerPictureBox16_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (pageComboBox.SelectedIndex * 16 + 15 >= game.PlayersInSave.Count) return;
            if (movedPlayerPictureBox.Visible == false) currentPlayer = pageComboBox.SelectedIndex * 16 + 1;

            MovePictureBox(playerPictureBox16, sender, e);
        }

        private void PlayerPictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            SwitchPlayer();

            currentPlayer = -1;
            tabControl1.Enabled = false;
            playerPictureBox1.Visible = true;
            movedPlayerPictureBox.Visible = false;
            PageComboBox_SelectedIndexChanged(sender, e);
        }

        private void PlayerPictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            SwitchPlayer();
            currentPlayer = -1;
            tabControl1.Enabled = false;
            playerPictureBox2.Visible = true;
            movedPlayerPictureBox.Visible = false;
            PageComboBox_SelectedIndexChanged(sender, e);
        }

        private void PlayerPictureBox3_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            SwitchPlayer();
            currentPlayer = -1;
            tabControl1.Enabled = false;
            playerPictureBox3.Visible = true;
            movedPlayerPictureBox.Visible = false;
            PageComboBox_SelectedIndexChanged(sender, e);
        }

        private void PlayerPictureBox4_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            SwitchPlayer();
            currentPlayer = -1;
            tabControl1.Enabled = false;
            playerPictureBox4.Visible = true;
            movedPlayerPictureBox.Visible = false;
            PageComboBox_SelectedIndexChanged(sender, e);
        }

        private void PlayerPictureBox5_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            SwitchPlayer();
            currentPlayer = -1;
            tabControl1.Enabled = false;
            playerPictureBox5.Visible = true;
            movedPlayerPictureBox.Visible = false;
            PageComboBox_SelectedIndexChanged(sender, e);
        }

        private void PlayerPictureBox6_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            SwitchPlayer();
            currentPlayer = -1;
            tabControl1.Enabled = false;
            playerPictureBox6.Visible = true;
            movedPlayerPictureBox.Visible = false;
            PageComboBox_SelectedIndexChanged(sender, e);
        }

        private void PlayerPictureBox7_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            SwitchPlayer();
            currentPlayer = -1;
            tabControl1.Enabled = false;
            playerPictureBox7.Visible = true;
            movedPlayerPictureBox.Visible = false;
            PageComboBox_SelectedIndexChanged(sender, e);
        }

        private void PlayerPictureBox8_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            SwitchPlayer();
            currentPlayer = -1;
            tabControl1.Enabled = false;
            playerPictureBox8.Visible = true;
            movedPlayerPictureBox.Visible = false;
            PageComboBox_SelectedIndexChanged(sender, e);
        }

        private void PlayerPictureBox9_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            SwitchPlayer();
            currentPlayer = -1;
            tabControl1.Enabled = false;
            playerPictureBox9.Visible = true;
            movedPlayerPictureBox.Visible = false;
            PageComboBox_SelectedIndexChanged(sender, e);
        }

        private void PlayerPictureBox10_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            SwitchPlayer();
            currentPlayer = -1;
            tabControl1.Enabled = false;
            playerPictureBox10.Visible = true;
            movedPlayerPictureBox.Visible = false;
            PageComboBox_SelectedIndexChanged(sender, e);
        }

        private void PlayerPictureBox11_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            SwitchPlayer();
            currentPlayer = -1;
            tabControl1.Enabled = false;
            playerPictureBox11.Visible = true;
            movedPlayerPictureBox.Visible = false;
            PageComboBox_SelectedIndexChanged(sender, e);
        }

        private void PlayerPictureBox12_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            SwitchPlayer();
            currentPlayer = -1;
            tabControl1.Enabled = false;
            playerPictureBox12.Visible = true;
            movedPlayerPictureBox.Visible = false;
            PageComboBox_SelectedIndexChanged(sender, e);
        }

        private void PlayerPictureBox13_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            SwitchPlayer();
            currentPlayer = -1;
            tabControl1.Enabled = false;
            playerPictureBox13.Visible = true;
            movedPlayerPictureBox.Visible = false;
            PageComboBox_SelectedIndexChanged(sender, e);
        }

        private void PlayerPictureBox14_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            SwitchPlayer();
            currentPlayer = -1;
            tabControl1.Enabled = false;
            playerPictureBox14.Visible = true;
            movedPlayerPictureBox.Visible = false;
            PageComboBox_SelectedIndexChanged(sender, e);
        }

        private void PlayerPictureBox15_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            SwitchPlayer();
            currentPlayer = -1;
            tabControl1.Enabled = false;
            playerPictureBox15.Visible = true;
            movedPlayerPictureBox.Visible = false;
            PageComboBox_SelectedIndexChanged(sender, e);
        }

        private void PlayerPictureBox16_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            SwitchPlayer();
            currentPlayer = -1;
            tabControl1.Enabled = false;
            playerPictureBox16.Visible = true;
            movedPlayerPictureBox.Visible = false;
            PageComboBox_SelectedIndexChanged(sender, e);
        }

        private void NameBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!nameBox.Focused) return;

            Player selectedPlayer = game.GetPlayer(currentPlayer);

            // Get New Player Informations
            var newPlayer = game.Players.FirstOrDefault(x => x.Value.Name == nameBox.Items[nameBox.SelectedIndex].ToString());
            selectedPlayer.Name = newPlayer.Value.Name;
            selectedPlayer.ID = newPlayer.Key;
            selectedPlayer.Freedom = newPlayer.Value.Freedom;
            selectedPlayer.Element = newPlayer.Value.Element;
            selectedPlayer.Position = newPlayer.Value.Position;
            selectedPlayer.Gender = newPlayer.Value.Gender;;

            // Get New Player Stats
            for (int i = 0; i < selectedPlayer.Stat.Count; i++)
            {
                selectedPlayer.Stat[i] = newPlayer.Value.Stat[i];
            }

            // Get New Player Moves
            for (int i = 0; i < newPlayer.Value.UInt32Moves.Count; i++)
            {
                Move newMove = game.Moves[newPlayer.Value.UInt32Moves[i]];
                newMove.Level = 1;
                newMove.TimeLevel = newMove.EvolutionSpeed.TimeLevel[0];
                if (selectedPlayer.Level < 99)
                {
                    newMove.Unlock = false;
                } else
                {
                    newMove.Unlock = true;
                }
                selectedPlayer.Moves[i] = newMove;
            }

            // Reset Invested Point + Print Current Page
            ResetButton_Click(sender, e);
            PrintPlayer(game.GetPlayer(currentPlayer));
            PageComboBox_SelectedIndexChanged(sender, e);
        }

        private void LevelNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!levelNumericUpDown.Focused) return;

            game.GetPlayer(currentPlayer).Level = Convert.ToInt32(levelNumericUpDown.Value);
        }

        private void StyleBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!styleBox.Focused) return;

            game.GetPlayer(currentPlayer).Style = styleBox.SelectedIndex;
        }

        private void InvestedNumericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            if (!investedNumericUpDown3.Focused) return;

            TrainPlayer(game.GetPlayer(currentPlayer), investedNumericUpDown3, 0);
        }

        private void InvestedNumericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            if (!investedNumericUpDown4.Focused) return;

            TrainPlayer(game.GetPlayer(currentPlayer), investedNumericUpDown4, 1);
        }

        private void InvestedNumericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            if (!investedNumericUpDown6.Focused) return;

            TrainPlayer(game.GetPlayer(currentPlayer), investedNumericUpDown6, 3);
        }

        private void InvestedNumericUpDown9_ValueChanged(object sender, EventArgs e)
        {
            if (!investedNumericUpDown9.Focused) return;

            TrainPlayer(game.GetPlayer(currentPlayer), investedNumericUpDown9, 6);
        }

        private void InvestedNumericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            if (!investedNumericUpDown5.Focused) return;

            TrainPlayer(game.GetPlayer(currentPlayer), investedNumericUpDown5, 2);
        }

        private void InvestedNumericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            if (!investedNumericUpDown7.Focused) return;

            TrainPlayer(game.GetPlayer(currentPlayer), investedNumericUpDown7, 4);
        }

        private void InvestedNumericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            if (!investedNumericUpDown8.Focused) return;

            TrainPlayer(game.GetPlayer(currentPlayer), investedNumericUpDown8, 5);
        }

        private void InvestedNumericUpDown10_ValueChanged(object sender, EventArgs e)
        {
            if (!investedNumericUpDown10.Focused) return;

            TrainPlayer(game.GetPlayer(currentPlayer), investedNumericUpDown10, 7);
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            Player player = game.GetPlayer(currentPlayer);

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
                statBox.Text = (Convert.ToInt32(statBox.Text) + investedNumericUpDown.Value).ToString();
            }

            resetButton.Enabled = false;
        }

        private void AvatarNameBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!avatarNameBox.Focused) return;

            Player player = game.GetPlayer(currentPlayer);
            Avatar oldAvatar = player.Avatar;

            player.Avatar = game.Avatars.FirstOrDefault(x => x.Value.Name == avatarNameBox.Text).Value;
            player.Avatar.Level = oldAvatar.Level;
            player.Invoke = true;
        }

        private void AvatarNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!avatarNumericUpDown.Focused) return;

            game.GetPlayer(currentPlayer).Avatar.Level = Convert.ToInt32(avatarNumericUpDown.Value);
        }

        private void InvokeBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!invokeBox.Focused) return;

            Player player = game.GetPlayer(currentPlayer);

            if (invokeBox.Checked == false)
            {
                Avatar newAvatar = game.Avatars[0x0];
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

            game.GetPlayer(currentPlayer).Armed = armedBox.Checked;
        }

        private void MoveBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!moveBox1.Focused) return;

            Player player = game.GetPlayer(currentPlayer);

            Move newMove = game.Moves.FirstOrDefault(x => x.Value.Name == moveBox1.Text).Value;
            newMove.Level = 1;
            newMove.TimeLevel = newMove.EvolutionSpeed.TimeLevel[0];
            newMove.Unlock = true;
            player.Moves[0] = newMove;

            moveNumericUpDown1.Maximum = newMove.EvolutionCount;
        }

        private void MoveBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!moveBox2.Focused) return;

            Player player = game.GetPlayer(currentPlayer);

            Move newMove = game.Moves.FirstOrDefault(x => x.Value.Name == moveBox2.Text).Value;
            newMove.Level = 1;
            newMove.TimeLevel = newMove.EvolutionSpeed.TimeLevel[0];
            newMove.Unlock = true;
            player.Moves[1] = newMove;

            moveNumericUpDown2.Maximum = newMove.EvolutionCount;
        }

        private void MoveBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!moveBox3.Focused) return;

            Player player = game.GetPlayer(currentPlayer);

            Move newMove = game.Moves.FirstOrDefault(x => x.Value.Name == moveBox3.Text).Value;
            newMove.Level = 1;
            newMove.TimeLevel = newMove.EvolutionSpeed.TimeLevel[0];
            newMove.Unlock = true;
            player.Moves[2] = newMove;

            moveNumericUpDown3.Maximum = newMove.EvolutionCount;
        }

        private void MoveBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!moveBox4.Focused) return;

            Player player = game.GetPlayer(currentPlayer);

            Move newMove = game.Moves.FirstOrDefault(x => x.Value.Name == moveBox4.Text).Value;
            newMove.Level = 1;
            newMove.TimeLevel = newMove.EvolutionSpeed.TimeLevel[0];
            newMove.Unlock = true;
            player.Moves[3] = newMove;

            moveNumericUpDown4.Maximum = newMove.EvolutionCount;
        }

        private void MoveBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!moveBox5.Focused) return;

            Player player = game.GetPlayer(currentPlayer);

            Move newMove = game.Moves.FirstOrDefault(x => x.Value.Name == moveBox5.Text).Value;
            newMove.Level = 1;
            newMove.TimeLevel = newMove.EvolutionSpeed.TimeLevel[0];
            newMove.Unlock = true;
            player.Moves[4] = newMove;

            moveNumericUpDown5.Maximum = newMove.EvolutionCount;
        }

        private void MoveBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!moveBox6.Focused) return;

            Player player = game.GetPlayer(currentPlayer);

            Move newMove = game.Moves.FirstOrDefault(x => x.Value.Name == moveBox6.Text).Value;
            newMove.Level = 1;
            newMove.TimeLevel = newMove.EvolutionSpeed.TimeLevel[0];
            newMove.Unlock = true;
            player.Moves[5] = newMove;

            moveNumericUpDown6.Maximum = newMove.EvolutionCount;
        }

        private void MoveNumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (!moveNumericUpDown1.Focused) return;

            Player player = game.GetPlayer(currentPlayer);
            player.Moves[0].Level = Convert.ToInt32(moveNumericUpDown1.Value);
            player.Moves[0].TimeLevel = player.Moves[0].EvolutionSpeed.TimeLevel[Convert.ToInt32(moveNumericUpDown1.Value)];
        }

        private void MoveNumericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (!moveNumericUpDown2.Focused) return;

            Player player = game.GetPlayer(currentPlayer);
            player.Moves[1].Level = Convert.ToInt32(moveNumericUpDown2.Value);
            player.Moves[1].TimeLevel = player.Moves[1].EvolutionSpeed.TimeLevel[Convert.ToInt32(moveNumericUpDown2.Value)];
        }

        private void MoveNumericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            if (!moveNumericUpDown3.Focused) return;

            Player player = game.GetPlayer(currentPlayer);
            player.Moves[2].Level = Convert.ToInt32(moveNumericUpDown3.Value);
            player.Moves[2].TimeLevel = player.Moves[2].EvolutionSpeed.TimeLevel[Convert.ToInt32(moveNumericUpDown3.Value)];
        }

        private void MoveNumericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            if (!moveNumericUpDown4.Focused) return;

            Player player = game.GetPlayer(currentPlayer);
            player.Moves[3].Level = Convert.ToInt32(moveNumericUpDown4.Value);
            player.Moves[3].TimeLevel = player.Moves[3].EvolutionSpeed.TimeLevel[Convert.ToInt32(moveNumericUpDown4.Value)];
        }

        private void MoveNumericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            if (!moveNumericUpDown5.Focused) return;

            Player player = game.GetPlayer(currentPlayer);
            player.Moves[4].Level = Convert.ToInt32(moveNumericUpDown5.Value);
            player.Moves[4].TimeLevel = player.Moves[4].EvolutionSpeed.TimeLevel[Convert.ToInt32(moveNumericUpDown5.Value)];
        }

        private void MoveNumericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            if (!moveNumericUpDown6.Focused) return;

            Player player = game.GetPlayer(currentPlayer);
            player.Moves[5].Level = Convert.ToInt32(moveNumericUpDown6.Value);
            player.Moves[5].TimeLevel = player.Moves[5].EvolutionSpeed.TimeLevel[Convert.ToInt32(moveNumericUpDown6.Value)];
        }

        private void MoveCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!moveCheckBox1.Focused) return;

            game.GetPlayer(currentPlayer).Moves[0].Unlock = moveCheckBox1.Checked;
            moveBox1.Enabled = moveCheckBox1.Checked;
            moveNumericUpDown1.Enabled = moveCheckBox1.Checked;
        }

        private void MoveCheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (!moveCheckBox2.Focused) return;

            game.GetPlayer(currentPlayer).Moves[1].Unlock = moveCheckBox2.Checked;
            moveBox2.Enabled = moveCheckBox2.Checked;
            moveNumericUpDown2.Enabled = moveCheckBox2.Checked;
        }

        private void MoveCheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (!moveCheckBox3.Focused) return;

            game.GetPlayer(currentPlayer).Moves[2].Unlock = moveCheckBox3.Checked;
            moveBox3.Enabled = moveCheckBox3.Checked;
            moveNumericUpDown3.Enabled = moveCheckBox3.Checked;
        }

        private void MoveCheckBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (!moveCheckBox1.Focused) return;

            game.GetPlayer(currentPlayer).Moves[3].Unlock = moveCheckBox4.Checked;
            moveBox4.Enabled = moveCheckBox4.Checked;
            moveNumericUpDown4.Enabled = moveCheckBox4.Checked;
        }

        private void MoveCheckBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (!moveCheckBox5.Focused) return;

            game.GetPlayer(currentPlayer).Moves[4].Unlock = moveCheckBox5.Checked;
            moveBox5.Enabled = moveCheckBox5.Checked;
            moveNumericUpDown5.Enabled = moveCheckBox5.Checked;
        }

        private void MoveCheckBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (!moveCheckBox6.Focused) return;

            game.GetPlayer(currentPlayer).Moves[5].Unlock = moveCheckBox6.Checked;
            moveBox6.Enabled = moveCheckBox6.Checked;
            moveNumericUpDown6.Enabled = moveCheckBox6.Checked;
        }

        private void MoveBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!moveBox7.Focused) return;

            Player player = game.GetPlayer(currentPlayer);

            if (moveBox7.SelectedIndex != moveBox8.SelectedIndex)
            {
                if (moveBox7.SelectedIndex == moveBox7.Items.Count - 1 && player.MixiMax.BestMatch != null)
                {
                    player.MixiMax.MixiMaxMoveNumber[0] = 6;
                    moveNumericUpDown7.Maximum = 1;
                    moveNumericUpDown7.Value = 1;
                    moveNumericUpDown7.Enabled = false;
                }
                else
                {
                    player.MixiMax.MixiMaxMoveNumber[0] = moveBox7.SelectedIndex;
                    moveNumericUpDown7.Maximum = player.MixiMax.AuraPlayer.Moves[moveBox7.SelectedIndex].EvolutionCount;
                    moveNumericUpDown7.Value = player.MixiMax.AuraPlayer.Moves[moveBox7.SelectedIndex].Level;
                    moveNumericUpDown7.Enabled = true;
                }
            } else
            {
                moveBox7.SelectedIndex = player.MixiMax.MixiMaxMoveNumber[0];
            }
        }

        private void MoveBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!moveBox8.Focused) return;

            Player player = game.GetPlayer(currentPlayer);

            if (moveBox8.SelectedIndex != moveBox7.SelectedIndex)
            {
                if (moveBox8.SelectedIndex == moveBox8.Items.Count - 1 && player.MixiMax.BestMatch != null)
                {
                    player.MixiMax.MixiMaxMoveNumber[1] = 6;
                    moveNumericUpDown8.Maximum = 1;
                    moveNumericUpDown8.Value = 1;
                    moveNumericUpDown8.Enabled = false;
                }
                else
                {
                    player.MixiMax.MixiMaxMoveNumber[1] = moveBox8.SelectedIndex;
                    moveNumericUpDown8.Maximum = player.MixiMax.AuraPlayer.Moves[moveBox8.SelectedIndex].EvolutionCount;
                    moveNumericUpDown8.Value = player.MixiMax.AuraPlayer.Moves[moveBox8.SelectedIndex].Level;
                    moveNumericUpDown8.Enabled = true;
                }
            } else
            {
                moveBox8.SelectedIndex = player.MixiMax.MixiMaxMoveNumber[1];
            }
        }

        private void MoveNumericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            if (!moveNumericUpDown7.Focused) return;

            Player player = game.GetPlayer(currentPlayer).MixiMax.AuraPlayer;
            player.Moves[moveBox7.SelectedIndex].Level = Convert.ToInt32(moveNumericUpDown7.Value);
            player.Moves[moveBox7.SelectedIndex].TimeLevel = player.Moves[moveBox7.SelectedIndex].EvolutionSpeed.TimeLevel[Convert.ToInt32(moveNumericUpDown7.Value)];
        }

        private void MoveNumericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            if (!moveNumericUpDown8.Focused) return;

            Player player = game.GetPlayer(currentPlayer).MixiMax.AuraPlayer;
            player.Moves[moveBox8.SelectedIndex].Level = Convert.ToInt32(moveNumericUpDown8.Value);
            player.Moves[moveBox8.SelectedIndex].TimeLevel = player.Moves[moveBox8.SelectedIndex].EvolutionSpeed.TimeLevel[Convert.ToInt32(moveNumericUpDown8.Value)];
        }

        private void MiximaxAvatarNameBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!miximaxAvatarNameBox.Focused) return;

            Player player = game.GetPlayer(currentPlayer).MixiMax.AuraPlayer;
            Avatar oldAvatar = player.Avatar;

            player.Avatar = game.Avatars.FirstOrDefault(x => x.Value.Name == miximaxAvatarNameBox.Text).Value;
            player.Avatar.Level = oldAvatar.Level;
        }

        private void MiximaxAvatarNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!miximaxAvatarNumericUpDown.Focused) return;

            game.GetPlayer(currentPlayer).MixiMax.AuraPlayer.Avatar.Level = Convert.ToInt32(avatarNumericUpDown.Value);
        }

        private void TabControl3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl3.SelectedIndex == 1)
            {
                PrintPlayerFullStat(game.GetPlayer(currentPlayer));
            }
        }

        private void BootsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!bootsBox.Focused) return;

            Player player = game.GetPlayer(currentPlayer);
            var newEquipment = game.Equipments.FirstOrDefault(x => x.Value.Name == bootsBox.Text);

            player.Equipments[0] = newEquipment.Value;

            PrintPlayerFullStat(player);
        }

        private void BraceletBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!braceletBox.Focused) return;

            Player player = game.GetPlayer(currentPlayer);
            var newEquipment = game.Equipments.FirstOrDefault(x => x.Value.Name == braceletBox.Text);
            
            if (newEquipment.Key == 0x0)
            {
                player.Equipments[1] = game.Equipments[0x1];
            } else
            {
                player.Equipments[1] = newEquipment.Value;
            }

            PrintPlayerFullStat(player);
        }

        private void PendantBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Player player = game.GetPlayer(currentPlayer);
            var newEquipment = game.Equipments.FirstOrDefault(x => x.Value.Name == pendantBox.Text);

            if (newEquipment.Key == 0x0)
            {
                player.Equipments[2] = game.Equipments[0x2];
            }
            else
            {
                player.Equipments[2] = newEquipment.Value;
            }

            PrintPlayerFullStat(player);
        }

        private void GlovesBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Player player = game.GetPlayer(currentPlayer);
            var newEquipment = game.Equipments.FirstOrDefault(x => x.Value.Name == glovesBox.Text);

            if (newEquipment.Key == 0x0)
            {
                player.Equipments[3] = game.Equipments[0x3];
            }
            else
            {
                player.Equipments[3] = newEquipment.Value;
            }

            PrintPlayerFullStat(player);
        }
    }
}
