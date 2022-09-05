using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;
using InazumaElevenSaveEditor.Tools;
using InazumaElevenSaveEditor.Logic;

namespace InazumaElevenSaveEditor.Formats.Games
{
    public class Galaxy : IGame
    {
        public DataReader File { get; set; }

        public string GameNameCode => "IEGOGALAXY";

        public int MaximumPlayer => 336;

        public IDictionary<UInt32, Player> Players { get; set; }

        public IDictionary<UInt32, Move> Moves { get; set; }

        public IDictionary<UInt32, Avatar> Avatars { get; set; }

        public IDictionary<UInt32, Equipment> Equipments { get; set; }

        public IDictionary<UInt32, Item> Items { get; set; }

        public Save SaveInfo { get; set; }

        public Dictionary<UInt32, Player> PlayersInSave { get; set; }

        public List<UInt32> PlayersInSaveSort { get; set; }

        public Dictionary<UInt32, Player> AuraInSave { get; set; }

        private IList<BestMatch> BestMatchs = Common.InazumaElevenGo.BestMatchs.Cs;

        public Galaxy()
        {

        }

        public Galaxy(DataReader file)
        {
            File = file;
        }

        public Player GetPlayer(int index)
        {
            return PlayersInSave[PlayersInSaveSort[index]];
        }

        public void ChangePlayer(Player selectedPlayer, KeyValuePair<UInt32, Player> newPlayer)
        {
            selectedPlayer.Name = newPlayer.Value.Name;
            selectedPlayer.ID = newPlayer.Key;
            selectedPlayer.Freedom = newPlayer.Value.Freedom;
            selectedPlayer.Element = newPlayer.Value.Element;
            selectedPlayer.Position = newPlayer.Value.Position;
            selectedPlayer.Gender = newPlayer.Value.Gender; ;
            selectedPlayer.Participation = 1;
            selectedPlayer.Score = 1;

            // Get New Player Stats
            for (int i = 0; i < selectedPlayer.Stat.Count; i++)
            {
                selectedPlayer.Stat[i] = newPlayer.Value.Stat[i];
            }

            // Get New Player Moves
            for (int i = 0; i < newPlayer.Value.UInt32Moves.Count; i++)
            {
                Move newMove = Moves[newPlayer.Value.UInt32Moves[i]];
                newMove.Level = 1;
                newMove.TimeLevel = newMove.EvolutionSpeed.TimeLevel[0];
                if (selectedPlayer.Level < 99)
                {
                    newMove.Unlock = false;
                }
                else
                {
                    newMove.Unlock = true;
                }
                selectedPlayer.Moves[i] = newMove;
            }
        }

        public void RecruitPlayer(KeyValuePair<UInt32, Player> playerKeyValuePair)
        {
            int getPlayerIndex(UInt32 x)
            {
                UInt16 uint16 = Convert.ToUInt16(x / 0x10000);
                uint16 = (UInt16)((uint16 & 0xFFU) << 8 | (uint16 & 0xFF00U) >> 8);
                return Convert.ToInt32(uint16);
            }

            var playerListIndex = PlayersInSaveSort.Select(x => getPlayerIndex(x)).ToList();
            playerListIndex.Sort();
            int missingIndex = Enumerable.Range(0, MaximumPlayer).Except(playerListIndex).First();

            Player newPlayer = new Player(playerKeyValuePair.Value);
            newPlayer.ID = playerKeyValuePair.Key;
            newPlayer.Style = 0;
            newPlayer.Level = 1;
            newPlayer.InvestedPoint = new List<int>(new int[8]);
            newPlayer.Avatar = Avatars[0x0];
            newPlayer.Invoke = false;
            newPlayer.Armed = false;

            newPlayer.Moves = new List<Move>();
            for (int i = 0; i < 4; i++)
            {
                newPlayer.Moves.Add(Moves[playerKeyValuePair.Value.UInt32Moves[i]]);
            }
            newPlayer.Moves.Add(Moves[0x00]);
            newPlayer.Moves.Add(Moves[0x00]);

            newPlayer.Equipments = new List<Equipment>();
            for (int i = 0; i < 4; i++)
            {
                newPlayer.Equipments.Add(Equipments[(uint)i]);
            }

            UInt32 newIndex = (uint)missingIndex * 0x1000000 + (uint)(missingIndex + 1) * 0x100;
            PlayersInSaveSort.Add(newIndex);
            newPlayer.PositionInFile = 23324 + 260 * PlayersInSaveSort.Count;
            PlayersInSave.Add(newIndex, newPlayer);
        }

