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

        private IList<BestMatch> BestMatchs = Common.InazumaElevenGo.BestMatchs.Galaxy;

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

        public void ChangePlayer(Player selectedPlayer, Player newPlayer)
        {
            selectedPlayer.Name = newPlayer.Name;
            selectedPlayer.ID = newPlayer.ID;
            selectedPlayer.Element = newPlayer.Element;
            selectedPlayer.Position = newPlayer.Position;
            selectedPlayer.Gender = newPlayer.Gender;
            selectedPlayer.Stat = newPlayer.Stat;
            selectedPlayer.Freedom = newPlayer.Freedom;
            selectedPlayer.Moves = newPlayer.Moves;
            selectedPlayer.UInt32Moves = newPlayer.UInt32Moves;
            selectedPlayer.Level = newPlayer.Level;
            selectedPlayer.Invoke = newPlayer.Invoke;
            selectedPlayer.Armed = newPlayer.Armed;
            selectedPlayer.Style = newPlayer.Style;
            selectedPlayer.InvestedPoint = newPlayer.InvestedPoint;
            selectedPlayer.Avatar = newPlayer.Avatar;
            selectedPlayer.Equipments = newPlayer.Equipments;
            selectedPlayer.MixiMax = newPlayer.MixiMax;
            selectedPlayer.Participation = newPlayer.Participation;
            selectedPlayer.Score = newPlayer.Score;
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

        public void RecruitPlayer(Player player)
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

            UInt32 newIndex = BitConverter.ToUInt32(new byte[4]
            {
                    Convert.ToByte(0),
                    Convert.ToByte((missingIndex+1) % 0x100),
                    Convert.ToByte(0),
                    Convert.ToByte(missingIndex % 0x100),
            }, 0);
            PlayersInSaveSort.Add(newIndex);
            player.PositionInFile = 23324 + 260 * PlayersInSaveSort.Count;
            PlayersInSave.Add(newIndex, player);
        }

        public void Open()
        {
            // Initialize Resources
            Players = Common.InazumaElevenGo.Players.Galaxy;
            Moves = Common.InazumaElevenGo.Moves.Galaxy;
            Avatars = Common.InazumaElevenGo.Avatars.Galaxy;
            Items = Common.InazumaElevenGo.Items.Galaxy;
            AuraInSave = new Dictionary<uint, Player>();

            IDictionary<uint, Equipment> AllEquipments = Common.InazumaElevenGo.Equipments.Galaxy;
            Equipments = new Dictionary<UInt32, Equipment>();

            SaveInfo = new Save();

            // First Item Category - 608 maximum items
            File.Seek(0xA394);
            for (int i = 0; i < 608; i++)
            {
                // Check If The Key/Index Is Valid
                UInt32 index = File.Reverse(File.ReadUInt32());
                if (index == 0x0 || ((index >> (8 * 3)) & 0xff) + ((index >> (8 * 2)) & 0xff - 16) * 256 != i)
                {
                    index = BitConverter.ToUInt32(new byte[4]
                    {
                        Convert.ToByte((i+1)/256),
                        Convert.ToByte((i+1) % 0x100),
                        Convert.ToByte(i/256),
                        Convert.ToByte(i % 0x100),
                    }, 0);
                }

                // Create Empty Item
                Item newItem = new Item(" ", 1, -1);

                // Load Item Values
                UInt32 itemID = File.Reverse(File.ReadUInt32());
                short quantity = File.ReadByte();

                // Check If The Item Exists
                if (itemID != 0x0)
                {
                    if (Items.ContainsKey(itemID))
                        newItem = Items[itemID];
                    else
                        newItem = Items[0x01];

                    newItem.Quantity = quantity;
                }

                SaveInfo.Inventory.Add(index, newItem);
                File.Skip(3);
            }

            // Second Item Category - 452 maximum items
            File.Seek(0xC020);
            for (int i = 0; i < 452; i++)
            {
                // Check If The Key/Index Is Valid
                UInt32 index = File.Reverse(File.ReadUInt32());
                if (index == 0x0 || ((index >> (8 * 3)) & 0xff) + ((index >> (8 * 2)) & 0xff - 16) * 256 != i)
                {
                    index = BitConverter.ToUInt32(new byte[4]
                    {
                        Convert.ToByte((i+1)/256),
                        Convert.ToByte((i+1) % 0x100),
                        Convert.ToByte(0x10 + i/256),
                        Convert.ToByte(i % 0x100),
                    }, 0);
                }

                // Create Empty Item
                Item newItem = new Item(" ", 2, -1);

                // Load Item Values
                UInt32 itemID = File.Reverse(File.ReadUInt32());
                short quantity = File.ReadByte();

                // Check If The Item Exists
                if (itemID != 0x0)
                {
                    if (Items.ContainsKey(itemID))
                        newItem = Items[itemID];
                    else
                        newItem = Items[0x01];

                    newItem.Quantity = quantity;

                    if (AllEquipments.ContainsKey(itemID))
                    {
                        Equipment newEquipment = AllEquipments[itemID];
                        newEquipment.ID = itemID;
                        Equipments.Add(index, newEquipment);
                    }
                }

                SaveInfo.Inventory.Add(index, newItem);
                File.Skip(7);
            }

            // Third Item Category - 888 Maximum Items
            File.Seek(0xDC6C);
            for (int i = 0; i < 888; i++)
            {
                // Check If The Key/Index Is Valid
                UInt32 index = File.Reverse(File.ReadUInt32());
                if (index == 0x0 || ((index >> (8 * 3)) & 0xff) + ((index >> (8 * 2)) & 0xff - 16) * 256 != i)
                {
                    index = BitConverter.ToUInt32(new byte[4]
                    {
                        Convert.ToByte((i+1)/256),
                        Convert.ToByte((i+1) % 0x100),
                        Convert.ToByte(0x20 + i/256),
                        Convert.ToByte(i % 0x100),
                    }, 0);
                }

                // Create Empty Item
                Item newItem = new Item(" ", 3, -1);

                // Load Item Values
                UInt32 itemID = File.Reverse(File.ReadUInt32());

                // Check If The Item Exists
                if (itemID != 0x0)
                {
                    if (Items.ContainsKey(itemID))
                        newItem = Items[itemID];
                    else
                        newItem = Items[0x01];

                    newItem.Quantity = 1;
                }

                SaveInfo.Inventory.Add(index, newItem);
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

                    NewMixiMax(player.Value, playerAura, mixiMaxMove1, mixiMaxMove2);
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
                for (int i = 0; i < 5; i++)
                {
                    SaveInfo.Coins[i] = File.ReadInt16();
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
            byte[] saveData = File.GetSection(0, (int)File.Length);
            DataWriter dataWriter = new DataWriter(saveData);

            // Save Info
            if (SaveInfo.Name != null)
            {
                dataWriter.Seek(0x23);
                int time = SaveInfo.Hours * 3600 + SaveInfo.Min * 60;
                dataWriter.Write(time);

                dataWriter.Seek(0x3C);
                dataWriter.Write(Encoding.UTF8.GetBytes(SaveInfo.Name));
                dataWriter.WriteByte(0x0);
                dataWriter.WriteByte(0x88);

                dataWriter.Seek(0x90B4);
                dataWriter.WriteByte(SaveInfo.SecretLinkLevel);

                dataWriter.Seek(0x268D0);
                dataWriter.Write(SaveInfo.Prestige);

                dataWriter.Seek(0x26CC8);
                for (int i = 0; i < 5; i++)
                {
                    dataWriter.WriteInt16(SaveInfo.Coins[i]);
                }

                if (SaveInfo.UnlockAllData == true)
                {
                    dataWriter.Seek(0x8F62);
                    dataWriter.Write(new byte[2] { 0xB9, 0x08});
                    dataWriter.Skip(6);
                    dataWriter.Write(new byte[2] { 0x46, 0xC9 });

                    dataWriter.Seek(0x8FFD);
                    dataWriter.Write(new byte[3] { 0x1C, 0x0E, 0xFE});

                    dataWriter.Seek(0x9000);
                    dataWriter.Write(new byte[11] { 0x1F, 0x00, 0x00, 0x00, 0x00, 0xFC, 0xF9, 0xFF, 0xFF, 0xFF, 0x7F });

                    dataWriter.Seek(0x902F);
                    dataWriter.Write(new byte[9] { 0xF8, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F, 0x00, 0x0E });

                    // Ugly Write
                    dataWriter.Seek(0x2ECB0);
                    dataWriter.Write(new byte[948] { 0x76, 0x00, 0x00, 0x00, 0x04, 0x44, 0x9D, 0x00, 0xC4, 0xB4, 0x8A, 0x81, 0x04, 0x44, 0x9D, 0x00, 0x52, 0x84, 0x8D, 0xF6, 0x04, 0x44, 0x9D, 0x00, 0x7E, 0xE5, 0x83, 0x18, 0x0D, 0x44, 0x9D, 0x00, 0x01, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x02, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x03, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x04, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x05, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x06, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x07, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x08, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x09, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x0A, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x0B, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x0D, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x0E, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x10, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x11, 0x00, 0x00, 0x00, 0x0D, 0x44, 0x9D, 0x00, 0x12, 0x00, 0x00, 0x00, 0x14, 0x44, 0x9D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x44, 0x9D, 0x00, 0x99, 0xCA, 0xBD, 0x07, 0x16, 0x44, 0x9D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x44, 0x9D, 0x00, 0x62, 0x10, 0x4C, 0x3B, 0x00, 0x44, 0x9D, 0x00, 0xCA, 0x40, 0x78, 0xBB, 0x00, 0x44, 0x9D, 0x00, 0xEC, 0x16, 0xC5, 0x8D, 0x00, 0x44, 0x9D, 0x00, 0xBA, 0xF2, 0x4E, 0x43, 0x00, 0x44, 0x9D, 0x00, 0x3B, 0x7F, 0x3F, 0xC9, 0x04, 0x44, 0x9D, 0x00, 0xA8, 0x92, 0x66, 0x99, 0x10, 0x44, 0x9D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x44, 0x9D, 0x00, 0x20, 0x74, 0xE7, 0xF2, 0x04, 0x44, 0x9D, 0x00, 0xEC, 0xC0, 0x9C, 0xB2, 0x00, 0x44, 0x9D, 0x00, 0x76, 0x01, 0x91, 0x7C, 0x0E, 0x44, 0x9D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x44, 0x9D, 0x00, 0xC1, 0xF3, 0x40, 0x00, 0x00, 0x44, 0x9D, 0x00, 0x74, 0x37, 0x3F, 0x2A, 0x00, 0x44, 0x9D, 0x00, 0xC9, 0x4B, 0x8E, 0x54, 0x00, 0x44, 0x9D, 0x00, 0xCE, 0x66, 0x36, 0xB3, 0x00, 0x44, 0x9D, 0x00, 0x17, 0x1E, 0x31, 0x27, 0x00, 0x44, 0x9D, 0x00, 0x81, 0x2E, 0x36, 0x50, 0x00, 0x44, 0x9D, 0x00, 0x14, 0xDD, 0xEF, 0x8C, 0x00, 0x44, 0x9D, 0x00, 0x58, 0x56, 0x31, 0xC4, 0x00, 0x44, 0x9D, 0x00, 0x06, 0x85, 0x13, 0x45, 0x00, 0x44, 0x9D, 0x00, 0xC8, 0x40, 0x62, 0x2C, 0x00, 0x44, 0x9D, 0x00, 0xE4, 0x21, 0x6C, 0xC2, 0x0E, 0x44, 0x9D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E, 0x44, 0x9D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E, 0x44, 0x9D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x28, 0xEB, 0x00, 0x57, 0xC3, 0x47, 0x77, 0x00, 0x28, 0xEB, 0x00, 0xEB, 0x53, 0x9B, 0x40, 0x00, 0x28, 0xEB, 0x00, 0xB4, 0x8B, 0x55, 0xB9, 0x00, 0x28, 0xEB, 0x00, 0x7D, 0x63, 0x9C, 0x37, 0x00, 0x28, 0xEB, 0x00, 0xC0, 0x1F, 0x2D, 0x49, 0x00, 0x28, 0xEB, 0x00, 0x41, 0xBE, 0xC5, 0xD0, 0x00, 0x28, 0xEB, 0x00, 0xFB, 0xEF, 0xCC, 0x49, 0x0E, 0x28, 0xEB, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x28, 0xEB, 0x00, 0x55, 0xEC, 0xF4, 0x95, 0x00, 0x28, 0xEB, 0x00, 0xC3, 0xDC, 0xF3, 0xE2, 0x00, 0x28, 0xEB, 0x00, 0x79, 0x8D, 0xFA, 0x7B, 0x00, 0x28, 0xEB, 0x00, 0xB7, 0x48, 0x8B, 0x12, 0x00, 0xBC, 0x0E, 0x03, 0x8F, 0x7B, 0xB4, 0xB9, 0x00, 0xBC, 0x0E, 0x03, 0x1E, 0x66, 0x0B, 0x29, 0x00, 0xBC, 0x0E, 0x03, 0x0F, 0xD6, 0x23, 0x63, 0x00, 0xBC, 0x0E, 0x03, 0x20, 0x1C, 0x15, 0x0C, 0x10, 0xBC, 0x0E, 0x03, 0x41, 0xB6, 0xFE, 0xFE, 0x0D, 0xBC, 0x0E, 0x03, 0x13, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x14, 0x00, 0x00, 0x00, 0x06, 0xBC, 0x0E, 0x03, 0x00, 0x00, 0x00, 0x00, 0x08, 0xBC, 0x0E, 0x03, 0x00, 0x00, 0x00, 0x00, 0x0A, 0xBC, 0x0E, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0xBC, 0x0E, 0x03, 0xEE, 0xD9, 0x64, 0x65, 0x00, 0xBC, 0x0E, 0x03, 0x0E, 0xDA, 0x5C, 0x20, 0x00, 0xBC, 0x0E, 0x03, 0x98, 0xEA, 0x5B, 0x57, 0x00, 0xBC, 0x0E, 0x03, 0x22, 0xBB, 0x52, 0xCE, 0x10, 0xBC, 0x0E, 0x03, 0x4F, 0xC8, 0x76, 0x3F, 0x10, 0xBC, 0x0E, 0x03, 0xF5, 0x99, 0x7F, 0xA6, 0x0D, 0xBC, 0x0E, 0x03, 0x15, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x16, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x17, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x18, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x19, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x1A, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x1B, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x1C, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x1D, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x1E, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x1F, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x20, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x21, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x22, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x23, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x24, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x25, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x26, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x27, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x28, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x29, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x2A, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x2B, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x2C, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x2D, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x2E, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x2F, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x30, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x31, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x32, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x33, 0x00, 0x00, 0x00, 0x0D, 0xBC, 0x0E, 0x03, 0x34, 0x00, 0x00, 0x00, 0x00, 0xBC, 0x0E, 0x03, 0x06, 0x50, 0x85, 0xE8, 0x00, 0xBC, 0x0E, 0x03, 0xE6, 0x3B, 0x5B, 0x87, 0x00, 0xBC, 0x0E, 0x03, 0xC8, 0x95, 0xF4, 0x81, 0x00, 0xBC, 0x0E, 0x03, 0x8D, 0x0D, 0xC6, 0x8B, 0x00, 0xBC, 0x0E, 0x03, 0xEA, 0xE6, 0x37, 0xD9, 0x00, 0xBC, 0x0E, 0x03, 0xC7, 0xCC, 0x09, 0x2B, 0x00, 0xBC, 0x0E, 0x03, 0xBA, 0xDE, 0xD7, 0x50, 0x00, 0xBC, 0x0E, 0x03, 0x34, 0xD8, 0x5E, 0xE6, 0x00, 0xBC, 0x0E, 0x03, 0x0B, 0xC6, 0xD9, 0xAA });
                }
            }

            // Play Records
            if (SaveInfo.PlayRecords != null)
            {
                dataWriter.Seek(0x8FEB);

                foreach (KeyValuePair<int, List<PlayRecord>> playRecord in SaveInfo.PlayRecords)
                {
                    List<bool> listBool = playRecord.Value.Select(x => x.Unlocked).ToList();

                    while (listBool.Count < 8)
                    {
                        listBool.Add(true);
                    }

                    BitArray bits = new BitArray(listBool.ToArray());
                    byte[] bytes = new byte[1];
                    bits.CopyTo(bytes, 0);
                    dataWriter.WriteByte(bytes[0]);
                }
            }

            // Inventory
            // First Item Category - 608 maximum items
            dataWriter.Seek(0xA394);
            var firstItemCategory = SaveInfo.Inventory.Where(x => x.Value.Category == 1).ToDictionary(x => x.Key, x => x.Value);
            foreach (KeyValuePair<UInt32, Item> item in firstItemCategory)
            {
                if (item.Value.SubCategory != -1)
                {
                    dataWriter.WriteUInt32(item.Key);
                    var newItem = Items.FirstOrDefault(x => x.Value.Name == item.Value.Name && x.Value.SubCategory == item.Value.SubCategory);
                    dataWriter.WriteUInt32(newItem.Key);
                    dataWriter.WriteByte(item.Value.Quantity);
                    dataWriter.Skip(3);
                }
                else
                {
                    dataWriter.Skip(12);
                }
            }

            // Second Item Category - 452 maximum items
            dataWriter.Seek(0xC020);
            var secondItemCategory = SaveInfo.Inventory.Where(x => x.Value.Category == 2).ToDictionary(x => x.Key, x => x.Value);
            foreach (KeyValuePair<UInt32, Item> item in secondItemCategory)
            {
                if (item.Value.SubCategory != -1)
                {
                    dataWriter.WriteUInt32(item.Key);
                    var newItem = Items.FirstOrDefault(x => x.Value.Name == item.Value.Name && x.Value.SubCategory == item.Value.SubCategory);
                    dataWriter.WriteUInt32(newItem.Key);
                    dataWriter.WriteByte(item.Value.Quantity);
                    dataWriter.Skip(7);
                }
                else
                {
                    dataWriter.Skip(16);
                }
            }

            // Third Item Category - 888 Maximum Items
            dataWriter.Seek(0xDC6C);
            var thirdItemCategory = SaveInfo.Inventory.Where(x => x.Value.Category == 3).ToDictionary(x => x.Key, x => x.Value);
            foreach (KeyValuePair<UInt32, Item> item in thirdItemCategory)
            {
                if (item.Value.SubCategory != -1)
                {
                    dataWriter.WriteUInt32(item.Key);
                    var newItem = Items.FirstOrDefault(x => x.Value.Name == item.Value.Name && x.Value.SubCategory == item.Value.SubCategory);
                    dataWriter.WriteUInt32(newItem.Key);
                }
                else
                {
                    dataWriter.Skip(8);
                }
            }

            // Save Player Order
            dataWriter.Seek(0x26E28);
            for (int i = 0; i < 336; i++)
            {
                if (i < PlayersInSaveSort.Count)
                {
                    dataWriter.WriteUInt32(PlayersInSaveSort[i]);
                }
                else
                {
                    dataWriter.WriteUInt32(0x00000000);
                }
            }

            // Save Each Player from Save
            var playerInSaveOrdered = PlayersInSave.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            dataWriter.Seek(0xF83C);
            for (int playerLoop = 0; playerLoop < 336; playerLoop++)
            {
                if (playerLoop < PlayersInSave.Count)
                {
                    var player = PlayersInSave.FirstOrDefault(x => x.Key == PlayersInSaveSort[playerLoop]);

                    if (player.Value.Avatar.IsFightingSpirit == true)
                    {
                        dataWriter.WriteUInt32(0x00);
                    } else
                    {
                        dataWriter.WriteUInt32(Avatars.FirstOrDefault(x => x.Value == player.Value.Avatar).Key);
                    }

                    dataWriter.WriteUInt32(player.Key);
                    dataWriter.WriteUInt32(player.Value.ID);

                    // Save MixiMax Information
                    if (player.Value.MixiMax != null)
                    {
                        UInt32 miximaxPositionID = PlayersInSave.FirstOrDefault(x => x.Value == player.Value.MixiMax.AuraPlayer).Key;
                        dataWriter.WriteUInt32(miximaxPositionID);
                    }
                    else
                    {
                        dataWriter.WriteUInt32(0x0);
                    }

                    dataWriter.Skip(12);
                    dataWriter.WriteInt16(player.Value.Stat[0]);
                    dataWriter.WriteInt16(player.Value.Stat[1]);
                    dataWriter.WriteInt16(player.Value.Freedom);
                    dataWriter.WriteByte(player.Value.Level);

                    // Save MixiMax Moves
                    dataWriter.Skip(1);
                    if (player.Value.MixiMax != null)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            // Console.WriteLine(player.Value.Name + " " + player.Value.MixiMax.MixiMaxMoveNumber[i]);
                            if (player.Value.MixiMax.MixiMaxMoveNumber[i] == 255)
                            {
                                dataWriter.WriteByte(255);
                            }
                            else if (player.Value.MixiMax.MixiMaxMoveNumber[i] > player.Value.MixiMax.AuraPlayer.Moves.Count)
                            {
                                dataWriter.WriteByte(6);
                            }
                            else
                            {
                                dataWriter.WriteByte(player.Value.MixiMax.MixiMaxMoveNumber[i]);
                            }
                        }
                    } 
                    else
                    {
                        dataWriter.WriteByte(0);
                        dataWriter.WriteByte(0);
                    }

                    // Determines the Invoker value and saves it
                    int canInvokeArmed = Convert.ToInt32(player.Value.Invoke) * 8 + Convert.ToInt32(player.Value.Armed) * 16;
                    if (player.Value.MixiMax != null && player.Value.MixiMax.AuraData == true) canInvokeArmed += 1;
                    var playerIsAura = PlayersInSave.FirstOrDefault(x => x.Value.MixiMax != null && x.Value.MixiMax.AuraPlayer == player.Value);
                    if (playerIsAura.Key != 0x0) canInvokeArmed += 2;
                    dataWriter.WriteByte(canInvokeArmed);

                    dataWriter.WriteByte(player.Value.Style * 16);
                    dataWriter.WriteInt16(player.Value.Participation);
                    dataWriter.WriteInt16(player.Value.Score);

                    // Won Duels
                    dataWriter.Skip(2);

                    // Unknown
                    dataWriter.Skip(2);

                    // Save Stat
                    dataWriter.Skip(4);
                    for (int i = 0; i < 8; i++)
                    {
                        dataWriter.WriteInt16(player.Value.InvestedPoint[i]);
                    }

                    // Save Fighting Spirit
                    if (player.Value.Avatar.IsFightingSpirit == true)
                    {
                        dataWriter.WriteUInt32(Avatars.FirstOrDefault(x => x.Value == player.Value.Avatar).Key);
                    } 
                    else
                    {
                        dataWriter.WriteUInt32(0x00);
                    }
                    dataWriter.WriteByte(player.Value.Avatar.Level);

                    // Save Equipments
                    dataWriter.Skip(3);
                    for (int i = 0; i < 4; i++)
                    {
                        var equipmentKeyValue = Equipments.FirstOrDefault(x => x.Value == player.Value.Equipments[i]);
                        if (equipmentKeyValue.Key < 0x4)
                        {
                            dataWriter.WriteUInt32(0x00);
                        }
                        else
                        {
                            dataWriter.WriteUInt32(equipmentKeyValue.Key);
                        }
                    }

                    // Save Moves
                    dataWriter.Skip(4);
                    for (int i = 0; i < 6; i++)
                    {
                        var moveKeyValue = Moves.FirstOrDefault(x => x.Value.Name == player.Value.Moves[i].Name);
                        dataWriter.WriteUInt32(moveKeyValue.Key);
                        dataWriter.WriteByte(player.Value.Moves[i].Level);
                        dataWriter.WriteByte(player.Value.Moves[i].TimeLevel);
                        dataWriter.WriteByte(Convert.ToInt32(player.Value.Moves[i].Unlock));
                        dataWriter.Skip(5);
                    }

                    dataWriter.Skip(100);
                }
                else
                {
                    dataWriter.Write(new byte[268]);
                }
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = Path.GetFileName(initialDirectory.FileName);
            saveFileDialog.Title = "Save IEGOGALAXY save file";
            saveFileDialog.Filter = "IE files|*.ie|IE files decrypted|*.ie";
            saveFileDialog.InitialDirectory = initialDirectory.InitialDirectory;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string saveFileName = saveFileDialog.FileName;

                if (saveFileDialog.FilterIndex == 1)
                {
                    saveData = Level5.Encrypt(saveData);
                }

                System.IO.File.WriteAllBytes(saveFileName, saveData);
                MessageBox.Show("Saved");
            }

            dataWriter.Dispose();
        }

        public void UpdateResource()
        {
            IDictionary<uint, Equipment> AllEquipments = Common.InazumaElevenGo.Equipments.Galaxy;
            Equipments = new Dictionary<UInt32, Equipment>();

            foreach (KeyValuePair<UInt32, Item> item in SaveInfo.Inventory)
            {
                if (item.Value.SubCategory == -1) continue;

                if (AllEquipments.Values.Any(x => x.Name == item.Value.Name) == true)
                {
                    var equipmentKeyValuePair = AllEquipments.FirstOrDefault(x => x.Value.Name == item.Value.Name);
                    Equipment newEquipment = equipmentKeyValuePair.Value;
                    newEquipment.ID = equipmentKeyValuePair.Key;
                    Equipments.Add(item.Key, newEquipment);
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
            newPlayer.Avatar = new Avatar(newAvatar);
            newPlayer.Invoke = invoke;
            newPlayer.Armed = armed;
            newPlayer.Avatar.Level = File.ReadByte();

            File.Skip(3);
            newPlayer.Equipments = new List<Equipment>();
            for (int i = 0; i < 4; i++)
            {
                Equipment newEquipment = null;
                UInt32 equipmentPositionID = File.Reverse(File.ReadUInt32());
                if (equipmentPositionID != 0x0 && Equipments.ContainsKey(equipmentPositionID))
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

        public void NewMixiMax(Player player, UInt32 playerAura, int mixiMaxMove1, int mixiMaxMove2)
        {
            BestMatch isBestMatch = IsBestMatch(player.ID, PlayersInSave[playerAura].ID);

            if (isBestMatch != null)
            {
                player.MixiMax = new MixiMax(PlayersInSave[playerAura], (mixiMaxMove1, mixiMaxMove2), isBestMatch);
            }
            else
            {
                player.MixiMax = new MixiMax(PlayersInSave[playerAura], (mixiMaxMove1, mixiMaxMove2));
            }

            PlayersInSave[playerAura].IsAura = true;
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
