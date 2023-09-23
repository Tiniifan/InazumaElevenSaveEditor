using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using InazumaElevenSaveEditor.Tools;
using InazumaElevenSaveEditor.InazumaEleven.Logic;

namespace InazumaElevenSaveEditor.InazumaEleven.Games.IEGO
{
    public class GO : IGame
    {
        public string Code => "IEGO";

        public string Name => "Inazuma Eleven Go";

        public int MaximumPlayer => 112;

        public BinaryDataReader Data { get; set; }

        public List<Player> Reserve { get; set; }

        public Dictionary<int, Player> Auras { get; set; }

        public Dictionary<int, Equipment> Equipments { get; set; }

        public Dictionary<int, Item> Inventory { get; set; }

        public Dictionary<int, List<PlayRecord>> PlayRecords { get; set; }

        public SaveInfo SaveInfo { get; set; }

        public List<Team> Teams { get; set; }

        public Dictionary<uint, Player> Players => Common.GO.Players.Go;

        public Dictionary<uint, Move> Moves => Common.GO.Moves.Go;

        public Dictionary<uint, Avatar> Avatars => Common.GO.Avatars.Go;

        public Dictionary<uint, Item> Items => Common.GO.Items.Go;

        private Dictionary<int, Item> GetInventory()
        {
            // Group Item 1
            Data.Seek((uint)IEGOHelper.ItemBlockGroup1Offset);
            Dictionary<int, Item> itemBlockGroup1 = Data.ReadMultipleStruct<IEGOHelper.ItemBlockGroup1>(256).Where(x => Items.ContainsKey(x.ID) && x.ID != 0x00).ToDictionary(x => x.Index, y => new Item(y.ID, Items[y.ID], y.Quantity));

            // Group Item 2
            Data.Seek((uint)IEGOHelper.ItemBlockGroup2Offset);
            Dictionary<int, Item> itemBlockGroup2 = Data.ReadMultipleStruct<IEGOHelper.ItemBlockGroup2>(224).Where(x => Items.ContainsKey(x.ID) && x.ID != 0x00).ToDictionary(x => x.Index, y => new Item(y.ID, Items[y.ID], y.Quantity, y.QuantityEquiped));

            // Group Item 3
            Data.Seek((uint)IEGOHelper.ItemBlockGroup3Offset);
            Dictionary<int, Item> itemBlockGroup3 = Data.ReadMultipleStruct<IEGOHelper.ItemBlockGroup3>(512).Where(x => Items.ContainsKey(x.ID) && x.ID != 0x00).ToDictionary(x => x.Index, y => new Item(y.ID, Items[y.ID]));

            // Find Equipment
            Equipments = itemBlockGroup2.Where(x => Common.GO.Equipments.Go.ContainsKey(x.Value.ID)).ToDictionary(x => x.Key, y => Common.GO.Equipments.Go[y.Value.ID]);
            Equipments.Add(0, Common.GO.Equipments.Go[0x0]);

            // Merge
            return itemBlockGroup1.Union(itemBlockGroup2).Union(itemBlockGroup3).ToDictionary(x => x.Key, x => x.Value);
        }

