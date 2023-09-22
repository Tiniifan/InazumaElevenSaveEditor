using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using InazumaElevenSaveEditor.Tools;
using InazumaElevenSaveEditor.InazumaEleven.Logic;

namespace InazumaElevenSaveEditor.InazumaEleven.Games.IEGOCS
{
    public class CS : IGame
    {
        public string Code => "IEGOCS";

        public string Name => "Inazuma Eleven Go Chrono Stone";

        public int MaximumPlayer => 336;

        public BinaryDataReader Data { get; set; }

        public List<Player> Reserve { get; set; }

        public Dictionary<int, Player> Auras { get; set; }

        public Dictionary<int, Equipment> Equipments { get; set; }

        public Dictionary<int, Item> Inventory { get; set; }

        public Dictionary<int, List<PlayRecord>> PlayRecords { get; set; }

        public SaveInfo SaveInfo { get; set; }

        public List<Team> Teams { get; set; }

        public Dictionary<uint, Player> Players => Common.GO.Players.Cs;

        public Dictionary<uint, Move> Moves => Common.GO.Moves.Cs;

        public Dictionary<uint, Avatar> Avatars => Common.GO.Avatars.Cs;

        public Dictionary<uint, Item> Items => Common.GO.Items.Cs;

        private Dictionary<int, Item> GetInventory()
        {
            // Group Item 1
            Data.Seek((uint)IEGOCSHelper.ItemBlockGroup1Offset);
            Dictionary<int, Item> itemBlockGroup1 = Data.ReadMultipleStruct<IEGOCSHelper.ItemBlockGroup1>(512).Where(x => Items.ContainsKey(x.ID) && x.ID != 0x00).ToDictionary(x => x.Index, y => new Item(y.ID, Common.GO.Items.Cs[y.ID], y.Quantity));

            // Group Item 2
            Data.Seek((uint)IEGOCSHelper.ItemBlockGroup2Offset);
            Dictionary<int, Item> itemBlockGroup2 = Data.ReadMultipleStruct<IEGOCSHelper.ItemBlockGroup2>(336).Where(x => Items.ContainsKey(x.ID) && x.ID != 0x00).ToDictionary(x => x.Index, y => new Item(y.ID, Common.GO.Items.Cs[y.ID], y.Quantity, y.QuantityEquiped));

            // Group Item 3
            Data.Seek((uint)IEGOCSHelper.ItemBlockGroup3Offset);
            Dictionary<int, Item> itemBlockGroup3 = Data.ReadMultipleStruct<IEGOCSHelper.ItemBlockGroup3>(800).Where(x => Items.ContainsKey(x.ID) && x.ID != 0x00).ToDictionary(x => x.Index, y => new Item(y.ID, Common.GO.Items.Cs[y.ID]));

            // Find Equipment
            Equipments = itemBlockGroup2.Where(x => Common.GO.Equipments.Cs.ContainsKey(x.Value.ID)).ToDictionary(x => x.Key, y => Common.GO.Equipments.Cs[y.Value.ID]);
            Equipments.Add(0, Common.GO.Equipments.Cs[0x0]);

            // Find Aura
            Auras = itemBlockGroup3.Where(x => Common.GO.Auras.Cs.ContainsKey(x.Value.ID)).ToDictionary(x => x.Key, y => Common.GO.Auras.Cs[y.Value.ID]);

            // Merge
            return itemBlockGroup1.Union(itemBlockGroup2).Union(itemBlockGroup3).ToDictionary(x => x.Key, x => x.Value);
        }

