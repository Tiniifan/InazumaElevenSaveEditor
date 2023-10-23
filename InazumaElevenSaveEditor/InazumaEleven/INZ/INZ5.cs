using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using InazumaElevenSaveEditor.Tools;
using InazumaElevenSaveEditor.InazumaEleven.Logic;

namespace InazumaElevenSaveEditor.InazumaEleven.INZ
{
    public class INZ5 : IPlayerFiles
    {
        public string Extension => ".inz5";

        public string GameNameCode => "IEGOCS";

        private IDictionary<UInt32, Player> Players => Common.GO.Players.Cs;

        private IDictionary<UInt32, Move> Moves => Common.GO.Moves.Cs;

        private IDictionary<UInt32, Avatar> Avatars => Common.GO.Avatars.Cs;

        private IDictionary<UInt32, Equipment> Equipments => Common.GO.Equipments.Cs;

        private IList<BestMatch> BestMatchs = Common.GO.BestMatchs.Cs;

        public INZ5()
        {
        }

        private string PlayerToString(Player player)
        {
            string exportContent = string.Join("\n", new String[]
            {
                player.Name,
                player.Level.ToString(),
                player.Stat[0].ToString() + "|" + player.Stat[1].ToString(),
                player.Style.ToString(),
                player.Score.ToString() + "|" + player.Participation.ToString(),
                string.Join( "|", player.InvestedPoint),
                player.Avatar.Name + "|" + player.Avatar.Level.ToString() + "|" + player.Invoke + "|" + player.Armed,
                player.Moves[0].Name + "|" + player.Moves[0].Level.ToString() + "|" + player.Moves[0].UsedCount + "|" + player.Moves[0].Unlock,
                player.Moves[1].Name + "|" + player.Moves[1].Level.ToString() + "|" + player.Moves[1].UsedCount + "|" + player.Moves[1].Unlock,
                player.Moves[2].Name + "|" + player.Moves[2].Level.ToString() + "|" + player.Moves[2].UsedCount + "|" + player.Moves[2].Unlock,
                player.Moves[3].Name + "|" + player.Moves[3].Level.ToString() + "|" + player.Moves[3].UsedCount + "|" + player.Moves[3].Unlock,
                player.Moves[4].Name + "|" + player.Moves[4].Level.ToString() + "|" + player.Moves[4].UsedCount + "|" + player.Moves[4].Unlock,
                player.Moves[5].Name + "|" + player.Moves[5].Level.ToString() + "|" + player.Moves[5].UsedCount + "|" + player.Moves[5].Unlock,
                player.Equipments[0].Name,
                player.Equipments[1].Name,
                player.Equipments[2].Name,
                player.Equipments[3].Name,
            });

            return exportContent;
        }

