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
    public class CS : IGame
    {
        public DataReader File { get; set; }

        public string GameNameCode => "IEGOCS";

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

        public CS()
        {

        }

        public CS(DataReader file)
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

            UInt32 newIndex = (uint)missingIndex * 0x1000000 + (uint)(missingIndex + 1) * 0x100;
            PlayersInSaveSort.Add(newIndex);
            player.PositionInFile = 23324 + 260 * PlayersInSaveSort.Count;
            PlayersInSave.Add(newIndex, player);
        }

        public void Open()
        {
            // Initialize Resources
            Players = Common.InazumaElevenGo.Players.Cs;
            Moves = Common.InazumaElevenGo.Moves.Cs;
            Avatars = Common.InazumaElevenGo.Avatars.Cs;
            Items = Common.InazumaElevenGo.Items.Cs;

            IDictionary<uint, Equipment> AllEquipments = Common.InazumaElevenGo.Equipments.Cs;
            Equipments = new Dictionary<UInt32, Equipment>();
            IDictionary<uint, Player> Auras = Common.InazumaElevenGo.Auras.Cs;
            AuraInSave = new Dictionary<UInt32, Player>();

            SaveInfo = new Save();

            // First Item Category - 512 maximum items
            File.Seek(0x14F4);
            for (int i = 0; i < 512; i++)
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

            // Second Item Category - 336 maximum items
            File.Seek(0x2D00);
            for (int i = 0; i < 336; i++)
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

            // Third Item Category - 800 Maximum Items
            File.Seek(0x420C);
            for (int i = 0; i < 800; i++)
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

                    if (Auras.ContainsKey(itemID))
                    {
                        Player newAura = Auras[itemID];
                        newAura.ID = itemID;
                        newAura.Moves = new List<Move>();
                        foreach (UInt32 move in Auras[itemID].UInt32Moves)
                        {
                            newAura.Moves.Add(Moves[move]);
                        }
                        AuraInSave.Add(index, newAura);
                    }
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
            File.Seek(0x1C4E0);
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
            File.Seek(0x5B1C);
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
                    File.Skip(20);
                    UInt32 playerAura = File.Reverse(File.ReadUInt32());

                    File.Skip(8);
                    int mixiMaxMove1 = File.ReadByte();
                    int mixiMaxMove2 = File.ReadByte();

                    NewMixiMax(player.Value, playerAura, mixiMaxMove1, mixiMaxMove2);
                }
            }
        }

        public void OpenTactics()
        {
            SaveInfo.Teams = new List<Team>();

            // Chrono Storm Team
            SaveInfo.Teams.Add(LoadTeam(0x1C0C4, 0x1C494, 0x1C4E0));

            // Custom Teams
            uint teamInfo = 0x1C0F4;
            uint teamName = 0x1C304;
            uint teamPlayers = 0x1CA60;
            for (int i = 0; i < 10; i++)
            {
                SaveInfo.Teams.Add(LoadTeam(teamInfo, teamName, teamPlayers));
                teamInfo += 0x30;
                teamName += 0x28;
                teamPlayers += 0x40;
            }
        }

        public void OpenSaveInfo()
        {
            if (SaveInfo.Name == null)
            {
                // Name
                File.Seek(0);
                SaveInfo.Name = Encoding.UTF8.GetString(File.GetSection(60, 12).TakeWhile((x, index) => x != 0x0 && index < 11).ToArray());
                SaveInfo.TeamName = Encoding.UTF8.GetString(File.GetSection(100, 12).TakeWhile((x, index) => x != 0x0 && index < 12).ToArray());

                // Time Hours
                File.Skip(32);
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
                File.Skip(912);
                SaveInfo.SecretLinkLevel = File.ReadByte();

                // Current Chapter
                File.Skip(3399);
                SaveInfo.Chapter = File.ReadByte();

                // Money
                File.Skip(110515);
                SaveInfo.Prestige = File.ReadInt32();
                SaveInfo.Friendship = File.ReadInt32();
            }
        }

        public void OpenPlayRecords()
        {
            if (SaveInfo.PlayRecords == null)
            {
                SaveInfo.PlayRecords = Common.InazumaElevenGo.PlayRecords.Cs;

                File.Seek(0x2EC);

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
                dataWriter.Seek(0x20);
                int time = SaveInfo.Hours * 3600 + SaveInfo.Min * 60;
                dataWriter.Write(time);

                dataWriter.Seek(0x3C);
                dataWriter.Write(Encoding.UTF8.GetBytes(SaveInfo.Name));
                dataWriter.WriteByte(0x0);
                dataWriter.WriteByte(0x88);
                dataWriter.Seek(0x64);
                dataWriter.Write(Encoding.UTF8.GetBytes(SaveInfo.TeamName));
                dataWriter.WriteByte(0x0);
                dataWriter.WriteByte(0x88);

                dataWriter.Seek(0x3B4);
                dataWriter.WriteByte(SaveInfo.SecretLinkLevel);

                dataWriter.Seek(0x1C0B0);
                dataWriter.Write(SaveInfo.Prestige);
                dataWriter.Write(SaveInfo.Friendship);

                if (SaveInfo.UnlockAllData == true)
                {
                    dataWriter.Seek(0x2FB);
                    dataWriter.Write(new byte[6] { 0xFF, 0xFF, 0x01, 0x00, 0xFE, 0x3F });

                    dataWriter.Seek(0x305);
                    dataWriter.Write(new byte[6] { 0xE0, 0xF8, 0xFF, 0xFF, 0x0F, 0xFF });

                    dataWriter.Seek(0x322);
                    dataWriter.WriteByte(0x18);

                    dataWriter.Seek(0x32F);
                    dataWriter.Write(new byte[9] { 0xF8, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F, 0x50, 0x02 });

                    dataWriter.Seek(0x339);
                    dataWriter.WriteByte(0x78);

                    // Ugly Write
                    dataWriter.Seek(0x24A40);
                    dataWriter.Write(new byte[864] { 0xFE, 0xFF, 0x76, 0x08, 0x0D, 0x04, 0x04, 0x00, 0x6B, 0x00, 0x00, 0x00, 0xE4, 0xFF, 0xFF, 0xFF, 0x91, 0x07, 0xD4, 0x64, 0xE4, 0xFF, 0xFF, 0xFF, 0x1F, 0x55, 0x33, 0x42, 0xE0, 0xFF, 0xFF, 0xFF, 0xB2, 0xF0, 0x63, 0xAB, 0xE0, 0xFF, 0xFF, 0xFF, 0xCA, 0x40, 0x78, 0xBB, 0xE0, 0xFF, 0xFF, 0xFF, 0x60, 0x7F, 0x23, 0xBD, 0xE1, 0xFF, 0xFF, 0xFF, 0x7D, 0xB2, 0xA9, 0xC7, 0xE1, 0xFF, 0xFF, 0xFF, 0x83, 0xBF, 0xC5, 0x53, 0xE1, 0xFF, 0xFF, 0xFF, 0x1D, 0x97, 0xC2, 0x1E, 0xE1, 0xFF, 0xFF, 0xFF, 0x1A, 0xE3, 0xBB, 0x29, 0xE1, 0xFF, 0xFF, 0xFF, 0x53, 0xCA, 0xA0, 0x0A, 0xE1, 0xFF, 0xFF, 0xFF, 0x0E, 0x84, 0x0E, 0xCB, 0xE2, 0xFF, 0xFF, 0xFF, 0xC4, 0xB4, 0x8A, 0x81, 0xE0, 0xFF, 0xFF, 0xFF, 0x75, 0xAA, 0xA9, 0xFD, 0xE0, 0xFF, 0xFF, 0xFF, 0x74, 0x1B, 0xA6, 0x39, 0xE0, 0xFF, 0xFF, 0xFF, 0xE2, 0x2B, 0xA1, 0x4E, 0xE1, 0xFF, 0xFF, 0xFF, 0xD7, 0x18, 0xB8, 0x08, 0xE1, 0xFF, 0xFF, 0xFF, 0xF6, 0x39, 0x4C, 0x6F, 0xE1, 0xFF, 0xFF, 0xFF, 0x6D, 0x54, 0x0F, 0x78, 0xE1, 0xFF, 0xFF, 0xFF, 0x3A, 0x06, 0x18, 0x49, 0xE1, 0xFF, 0xFF, 0xFF, 0xE5, 0x90, 0x63, 0x06, 0xE1, 0xFF, 0xFF, 0xFF, 0x35, 0xD0, 0x82, 0xF8, 0xE4, 0xFF, 0xFF, 0xFF, 0x0A, 0x1F, 0x24, 0x90, 0xE0, 0xFF, 0xFF, 0xFF, 0x58, 0x7A, 0xA8, 0xD7, 0xE0, 0xFF, 0xFF, 0xFF, 0xBD, 0x09, 0x50, 0x6F, 0xE0, 0xFF, 0xFF, 0xFF, 0xAF, 0x3D, 0xE9, 0xD1, 0xE2, 0xFF, 0xFF, 0xFF, 0x7E, 0xE5, 0x83, 0x18, 0xE0, 0xFF, 0xFF, 0xFF, 0x3D, 0xC8, 0x82, 0xC2, 0xE0, 0xFF, 0xFF, 0xFF, 0xE4, 0xB7, 0x16, 0x6D, 0xE1, 0xFF, 0xFF, 0xFF, 0x0A, 0x74, 0xE6, 0x08, 0xE1, 0xFF, 0xFF, 0xFF, 0x0C, 0xA8, 0x8D, 0x4F, 0xE1, 0xFF, 0xFF, 0xFF, 0x63, 0x37, 0x39, 0x50, 0xE1, 0xFF, 0xFF, 0xFF, 0x79, 0x94, 0xE7, 0xCF, 0xE1, 0xFF, 0xFF, 0xFF, 0x40, 0x80, 0xAD, 0x0F, 0xE1, 0xFF, 0xFF, 0xFF, 0x60, 0x50, 0x8A, 0xC8, 0xE0, 0xFF, 0xFF, 0xFF, 0x91, 0x04, 0x1B, 0xF6, 0xE0, 0xFF, 0xFF, 0xFF, 0x7A, 0x9F, 0x11, 0x20, 0xE0, 0xFF, 0xFF, 0xFF, 0x37, 0xE1, 0xBF, 0x95, 0xE0, 0xFF, 0xFF, 0xFF, 0xCE, 0x4A, 0xAF, 0xA0, 0xE1, 0xFF, 0xFF, 0xFF, 0xE9, 0xAE, 0x2D, 0x34, 0xE1, 0xFF, 0xFF, 0xFF, 0x44, 0x45, 0xC1, 0x6B, 0xE1, 0xFF, 0xFF, 0xFF, 0x86, 0xD2, 0xBB, 0x47, 0xE1, 0xFF, 0xFF, 0xFF, 0xD0, 0x35, 0x00, 0xEF, 0xE1, 0xFF, 0xFF, 0xFF, 0x15, 0x6C, 0xE0, 0x48, 0xE1, 0xFF, 0xFF, 0xFF, 0x77, 0x65, 0x08, 0x15, 0xE4, 0xFF, 0xFF, 0xFF, 0x22, 0xBB, 0x46, 0x1A, 0xE0, 0xFF, 0xFF, 0xFF, 0xD7, 0x74, 0xFD, 0x7F, 0xE0, 0xFF, 0xFF, 0xFF, 0xBA, 0xF2, 0x4E, 0x43, 0xE0, 0xFF, 0xFF, 0xFF, 0xD7, 0x8E, 0xC2, 0xA7, 0xE4, 0xFF, 0xFF, 0xFF, 0x79, 0xA6, 0xE4, 0x87, 0xE4, 0xFF, 0xFF, 0xFF, 0x33, 0x34, 0x3D, 0xAC, 0xE4, 0xFF, 0xFF, 0xFF, 0xCE, 0x9E, 0x22, 0xAA, 0xE1, 0xFF, 0xFF, 0xFF, 0xE5, 0x46, 0xC5, 0xCD, 0xE1, 0xFF, 0xFF, 0xFF, 0x38, 0x46, 0xDE, 0xBA, 0xE1, 0xFF, 0xFF, 0xFF, 0x84, 0xFE, 0x38, 0xC3, 0xE1, 0xFF, 0xFF, 0xFF, 0x6E, 0x77, 0xC3, 0xD9, 0xE1, 0xFF, 0xFF, 0xFF, 0xE4, 0x49, 0x8A, 0xE8, 0xE1, 0xFF, 0xFF, 0xFF, 0x6E, 0x42, 0x47, 0x7E, 0xE0, 0xFF, 0xFF, 0xFF, 0xAE, 0x8C, 0xE6, 0x15, 0xE0, 0xFF, 0xFF, 0xFF, 0x5F, 0x57, 0x10, 0x30, 0xE0, 0xFF, 0xFF, 0xFF, 0xC9, 0x67, 0x17, 0x47, 0xE1, 0xFF, 0xFF, 0xFF, 0xE8, 0x6A, 0x7A, 0x33, 0xE1, 0xFF, 0xFF, 0xFF, 0xE4, 0x38, 0x71, 0x76, 0xE1, 0xFF, 0xFF, 0xFF, 0xA0, 0x15, 0xEF, 0xE5, 0xE1, 0xFF, 0xFF, 0xFF, 0x63, 0xB8, 0x5E, 0x4B, 0xE1, 0xFF, 0xFF, 0xFF, 0x77, 0xEA, 0x6F, 0x0E, 0xE1, 0xFF, 0xFF, 0xFF, 0x70, 0x24, 0xF5, 0x85, 0xE4, 0xFF, 0xFF, 0xFF, 0x0C, 0x14, 0xE6, 0x6F, 0xE4, 0xFF, 0xFF, 0xFF, 0xB4, 0x35, 0xEA, 0xAC, 0xE4, 0xFF, 0xFF, 0xFF, 0x90, 0xA1, 0x59, 0x32, 0xE1, 0xFF, 0xFF, 0xFF, 0x22, 0xE5, 0x00, 0x25, 0xE1, 0xFF, 0xFF, 0xFF, 0x41, 0x44, 0xFA, 0x08, 0xE1, 0xFF, 0xFF, 0xFF, 0x6E, 0xCD, 0x20, 0x65, 0xE1, 0xFF, 0xFF, 0xFF, 0xA4, 0x42, 0x5A, 0x73, 0xE1, 0xFF, 0xFF, 0xFF, 0xFB, 0x64, 0x08, 0x0F, 0xE1, 0xFF, 0xFF, 0xFF, 0x92, 0x63, 0xA8, 0x6E, 0xE0, 0xFF, 0xFF, 0xFF, 0x74, 0x37, 0x3F, 0x2A, 0xE0, 0xFF, 0xFF, 0xFF, 0xCE, 0x66, 0x36, 0xB3, 0xE0, 0xFF, 0xFF, 0xFF, 0x58, 0x56, 0x31, 0xC4, 0xE0, 0xFF, 0xFF, 0xFF, 0xC9, 0x4B, 0x8E, 0x54, 0xE0, 0xFF, 0xFF, 0xFF, 0x14, 0xDD, 0xEF, 0x8C, 0xE0, 0xFF, 0xFF, 0xFF, 0xB7, 0x48, 0x8B, 0x12, 0xE1, 0xFF, 0xFF, 0xFF, 0x07, 0xD7, 0x3E, 0xED, 0xE1, 0xFF, 0xFF, 0xFF, 0x7B, 0x37, 0x03, 0x50, 0xE1, 0xFF, 0xFF, 0xFF, 0x35, 0x6A, 0x61, 0x44, 0xE1, 0xFF, 0xFF, 0xFF, 0xE1, 0x83, 0xA9, 0xA9, 0xE1, 0xFF, 0xFF, 0xFF, 0xFB, 0xF6, 0xD1, 0xFD, 0xE1, 0xFF, 0xFF, 0xFF, 0xD4, 0x13, 0x4E, 0xE7, 0xE4, 0xFF, 0xFF, 0xFF, 0x26, 0x7E, 0x2A, 0x7E, 0xE4, 0xFF, 0xFF, 0xFF, 0x73, 0x1D, 0x00, 0x22, 0xE4, 0xFF, 0xFF, 0xFF, 0xF1, 0x7F, 0x36, 0x10, 0xE4, 0xFF, 0xFF, 0xFF, 0x88, 0x7D, 0x12, 0xA2, 0xE1, 0xFF, 0xFF, 0xFF, 0x48, 0x98, 0xAD, 0x35, 0xE1, 0xFF, 0xFF, 0xFF, 0xFD, 0xFC, 0x1C, 0x71, 0xE1, 0xFF, 0xFF, 0xFF, 0xED, 0x07, 0x04, 0x27, 0xE1, 0xFF, 0xFF, 0xFF, 0x0C, 0x4B, 0xAF, 0x23, 0xE1, 0xFF, 0xFF, 0xFF, 0x6D, 0x25, 0xF4, 0xE6, 0xE1, 0xFF, 0xFF, 0xFF, 0x6A, 0x64, 0x09, 0x76, 0xE4, 0xFF, 0xFF, 0xFF, 0x5F, 0x7C, 0x0E, 0xCC, 0xE4, 0xFF, 0xFF, 0xFF, 0xA3, 0xCE, 0x37, 0x5D, 0xE0, 0xFF, 0xFF, 0xFF, 0x88, 0x52, 0xAF, 0x03, 0xE0, 0xFF, 0xFF, 0xFF, 0xF3, 0x0D, 0xF3, 0xAB, 0xE0, 0xFF, 0xFF, 0xFF, 0x65, 0x3D, 0xF4, 0xDC, 0xE8, 0xFF, 0xFF, 0xFF, 0x52, 0x84, 0x8D, 0xF6, 0xEC, 0xFF, 0xFF, 0xFF, 0x3F, 0xF2, 0xF4, 0x4F, 0xE6, 0xFF, 0xFF, 0xFF, 0xFB, 0xEF, 0xCC, 0x49, 0xE6, 0xFF, 0xFF, 0xFF, 0x41, 0xBE, 0xC5, 0xD0, 0xE6, 0xFF, 0xFF, 0xFF });
                }
            }

            // Play Records
            if (SaveInfo.PlayRecords != null)
            {
                dataWriter.Seek(0x2EC);

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
            // First Item Category - 512 maximum items
            dataWriter.Seek(0x14F4);
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

            // Second Item Category - 336 maximum items
            dataWriter.Seek(0x2D00);
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

            // Third Item Category - 800 Maximum Items
            dataWriter.Seek(0x420C);
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
            dataWriter.Seek(0x1C4E0);
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
            dataWriter.Seek(0x5B1C);
            for (int playerLoop = 0; playerLoop < 336; playerLoop++)
            {
                if (playerLoop < PlayersInSave.Count)
                {
                    var player = PlayersInSave.FirstOrDefault(x => x.Key == PlayersInSaveSort[playerLoop]);

                    dataWriter.WriteUInt32(player.Key);
                    dataWriter.WriteUInt32(player.Value.ID);

                    dataWriter.Skip(4);
                    dataWriter.WriteInt16(player.Value.Stat[0]);
                    dataWriter.WriteInt16(player.Value.Stat[1]);
                    dataWriter.WriteInt16(player.Value.Freedom);
                    dataWriter.WriteByte(player.Value.Level);

                    // Save MixiMax Information
                    dataWriter.Skip(1);
                    if (player.Value.MixiMax != null)
                    {
                        UInt32 miximaxPositionID = 0;

                        if (player.Value.MixiMax.AuraData == true)
                        {
                            miximaxPositionID = AuraInSave.FirstOrDefault(x => x.Value == player.Value.MixiMax.AuraPlayer).Key;

                        }
                        else
                        {
                            miximaxPositionID = PlayersInSave.FirstOrDefault(x => x.Value == player.Value.MixiMax.AuraPlayer).Key;
                        }

                        dataWriter.WriteUInt32(miximaxPositionID);

                        // Moves Obtained
                        dataWriter.Skip(8);
                        for (int i = 0; i < 2; i++)
                        {
                            if (player.Value.MixiMax.MixiMaxMoveNumber[i] > player.Value.MixiMax.AuraPlayer.Moves.Count)
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
                        dataWriter.WriteUInt32(0x0);
                        dataWriter.Skip(10);
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

                    // Save Stat
                    dataWriter.Skip(4);
                    for (int i = 0; i < 8; i++)
                    {
                        dataWriter.WriteInt16(player.Value.InvestedPoint[i]);
                    }

                    // Save Fighting Spirit
                    dataWriter.WriteUInt32(Avatars.FirstOrDefault(x => x.Value == player.Value.Avatar).Key);
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
                    dataWriter.Write(new byte[260]);
                }
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = Path.GetFileName(initialDirectory.FileName);
            saveFileDialog.Title = "Save IEGOCS save file";
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
            IDictionary<uint, Equipment> AllEquipments = Common.InazumaElevenGo.Equipments.Cs;
            Equipments = new Dictionary<UInt32, Equipment>();
            IDictionary<uint, Player> Auras = Common.InazumaElevenGo.Auras.Cs;
            AuraInSave = new Dictionary<UInt32, Player>();

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
            UInt32 playerID = File.Reverse(File.ReadUInt32());
            Player newPlayer = new Player(Players[playerID]);
            newPlayer.ID = playerID;
            newPlayer.PositionInFile = File.BaseStream.Position - 8;

            File.Skip(4);
            newPlayer.Stat[0] = File.ReadInt16();
            newPlayer.Stat[1] = File.ReadInt16();

            File.Skip(2);
            newPlayer.Level = File.ReadByte();
            if (newPlayer.Level > 99)
            {
                newPlayer.Level = 99;
            }

            File.Skip(1);
            UInt32 miximaxPositionID = File.Reverse(File.ReadUInt32());
            if (miximaxPositionID != 0x0)
            {
                newPlayer.MixiMax = new MixiMax();
            }

            File.Skip(10);
            int canInvokeArmed = File.ReadByte();
            bool invoke = (canInvokeArmed & 8) != 0;
            bool armed = (canInvokeArmed & 16) != 0;

            newPlayer.Style = (File.ReadByte() & 0xF0) >> 4;
            newPlayer.Participation = File.ReadUInt16();
            newPlayer.Score = File.ReadUInt16();

            File.Skip(4);
            newPlayer.InvestedPoint = new List<int>();
            for (int i = 0; i < 8; i++)
            {
                int investedPoint = File.ReadInt16();
                newPlayer.InvestedPoint.Add(investedPoint);
            }

            Avatar newAvatar = Avatars[File.Reverse(File.ReadUInt32())];
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

            File.Skip(100);

            return newPlayer;
        }

        public void NewMixiMax(Player player, UInt32 playerAura, int mixiMaxMove1, int mixiMaxMove2)
        {
            // True = The aura is a Player & False = The aura is a Item
            if (PlayersInSave.ContainsKey(playerAura))
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
            else
            {
                BestMatch isBestMatch = IsBestMatch(player.ID, AuraInSave[playerAura].ID);
                MixiMax newMixiMax;

                if (isBestMatch != null)
                {
                    newMixiMax = new MixiMax(AuraInSave[playerAura], (mixiMaxMove1, mixiMaxMove2), isBestMatch);
                }
                else
                {
                    newMixiMax = new MixiMax(AuraInSave[playerAura], (mixiMaxMove1, mixiMaxMove2));
                }

                newMixiMax.AuraData = true;
                player.MixiMax = newMixiMax;
                AuraInSave[playerAura].IsAura = true;
            }
        }

        private Team LoadTeam(uint teamInfo, uint teamName, uint teamPlayers)
        {
            File.Seek(teamInfo);

            Dictionary<UInt32, Player> players = new Dictionary<UInt32, Player>();
            List<int> playersFormationIndex = new List<int>();
            List<int> playersKitNumber = new List<int>();

            Item coach = SaveInfo.Inventory[File.Reverse(File.ReadUInt32())];
            Item formation = SaveInfo.Inventory[File.Reverse(File.ReadUInt32())];
            Item kit = SaveInfo.Inventory[File.Reverse(File.ReadUInt32())];
            Item emblem = SaveInfo.Inventory[File.Reverse(File.ReadUInt32())];

            for (int i = 0; i < 16; i++)
            {
                playersFormationIndex.Add(File.ReadByte());
            }
            for (int i = 0; i < 16; i++)
            {
                playersKitNumber.Add(File.ReadByte());
            }

            File.Seek(teamName);
            string name = Encoding.UTF8.GetString(File.GetSection((uint)File.BaseStream.Position, 12).TakeWhile((x, index) => x != 0x0 && index < 12).ToArray());

            File.Seek(teamPlayers);
            for (int i = 0; i < 16; i++)
            {
                UInt32 playerPositionID = File.Reverse(File.ReadUInt32());

                if (playerPositionID != 0)
                {
                    players.Add(playerPositionID, PlayersInSave[playerPositionID]);
                }
                else
                {
                    players.Add((uint)(playerPositionID+i), null);
                }
            }

            return new Team(name, emblem, kit, formation, coach, players, playersFormationIndex, playersKitNumber);
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
            } else
            {
                return null;
            }
        }

        public (int, int, string, bool) Training(Player player, int newStat, int statIndex)
        {
            // (int, int, string, bool) = (numericUpDown.Minimum, numericUpDown.Maximum, numericUpDownName, numericUpDownEnabled)

            if (player.InvestedFreedom.Sum() < player.Freedom)
            {
                player.InvestedFreedom[statIndex] = newStat;
                player.InvestedPoint[statIndex] = newStat;
                return (0, player.Freedom+1, "investedNumericUpDown" + (statIndex+3), true);
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

                return (player.InvestedFreedom[statIndex], player.InvestedFreedom[statIndex] + player.Stat[statRemoveIndex + 2] - 1, "investedNumericUpDown"+(statRemoveIndex+3), false);
            }
        }
    }
}