        private void SaveInventory(BinaryDataWriter writer)
        {
            // Group Item 1
            writer.Seek((uint)IEGOCSHelper.ItemBlockGroup1Offset);
            Dictionary<int, Item> itemGroup1 = Inventory.Where(x => x.Value.Category == 1).ToDictionary(x => x.Key, y => y.Value);
            for (int i =0; i < 512; i++)
            {
                if (i < itemGroup1.Count)
                {
                    KeyValuePair<int, Item> item = itemGroup1.ElementAt(i);
                    writer.Write(item.Key);
                    writer.Write(item.Value.ID);
                    writer.Write(item.Value.Quantity);
                } else
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
            writer.Seek((uint)IEGOCSHelper.ItemBlockGroup2Offset);
            Dictionary<int, Item> itemGroup2 = Inventory.Where(x => x.Value.Category == 2).ToDictionary(x => x.Key, y => y.Value);
            for (int i = 0; i < 336; i++)
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
            writer.Seek((uint)IEGOCSHelper.ItemBlockGroup3Offset);
            Dictionary<int, Item> itemGroup3 = Inventory.Where(x => x.Value.Category == 3).ToDictionary(x => x.Key, y => y.Value);
            for (int i = 0; i < 800; i++)
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
            Data.Seek((uint)IEGOCSHelper.PlayerIndexOffset);
            int[] index = Data.ReadMultipleStruct<int>(IEGOCSHelper.MaximumPlayer)
                .Where(x => x != 0x0)
                .ToArray();

            // Get player data according to their index
            Data.Seek((uint)IEGOCSHelper.PlayerDataOffset);
            List<IEGOCSHelper.PlayerBlock> playerBlocks = Data.ReadMultipleStruct<IEGOCSHelper.PlayerBlock>(index.Length)
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
                    player.Invoke = (playerBlock.InvokeStatus & 8) != 0;
                    player.Armed = (playerBlock.InvokeStatus & 16) != 0;

                    player.Equipments = new List<Equipment>
                    {
                        Equipments[playerBlock.BootsIndex],
                        Equipments[playerBlock.BraceletIndex],
                        Equipments[playerBlock.PendantIndex],
                        Equipments[playerBlock.GlovesIndex]
                    };

                    if (playerBlock.MiximaxIndex != 0x00)
                    {
                        player.MixiMax = new MixiMax();
                    }

                    player.Index = playerBlock.Index;
                    player.Style = (playerBlock.Style & 0xF0) >> 4;
                    player.Participation = playerBlock.ParticipationPoint;
                    player.Score = playerBlock.ScorePoint;

                    return player;
                })
                .ToList();

            // Link Miximax
            foreach (Player player in players.Where(p => p.MixiMax != null))
            {
                int miximaxIndex = playerBlocks.First(x => x.Index == player.Index).MiximaxIndex;
                int miximaxMove1 = playerBlocks.First(x => x.Index == player.Index).MixiMaxMove1Index;
                int miximaxMove2 = playerBlocks.First(x => x.Index == player.Index).MixiMaxMove2Index;

                if (players.Any(x => x.Index == miximaxIndex))
                {
                    // The aura is a Player
                    BestMatch isBestMatch = IsBestMatch(player.ID, players.Find(x => x.Index == miximaxIndex).ID);

                    if (isBestMatch != null)
                    {
                        player.MixiMax = new MixiMax(players.Find(x => x.Index == miximaxIndex), (miximaxMove1, miximaxMove2), isBestMatch);
                    }
                    else
                    {
                        player.MixiMax = new MixiMax(players.Find(x => x.Index == miximaxIndex), (miximaxMove1, miximaxMove2));
                    }

                    players.Find(x => x.Index == miximaxIndex).IsAura = true;
                }
                else
                {
                    // The aura is a Item
                    BestMatch isBestMatch = IsBestMatch(player.ID, Auras[miximaxIndex].ID);
                    MixiMax newMixiMax;

                    if (isBestMatch != null)
                    {
                        newMixiMax = new MixiMax(Auras[miximaxIndex], (miximaxMove1, miximaxMove2), isBestMatch);
                    }
                    else
                    {
                        newMixiMax = new MixiMax(Auras[miximaxIndex], (miximaxMove1, miximaxMove2));
                    }

                    newMixiMax.AuraData = true;
                    player.MixiMax = newMixiMax;
                    Auras[miximaxIndex].IsAura = true;
                }
            }

            // Sort
            return players.OrderBy(player => Array.IndexOf(index, player.Index)).ToList();
        }

