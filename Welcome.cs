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

namespace NoFarmForMeOpenSource
{
    public partial class Welcome : Form
    {
        private DataReader FileData = null;

        private IFormat Format = null;

        private IGame Game = null;

        private int SelectedPlayer = -1;

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
            this.Text = Game.GameNameCode + " Save Editor";

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

            foreach (KeyValuePair<UInt32, Player> entry in Game.Players) { nameBox.Items.Add(entry.Value.Name); }
            nameBox.Items.Remove("");
            foreach (KeyValuePair<UInt32, Avatar> entry in Game.Avatars) { avatarNameBox.Items.Add(entry.Value.Name); }
            foreach (KeyValuePair<UInt32, Move> entry in Game.Moves) { moveBox1.Items.Add(entry.Value.Name); }
            moveBox2.Items.AddRange(moveBox1.Items.Cast<Object>().ToArray());
            moveBox3.Items.AddRange(moveBox1.Items.Cast<Object>().ToArray());
            moveBox4.Items.AddRange(moveBox1.Items.Cast<Object>().ToArray());
            moveBox5.Items.AddRange(moveBox1.Items.Cast<Object>().ToArray());
            moveBox6.Items.AddRange(moveBox1.Items.Cast<Object>().ToArray());
            miximaxAvatarNameBox.Items.AddRange(avatarNameBox.Items.Cast<Object>().ToArray());
            foreach (KeyValuePair<UInt32, Equipment> entry in Game.Equipments)
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
                    moveBox7.Items.Add(Game.Moves[player.MixiMax.BestMatch.MixiMaxMove].Name);
                    moveBox8.Items.Add(Game.Moves[player.MixiMax.BestMatch.MixiMaxMove].Name);
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
                    miximaxAvatarNumericUpDown.Enabled = player.MixiMax.AuraPlayer.Avatar.IsFightingSpirit;
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
            if (pageComboBox.SelectedIndex == SelectedPlayer / 16)
            {
                pictureBox.Visible = false;
            }
            else
            {
                pictureBox.Visible = true;
            }

            // Draw Moved Player Picture Box
            movedPlayerPictureBox.Image = Draw.DrawString(InazumaElevenSaveEditor.Properties.Resources.MovedPlayerRectangleBox, Game.GetPlayer(SelectedPlayer).Name, 3, 2);
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
                    if (newIndex != SelectedPlayer && newIndex <= Game.PlayersInSave.Count)
                    {
                        UInt32 playerPositionID = Game.PlayersInSaveSort[SelectedPlayer];
                        UInt32 newPlayerPositionID = Game.PlayersInSaveSort[newIndex];
                        Game.PlayersInSaveSort[SelectedPlayer] = newPlayerPositionID;
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

            SelectedPlayer = -1;
            managePlayerToolStripMenuItem.Enabled = true;
            inventoryButton.Enabled = true;
            // teamButton.Enabled = true;
            saveInformationButton.Enabled = true;
            playRecordsButton.Enabled = true;
            saveToolStripMenuItem1.Enabled = true;
            tabControl1.Enabled = false;

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
            DialogResult dialogResult = MessageBox.Show("Be careful you're trying to acces to untested content.\nEdit your inventory can cause unwanted action.\n\nDo you want to edit your Inventory?", "Inventory Access", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                InventoryWindow inventoryWindow = new InventoryWindow(Game);
                inventoryWindow.ShowDialog();
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

        private void PlayerPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            PictureBox playerPictureBox = (PictureBox)sender;
            int playerNumber = Convert.ToInt32(playerPictureBox.Name.Replace("playerPictureBox", "")) - 1;

            SelectedPlayer = pageComboBox.SelectedIndex * 16 + playerNumber;

            if (SelectedPlayer >= Game.PlayersInSave.Count)
            {
                RecruitToolStripMenuItem_Click(sender, e);
            }
            else
            {
                PrintPlayer(Game.GetPlayer(SelectedPlayer));
            }
        }

        private void PlayerPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            PictureBox playerPictureBox = (PictureBox)sender;
            int playerNumber = Convert.ToInt32(playerPictureBox.Name.Replace("playerPictureBox", "")) - 1;

            if (pageComboBox.SelectedIndex * 16 + playerNumber >= Game.PlayersInSave.Count) return;
            if (movedPlayerPictureBox.Visible == false) SelectedPlayer = pageComboBox.SelectedIndex * 16 + playerNumber;

            MovePictureBox(playerPictureBox, sender, e);
        }

