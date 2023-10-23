using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using InazumaElevenSaveEditor.Tools;
using InazumaElevenSaveEditor.InazumaEleven.Logic;

namespace InazumaElevenSaveEditor.InazumaEleven.INZ
{
    public class INZ4 : IPlayerFiles
    {
        public string Extension => ".inz4";

        public string GameNameCode => "IEGO";

        private IDictionary<UInt32, Player> Players => Common.GO.Players.Go;

        private IDictionary<UInt32, Move> Moves => Common.GO.Moves.Go;

        private IDictionary<UInt32, Avatar> Avatars => Common.GO.Avatars.Go;

        private IDictionary<UInt32, Equipment> Equipments => Common.GO.Equipments.Go;

        public INZ4()
        {
        }

        private string PlayerToString(Player player)
        {
            string exportContent = string.Join("\n", new String[]
            {
                player.Name,
                player.Level.ToString(),
                player.Stat[0].ToString() + "|" + player.Stat[1].ToString(),
                player.Score.ToString() + "|" + player.Participation.ToString(),
                string.Join( "|", player.InvestedPoint),
                player.Avatar.Name + "|" + player.Avatar.Level.ToString() + "|" + player.Invoke,
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

            List<int> scores = playerString[3].Split('|').Select(Int32.Parse).ToList();
            player.Score = scores[0];
            player.Participation = scores[1];

            player.InvestedPoint = new List<int>();
            List<int> investedPoints = playerString[4].Split('|').Select(Int32.Parse).ToList();
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

            List<string> avatarValues = playerString[5].Split('|').ToList();
            Avatar avatar = Avatars.FirstOrDefault(x => x.Value.Name == avatarValues[0]).Value;
            if (avatar == null)
            {
                avatar = Avatars[0x0];
            }
            player.Avatar = avatar;
            player.Avatar.Level = Convert.ToInt32(avatarValues[1]);
            player.Invoke = Convert.ToBoolean(avatarValues[2]);

            player.Moves = new List<Move>();
            for (int i = 0; i < 6; i++)
            {
                List<string> moveValues = playerString[6 + i].Split('|').ToList();
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
                List<string> equipmentName = playerString[12 + i].Split('|').ToList();
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
                    newFileContent
                });
            }

            return BitConverter.ToString(new Crc32().ComputeHash(Encoding.UTF8.GetBytes(newFileContent))).Replace("-", string.Empty) + "\n" + newFileContent;
        }

        public Player NewPlayer(string path)
        {
            bool isSameGame = File.ReadLines(path).Skip(1).First() == GameNameCode;

            Player player = StringToPlayer(File.ReadLines(path).Skip(2).Take(18).ToList(), isSameGame);

            return player;
        }
    }
}