        public void Open()
        {
            // Initialize Resources
            Players = Common.InazumaElevenGo.Players.Galaxy;
            Moves = Common.InazumaElevenGo.Moves.Galaxy;
            Avatars = Common.InazumaElevenGo.Avatars.Galaxy;
            Items = Common.InazumaElevenGo.Items.Galaxy;

            IDictionary<uint, Equipment> AllEquipments = Common.InazumaElevenGo.Equipments.Galaxy;
            Equipments = new Dictionary<UInt32, Equipment>();

            SaveInfo = new Save();

            // First Item Category - 608 maximum items
            File.Seek(0xA394);
            for (int i = 0; i < 608; i++)
            {
                UInt32 itemPositionID = File.Reverse(File.ReadUInt32());

                if (itemPositionID == 0x0)
                {
                    break;
                }
                else
                {
                    Item newItem = Items[File.Reverse(File.ReadUInt32())];
                    newItem.Quantity = File.ReadByte();
                    SaveInfo.Inventory.Add(itemPositionID, newItem);
                    File.Skip(3);
                }
            }

            // Second Item Category - 452 maximum items
            File.Seek(0xC020);
            for (int i = 0; i < 452; i++)
            {
                UInt32 itemPositionID = File.Reverse(File.ReadUInt32());

                if (itemPositionID == 0x0)
                {
                    break;
                }
                else
                {
                    UInt32 itemID = File.Reverse(File.ReadUInt32());

                    Item newItem = Items[itemID];
                    newItem.Quantity = File.ReadByte();

                    // Fills The Equipment List
                    if (AllEquipments.ContainsKey(itemID))
                    {
                        Equipment newEquipment = AllEquipments[itemID];
                        newEquipment.ID = itemID;
                        Equipments.Add(itemPositionID, newEquipment);
                    }

                    SaveInfo.Inventory.Add(itemPositionID, newItem);
                    File.Skip(7);
                }
            }

            // Third Item Category - 888 Maximum Items
            File.Seek(0xDC6C);
            for (int i = 0; i < 888; i++)
            {
                UInt32 itemPositionID = File.Reverse(File.ReadUInt32());

                if (itemPositionID == 0x0)
                {
                    break;
                }
                else
                {
                    UInt32 itemID = File.Reverse(File.ReadUInt32());

                    Item newItem = Items[itemID];
                    newItem.Quantity = 1;

                    SaveInfo.Inventory.Add(itemPositionID, newItem);
                }
            }

            // Add Empty Equipment
            for (int i = 0; i < 4; i++)
            {
                Equipment emptyEquiment = AllEquipments[(uint)i];
                emptyEquiment.ID = 0x00000000;
                Equipments.Add((uint)i, emptyEquiment);
            }

            // Get Player List
            File.Seek(0x26E28);
            PlayersInSaveSort = new List<UInt32>();
            for (int i = 0; i < 336; i++)
            {
                UInt32 playerPositionID = File.Reverse(File.ReadUInt32());
                if (playerPositionID != 0x0)
                {
                    PlayersInSaveSort.Add(playerPositionID);
                }
            }

            // Create Player Dic
            File.Seek(0xF840);
            PlayersInSave = new Dictionary<uint, Player>();
            for (int i = 0; i < PlayersInSaveSort.Count; i++)
            {
                PlayersInSave.Add(File.Reverse(File.ReadUInt32()), LoadPlayer());
            }

            // Link MixiMax
            foreach (KeyValuePair<UInt32, Player> player in PlayersInSave)
            {
                if (player.Value.MixiMax != null)
                {
                    File.Seek((uint)player.Value.PositionInFile);
                    File.Skip(8);
                    UInt32 playerAura = File.Reverse(File.ReadUInt32());

                    File.Skip(0x14);
                    int mixiMaxMove1 = File.ReadByte();
                    int mixiMaxMove2 = File.ReadByte();

                    BestMatch isBestMatch = IsBestMatch(player.Value.ID, PlayersInSave[playerAura].ID);
                    if (isBestMatch != null)
                    {
                        player.Value.MixiMax = new MixiMax(PlayersInSave[playerAura], (mixiMaxMove1, mixiMaxMove2), isBestMatch);
                    }
                    else
                    {
                        player.Value.MixiMax = new MixiMax(PlayersInSave[playerAura], (mixiMaxMove1, mixiMaxMove2));
                    }
                }
            }
        }

