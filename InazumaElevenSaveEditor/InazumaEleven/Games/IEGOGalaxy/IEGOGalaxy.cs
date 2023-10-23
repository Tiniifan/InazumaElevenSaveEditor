using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using InazumaElevenSaveEditor.Tools;
using InazumaElevenSaveEditor.InazumaEleven.Logic;

namespace InazumaElevenSaveEditor.InazumaEleven.Games.IEGOGalaxy
{
    public class Galaxy : IGame
    {
        public string Code => "IEGOGALAXY";

        public string Name => "Inazuma Eleven Go Galaxy";

        public int MaximumPlayer => 336;

        public BinaryDataReader Data { get; set; }

        public List<Player> Reserve { get; set; }

        public Dictionary<int, Player> Auras { get; set; }

        public Dictionary<int, Equipment> Equipments { get; set; }

        public Dictionary<int, Item> Inventory { get; set; }

        public Dictionary<int, List<PlayRecord>> PlayRecords { get; set; }

        public SaveInfo SaveInfo { get; set; }

        public List<Team> Teams { get; set; }

        public Dictionary<uint, Player> Players => Common.GO.Players.Galaxy;

        public Dictionary<uint, Move> Moves => Common.GO.Moves.Galaxy;

        public Dictionary<uint, Avatar> Avatars => Common.GO.Avatars.Galaxy;

        public Dictionary<uint, Item> Items => Common.GO.Items.Galaxy;

        private Dictionary<int, Item> GetInventory()
        {
            // Group Item 1
            Data.Seek((uint)IEGOGalaxyHelper.ItemBlockGroup1Offset);
            Dictionary<int, Item> itemBlockGroup1 = Data.ReadMultipleStruct<IEGOGalaxyHelper.ItemBlockGroup1>(608).Where(x => Items.ContainsKey(x.ID) && x.ID != 0x00).ToDictionary(x => x.Index, y => new Item(y.ID, Common.GO.Items.Galaxy[y.ID], y.Quantity));

            // Group Item 2
            Data.Seek((uint)IEGOGalaxyHelper.ItemBlockGroup2Offset);
            Dictionary<int, Item> itemBlockGroup2 = Data.ReadMultipleStruct<IEGOGalaxyHelper.ItemBlockGroup2>(452).Where(x => Items.ContainsKey(x.ID) && x.ID != 0x00).ToDictionary(x => x.Index, y => new Item(y.ID, Common.GO.Items.Galaxy[y.ID], y.Quantity, y.QuantityEquiped));

            // Group Item 3
            Data.Seek((uint)IEGOGalaxyHelper.ItemBlockGroup3Offset);
            Dictionary<int, Item> itemBlockGroup3 = Data.ReadMultipleStruct<IEGOGalaxyHelper.ItemBlockGroup3>(888).Where(x => Items.ContainsKey(x.ID) && x.ID != 0x00).ToDictionary(x => x.Index, y => new Item(y.ID, Common.GO.Items.Galaxy[y.ID]));

            // Find Equipment
            Equipments = itemBlockGroup2.Where(x => Common.GO.Equipments.Galaxy.ContainsKey(x.Value.ID)).ToDictionary(x => x.Key, y => Common.GO.Equipments.Galaxy[y.Value.ID]);
            Equipments.Add(0, Common.GO.Equipments.Cs[0x0]);

            // Merge
            return itemBlockGroup1.Union(itemBlockGroup2).Union(itemBlockGroup3).ToDictionary(x => x.Key, x => x.Value);
        }