        private void PlayerPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            PictureBox playerPictureBox = (PictureBox)sender;

            SwitchPlayer();

            SelectedPlayer = -1;
            tabControl1.Enabled = false;
            playerPictureBox.Visible = true;
            movedPlayerPictureBox.Visible = false;
            PageComboBox_SelectedIndexChanged(sender, e);
        }

        private void NameBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!nameBox.Focused) return;

            // Update Player
            Player selectedPlayer = Game.GetPlayer(SelectedPlayer);
            Game.ChangePlayer(selectedPlayer, Game.Players.FirstOrDefault(x => x.Value.Name == nameBox.Items[nameBox.SelectedIndex].ToString()));

            // Reset Invested Point + Print Current Page
            ResetButton_Click(sender, e);
            PrintPlayer(Game.GetPlayer(SelectedPlayer));
            PageComboBox_SelectedIndexChanged(sender, e);
        }

        private void LevelNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!levelNumericUpDown.Focused) return;

            Game.GetPlayer(SelectedPlayer).Level = Convert.ToInt32(levelNumericUpDown.Value);
        }

        private void StyleBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!styleBox.Focused) return;

            Game.GetPlayer(SelectedPlayer).Style = styleBox.SelectedIndex;
        }

        private void InvestedNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown investedNumericUpDown = (NumericUpDown)sender;

            if (!investedNumericUpDown.Focused) return;

            Player player = Game.GetPlayer(SelectedPlayer);
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
            Player player = Game.GetPlayer(SelectedPlayer);

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

            Player player = Game.GetPlayer(SelectedPlayer);
            Avatar oldAvatar = player.Avatar;

            player.Invoke = true;
            player.Avatar = Game.Avatars.FirstOrDefault(x => x.Value.Name == avatarNameBox.Text).Value;
            player.Avatar.Level = oldAvatar.Level;

            avatarNumericUpDown.Enabled = player.Avatar.IsFightingSpirit;
        }

        private void AvatarNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!avatarNumericUpDown.Focused) return;

            Game.GetPlayer(SelectedPlayer).Avatar.Level = Convert.ToInt32(avatarNumericUpDown.Value);
        }

        private void InvokeBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!invokeBox.Focused) return;

            Player player = Game.GetPlayer(SelectedPlayer);

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

            Game.GetPlayer(SelectedPlayer).Armed = armedBox.Checked;
        }

        private void MoveBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox moveBox = (ComboBox)sender;
            int moveBoxNumber = Convert.ToInt32(moveBox.Name.Replace("moveBox", "")) - 1;

            if (!moveBox.Focused) return;

            Player player = Game.GetPlayer(SelectedPlayer);

            Move newMove = Game.Moves.FirstOrDefault(x => x.Value.Name == moveBox.Text).Value;
            newMove.Level = 1;
            newMove.TimeLevel = newMove.EvolutionSpeed.TimeLevel[0];
            newMove.Unlock = true;
            player.Moves[moveBoxNumber] = newMove;

            NumericUpDown moveNumericUpDown = (NumericUpDown)tabPage5.Controls.Find("moveNumericUpDown" + (moveBoxNumber+1), false)[0];
            moveNumericUpDown.Value = 1;
            moveNumericUpDown.Maximum = newMove.EvolutionCount;
        }

        private void MoveNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown moveNumericUpDown = (NumericUpDown)sender;
            int moveNumericUpDownNumber = Convert.ToInt32(moveNumericUpDown.Name.Replace("moveNumericUpDown", "")) - 1;

            if (!moveNumericUpDown.Focused) return;

            Player player = Game.GetPlayer(SelectedPlayer);
            player.Moves[moveNumericUpDownNumber].Level = Convert.ToInt32(moveNumericUpDown.Value);

            if (moveNumericUpDown.Value < 6)
            {
                player.Moves[moveNumericUpDownNumber].TimeLevel = player.Moves[moveNumericUpDownNumber].EvolutionSpeed.TimeLevel[Convert.ToInt32(moveNumericUpDown.Value)];
            }
        }

        private void MoveCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox moveCheckBox = (CheckBox)sender;
            int moveCheckBoxNumber = Convert.ToInt32(moveCheckBox.Name.Replace("moveCheckBox", "")) - 1;

            if (!moveCheckBox.Focused) return;

            Game.GetPlayer(SelectedPlayer).Moves[moveCheckBoxNumber].Unlock = moveCheckBox.Checked;

            ComboBox moveBox = (ComboBox)tabPage5.Controls.Find("moveBox" + (moveCheckBoxNumber + 1), false)[0];
            moveBox.Enabled = moveCheckBox.Checked;

            NumericUpDown moveNumericUpDown = (NumericUpDown)tabPage5.Controls.Find("moveNumericUpDown" + (moveCheckBoxNumber + 1), false)[0];
            moveNumericUpDown.Enabled = moveCheckBox.Checked;
        }

        private void MoveBoxMixiMax_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox moveBox = (ComboBox)sender;
            int moveBoxNumber = Convert.ToInt32(moveBox.Name.Replace("moveBox", "")) - 1;
            int mixiMaxMove = moveBoxNumber - 6;

            if (!moveBox.Focused) return;

            NumericUpDown moveNumericUpDown = (NumericUpDown)tabPage5.Controls.Find("moveNumericUpDown" + (moveBoxNumber + 1), false)[0];

            Player player = Game.GetPlayer(SelectedPlayer);

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
            int moveNumericUpDownNumber = Convert.ToInt32(moveNumericUpDown.Name.Replace("moveNumericUpDown", "")) - 1;

            if (!moveNumericUpDown.Focused) return;

            Player player = Game.GetPlayer(SelectedPlayer).MixiMax.AuraPlayer;

            ComboBox moveBox = (ComboBox)tabPage5.Controls.Find("moveBox" + (moveNumericUpDownNumber + 1), false)[0];
            player.Moves[moveBox.SelectedIndex].Level = Convert.ToInt32(moveNumericUpDown.Value);
            player.Moves[moveBox.SelectedIndex].TimeLevel = player.Moves[moveBox.SelectedIndex].EvolutionSpeed.TimeLevel[Convert.ToInt32(moveNumericUpDown.Value)];
        }

        private void MiximaxAvatarNameBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!miximaxAvatarNameBox.Focused) return;

            Player player = Game.GetPlayer(SelectedPlayer).MixiMax.AuraPlayer;
            Avatar oldAvatar = player.Avatar;

            player.Avatar = Game.Avatars.FirstOrDefault(x => x.Value.Name == miximaxAvatarNameBox.Text).Value;
            player.Avatar.Level = oldAvatar.Level;

            miximaxAvatarNumericUpDown.Enabled = player.Avatar.IsFightingSpirit;
        }

        private void MiximaxAvatarNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!miximaxAvatarNumericUpDown.Focused) return;

            Game.GetPlayer(SelectedPlayer).MixiMax.AuraPlayer.Avatar.Level = Convert.ToInt32(avatarNumericUpDown.Value);
        }

        private void TabControl3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl3.SelectedIndex == 1)
            {
                PrintPlayerFullStat(Game.GetPlayer(SelectedPlayer));
            }
        }

        private void BootsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!bootsBox.Focused) return;

            Player player = Game.GetPlayer(SelectedPlayer);
            var newEquipment = Game.Equipments.FirstOrDefault(x => x.Value.Name == bootsBox.Text);

            player.Equipments[0] = newEquipment.Value;

            PrintPlayerFullStat(player);
        }

        private void BraceletBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!braceletBox.Focused) return;

            Player player = Game.GetPlayer(SelectedPlayer);
            var newEquipment = Game.Equipments.FirstOrDefault(x => x.Value.Name == braceletBox.Text);
            
            if (newEquipment.Key == 0x0)
            {
                player.Equipments[1] = Game.Equipments[0x1];
            } else
            {
                player.Equipments[1] = newEquipment.Value;
            }

            PrintPlayerFullStat(player);
        }

        private void PendantBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Player player = Game.GetPlayer(SelectedPlayer);
            var newEquipment = Game.Equipments.FirstOrDefault(x => x.Value.Name == pendantBox.Text);

            if (newEquipment.Key == 0x0)
            {
                player.Equipments[2] = Game.Equipments[0x2];
            }
            else
            {
                player.Equipments[2] = newEquipment.Value;
            }

            PrintPlayerFullStat(player);
        }

        private void GlovesBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Player player = Game.GetPlayer(SelectedPlayer);
            var newEquipment = Game.Equipments.FirstOrDefault(x => x.Value.Name == glovesBox.Text);

            if (newEquipment.Key == 0x0)
            {
                player.Equipments[3] = Game.Equipments[0x3];
            }
            else
            {
                player.Equipments[3] = newEquipment.Value;
            }

            PrintPlayerFullStat(player);
        }

        private void RecruitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Game.PlayersInSave.Count < Game.MaximumPlayer)
            {
                ComboBox customNameBox = nameBox;

                MessageComboBox inviteWindow = new MessageComboBox("Invite Player", "Select the player you want to invite", "Invite", customNameBox);
                DialogResult dialogResult = inviteWindow.ShowDialog();
                if (dialogResult == DialogResult.Yes && inviteWindow.nameBox.SelectedIndex != -1)
                {
                    Game.RecruitPlayer(Game.Players.FirstOrDefault(x => x.Value.Name == inviteWindow.nameBox.Text.ToString()));

                    CreatePage(Convert.ToInt32(Math.Ceiling((double)Game.PlayersInSave.Count / 16.0)));
                    pageComboBox.SelectedIndex = pageComboBox.Items.Count - 1;

                    SelectedPlayer = Game.PlayersInSave.Count - 1;
                    PrintPlayer(Game.GetPlayer(Game.PlayersInSave.Count - 1));

                    MessageBox.Show(inviteWindow.nameBox.Text + " has joined you");
                }
            } else
            {
                MessageBox.Show("Maximum player reached");
            }
        }

        private void DismissToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ComboBox customNameBox = new ComboBox();
            for (int i = 0; i < Game.PlayersInSaveSort.Count; i++)
            {
                customNameBox.Items.Add(Game.GetPlayer(i).Name);
            }
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

                Game.PlayersInSaveSort.Remove(playerIndex);
                Game.PlayersInSave.Remove(playerIndex);

                CreatePage(Convert.ToInt32(Math.Ceiling((double)Game.PlayersInSave.Count / 16.0)));
                pageComboBox.SelectedIndex = 0;
                SelectedPlayer = -1;
                tabControl1.Enabled = false;

                MessageBox.Show(inviteWindow.nameBox.Text + " left the team");
            }
        }

        private void ScoreNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!scoreNumericUpDown.Focused) return;

            Game.GetPlayer(SelectedPlayer).Score = Convert.ToInt32(scoreNumericUpDown.Value);
        }

        private void ParticipationNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!participationNumericUpDown.Focused) return;

            Game.GetPlayer(SelectedPlayer).Participation = Convert.ToInt32(participationNumericUpDown.Value);
        }
    }
}