        public void OpenTactics()
        {

        }

        public void OpenSaveInfo()
        {
            if (SaveInfo.Name == null)
            {
                // Name
                File.Seek(0);
                SaveInfo.Name = Encoding.UTF8.GetString(File.GetSection(60, 32).TakeWhile((x, index) => x != 0x0 && index < 32).ToArray());
                SaveInfo.TeamName = Encoding.UTF8.GetString(File.GetSection(92, 32).TakeWhile((x, index) => x != 0x0 && index < 32).ToArray());

                // Time Hours
                File.Seek(35);
                int seconds = (int)File.ReadUInt32();
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
                File.Seek(37044);
                SaveInfo.SecretLinkLevel = File.ReadByte();

                // Current Chapter
                File.Seek(0x9F1C);
                SaveInfo.Chapter = File.ReadByte();

                // Money
                File.Seek(0x268D0);
                SaveInfo.Prestige = File.ReadInt32();
                SaveInfo.Friendship = 0;

                // Coins
                File.Seek(0x26CC8);
                SaveInfo.Coins = new List<int>();
                for (int i = 0; i < 5; i++)
                {
                    SaveInfo.Coins.Add(File.ReadInt16());
                }
            }
        }

        public void OpenPlayRecords()
        {
            if (SaveInfo.PlayRecords == null)
            {
                SaveInfo.PlayRecords = Common.InazumaElevenGo.PlayRecords.Galaxy;

                File.Seek(0x8FEB);

                foreach (KeyValuePair<int, List<PlayRecord>> playRecord in SaveInfo.PlayRecords)
                {
                    BitArray bits = new BitArray(new int[] { File.ReadByte() });

                    for (int i = 0; i < playRecord.Value.Count; i++)
                    {
                        playRecord.Value[playRecord.Value.Count - 1 - i].Unlocked = bits[i];
                    }
                }
            }
        }

        public void Save(OpenFileDialog initialDirectory)
        {

        }

        public void UpdateResource()
        {
            IDictionary<uint, Equipment> AllEquipments = Common.InazumaElevenGo.Equipments.Cs;
            Equipments = new Dictionary<UInt32, Equipment>();
            IDictionary<uint, Player> Auras = Common.InazumaElevenGo.Auras.Cs;
            AuraInSave = new Dictionary<UInt32, Player>();

            foreach (KeyValuePair<UInt32, Item> item in SaveInfo.Inventory)
            {
                if (AllEquipments.Values.Any(x => x.Name == item.Value.Name) == true)
                {
                    // Console.WriteLine(item.Key.ToString("X8") + " | " + item.Value.Name);
                    var equipmentKeyValuePair = AllEquipments.FirstOrDefault(x => x.Value.Name == item.Value.Name);
                    Equipment newEquipment = equipmentKeyValuePair.Value;
                    newEquipment.ID = equipmentKeyValuePair.Key;
                    Equipments.Add(item.Key, newEquipment);
                }
                else if (AllEquipments.Values.Any(x => x.Name == item.Value.Name) == true)
                {
                    var auraKeyValuePair = Auras.FirstOrDefault(x => x.Value.Name == item.Value.Name);
                    Player newAura = auraKeyValuePair.Value;
                    newAura.ID = auraKeyValuePair.Key;
                    newAura.Moves = new List<Move>();
                    foreach (UInt32 move in auraKeyValuePair.Value.UInt32Moves)
                    {
                        newAura.Moves.Add(Moves[move]);
                    }
                    AuraInSave.Add(item.Key, newAura);
                }
            }

            // Add Empty Equipment
            for (int i = 0; i < 4; i++)
            {
                Equipment emptyEquiment = AllEquipments[(uint)i];
                emptyEquiment.ID = 0x00000000;
                Equipments.Add((uint)i, emptyEquiment);
            }
        }