        private void SaveInventory(BinaryDataWriter writer)
        {
            // Group Item 1
            writer.Seek((uint)IEGOGalaxyHelper.ItemBlockGroup1Offset);
            Dictionary<int, Item> itemGroup1 = Inventory.Where(x => x.Value.Category == 1).ToDictionary(x => x.Key, y => y.Value);
            for (int i = 0; i < 608; i++)
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
            writer.Seek((uint)IEGOGalaxyHelper.ItemBlockGroup2Offset);
            Dictionary<int, Item> itemGroup2 = Inventory.Where(x => x.Value.Category == 2).ToDictionary(x => x.Key, y => y.Value);
            for (int i = 0; i < 452; i++)
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
            writer.Seek((uint)IEGOGalaxyHelper.ItemBlockGroup3Offset);
            Dictionary<int, Item> itemGroup3 = Inventory.Where(x => x.Value.Category == 3).ToDictionary(x => x.Key, y => y.Value);
            for (int i = 0; i < 888; i++)
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
            Data.Seek((uint)IEGOGalaxyHelper.PlayerIndexOffset);
            int[] index = Data.ReadMultipleStruct<int>(IEGOGalaxyHelper.MaximumPlayer)
                .Where(x => x != 0x0)
                .ToArray();

            // Get player data according to their index
            Data.Seek((uint)IEGOGalaxyHelper.PlayerDataOffset);
            List<IEGOGalaxyHelper.PlayerBlock> playerBlocks = Data.ReadMultipleStruct<IEGOGalaxyHelper.PlayerBlock>(index.Length)
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

                    if (playerBlock.TotemID != 0x00)
                    {
                        player.Avatar = new Avatar(Avatars[playerBlock.TotemID], 1, 1);
                    } else
                    {
                        player.Avatar = new Avatar(Avatars[playerBlock.AvatarID], playerBlock.AvatarLevel, playerBlock.AvatarUsage);
                    }
                
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

            // Sort
            return players.OrderBy(player => Array.IndexOf(index, player.Index)).ToList();
        }

        private void SavePlayers(BinaryDataWriter writer)
        {
            // Save player index
            writer.Seek((uint)IEGOGalaxyHelper.PlayerIndexOffset);
            for (int i = 0; i < IEGOGalaxyHelper.MaximumPlayer; i++)
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
            writer.Seek((uint)IEGOGalaxyHelper.PlayerDataOffset);
            List<Player> sortedReserve = Reserve.OrderBy(player => player.Index).ToList();
            for (int i = 0; i < IEGOGalaxyHelper.MaximumPlayer; i++)
            {
                if (i < sortedReserve.Count)
                {
                    Player player = sortedReserve[i];

                    writer.Write(player.Avatar.IsFightingSpirit ? 0x0 : Avatars.FirstOrDefault(x => x.Value.Name == player.Avatar.Name).Key);
                    writer.Write(player.Index);
                    writer.Write(player.ID);
                    writer.Write(player.MixiMax != null ? player.MixiMax.AuraPlayer.Index : 0x00);
                    writer.Skip(0x0C);
                    writer.Write((short)player.FP);
                    writer.Write((short)player.TP);
                    writer.Write((short)player.Freedom);
                    writer.Write((byte)player.Level);
                    writer.Skip(0x01);
                    writer.Write((byte)(player.MixiMax != null ? player.MixiMax.MixiMaxMoveNumber[0] : 0x00));
                    writer.Write((byte)(player.MixiMax != null ? player.MixiMax.MixiMaxMoveNumber[1] : 0x00));

                    // Determines the Invoker value and saves it
                    int canInvokeArmed = Convert.ToInt32(player.Invoke) * 8 + Convert.ToInt32(player.Armed) * 16;
                    if (player.MixiMax != null && player.MixiMax.AuraData == true) canInvokeArmed += 1;
                    var playerIsAura = Reserve.FirstOrDefault(x => x.MixiMax != null && x.MixiMax.AuraPlayer == player);
                    if (playerIsAura != null) canInvokeArmed += 2;
                    writer.Write((byte)canInvokeArmed);

                    writer.Write((byte)((player.Style << 4) & 0xF0));
                    writer.Write((short)player.Participation);
                    writer.Write((short)player.Score);
                    writer.Skip(0x04);
                    writer.Skip(0x04);
                    writer.Write(player.InvestedPoint.SelectMany(x => BitConverter.GetBytes((short)x)).ToArray());
                    writer.Write(player.Avatar.IsFightingSpirit ? Avatars.FirstOrDefault(x => x.Value.Name == player.Avatar.Name).Key : 0x00);
                    writer.Write(player.Avatar.IsFightingSpirit ? (byte)player.Avatar.Level : (byte)1);
                    writer.Write(player.Avatar.IsFightingSpirit ? (byte)player.Avatar.Usage : (byte)0);
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
                    int[] emptyPlayerData = Enumerable.Repeat(0, 67).ToArray();
                    writer.Write(emptyPlayerData.SelectMany(x => BitConverter.GetBytes(x)).ToArray());
                }
            }
        }