        private Player StringToPlayer(List<string> playerString, bool keepInvestedPoint)
        {
            KeyValuePair<UInt32, Player> playerKeyValuePair = Players.FirstOrDefault(x => x.Value.Name == playerString[0]);
            if (playerKeyValuePair.Value == null) return null;

            Player player = new Player(playerKeyValuePair.Value);
            player.ID = playerKeyValuePair.Key;

            player.Level = Convert.ToInt32(playerString[1]);
            List<int> gpTp = playerString[2].Split('|').Select(Int32.Parse).ToList();
            for (int i = 0; i < 2; i++)
            {
                player.Stat[i] = gpTp[i];
            }
            player.Style = Convert.ToInt32(playerString[3]);
            List<int> scores = playerString[4].Split('|').Select(Int32.Parse).ToList();
            player.Score = scores[0];
            player.Participation = scores[1];

            player.InvestedPoint = new List<int>();
            List<int> investedPoints = playerString[5].Split('|').Select(Int32.Parse).ToList();
            for (int i = 0; i < investedPoints.Count; i++)
            {
                if (keepInvestedPoint == true)
                {
                    player.InvestedPoint.Add(investedPoints[i]);
                }
                else
                {
                    player.InvestedPoint.Add(0);
                }
            }

            List<string> avatarValues = playerString[6].Split('|').ToList();
            Avatar avatar = Avatars.FirstOrDefault(x => x.Value.Name == avatarValues[0]).Value;
            if (avatar == null)
            {
                avatar = Avatars[0x0];
            }
            player.Avatar = avatar;
            player.Avatar.Level = Convert.ToInt32(avatarValues[1]);
            player.Invoke = Convert.ToBoolean(avatarValues[2]);
            player.Armed = Convert.ToBoolean(avatarValues[3]);

            player.Moves = new List<Move>();
            for (int i = 0; i < 6; i++)
            {
                List<string> moveValues = playerString[7 + i].Split('|').ToList();
                Move move = Moves.FirstOrDefault(x => x.Value.Name == moveValues[0]).Value;

                if (move != null)
                {
                    move.Level = Convert.ToInt32(moveValues[1]);
                    move.UsedCount = Convert.ToInt32(moveValues[2]);
                    move.Unlock = Convert.ToBoolean(moveValues[3]);
                }
                else
                {
                    if (i < 4)
                    {
                        move = Moves[player.UInt32Moves[i]];
                        move.UsedCount = move.EvolutionSpeed.TimeLevel[0];
                    }
                    else
                    {
                        move = Moves[0x0];
                        move.UsedCount = 0;
                    }

                    move.Level = 1;
                    move.Unlock = Convert.ToBoolean(moveValues[3]);
                }

                player.Moves.Add(move);
            }

            player.Equipments = new List<Equipment>();
            for (int i = 0; i < 4; i++)
            {
                List<string> equipmentName = playerString[13 + i].Split('|').ToList();
                Equipment equipment = Equipments.FirstOrDefault(x => x.Value.Name == equipmentName[0]).Value;

                if (equipment == null)
                {
                    equipment = Equipments[0x00];
                }

                player.Equipments.Add(equipment);
            }

            return player;
        }

        public string NewFile(Player player)
        {
            string newFileContent = GameNameCode;
            newFileContent += "\n" + PlayerToString(player);

            if (player.MixiMax != null && player.MixiMax.AuraData == false)
            {
                newFileContent = string.Join("\n", new String[]
                {
                    newFileContent,
                    "_____________________________",
                    player.MixiMax.MixiMaxMoveNumber[0] + "|" + player.MixiMax.MixiMaxMoveNumber[1],
                    PlayerToString(player.MixiMax.AuraPlayer)
                });
            }

            return BitConverter.ToString(new Crc32().ComputeHash(Encoding.UTF8.GetBytes(newFileContent))).Replace("-", string.Empty) + "\n" + newFileContent;
        }

        public Player NewPlayer(string path)
        {
            bool isSameGame = File.ReadLines(path).Skip(1).First() == GameNameCode;
            bool hasMiximax = File.ReadLines(path).Count() > 19;

            Player player = StringToPlayer(File.ReadLines(path).Skip(2).Take(18).ToList(), isSameGame);

            if (player != null & hasMiximax == true)
            {
                Player playerAura = StringToPlayer(File.ReadLines(path).Skip(21).Take(18).ToList(), isSameGame);

                if (playerAura != null)
                {
                    List<int> miximaxMoves = File.ReadLines(path).Skip(20).First().Split('|').Select(Int32.Parse).ToList();
                    NewMixiMax(player, playerAura, miximaxMoves[0], miximaxMoves[1]);
                }
            }

            return player;
        }

        private void NewMixiMax(Player player, Player playerAura, int mixiMaxMove1, int mixiMaxMove2)
        {
            BestMatch isBestMatch = IsBestMatch(player.ID, playerAura.ID);
            MixiMax newMixiMax;

            if (isBestMatch != null)
            {
                newMixiMax = new MixiMax(playerAura, (mixiMaxMove1, mixiMaxMove2), isBestMatch);
            }
            else
            {
                newMixiMax = new MixiMax(playerAura, (mixiMaxMove1, mixiMaxMove2));
            }

            newMixiMax.AuraData = false;
            player.MixiMax = newMixiMax;
            playerAura.IsAura = true;
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
    }
}
