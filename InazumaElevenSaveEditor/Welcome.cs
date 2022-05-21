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

            // IDictionary<uint, Avatar> test = Avatars.Cs;
            // Console.WriteLine(test[0x49E09112].Name);
        }

        private void InitializeRessource()
        {
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
            int investedPointCount = 0;
            for (int i = 0; i < player.InvestedPoint.Count; i++)
            {
                NumericUpDown investedNumericUpDown = this.Controls.Find("investedNumericUpDown" + (i + 3), true).First() as NumericUpDown;
                investedNumericUpDown.Maximum = 65535;
                investedNumericUpDown.Minimum = -65535;
                TextBox statBox = this.Controls.Find("statBox" + (i + 3), true).First() as TextBox;
                investedNumericUpDown.Value = player.InvestedPoint[i];
                statBox.Text = (Convert.ToInt32(statBox.Text) + investedNumericUpDown.Value).ToString();
                if (player.InvestedPoint[i] > 0)
                {
                    investedPointCount += player.InvestedPoint[i];
                }
            }

            // Check if the player is trained
            if (investedPointCount != 0)
            {
                resetButton.Enabled = true;
                for (int i = 0; i < player.InvestedPoint.Count; i++)
                {
                    NumericUpDown investedNumericUpDown = this.Controls.Find("investedNumericUpDown" + (i + 3), true).First() as NumericUpDown;
                    investedNumericUpDown.Enabled = false;
                }
            }
            else
            {
                resetButton.Enabled = false;
                for (int i = 0; i < player.InvestedPoint.Count; i++)
                {
                    NumericUpDown investedNumericUpDown = this.Controls.Find("investedNumericUpDown" + (i + 3), true).First() as NumericUpDown;
                    investedNumericUpDown.Enabled = true;
                }
                player.IsTrained = false;
            }

            // Print Figthing Spirit
            avatarNameBox.SelectedIndex = avatarNameBox.Items.IndexOf(player.Avatar.Name);
            avatarNumericUpDown.Value = player.Avatar.Level;
            invokeBox.Checked = player.Invoke;
            avatarNameBox.Enabled = player.Invoke;
            avatarNumericUpDown.Enabled = player.Invoke;
            armedBox.Enabled = player.Invoke;
            armedBox.Visible = player.Armed;

            // Print Moves
            for (int i = 0; i < player.Moves.Count; i++)
            {
                ComboBox moveBox = this.Controls.Find("moveBox" + (i + 1), true).First() as ComboBox;
                NumericUpDown moveNumericUpDown = this.Controls.Find("moveNumericUpDown" + (i + 1), true).First() as NumericUpDown;
                CheckBox moveCheckBox = this.Controls.Find("moveCheckBox" + (i + 1), true).First() as CheckBox;
                moveBox.SelectedIndex = moveBox.Items.IndexOf(player.Moves[i].Name);
                moveNumericUpDown.Value = player.Moves[i].Level;
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
                    moveBox7.Items.Add(game.Moves[player.MixiMax.AuraPlayer.UInt32Moves[t]].Name);
                    moveBox8.Items.Add(game.Moves[player.MixiMax.AuraPlayer.UInt32Moves[t]].Name);
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
                        moveNumericUpDown.Value = 1;
                        moveNumericUpDown.Enabled = false;
                    }
                    else
                    {
                        moveBox.SelectedIndex = player.MixiMax.MixiMaxMoveNumber[i];
                        moveNumericUpDown.Value = player.MixiMax.AuraPlayer.Moves[player.MixiMax.MixiMaxMoveNumber[i]].Level;
                        moveNumericUpDown.Enabled = true;
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
                if (!control.Equals(movedPlayerPictureBox) && control is Button && movedPlayerPictureBox.Bounds.IntersectsWith(control.Bounds))
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
            inventoryButton.Enabled = true;
            // streetpassButton.Enabled = true;
            // streetpassButton.Visible = true;
            playRecordsButton.Enabled = true;
            saveInformationButton.Enabled = true;
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
    }
}