        public void OpenPlayRecords()
        {
            if (PlayRecords == null)
            {
                PlayRecords = Common.GO.PlayRecords.Galaxy;

                Data.Seek((uint)IEGOGalaxyHelper.PlayRecordsOffset);

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

            writer.Seek((uint)IEGOGalaxyHelper.PlayRecordsOffset);

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
                Data.Seek((uint)IEGOGalaxyHelper.NameOffset);
                SaveInfo.Name = Data.ReadString(Encoding.UTF8);
                Data.Seek((uint)IEGOGalaxyHelper.TeamNameOffset);
                SaveInfo.TeamName = Data.ReadString(Encoding.UTF8);

                // Time Hours
                Data.Seek((uint)IEGOGalaxyHelper.TimeOffset);
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
                Data.Seek((uint)IEGOGalaxyHelper.LinkOffset);
                SaveInfo.SecretLinkLevel = Data.ReadValue<byte>();

                // Current Chapter
                Data.Seek((uint)IEGOGalaxyHelper.ChapterOffset);
                SaveInfo.Chapter = Data.ReadValue<byte>();

                // Money
                Data.Seek((uint)IEGOGalaxyHelper.MoneyOffset);
                SaveInfo.Prestige = Data.ReadValue<int>();
                SaveInfo.Friendship = Data.ReadValue<int>();

                // Coins
                Data.Seek((uint)IEGOGalaxyHelper.CoinOffset);
                Enumerable.Range(0, 5).ToList().ForEach(i => SaveInfo.Coins[i] = Data.ReadValue<short>());
            }
        }