        private void SaveInventory(BinaryDataWriter writer)
        {
            // Group Item 1
            writer.Seek((uint)IEGOHelper.ItemBlockGroup1Offset);
            Dictionary<int, Item> itemGroup1 = Inventory.Where(x => x.Value.Category == 1).ToDictionary(x => x.Key, y => y.Value);
            for (int i = 0; i < 256; i++)
            {
                if (i < itemGroup1.Count)
                {
                    KeyValuePair<int, Item> item = itemGroup1.ElementAt(i);
                    writer.Write(item.Key);
                    writer.Write(item.Value.ID);
                    writer.Write(item.Value.Quantity);
                }
                else
                {
                    writer.Write(0x00);
                    writer.Write(0x00);
                    writer.Write(0x00);
                }
            }

            // Calculates the number of Fighting Spirits used
            foreach (KeyValuePair<int, Item> item in Inventory.Where(x => x.Value.SubCategory == 21))
            {
                int count = Reserve.Count(x => x.Avatar != null & Avatars.Where(y => y.Value.Name == x.Avatar.Name).FirstOrDefault().Key == item.Value.ID);

                if (count > item.Value.Quantity)
                {
                    item.Value.Quantity = count;
                }

                item.Value.QuantityEquiped = count;
            }

            // Calculates the number of Fighting Spirits used
            foreach (KeyValuePair<int, Item> item in Inventory.Where(x => x.Value.SubCategory > 0 & x.Value.SubCategory < 5))
            {
                int count = Reserve.Count(x => x.Equipments != null & x.Equipments.Where(y => y.Name == item.Value.Name).FirstOrDefault() != null);

                if (count > item.Value.Quantity)
                {
                    item.Value.Quantity = count;
                }

                item.Value.QuantityEquiped = count;
            }

            // Group Item 2
            writer.Seek((uint)IEGOHelper.ItemBlockGroup2Offset);
            Dictionary<int, Item> itemGroup2 = Inventory.Where(x => x.Value.Category == 2).ToDictionary(x => x.Key, y => y.Value);
            for (int i = 0; i < 224; i++)
            {
                if (i < itemGroup2.Count)
                {
                    KeyValuePair<int, Item> item = itemGroup2.ElementAt(i);
                    writer.Write(item.Key);
                    writer.Write(item.Value.ID);
                    writer.Write(item.Value.Quantity);
                    writer.Write(item.Value.QuantityEquiped);
                }
                else
                {
                    writer.Write(0x00);
                    writer.Write(0x00);
                    writer.Write(0x00);
                    writer.Write(0x00);
                }
            }

            // Group Item 3
            writer.Seek((uint)IEGOHelper.ItemBlockGroup3Offset);
            Dictionary<int, Item> itemGroup3 = Inventory.Where(x => x.Value.Category == 3).ToDictionary(x => x.Key, y => y.Value);
            for (int i = 0; i < 512; i++)
            {
                if (i < itemGroup3.Count)
                {
                    KeyValuePair<int, Item> item = itemGroup3.ElementAt(i);
                    writer.Write(item.Key);
                    writer.Write(item.Value.ID);
                }
                else
                {
                    writer.Write(0x00);
                    writer.Write(0x00);
                }
            }
        }

        private List<Player> GetPlayers()
        {
            // Get player index
            Data.Seek((uint)IEGOHelper.PlayerIndexOffset);
            int[] index = Data.ReadMultipleStruct<int>(IEGOHelper.MaximumPlayer)
                .Where(x => x != 0x0)
                .ToArray();

            // Get player data according to their index
            Data.Seek((uint)IEGOHelper.PlayerDataOffset);
            List<IEGOHelper.PlayerBlock> playerBlocks = Data.ReadMultipleStruct<IEGOHelper.PlayerBlock>(index.Length)
                .ToList();

            // Create player objects and apply transformations
            List<Player> players = playerBlocks
                .Where(playerBlock => Players.Keys.Contains(playerBlock.ID))
                .Select(playerBlock =>
                {
                    Player player = new Player(Players[playerBlock.ID]);

                    player.ID = playerBlock.ID;
                    player.Index = playerBlock.Index;
                    player.Level = playerBlock.Level;
                    player.FP = playerBlock.GP;
                    player.TP = playerBlock.TP;

                    player.InvestedPoint = playerBlock.InvestedPoint.Select(x => Convert.ToInt32(x)).ToList();
                    player.IsTrained = player.InvestedPoint.Any(value => value > 0);

                    player.Moves = playerBlock.Moves
                        .Select(moveBlock => new Move(Moves.TryGetValue(moveBlock.MoveID, out var move) ? move : Moves[0x0], moveBlock.MoveLevel, moveBlock.MoveLearned, moveBlock.MoveUsage))
                        .ToList();

                    player.Avatar = new Avatar(Avatars[playerBlock.AvatarID], playerBlock.AvatarLevel, playerBlock.AvatarUsage);
                    player.Invoke = Convert.ToBoolean(playerBlock.CanInvoke);

                    player.Equipments = new List<Equipment>
                    {
                        Equipments[playerBlock.BootsIndex],
                        Equipments[playerBlock.BraceletIndex],
                        Equipments[playerBlock.PendantIndex],
                        Equipments[playerBlock.GlovesIndex]
                    };

                    player.Index = playerBlock.Index;
                    player.Participation = playerBlock.ParticipationPoint;
                    player.Score = playerBlock.ScorePoint;

                    return player;
                })
                .ToList();

            // Sort
            return players.OrderBy(player => Array.IndexOf(index, player.Index)).ToList();
        }

