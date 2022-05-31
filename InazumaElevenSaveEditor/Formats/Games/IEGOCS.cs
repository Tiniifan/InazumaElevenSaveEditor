using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using InazumaElevenSaveEditor.Tools;
using InazumaElevenSaveEditor.Logic;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using InazumaElevenSaveEditor.Common.InazumaElevenGo;


namespace InazumaElevenSaveEditor.Formats.Games
{
    public class CS : ContainerGames
    {
        public DataReader File { get; set; }

        public string GameNameCode => "IEGOCS";

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

            // Second Item Category - 336 maximum items
            File.Seek(0x2D00);
            for (int i = 0; i < 336; i++)
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

            // Third Item Category - 800 Maximum Items
            File.Seek(0x420C);
            for (int i = 0; i < 800; i++)
            {
                UInt32 itemPositionID = File.Reverse(File.ReadUInt32());

                if (itemPositionID == 0x0)
                {
                    break;
                }
                else
                {
                    UInt32 itemID = File.Reverse(File.ReadUInt32());

                    // Fills The Aura List
                    if (Auras.ContainsKey(itemID))
                    {
                        Player newAura = Auras[itemID];
                        newAura.ID = itemID;
                        newAura.Moves = new List<Move>();
                        foreach (UInt32 move in Auras[itemID].UInt32Moves)
                        {
                            newAura.Moves.Add(Moves[move]);
                        }
                        AuraInSave.Add(itemPositionID, newAura);
                    }

                    SaveInfo.Inventory.Add(itemPositionID, Items[itemID]);
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

                    // True = The aura is a Player
                    // False = The aura is a Item
                    if (PlayersInSave.ContainsKey(playerAura))
                    {
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
                    else
                    {
                        BestMatch isBestMatch = IsBestMatch(player.Value.ID, AuraInSave[playerAura].ID);
                        MixiMax newMixiMax;
                        if (isBestMatch != null)
                        {
                            newMixiMax = new MixiMax(PlayersInSave[playerAura], (mixiMaxMove1, mixiMaxMove2), isBestMatch);
                        }
                        else
                        {
                            newMixiMax = new MixiMax(AuraInSave[playerAura], (mixiMaxMove1, mixiMaxMove2));
                        }
                        newMixiMax.AuraData = true;
                        player.Value.MixiMax = newMixiMax;
                    }
                }
            }
        }

        private Player LoadPlayer()
        {
            UInt32 playerID = File.Reverse(File.ReadUInt32());
            Player newPlayer = new Player(Players[playerID]);
            newPlayer.ID = playerID;
            newPlayer.PositionInFile = File.BaseStream.Position - 8;

            File.Skip(10);
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

            File.Skip(8);
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
                    newMove = Moves[moveID];
                }

                newMove.Level = File.ReadByte();
                File.Skip(1);
                newMove.Unlock = Convert.ToBoolean(File.ReadByte());
                newPlayer.Moves.Add(newMove);
                File.Skip(5);
            }

            File.Skip(100);

            return newPlayer;
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

        private BestMatch IsBestMatch(UInt32 playerID, UInt32 auraID)
        {
            foreach (BestMatch miximax in BestMatchs)
            {
                if (miximax.IsBestMatch(playerID, auraID))
                {
                    return miximax;
                }
                break;
            }
            return null;
        }

        public Player GetPlayer(int index)
        {
            return PlayersInSave[PlayersInSaveSort[index]];
        }

        public void GetStat(Player player, Control form)
        {
        }

        public string ConvertPlayerToString(Player player, bool clipboard)
        {
            return "";
        }

        public void Save(OpenFileDialog initialDirectory)
        {

        }

        public Player ReadPlayer(Player player)
        {
            return new Player();
        }

        public void NewStat(Player player, NumericUpDown upStat)
        {

        }
    }
}