        private void SaveSaveInfo(BinaryDataWriter writer)
        {
            if (SaveInfo == null) return;

            // Save Name
            writer.Seek((uint)IEGOGalaxyHelper.NameOffset);
            writer.Write(Encoding.UTF8.GetBytes(SaveInfo.Name));
            writer.Write((byte)0x0);
            writer.Write((byte)0x88);

            // Team Name
            writer.Seek((uint)IEGOGalaxyHelper.TeamNameOffset);
            writer.Write(Encoding.UTF8.GetBytes(SaveInfo.TeamName));
            writer.Write((byte)0x0);
            writer.Write((byte)0x88);

            // Time Hours
            writer.Seek((uint)IEGOGalaxyHelper.TimeOffset);
            int time = SaveInfo.Hours * 3600 + SaveInfo.Min * 60;
            writer.Write(time);

            // Link Level
            writer.Seek((uint)IEGOGalaxyHelper.LinkOffset);
            writer.Write((byte)SaveInfo.SecretLinkLevel);

            // Money
            writer.Seek((uint)IEGOGalaxyHelper.MoneyOffset);
            writer.Write(SaveInfo.Prestige);

            // Coins
            writer.Seek((uint)IEGOGalaxyHelper.CoinOffset);
            SaveInfo.Coins.ForEach(x => writer.Write((short)x));

            if (SaveInfo.UnlockAllData == true)
            {
                writer.Seek(0x8F62);
                writer.Write(new byte[2] { 0xB9, 0x08 });
                writer.Skip(6);
                writer.Write(new byte[2] { 0x46, 0xC9 });

                writer.Seek(0x8FFD);
                writer.Write(new byte[3] { 0x1C, 0x0E, 0xFE });

                writer.Seek(0x9000);
                writer.Write(new byte[11] { 0x1F, 0x00, 0x00, 0x00, 0x00, 0xFC, 0xF9, 0xFF, 0xFF, 0xFF, 0x7F });

                writer.Seek(0x902F);
                writer.Write(new byte[9] { 0xF8, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F, 0x00, 0x0E });

                // Ugly Write
                writer.Seek(0x2ECB0);
                writer.Write(new byte[948] { 0x76, 0x00, 0x00, 0x00, 0x04, 0x44, 0x9D, 0x00, 0xC4, 0xB4, 0x8A, 0x81, 0x04, 0x44, 0x9D, 0x00, 0x52, 0x84, 0x8D, 0xF6, 0x04, 0x44, 0x9D, 0x00, 0x7E, 0xE5, 0x83, 0x18, 0x0D, 0x44, 0x9D, 0x00, 0x01, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x02, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x03, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x04, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x05, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x06, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x07, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x08, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x09, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x0A, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x0B, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x0D, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x0E, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x10, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x11, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x12, 0x00, 0x00, 0x00, 0x14, 0x44, 0x9D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x44, 0x9D, 0x00, 0x99, 0xCA, 0xBD, 0x07, 0x16, 0x44, 0x9D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x44, 0x9D, 0x00, 0x62, 0x10, 0x4C, 0x3B, 0x00, 0x44, 0x9D, 0x00, 0xCA, 0x40, 0x78, 0xBB, 0x00, 0x44, 0x9D, 0x00, 0xEC, 0x16, 0xC5, 0x8D, 0x00, 0x44, 0x9D, 0x00, 0xBA, 0xF2, 0x4E, 0x43, 0x00, 0x44, 0x9D, 0x00, 0x3B, 0x7F, 0x3F, 0xC9, 0x04, 0x44, 0x9D, 0x00, 0xA8, 0x92, 0x66, 0x99, 0x10, 0x44, 0x9D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x44, 0x9D, 0x00, 0x20, 0x74, 0xE7, 0xF2, 0x04, 0x44, 0x9D, 0x00, 0xEC, 0xC0, 0x9C, 0xB2, 0x00, 0x44, 0x9D, 0x00, 0x76, 0x01, 0x91, 0x7C, 0x0E, 0x44, 0x9D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x44, 0x9D, 0x00, 0xC1, 0xF3, 0x40, 0x00, 0x00, 0x44, 0x9D, 0x00, 0x74, 0x37, 0x3F, 0x2A, 0x00, 0x44, 0x9D, 0x00, 0xC9, 0x4B, 0x8E, 0x54, 0x00, 0x44, 0x9D, 0x00, 0xCE, 0x66, 0x36, 0xB3, 0x00, 0x44, 0x9D, 0x00, 0x17, 0x1E, 0x31, 0x27, 0x00, 0x44, 0x9D, 0x00, 0x81, 0x2E, 0x36, 0x50, 0x00, 0x44, 0x9D, 0x00, 0x14, 0xDD, 0xEF, 0x8C, 0x00, 0x44, 0x9D, 0x00, 0x58, 0x56, 0x31, 0xC4, 0x00, 0x44, 0x9D, 0x00, 0x06, 0x85, 0x13, 0x45, 0x00, 0x44, 0x9D, 0x00, 0xC8, 0x40, 0x62, 0x2C, 0x00, 0x44, 0x9D, 0x00, 0xE4, 0x21, 0x6C, 0xC2, 0x0E, 0x44, 0x9D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E, 0x44, 0x9D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E, 0x44, 0x9D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x28, 0xEB, 0x00, 0x57, 0xC3, 0x47, 0x77, 0x00, 0x28, 0xEB, 0x00, 0xEB, 0x53, 0x9B, 0x40, 0x00, 0x28, 0xEB, 0x00, 0xB4, 0x8B, 0x55, 0xB9, 0x00, 0x28, 0xEB, 0x00, 0x7D, 0x63, 0x9C, 0x37, 0x00, 0x28, 0xEB, 0x00, 0xC0, 0x1F, 0x2D, 0x49, 0x00, 0x28, 0xEB, 0x00, 0x41, 0xBE, 0xC5, 0xD0, 0x00, 0x28, 0xEB, 0x00, 0xFB, 0xEF, 0xCC, 0x49, 0x0E, 0x28, 0xEB, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x28, 0xEB, 0x00, 0x55, 0xEC, 0xF4, 0x95, 0x00, 0x28, 0xEB, 0x00, 0xC3, 0xDC, 0xF3, 0xE2, 0x00, 0x28, 0xEB, 0x00, 0x79, 0x8D, 0xFA, 0x7B, 0x00, 0x28, 0xEB, 0x00, 0xB7, 0x48, 0x8B, 0x12, 0x00, 0xBC, 0x0E, 0x03, 0x8F, 0x7B, 0xB4, 0xB9, 0x00, 0xBC, 0x0E, 0x03, 0x1E, 0x66, 0x0B, 0x29, 0x00, 0xBC, 0x0E, 0x03, 0x0F, 0xD6, 0x23, 0x63, 0x00, 0xBC, 0x0E, 0x03, 0x20, 0x1C, 0x15, 0x0C, 0x10, 0xBC, 0x0E, 0x03, 0x41, 0xB6, 0xFE, 0xFE, 0x0D, 0xBC, 0x0E, 0x03, 0x13, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x14, 0x00, 0x00, 0x00, 0x06, 0xBC, 0x0E, 0x03, 0x00, 0x00, 0x00, 0x00, 0x08, 0xBC, 0x0E, 0x03, 0x00, 0x00, 0x00, 0x00, 0x0A, 0xBC, 0x0E, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0xBC, 0x0E, 0x03, 0xEE, 0xD9, 0x64, 0x65, 0x00, 0xBC, 0x0E, 0x03, 0x0E, 0xDA, 0x5C, 0x20, 0x00, 0xBC, 0x0E, 0x03, 0x98, 0xEA, 0x5B, 0x57, 0x00, 0xBC, 0x0E, 0x03, 0x22, 0xBB, 0x52, 0xCE, 0x10, 0xBC, 0x0E, 0x03, 0x4F, 0xC8, 0x76, 0x3F, 0x10, 0xBC, 0x0E, 0x03, 0xF5, 0x99, 0x7F, 0xA6, 0x0D, 0xBC, 0x0E, 0x03, 0x15, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x16, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x17, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x18, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x19, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x1A, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x1B, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x1C, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x1D, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x1E, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x1F, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x20, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x21, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x22, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x23, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x24, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x25, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x26, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x27, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x28, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x29, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x2A, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x2B, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x2C, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x2D, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x2E, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x2F, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x30, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x31, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x32, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x33, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x34, 0x00, 0x00, 0x00, 0x00, 0xBC, 0x0E, 0x03, 0x06, 0x50, 0x85, 0xE8, 0x00, 0xBC, 0x0E, 0x03, 0xE6, 0x3B, 0x5B, 0x87, 0x00, 0xBC, 0x0E, 0x03, 0xC8, 0x95, 0xF4, 0x81, 0x00, 0xBC, 0x0E, 0x03, 0x8D, 0x0D, 0xC6, 0x8B, 0x00, 0xBC, 0x0E, 0x03, 0xEA, 0xE6, 0x37, 0xD9, 0x00, 0xBC, 0x0E, 0x03, 0xC7, 0xCC, 0x09, 0x2B, 0x00, 0xBC, 0x0E, 0x03, 0xBA, 0xDE, 0xD7, 0x50, 0x00, 0xBC, 0x0E, 0x03, 0x34, 0xD8, 0x5E, 0xE6, 0x00, 0xBC, 0x0E, 0x03, 0x0B, 0xC6, 0xD9, 0xAA });
            }
        }