        private void SavePlayers(BinaryDataWriter writer)
        {
            // Save player index
            writer.Seek((uint)IEGOHelper.PlayerIndexOffset);
            for (int i = 0; i < IEGOHelper.MaximumPlayer; i++)
            {
                if (i < Reserve.Count)
                {
                    writer.Write(Reserve[i].Index);
                }
                else
                {
                    writer.Write(0x0);
                }
            }

            // Save player data
            writer.Seek((uint)IEGOHelper.PlayerDataOffset);
            List<Player> sortedReserve = Reserve.OrderBy(player => player.Index).ToList();
            for (int i = 0; i < IEGOHelper.MaximumPlayer; i++)
            {
                if (i < sortedReserve.Count)
                {
                    Player player = sortedReserve[i];

                    writer.Write(player.Index);
                    writer.Write(player.ID);
                    writer.Skip(0x04);
                    writer.Write((short)player.FP);
                    writer.Write((short)player.TP);
                    writer.Write((short)player.Freedom);
                    writer.Write((byte)player.Level);
                    writer.Skip(0x03);
                    writer.Write((byte) Convert.ToInt32(player.Invoke));
                    writer.Skip(0x01);
                    writer.Write((short)player.Participation);
                    writer.Write((short)player.Score);
                    writer.Skip(0x04);
                    writer.Write(player.InvestedPoint.SelectMany(x => BitConverter.GetBytes((short)x)).ToArray());
                    writer.Write(Avatars.FirstOrDefault(x => x.Value.Name == player.Avatar.Name).Key);
                    writer.Write((byte)player.Avatar.Level);
                    writer.Write((byte)player.Avatar.Usage);
                    writer.Skip(0x01);
                    writer.Skip(0x01);
                    writer.Write(Equipments.FirstOrDefault(x => x.Value == player.Equipments[0]).Key);
                    writer.Write(Equipments.FirstOrDefault(x => x.Value == player.Equipments[1]).Key);
                    writer.Write(Equipments.FirstOrDefault(x => x.Value == player.Equipments[2]).Key);
                    writer.Write(Equipments.FirstOrDefault(x => x.Value == player.Equipments[3]).Key);
                    writer.Skip(0x04);

                    for (int j = 0; j < 6; j++)
                    {
                        var moveKeyValue = Moves.FirstOrDefault(x => x.Value.Name == player.Moves[j].Name);
                        writer.Write(moveKeyValue.Key);
                        writer.Write((byte)player.Moves[j].Level);
                        writer.Write((byte)player.Moves[j].UsedCount);
                        writer.Write(Convert.ToInt32(player.Moves[j].Unlock));
                        writer.Skip(2);
                    }

                    writer.Skip(0x38);
                }
                else
                {
                    int[] emptyPlayerData = Enumerable.Repeat(0, 51).ToArray();
                    writer.Write(emptyPlayerData.SelectMany(x => BitConverter.GetBytes(x)).ToArray());
                }
            }
        }

        public void OpenPlayRecords()
        {
            // Doesn't exist on IEGO
        }

        public void OpenSaveInfo()
        {
            if (SaveInfo == null)
            {
                SaveInfo = new SaveInfo();

                // Name
                Data.Seek((uint)IEGOHelper.NameOffset);
                SaveInfo.Name = Data.ReadString(Encoding.UTF8);
                Data.Seek((uint)IEGOHelper.TeamNameOffset);
                SaveInfo.TeamName = Data.ReadString(Encoding.UTF8);

                // Time Hours
                Data.Seek((uint)IEGOHelper.TimeOffset);
                int seconds = Data.ReadValue<int>();
                int hours = seconds / 3600;
                int minutes = seconds / 60 % 60;
                if (hours > 999)
                {
                    hours = 999;
                }
                if (minutes > 59)
                {
                    minutes = 59;
                }
                SaveInfo.Hours = hours;
                SaveInfo.Min = minutes;

                // Link Level
                SaveInfo.SecretLinkLevel = -1;

                // Current Chapter
                SaveInfo.Chapter = -1;

                // Money
                Data.Seek((uint)IEGOHelper.MoneyOffset);
                SaveInfo.Prestige = Data.ReadValue<int>();
                SaveInfo.Friendship = Data.ReadValue<int>();
            }
        }