        private void SavePlayers(BinaryDataWriter writer)
        {
            // Save player index
            writer.Seek((uint)IEGOCSHelper.PlayerIndexOffset);
            for (int i = 0; i < IEGOCSHelper.MaximumPlayer; i++)
            {
                if (i < Reserve.Count)
                {
                    writer.Write(Reserve[i].Index);
                } else
                {
                    writer.Write(0x0);
                }               
            }

            // Save player data
            writer.Seek((uint)IEGOCSHelper.PlayerDataOffset);
            List<Player> sortedReserve = Reserve.OrderBy(player => player.Index).ToList();
            for (int i = 0; i < IEGOCSHelper.MaximumPlayer; i++)
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
                    writer.Skip(0x01);
                    
                    if (player.MixiMax != null)
                    {
                        writer.Write(player.MixiMax.AuraPlayer.Index);
                        writer.Skip(0x08);
                        writer.Write((byte)player.MixiMax.MixiMaxMoveNumber[0]);
                        writer.Write((byte)player.MixiMax.MixiMaxMoveNumber[1]);
                    } else
                    {
                        writer.Write(0x00);
                        writer.Skip(0x08);
                        writer.Write((byte)0x00);
                        writer.Write((byte)0x00);
                    }

                    // Determines the Invoker value and saves it
                    int canInvokeArmed = Convert.ToInt32(player.Invoke) * 8 + Convert.ToInt32(player.Armed) * 16;
                    if (player.MixiMax != null && player.MixiMax.AuraData == true) canInvokeArmed += 1;
                    var playerIsAura = Reserve.FirstOrDefault(x => x.MixiMax != null && x.MixiMax.AuraPlayer == player);
                    if (playerIsAura != null) canInvokeArmed += 2;
                    writer.Write((byte)canInvokeArmed);

                    writer.Write((byte) ((player.Style << 4) & 0xF0));
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

                    writer.Skip(0x64);
                }
                else
                {
                    int[] emptyPlayerData = Enumerable.Repeat(0, 65).ToArray();
                    writer.Write(emptyPlayerData.SelectMany(x => BitConverter.GetBytes(x)).ToArray());
                }
            }
        }

        public void OpenPlayRecords()
        {
            if (PlayRecords == null)
            {
                PlayRecords = Common.GO.PlayRecords.Cs;

                Data.Seek((uint)IEGOCSHelper.PlayRecordsOffset);

                foreach (KeyValuePair<int, List<PlayRecord>> playRecord in PlayRecords)
                {
                    BitArray bits = new BitArray(new int[] { Data.ReadValue<byte>() });

                    for (int i = 0; i < playRecord.Value.Count; i++)
                    {
                        playRecord.Value[playRecord.Value.Count - 1 - i].Unlocked = bits[i];
                    }
                }
            }
        }

        public void SavePlayRecords(BinaryDataWriter writer)
        {
            if (PlayRecords == null) return;

            writer.Seek((uint)IEGOCSHelper.PlayRecordsOffset);

            foreach (KeyValuePair<int, List<PlayRecord>> playRecord in PlayRecords)
            {
                List<bool> listBool = playRecord.Value.Select(x => x.Unlocked).ToList();

                while (listBool.Count < 8)
                {
                    listBool.Add(true);
                }

                BitArray bits = new BitArray(listBool.ToArray());
                byte[] bytes = new byte[1];
                bits.CopyTo(bytes, 0);
                writer.Write((byte)bytes[0]);
            }
        }

        public void OpenSaveInfo()
        {
            if (SaveInfo == null)
            {
                SaveInfo = new SaveInfo();

                // Name
                Data.Seek((uint)IEGOCSHelper.NameOffset);
                SaveInfo.Name = Data.ReadString(Encoding.UTF8);
                Data.Seek((uint)IEGOCSHelper.TeamNameOffset);
                SaveInfo.TeamName = Data.ReadString(Encoding.UTF8);

                // Time Hours
                Data.Seek((uint) IEGOCSHelper.TimeOffset);
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
                Data.Seek((uint)IEGOCSHelper.LinkOffset);
                SaveInfo.SecretLinkLevel = Data.ReadValue<byte>();

                // Current Chapter
                Data.Seek((uint)IEGOCSHelper.ChapterOffset);
                SaveInfo.Chapter = Data.ReadValue<byte>();

                // Money
                Data.Seek((uint)IEGOCSHelper.MoneyOffset);
                SaveInfo.Prestige = Data.ReadValue<int>();
                SaveInfo.Friendship = Data.ReadValue<int>();
            }
        }

        private void SaveSaveInfo(BinaryDataWriter writer)
        {
            if (SaveInfo == null) return;

            // Save Name
            writer.Seek((uint)IEGOCSHelper.NameOffset);
            writer.Write(Encoding.UTF8.GetBytes(SaveInfo.Name));
            writer.Write((byte)0x0);
            writer.Write((byte)0x88);

            // Team Name
            writer.Seek((uint)IEGOCSHelper.TeamNameOffset);
            writer.Write(Encoding.UTF8.GetBytes(SaveInfo.TeamName));
            writer.Write((byte)0x0);
            writer.Write((byte)0x88);

            // Time Hours
            writer.Seek((uint)IEGOCSHelper.TimeOffset);
            int time = SaveInfo.Hours * 3600 + SaveInfo.Min * 60;
            writer.Write(time);

            // Link Level
            writer.Seek((uint)IEGOCSHelper.LinkOffset);
            writer.Write((byte)SaveInfo.SecretLinkLevel);

            // Money
            writer.Seek((uint)IEGOCSHelper.MoneyOffset);
            writer.Write(SaveInfo.Prestige);
            writer.Write(SaveInfo.Friendship);

            if (SaveInfo.UnlockAllData == true)
            {
                writer.Seek(0x2FB);
                writer.Write(new byte[6] { 0xFF, 0xFF, 0x01, 0x00, 0xFE, 0x3F });

                writer.Seek(0x305);
                writer.Write(new byte[6] { 0xE0, 0xF8, 0xFF, 0xFF, 0x0F, 0xFF });

                writer.Seek(0x322);
                writer.Write((byte)0x18);

                writer.Seek(0x32F);
                writer.Write(new byte[9] { 0xF8, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F, 0x50, 0x02 });

                writer.Seek(0x339);
                writer.Write((byte)0x78);

                // Ugly Write
                writer.Seek(0x24A40);
                writer.Write(new byte[864] { 0xFE, 0xFF, 0x76, 0x08, 0x0D, 0x04, 0x04, 0x00, 0x6B, 0x00, 0x00, 0x00, 0xE4, 0xFF, 0xFF, 0xFF, 0x91, 0x07, 0xD4, 0x64, 0xE4, 0xFF, 0xFF, 0xFF, 0x1F, 0x55, 0x33, 0x42, 0xE0, 0xFF, 0xFF, 0xFF, 0xB2, 0xF0, 0x63, 0xAB, 0xE0, 0xFF, 0xFF, 0xFF, 0xCA, 0x40, 0x78, 0xBB, 0xE0, 0xFF, 0xFF, 0xFF, 0x60, 0x7F, 0x23, 0xBD, 0xE1, 0xFF, 0xFF, 0xFF, 0x7D, 0xB2, 0xA9, 0xC7, 0xE1, 0xFF, 0xFF, 0xFF, 0x83, 0xBF, 0xC5, 0x53, 0xE1, 0xFF, 0xFF, 0xFF, 0x1D, 0x97, 0xC2, 0x1E, 0xE1, 0xFF, 0xFF, 0xFF, 0x1A, 0xE3, 0xBB, 0x29, 0xE1, 0xFF, 0xFF, 0xFF, 0x53, 0xCA, 0xA0, 0x0A, 0xE1, 0xFF, 0xFF, 0xFF, 0x0E, 0x84, 0x0E, 0xCB, 0xE2, 0xFF, 0xFF, 0xFF, 0xC4, 0xB4, 0x8A, 0x81, 0xE0, 0xFF, 0xFF, 0xFF, 0x75, 0xAA, 0xA9, 0xFD, 0xE0, 0xFF, 0xFF, 0xFF, 0x74, 0x1B, 0xA6, 0x39, 0xE0, 0xFF, 0xFF, 0xFF, 0xE2, 0x2B, 0xA1, 0x4E, 0xE1, 0xFF, 0xFF, 0xFF, 0xD7, 0x18, 0xB8, 0x08, 0xE1, 0xFF, 0xFF, 0xFF, 0xF6, 0x39, 0x4C, 0x6F, 0xE1, 0xFF, 0xFF, 0xFF, 0x6D, 0x54, 0x0F, 0x78, 0xE1, 0xFF, 0xFF, 0xFF, 0x3A, 0x06, 0x18, 0x49, 0xE1, 0xFF, 0xFF, 0xFF, 0xE5, 0x90, 0x63, 0x06, 0xE1, 0xFF, 0xFF, 0xFF, 0x35, 0xD0, 0x82, 0xF8, 0xE4, 0xFF, 0xFF, 0xFF, 0x0A, 0x1F, 0x24, 0x90, 0xE0, 0xFF, 0xFF, 0xFF, 0x58, 0x7A, 0xA8, 0xD7, 0xE0, 0xFF, 0xFF, 0xFF, 0xBD, 0x09, 0x50, 0x6F, 0xE0, 0xFF, 0xFF, 0xFF, 0xAF, 0x3D, 0xE9, 0xD1, 0xE2, 0xFF, 0xFF, 0xFF, 0x7E, 0xE5, 0x83, 0x18, 0xE0, 0xFF, 0xFF, 0xFF, 0x3D, 0xC8, 0x82, 0xC2, 0xE0, 0xFF, 0xFF, 0xFF, 0xE4, 0xB7, 0x16, 0x6D, 0xE1, 0xFF, 0xFF, 0xFF, 0x0A, 0x74, 0xE6, 0x08, 0xE1, 0xFF, 0xFF, 0xFF, 0x0C, 0xA8, 0x8D, 0x4F, 0xE1, 0xFF, 0xFF, 0xFF, 0x63, 0x37, 0x39, 0x50, 0xE1, 0xFF, 0xFF, 0xFF, 0x79, 0x94, 0xE7, 0xCF, 0xE1, 0xFF, 0xFF, 0xFF, 0x40, 0x80, 0xAD, 0x0F, 0xE1, 0xFF, 0xFF, 0xFF, 0x60, 0x50, 0x8A, 0xC8, 0xE0, 0xFF, 0xFF, 0xFF, 0x91, 0x04, 0x1B, 0xF6, 0xE0, 0xFF, 0xFF, 0xFF, 0x7A, 0x9F, 0x11, 0x20, 0xE0, 0xFF, 0xFF, 0xFF, 0x37, 0xE1, 0xBF, 0x95, 0xE0, 0xFF, 0xFF, 0xFF, 0xCE, 0x4A, 0xAF, 0xA0, 0xE1, 0xFF, 0xFF, 0xFF, 0xE9, 0xAE, 0x2D, 0x34, 0xE1, 0xFF, 0xFF, 0xFF, 0x44, 0x45, 0xC1, 0x6B, 0xE1, 0xFF, 0xFF, 0xFF, 0x86, 0xD2, 0xBB, 0x47, 0xE1, 0xFF, 0xFF, 0xFF, 0xD0, 0x35, 0x00, 0xEF, 0xE1, 0xFF, 0xFF, 0xFF, 0x15, 0x6C, 0xE0, 0x48, 0xE1, 0xFF, 0xFF, 0xFF, 0x77, 0x65, 0x08, 0x15, 0xE4, 0xFF, 0xFF, 0xFF, 0x22, 0xBB, 0x46, 0x1A, 0xE0, 0xFF, 0xFF, 0xFF, 0xD7, 0x74, 0xFD, 0x7F, 0xE0, 0xFF, 0xFF, 0xFF, 0xBA, 0xF2, 0x4E, 0x43, 0xE0, 0xFF, 0xFF, 0xFF, 0xD7, 0x8E, 0xC2, 0xA7, 0xE4, 0xFF, 0xFF, 0xFF, 0x79, 0xA6, 0xE4, 0x87, 0xE4, 0xFF, 0xFF, 0xFF, 0x33, 0x34, 0x3D, 0xAC, 0xE4, 0xFF, 0xFF, 0xFF, 0xCE, 0x9E, 0x22, 0xAA, 0xE1, 0xFF, 0xFF, 0xFF, 0xE5, 0x46, 0xC5, 0xCD, 0xE1, 0xFF, 0xFF, 0xFF, 0x38, 0x46, 0xDE, 0xBA, 0xE1, 0xFF, 0xFF, 0xFF, 0x84, 0xFE, 0x38, 0xC3, 0xE1, 0xFF, 0xFF, 0xFF, 0x6E, 0x77, 0xC3, 0xD9, 0xE1, 0xFF, 0xFF, 0xFF, 0xE4, 0x49, 0x8A, 0xE8, 0xE1, 0xFF, 0xFF, 0xFF, 0x6E, 0x42, 0x47, 0x7E, 0xE0, 0xFF, 0xFF, 0xFF, 0xAE, 0x8C, 0xE6, 0x15, 0xE0, 0xFF, 0xFF, 0xFF, 0x5F, 0x57, 0x10, 0x30, 0xE0, 0xFF, 0xFF, 0xFF, 0xC9, 0x67, 0x17, 0x47, 0xE1, 0xFF, 0xFF, 0xFF, 0xE8, 0x6A, 0x7A, 0x33, 0xE1, 0xFF, 0xFF, 0xFF, 0xE4, 0x38, 0x71, 0x76, 0xE1, 0xFF, 0xFF, 0xFF, 0xA0, 0x15, 0xEF, 0xE5, 0xE1, 0xFF, 0xFF, 0xFF, 0x63, 0xB8, 0x5E, 0x4B, 0xE1, 0xFF, 0xFF, 0xFF, 0x77, 0xEA, 0x6F, 0x0E, 0xE1, 0xFF, 0xFF, 0xFF, 0x70, 0x24, 0xF5, 0x85, 0xE4, 0xFF, 0xFF, 0xFF, 0x0C, 0x14, 0xE6, 0x6F, 0xE4, 0xFF, 0xFF, 0xFF, 0xB4, 0x35, 0xEA, 0xAC, 0xE4, 0xFF, 0xFF, 0xFF, 0x90, 0xA1, 0x59, 0x32, 0xE1, 0xFF, 0xFF, 0xFF, 0x22, 0xE5, 0x00, 0x25, 0xE1, 0xFF, 0xFF, 0xFF, 0x41, 0x44, 0xFA, 0x08, 0xE1, 0xFF, 0xFF, 0xFF, 0x6E, 0xCD, 0x20, 0x65, 0xE1, 0xFF, 0xFF, 0xFF, 0xA4, 0x42, 0x5A, 0x73, 0xE1, 0xFF, 0xFF, 0xFF, 0xFB, 0x64, 0x08, 0x0F, 0xE1, 0xFF, 0xFF, 0xFF, 0x92, 0x63, 0xA8, 0x6E, 0xE0, 0xFF, 0xFF, 0xFF, 0x74, 0x37, 0x3F, 0x2A, 0xE0, 0xFF, 0xFF, 0xFF, 0xCE, 0x66, 0x36, 0xB3, 0xE0, 0xFF, 0xFF, 0xFF, 0x58, 0x56, 0x31, 0xC4, 0xE0, 0xFF, 0xFF, 0xFF, 0xC9, 0x4B, 0x8E, 0x54, 0xE0, 0xFF, 0xFF, 0xFF, 0x14, 0xDD, 0xEF, 0x8C, 0xE0, 0xFF, 0xFF, 0xFF, 0xB7, 0x48, 0x8B, 0x12, 0xE1, 0xFF, 0xFF, 0xFF, 0x07, 0xD7, 0x3E, 0xED, 0xE1, 0xFF, 0xFF, 0xFF, 0x7B, 0x37, 0x03, 0x50, 0xE1, 0xFF, 0xFF, 0xFF, 0x35, 0x6A, 0x61, 0x44, 0xE1, 0xFF, 0xFF, 0xFF, 0xE1, 0x83, 0xA9, 0xA9, 0xE1, 0xFF, 0xFF, 0xFF, 0xFB, 0xF6, 0xD1, 0xFD, 0xE1, 0xFF, 0xFF, 0xFF, 0xD4, 0x13, 0x4E, 0xE7, 0xE4, 0xFF, 0xFF, 0xFF, 0x26, 0x7E, 0x2A, 0x7E, 0xE4, 0xFF, 0xFF, 0xFF, 0x73, 0x1D, 0x00, 0x22, 0xE4, 0xFF, 0xFF, 0xFF, 0xF1, 0x7F, 0x36, 0x10, 0xE4, 0xFF, 0xFF, 0xFF, 0x88, 0x7D, 0x12, 0xA2, 0xE1, 0xFF, 0xFF, 0xFF, 0x48, 0x98, 0xAD, 0x35, 0xE1, 0xFF, 0xFF, 0xFF, 0xFD, 0xFC, 0x1C, 0x71, 0xE1, 0xFF, 0xFF, 0xFF, 0xED, 0x07, 0x04, 0x27, 0xE1, 0xFF, 0xFF, 0xFF, 0x0C, 0x4B, 0xAF, 0x23, 0xE1, 0xFF, 0xFF, 0xFF, 0x6D, 0x25, 0xF4, 0xE6, 0xE1, 0xFF, 0xFF, 0xFF, 0x6A, 0x64, 0x09, 0x76, 0xE4, 0xFF, 0xFF, 0xFF, 0x5F, 0x7C, 0x0E, 0xCC, 0xE4, 0xFF, 0xFF, 0xFF, 0xA3, 0xCE, 0x37, 0x5D, 0xE0, 0xFF, 0xFF, 0xFF, 0x88, 0x52, 0xAF, 0x03, 0xE0, 0xFF, 0xFF, 0xFF, 0xF3, 0x0D, 0xF3, 0xAB, 0xE0, 0xFF, 0xFF, 0xFF, 0x65, 0x3D, 0xF4, 0xDC, 0xE8, 0xFF, 0xFF, 0xFF, 0x52, 0x84, 0x8D, 0xF6, 0xEC, 0xFF, 0xFF, 0xFF, 0x3F, 0xF2, 0xF4, 0x4F, 0xE6, 0xFF, 0xFF, 0xFF, 0xFB, 0xEF, 0xCC, 0x49, 0xE6, 0xFF, 0xFF, 0xFF, 0x41, 0xBE, 0xC5, 0xD0, 0xE6, 0xFF, 0xFF, 0xFF });
            }
        }

        public void OpenTactics()
        {
            if (Teams == null)
            {
                Teams = new List<Team>();

                // Chrono Storm Team
                Teams.Add(LoadTeam(0x1C0C4, 0x1C494, 0x1C4E0));

                // Custom Teams
                uint teamInfo = 0x1C0F4;
                uint teamName = 0x1C304;
                uint teamPlayers = 0x1CA60;
                for (int i = 0; i < 10; i++)
                {
                    Teams.Add(LoadTeam(teamInfo, teamName, teamPlayers));
                    teamInfo += 0x30;
                    teamName += 0x28;
                    teamPlayers += 0x40;
                }
            }
        }

        private void SaveTactics(BinaryDataWriter writer)
        {
            if (Teams == null) return;

            // Chrono Storm Team
            SaveTeam(writer, Teams[0], 0x1C0C4, 0x1C494, 0x1C4E0);

            // Custom Teams
            uint teamInfo = 0x1C0F4;
            uint teamName = 0x1C304;
            uint teamPlayers = 0x1CA60;
            for (int i = 0; i < 10; i++)
            {
                SaveTeam(writer, Teams[i+1], teamInfo, teamName, teamPlayers);
                teamInfo += 0x30;
                teamName += 0x28;
                teamPlayers += 0x40;
            }
        }

        public byte[] Save()
        {
            byte[] saveData = Data.GetSection(0, (int)Data.Length);
            BinaryDataWriter dataWriter = new BinaryDataWriter(saveData);

            SaveSaveInfo(dataWriter);
            SavePlayRecords(dataWriter);
            SaveTactics(dataWriter);
            SaveInventory(dataWriter);
            SavePlayers(dataWriter);

            return saveData;
        }

        private Team LoadTeam(uint teamInfo, uint teamName, uint teamPlayers)
        {
            Data.Seek(teamInfo);

            IEGOCSHelper.Team team = Data.ReadStruct<IEGOCSHelper.Team>();
            Item coach = Inventory[team.CoachIndex];
            Item formation = Inventory[team.FormationIndex];
            Item kit = Inventory[team.KitIndex];
            Item emblem = Inventory[team.EmblemIndex];

            Data.Seek(teamName);
            string name = Data.ReadString(Encoding.UTF8);

            Data.Seek(teamPlayers);
            IEGOCSHelper.TeamPlayer teamPlayer = Data.ReadStruct<IEGOCSHelper.TeamPlayer>();
            List<Player> players = teamPlayer.PlayersIndex.Where(x => x != 0x00).Select(x => Reserve.FirstOrDefault(y => y.Index == x)).ToList();
            List<int> playersFormationIndex = team.PlayersFormationIndex.Select(x => Convert.ToInt32(x)).ToList();
            List<int> playersKitNumber = team.PlayerKitNumber.Select(x => Convert.ToInt32(x)).ToList();

            return new Team(name, emblem, kit, formation, coach, players, playersFormationIndex, playersKitNumber);
        }

        private void SaveTeam(BinaryDataWriter writer, Team team, uint teamInfo, uint teamName, uint teamPlayers)
        {
            writer.Seek(teamInfo);
            writer.Write(Inventory.FirstOrDefault(x => x.Value == team.Coach).Key);
            writer.Write(Inventory.FirstOrDefault(x => x.Value == team.Formation).Key);
            writer.Write(Inventory.FirstOrDefault(x => x.Value == team.Kit).Key);
            writer.Write(Inventory.FirstOrDefault(x => x.Value == team.Emblem).Key);
            writer.Write(team.PlayersFormationIndex.Select(x => (byte)x).ToArray());
            writer.Write(team.PlayersKitNumber.Select(x => (byte)x).ToArray());

            writer.Seek(teamName);
            writer.Write(Encoding.UTF8.GetBytes(team.Name));
            writer.Write((byte)0x0);
            writer.Write((byte)0x88);

            writer.Seek(teamPlayers);
            writer.Write(team.Players.SelectMany(x => BitConverter.GetBytes(x.Index)).ToArray());
        }

        private BestMatch IsBestMatch(UInt32 tryPlayerID, UInt32 tryAuraID)
        {
            List<BestMatch> compatibleBestMatchs = Common.GO.BestMatchs.Cs.Where(x => x.PlayerID == tryPlayerID).ToList();

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

        public void NewMixiMax(Player player, int miximaxIndex, int miximaxMove1, int miximaxMove2)
        {
            if (Reserve.Any(x => x.Index == miximaxIndex))
            {
                // The aura is a Player
                BestMatch isBestMatch = IsBestMatch(player.ID, Reserve.Find(x => x.Index == miximaxIndex).ID);

                if (isBestMatch != null)
                {
                    player.MixiMax = new MixiMax(Reserve.Find(x => x.Index == miximaxIndex), (miximaxMove1, miximaxMove2), isBestMatch);
                }
                else
                {
                    player.MixiMax = new MixiMax(Reserve.Find(x => x.Index == miximaxIndex), (miximaxMove1, miximaxMove2));
                }

                Reserve.Find(x => x.Index == miximaxIndex).IsAura = true;
            }
            else
            {
                // The aura is a Item
                BestMatch isBestMatch = IsBestMatch(player.ID, Auras[miximaxIndex].ID);
                MixiMax newMixiMax;

                if (isBestMatch != null)
                {
                    newMixiMax = new MixiMax(Auras[miximaxIndex], (miximaxMove1, miximaxMove2), isBestMatch);
                }
                else
                {
                    newMixiMax = new MixiMax(Auras[miximaxIndex], (miximaxMove1, miximaxMove2));
                }

                newMixiMax.AuraData = true;
                player.MixiMax = newMixiMax;
                Auras[miximaxIndex].IsAura = true;
            }
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

            // Find Aura
            Auras = Inventory.Where(x => Common.GO.Auras.Cs.ContainsKey(x.Value.ID)).ToDictionary(x => x.Key, y => Common.GO.Auras.Cs[y.Value.ID]);

            // Configure Aura
            foreach (KeyValuePair<int, Player> aura in Auras)
            {
                aura.Value.Index = aura.Key;
                aura.Value.Moves = new List<Move>();

                foreach (uint moveID in aura.Value.UInt32Moves)
                {
                    aura.Value.Moves.Add(new Move(Moves[moveID], 1, true, 0));
                }
            }
        }

        public CS(Stream data)
        {
            Data = new BinaryDataReader(data);

            Inventory = GetInventory();

            // Configure Aura
            foreach (KeyValuePair<int, Player> aura in Auras)
            {
                aura.Value.Index = aura.Key;
                aura.Value.Moves = new List<Move>();

                foreach (uint moveID in aura.Value.UInt32Moves)
                {
                    aura.Value.Moves.Add(new Move(Moves[moveID], 1, true, 0));
                }
            }

            Reserve = GetPlayers();
        }
    }
}