        private Player LoadPlayer()
        {
            File.Seek((uint)File.BaseStream.Position - 8);

            Avatar newAvatar = null;
            UInt32 totemID = File.Reverse(File.ReadUInt32());
            if (totemID != 0x0)
            {
                newAvatar = Avatars[totemID];
            }

            File.Skip(4);
            UInt32 playerID = File.Reverse(File.ReadUInt32());
            Player newPlayer = new Player(Players[playerID]);
            newPlayer.ID = playerID;
            newPlayer.PositionInFile = File.BaseStream.Position - 8;

            UInt32 miximaxPositionID = File.Reverse(File.ReadUInt32());
            if (miximaxPositionID != 0x0)
            {
                newPlayer.MixiMax = new MixiMax();
            }

            File.Skip(0x0C);
            newPlayer.Stat[0] = File.ReadInt16();
            newPlayer.Stat[1] = File.ReadInt16();

            File.Skip(2);
            newPlayer.Level = File.ReadByte();
            if (newPlayer.Level > 99)
            {
                newPlayer.Level = 99;
            }

            File.Skip(3);
            int canInvokeArmed = File.ReadByte();
            bool invoke = (canInvokeArmed & 8) != 0;
            bool armed = (canInvokeArmed & 16) != 0;

            newPlayer.Style = (File.ReadByte() & 0xF0) >> 4;
            newPlayer.Participation = File.ReadUInt16();
            newPlayer.Score = File.ReadUInt16();

            // Won Duels
            File.Skip(2);

            // Unknown
            File.Skip(2);

            File.Skip(4);
            newPlayer.InvestedPoint = new List<int>();
            for (int i = 0; i < 8; i++)
            {
                int investedPoint = File.ReadInt16();
                newPlayer.InvestedPoint.Add(investedPoint);
            }

            if (newAvatar == null)
            {
                newAvatar = Avatars[File.Reverse(File.ReadUInt32())];
            } else
            {
                File.Skip(4);
            }
            newPlayer.Avatar = newAvatar;
            newPlayer.Invoke = invoke;
            newPlayer.Armed = armed;
            newPlayer.Avatar.Level = File.ReadByte();

            File.Skip(3);
            newPlayer.Equipments = new List<Equipment>();
            for (int i = 0; i < 4; i++)
            {
                Equipment newEquipment = null;
                UInt32 equipmentPositionID = File.Reverse(File.ReadUInt32());
                if (equipmentPositionID != 0x0)
                {
                    newEquipment = Equipments[equipmentPositionID];
                }
                else
                {
                    newEquipment = Equipments[(uint)i];
                }
                newPlayer.Equipments.Add(newEquipment);
            }

            File.Skip(4);
            newPlayer.Moves = new List<Move>();
            for (int i = 0; i < 6; i++)
            {
                UInt32 moveID = File.Reverse(File.ReadUInt32());

                Move newMove = Moves[0x00000000];
                if (Moves.ContainsKey(moveID))
                {
                    newMove = new Move(Moves[moveID]);
                }

                newMove.Level = File.ReadByte();
                newMove.TimeLevel = File.ReadByte();
                newMove.Unlock = Convert.ToBoolean(File.ReadByte());
                newPlayer.Moves.Add(newMove);
                File.Skip(5);
            }

            File.Skip(104);

            return newPlayer;
        }

        private Team LoadTeam(uint teamInfo, uint teamName, uint teamPlayers)
        {
            return null;
        }

        private BestMatch IsBestMatch(UInt32 tryPlayerID, UInt32 tryAuraID)
        {
            List<BestMatch> compatibleBestMatchs = BestMatchs.Where(x => x.PlayerID == tryPlayerID).ToList();

            if (compatibleBestMatchs.Count != -1)
            {
                BestMatch bestMatch = compatibleBestMatchs.FirstOrDefault(x => x.PlayerID == tryPlayerID && x.AuraID == tryAuraID);

                // Player who can perform a miximax with everyone
                if (bestMatch == null)
                {
                    bestMatch = compatibleBestMatchs.FirstOrDefault(x => x.PlayerID == tryPlayerID && x.AuraID == 0x0);
                }

                return bestMatch;
            }
            else
            {
                return null;
            }
        }

        public (int, int, string, bool) Training(Player player, int newStat, int statIndex)
        {
            player.InvestedFreedom[statIndex] = newStat;

            if (player.InvestedFreedom.Sum() <= player.Freedom)
            {
                player.InvestedPoint[statIndex] = newStat;
                return (0, player.Freedom, "investedNumericUpDown" + (statIndex + 3), true);
            } else
            {
                return (0, player.InvestedPoint[statIndex], "investedNumericUpDown" + (statIndex + 3), true);
            }
        }
    }
}