        private void SaveSaveInfo(BinaryDataWriter writer)
        {
            if (SaveInfo == null) return;

            // Save Name
            writer.Seek((uint)IEGOHelper.NameOffset);
            writer.Write(Encoding.UTF8.GetBytes(SaveInfo.Name));
            writer.Write((byte)0x0);
            writer.Write((byte)0x88);

            // Team Name
            writer.Seek((uint)IEGOHelper.TeamNameOffset);
            writer.Write(Encoding.UTF8.GetBytes(SaveInfo.TeamName));
            writer.Write((byte)0x0);
            writer.Write((byte)0x88);

            // Time Hours
            writer.Seek((uint)IEGOHelper.TimeOffset);
            int time = SaveInfo.Hours * 3600 + SaveInfo.Min * 60;
            writer.Write(time);

            // Link Level
            if (SaveInfo.SecretLinkLevel == 3)
            {
                writer.Seek((uint)IEGOHelper.LinkOffset);
                writer.Write((short)0x01C0);
                writer.Write((short)0x00);
            }

            // Money
            writer.Seek((uint)IEGOHelper.MoneyOffset);
            writer.Write(SaveInfo.Prestige);
            writer.Write(SaveInfo.Friendship);

            if (SaveInfo.UnlockAllData == true)
            {
                // To Do
            }
        }

        public void OpenTactics()
        {
            if (Teams == null)
            {
                Teams = new List<Team>();

                // Main team
                Data.Seek((uint)IEGOHelper.TeamNameOffset);
                string name = Data.ReadString(Encoding.UTF8);
                Teams.Add(LoadTeam(0x8DF0, name, 0x8EC8));

                // Custom Teams
                uint teamInfo = 0x8E20;
                string[] names = new string[] { "Team A", "Team B", "Team C" };
                uint teamPlayers = 0x9088;
                for (int i = 0; i < 3; i++)
                {
                    Teams.Add(LoadTeam(teamInfo, names[i], teamPlayers));
                    teamInfo += 0x30;
                    teamPlayers += 0x40;
                }
            }
        }

        private void SaveTactics(BinaryDataWriter writer)
        {
            if (Teams == null) return;

            // Chrono Storm Team
            SaveTeam(writer, Teams[0], 0x8DF0, 0x8EC8, true);

            // Custom Teams
            uint teamInfo = 0x8E20;
            uint teamPlayers = 0x9088;
            for (int i = 0; i < 3; i++)
            {
                SaveTeam(writer, Teams[i + 1], teamInfo, teamPlayers);
                teamInfo += 0x30;
                teamPlayers += 0x40;
            }
        }

        public byte[] Save()
        {
            byte[] saveData = Data.GetSection(0, (int)Data.Length);
            BinaryDataWriter dataWriter = new BinaryDataWriter(saveData);

            SaveSaveInfo(dataWriter);
            SaveTactics(dataWriter);
            SaveInventory(dataWriter);
            SavePlayers(dataWriter);

            return saveData;
        }

        private Team LoadTeam(uint teamInfo, string teamName, uint teamPlayers)
        {
            Data.Seek(teamInfo);

            IEGOHelper.Team team = Data.ReadStruct<IEGOHelper.Team>();
            Item coach = Inventory.ContainsKey(team.CoachIndex) ? Inventory[team.CoachIndex] : null;
            Item formation = Inventory.ContainsKey(team.FormationIndex) ? Inventory[team.FormationIndex] : null;
            Item kit = Inventory.ContainsKey(team.KitIndex) ? Inventory[team.KitIndex] : null;
            Item emblem = Inventory.ContainsKey(team.EmblemIndex) ? Inventory[team.EmblemIndex] : null;

            Data.Seek(teamPlayers);
            IEGOHelper.TeamPlayer teamPlayer = Data.ReadStruct<IEGOHelper.TeamPlayer>();
            List<Player> players = teamPlayer.PlayersIndex.Where(x => x != 0x00).Select(x => Reserve.FirstOrDefault(y => y.Index == x)).ToList();
            List<int> playersFormationIndex = team.PlayersFormationIndex.Select(x => Convert.ToInt32(x)).ToList();
            List<int> playersKitNumber = team.PlayerKitNumber.Select(x => Convert.ToInt32(x)).ToList();

            return new Team(teamName, emblem, kit, formation, coach, players, playersFormationIndex, playersKitNumber);
        }