        public void OpenTactics()
        {
            if (Teams == null)
            {
                Teams = new List<Team>();

                // Selection Team
                Teams.Add(LoadTeam(0x268E0, (uint)IEGOGalaxyHelper.TeamNameOffset, 0x26E28));

                // Custom Teams
                uint teamInfo = 0x26914;
                uint teamName = 0x26B50;
                uint teamPlayers = 0x273A8;
                for (int i = 0; i < 10; i++)
                {
                    Teams.Add(LoadTeam(teamInfo, teamName, teamPlayers));
                    teamInfo += 0x34;
                    teamName += 0x20;
                    teamPlayers += 0x40;
                }
            }
        }

        private void SaveTactics(BinaryDataWriter writer)
        {
            if (Teams == null) return;

            // Chrono Storm Team
            SaveTeam(writer, Teams[0], 0x268E0, (uint)IEGOGalaxyHelper.TeamNameOffset, 0x26E28);

            // Custom Teams
            uint teamInfo = 0x26914;
            uint teamName = 0x26B50;
            uint teamPlayers = 0x273A8;
            for (int i = 0; i < 10; i++)
            {
                SaveTeam(writer, Teams[i + 1], teamInfo, teamName, teamPlayers);
                teamInfo += 0x34;
                teamName += 0x20;
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

            IEGOGalaxyHelper.Team team = Data.ReadStruct<IEGOGalaxyHelper.Team>();

            Item coach = Inventory.ContainsKey(team.CoachIndex) ? Inventory[team.CoachIndex] : null;
            Item formation = Inventory.ContainsKey(team.FormationIndex) ? Inventory[team.FormationIndex] : null;
            Item kit = Inventory.ContainsKey(team.KitIndex) ? Inventory[team.KitIndex] : null;
            Item emblem = Inventory.ContainsKey(team.EmblemIndex) ? Inventory[team.EmblemIndex] : null;

            Data.Seek(teamName);
            string name = Data.ReadString(Encoding.UTF8);

            Data.Seek(teamPlayers);
            IEGOGalaxyHelper.TeamPlayer teamPlayer = Data.ReadStruct<IEGOGalaxyHelper.TeamPlayer>();
            List<Player> players = teamPlayer.PlayersIndex.Where(x => x != 0x00).Select(x => Reserve.FirstOrDefault(y => y.Index == x)).ToList();
            List<int> playersFormationIndex = team.PlayersFormationIndex.Select(x => Convert.ToInt32(x)).ToList();
            List<int> playersKitNumber = team.PlayerKitNumber.Select(x => Convert.ToInt32(x)).ToList();

            return new Team(name, emblem, kit, formation, coach, players, playersFormationIndex, playersKitNumber);
        }

        private void SaveTeam(BinaryDataWriter writer, Team team, uint teamInfo, uint teamName, uint teamPlayers)
        {
            writer.Seek(teamInfo);
            writer.Write((team.Coach == null) ? 0x00 : Inventory.FirstOrDefault(x => x.Value == team.Coach).Key);
            writer.Write((team.Formation == null) ? 0x00 : Inventory.FirstOrDefault(x => x.Value == team.Formation).Key);
            writer.Write((team.Kit == null) ? 0x00 : Inventory.FirstOrDefault(x => x.Value == team.Kit).Key);
            writer.Write((team.Emblem == null) ? 0x00 : Inventory.FirstOrDefault(x => x.Value == team.Emblem).Key);
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

                for (int i = 0; i < 2; i++)
                {
                    Move newMove = Moves[0x00];
                    newMove.Level = 1;
                    newMove.UsedCount = 0;
                    player.Moves.Add(newMove);
                }

                player.Equipments = new List<Equipment>();
                for (int i = 0; i < 4; i++)
                {
                    player.Equipments.Add(Equipments[0x0]);
                }
            }

            int newIndex = Reserve.Last().Index;

            while (Reserve.Any(x => x.Index == newIndex))
            {
                short lowInt16 = (short)(newIndex & 0xFFFF);
                short hightInt16 = (short)((newIndex >> 16) & 0xFFFF);
                lowInt16++;
                hightInt16++;

                newIndex = (int)lowInt16 | ((int)hightInt16 << 16);
            }

            player.Index = newIndex;
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

                for (int i = 0; i < 2; i++)
                {
                    Move newMove = Moves[0x00];
                    newMove.Level = 1;
                    newMove.UsedCount = 0;
                    newPlayer.Moves.Add(newMove);
                }
            }

            return newPlayer;
        }

        public (int, int, string, bool) Training(Player player, int newStat, int statIndex)
        {
            player.InvestedFreedom[statIndex] = newStat;

            if (player.InvestedFreedom.Sum() <= player.Freedom)
            {
                player.InvestedPoint[statIndex] = newStat;
                return (0, player.Freedom, "investedNumericUpDown" + (statIndex + 3), true);
            }
            else
            {
                return (0, player.InvestedPoint[statIndex], "investedNumericUpDown" + (statIndex + 3), true);
            }
        }

        public void UpdateInventory()
        {
            // Find Equipment
            Equipments = Inventory.Where(x => Common.GO.Equipments.Cs.ContainsKey(x.Value.ID)).ToDictionary(x => x.Key, y => Common.GO.Equipments.Cs[y.Value.ID]);
            Equipments.Add(0, Common.GO.Equipments.Cs[0x0]);
        }

        public Galaxy(Stream data)
        {
            Data = new BinaryDataReader(data);

            Inventory = GetInventory();
            Reserve = GetPlayers();
        }
    }
}