        private void SaveTeam(BinaryDataWriter writer, Team team, uint teamInfo, uint teamPlayers, bool teamNameCanEdit = false)
        {
            writer.Seek(teamInfo);
            writer.Write((team.Coach == null) ? 0x00 : Inventory.FirstOrDefault(x => x.Value == team.Coach).Key);
            writer.Write((team.Formation == null) ? 0x00 : Inventory.FirstOrDefault(x => x.Value == team.Formation).Key);
            writer.Write((team.Kit == null) ? 0x00 : Inventory.FirstOrDefault(x => x.Value == team.Kit).Key);
            writer.Write((team.Emblem == null) ? 0x00 : Inventory.FirstOrDefault(x => x.Value == team.Emblem).Key);
            writer.Write(team.PlayersFormationIndex.Select(x => (byte)x).ToArray());
            writer.Write(team.PlayersKitNumber.Select(x => (byte)x).ToArray());

            if (teamNameCanEdit)
            {
                writer.Seek((uint)IEGOHelper.TeamNameOffset);
                writer.Write(Encoding.UTF8.GetBytes(team.Name));
                writer.Write((byte)0x0);
                writer.Write((byte)0x88);
            } 

            writer.Seek(teamPlayers);
            writer.Write(team.Players.SelectMany(x => BitConverter.GetBytes(x.Index)).ToArray());
        }


        public void NewMixiMax(Player player, int miximaxIndex, int miximaxMove1, int miximaxMove2)
        {
            // Doesn't exist on IEGO
        }

        private int GetPlayerIndex(uint x)
        {
            UInt16 uint16 = Convert.ToUInt16(x / 0x10000);
            uint16 = (UInt16)((uint16 & 0xFFU) << 8 | (uint16 & 0xFF00U) >> 8);
            return Convert.ToInt32(uint16);
        }

        public void RecruitPlayer(Player player, bool configure)
        {
            player = new Player(player);

            if (configure)
            {
                player.Style = 0;
                player.Level = 1;
                player.InvestedPoint = new List<int>(new int[8]);
                player.Avatar = Avatars[0x0];
                player.Invoke = false;
                player.Armed = false;

                player.Moves = new List<Move>();
                for (int i = 0; i < 4; i++)
                {
                    player.Moves.Add(Moves[player.UInt32Moves[i]]);
                }
                player.Moves.Add(Moves[0x00]);
                player.Moves.Add(Moves[0x00]);

                player.Equipments = new List<Equipment>();
                for (int i = 0; i < 4; i++)
                {
                    player.Equipments.Add(Equipments[0x0]);
                }
            }

            var playerListIndex = Reserve.Select(x => GetPlayerIndex((uint)x.Index)).ToList();
            playerListIndex.Sort();
            int missingIndex = Enumerable.Range(0, MaximumPlayer).Except(playerListIndex).First();

            player.Index = Convert.ToInt32((uint)missingIndex * 0x1000000 + (uint)(missingIndex + 1) * 0x100);
            Reserve.Add(player);
        }

        public Player ChangePlayer(Player selectedPlayer, Player newPlayer, bool keepOldPlayerInformation)
        {
            newPlayer = new Player(newPlayer);

            if (keepOldPlayerInformation)
            {
                newPlayer.Index = selectedPlayer.Index;
                newPlayer.Level = selectedPlayer.Level;
                newPlayer.Invoke = selectedPlayer.Invoke;
                newPlayer.Armed = selectedPlayer.Armed;
                newPlayer.Style = selectedPlayer.Style;
                newPlayer.Avatar = selectedPlayer.Avatar;
                newPlayer.Equipments = selectedPlayer.Equipments;
                newPlayer.MixiMax = selectedPlayer.MixiMax;
                newPlayer.InvestedPoint = selectedPlayer.InvestedPoint;
                newPlayer.InvestedFreedom = selectedPlayer.InvestedFreedom;
                newPlayer.IsAura = selectedPlayer.IsAura;
                newPlayer.IsTrained = selectedPlayer.IsTrained;
                newPlayer.Score = selectedPlayer.Score;
                newPlayer.Participation = selectedPlayer.Participation;
                newPlayer.Moves = new List<Move>();

                for (int i = 0; i < newPlayer.UInt32Moves.Count; i++)
                {
                    Move newMove = Moves[newPlayer.UInt32Moves[i]];
                    newMove.Level = 1;
                    newMove.UsedCount = newMove.EvolutionSpeed.TimeLevel[0];

                    if (selectedPlayer.Level < 99)
                    {
                        newMove.Unlock = false;
                    }
                    else
                    {
                        newMove.Unlock = true;
                    }

                    newPlayer.Moves.Add(newMove);
                }
            }

            return newPlayer;
        }

        public (int, int, string, bool) Training(Player player, int newStat, int statIndex)
        {
            if (player.InvestedFreedom.Sum() < player.Freedom)
            {
                player.InvestedFreedom[statIndex] = newStat;
                player.InvestedPoint[statIndex] = newStat;
                return (0, player.Freedom + 1, "investedNumericUpDown" + (statIndex + 3), true);
            }
            else
            {
                int statRemoveIndex = 0;

                if (player.Position.Name == "Forward")
                {
                    switch (statIndex)
                    {
                        case 0: // Kick
                            statRemoveIndex = 2;
                            break;
                        case 1: // Dribble
                            statRemoveIndex = 3;
                            break;
                        case 2: // Technique
                            statRemoveIndex = 0;
                            break;
                        case 3: // Block
                            statRemoveIndex = 1;
                            break;
                        case 4: // Speed
                            statRemoveIndex = 5;
                            break;
                        case 5: // Stamina
                            statRemoveIndex = 4;
                            break;
                        case 6: // Catch
                            statRemoveIndex = 7;
                            break;
                        case 7: // Luck
                            statRemoveIndex = 6;
                            break;

                    }
                }
                else if (player.Position.Name == "Midfielder")
                {
                    switch (statIndex)
                    {
                        case 0: // Kick
                            statRemoveIndex = 6;
                            break;
                        case 1: // Dribble
                            statRemoveIndex = 2;
                            break;
                        case 2: // Technique
                            statRemoveIndex = 1;
                            break;
                        case 3: // Block
                            statRemoveIndex = 7;
                            break;
                        case 4: // Speed
                            statRemoveIndex = 5;
                            break;
                        case 5: // Stamina
                            statRemoveIndex = 4;
                            break;
                        case 6: // Catch
                            statRemoveIndex = 0;
                            break;
                        case 7: // Luck
                            statRemoveIndex = 3;
                            break;

                    }
                }
                else if (player.Position.Name == "Defender")
                {
                    switch (statIndex)
                    {
                        case 0: // Kick
                            statRemoveIndex = 6;
                            break;
                        case 1: // Dribble
                            statRemoveIndex = 7;
                            break;
                        case 2: // Technique
                            statRemoveIndex = 3;
                            break;
                        case 3: // Block
                            statRemoveIndex = 2;
                            break;
                        case 4: // Speed
                            statRemoveIndex = 5;
                            break;
                        case 5: // Stamina
                            statRemoveIndex = 4;
                            break;
                        case 6: // Catch
                            statRemoveIndex = 0;
                            break;
                        case 7: // Luck
                            statRemoveIndex = 1;
                            break;

                    }
                }
                else
                {
                    switch (statIndex)
                    {
                        case 0: // Kick
                            statRemoveIndex = 7;
                            break;
                        case 1: // Dribble
                            statRemoveIndex = 3;
                            break;
                        case 2: // Technique
                            statRemoveIndex = 6;
                            break;
                        case 3: // Block
                            statRemoveIndex = 1;
                            break;
                        case 4: // Speed
                            statRemoveIndex = 5;
                            break;
                        case 5: // Stamina
                            statRemoveIndex = 4;
                            break;
                        case 6: // Catch
                            statRemoveIndex = 2;
                            break;
                        case 7: // Luck
                            statRemoveIndex = 0;
                            break;
                    }
                }

                if (newStat > player.InvestedPoint[statIndex])
                {
                    player.InvestedPoint[statRemoveIndex] -= 1;
                }
                else
                {
                    player.InvestedPoint[statRemoveIndex] += 1;
                }

                player.InvestedPoint[statIndex] = newStat;

                return (player.InvestedFreedom[statIndex], player.InvestedFreedom[statIndex] + player.Stat[statRemoveIndex + 2] - 1, "investedNumericUpDown" + (statRemoveIndex + 3), false);
            }
        }

        public void UpdateInventory()
        {
            // Find Equipment
            Equipments = Inventory.Where(x => Common.GO.Equipments.Cs.ContainsKey(x.Value.ID)).ToDictionary(x => x.Key, y => Common.GO.Equipments.Cs[y.Value.ID]);
            Equipments.Add(0, Common.GO.Equipments.Cs[0x0]);
        }

        public GO(Stream data)
        {
            Data = new BinaryDataReader(data);

            Inventory = GetInventory();
            Reserve = GetPlayers();
        }
    }
}
