using System.Collections;
using System.Collections.Generic;
using InazumaElevenSaveEditor.Logic;

namespace InazumaElevenSaveEditor.Common.InazumaElevenGo
{
    public static class PlayRecords
    {
        //  Missing GO and Galaxy Play Records

        public static Dictionary<int, List<PlayRecord>> Cs = new Dictionary<int, List<PlayRecord>>
        {
            {0, new List<PlayRecord>()
                {
                    new PlayRecord("Amateur Snapper"),
                    new PlayRecord("Photographer"),
                    new PlayRecord("Pro Photographer"),
                    new PlayRecord("Chatter"),
                    new PlayRecord("Gossiper"),
                    new PlayRecord("Conversationalist"),
                    new PlayRecord("Emblem Collector")
                }
            },
            {1, new List<PlayRecord>()
                {
                    new PlayRecord("Kit Collector"),
                    new PlayRecord("Treasure Finder"),
                    new PlayRecord("Treasure Seeker"),
                    new PlayRecord("Treasure Hunter"),
                    new PlayRecord("Capsule Worker"),
                    new PlayRecord("Capsule Warrior"),
                    new PlayRecord("All-Round Trainer")
                }
            },
            {2, new List<PlayRecord>()
                {
                    new PlayRecord("Baby Battler"),
                    new PlayRecord("Big Battler"),
                    new PlayRecord("Brilliant Battler"),
                    new PlayRecord("Pro Battler"),
                    new PlayRecord("Famed Battler"),
                    new PlayRecord("Legendary Battler"),
                    new PlayRecord("Novice Team"),
                }
            },
            {3, new List<PlayRecord>()
                {
                    new PlayRecord("Skilled Team"),
                    new PlayRecord("Veteran Team"),
                    new PlayRecord("Formidable Team"),
                    new PlayRecord("Dominating Team"),
                    new PlayRecord("Spirit Friends"),
                    new PlayRecord("Spirit Buddies"),
                    new PlayRecord("Spirit Family"),
                    new PlayRecord("Starter S-Ranker")
                }
            },
            {4, new List<PlayRecord>()
                {
                    new PlayRecord("Inferno S-Ranker"),
                    new PlayRecord("Brawl S-Ranker"),
                    new PlayRecord("Showdown S-Ranker"),
                    new PlayRecord("Truth S-Ranker"),
                    new PlayRecord("Revolution S-Ranker"),
                    new PlayRecord("Old-School S-Ranker"),
                    new PlayRecord("Hidden S-Ranker"),
                    new PlayRecord("Lost S-Ranker")
                }
            },
            {5, new List<PlayRecord>()
                {
                    new PlayRecord("Top-Secret S Ranker"),
                    new PlayRecord("Ultimate S-Ranker"),
                    new PlayRecord("Grandfather S-Ranker"),
                    new PlayRecord("Triple Chain"),
                    new PlayRecord("Quadruple Chain"),
                    new PlayRecord("Quintuple Chain"),
                    new PlayRecord("Sextuple Chain"),
                    new PlayRecord("Great Destruction")
                }
            },
            {6, new List<PlayRecord>()
                {
                    new PlayRecord("Awesome Destruction"),
                    new PlayRecord("Terrible Destruction"),
                    new PlayRecord("Monstrous Destruction"),
                    new PlayRecord("Immeasurable Destruction"),
                    new PlayRecord("Absolute Destruction"),
                    new PlayRecord("Striker"),
                    new PlayRecord("Serious Striker"),
                    new PlayRecord("Super Striker")
                }
            },
            {7, new List<PlayRecord>()
                {
                    new PlayRecord("Ace Striker"),
                    new PlayRecord("Miracle Striker"),
                    new PlayRecord("Virtuoso"),
                    new PlayRecord("Sample Data"),
                    new PlayRecord("Helpful Data"),
                    new PlayRecord("Reliable Data"),
                    new PlayRecord("Extensive Data"),
                    new PlayRecord("Flawless Data")
                }
            },
            {8, new List<PlayRecord>()
                {
                    new PlayRecord("Ideal Match 1"),
                    new PlayRecord("Ideal Match 2"),
                    new PlayRecord("Ideal Match 3"),
                    new PlayRecord("Ideal Match 4"),
                    new PlayRecord("Ideal Match 5"),
                    new PlayRecord("Ideal Match 6"),
                    new PlayRecord("Ideal Match 7"),
                    new PlayRecord("Ideal Match 8")
                }
            },
            {9, new List<PlayRecord>()
                {
                    new PlayRecord("Systme Online"),
                    new PlayRecord("In Demand"),
                    new PlayRecord("Up to the Challenge"),
                    new PlayRecord("Treasure Master")
                }
            },
        };

        public static byte[] NewBinary(List<PlayRecord> listPlayRecord)
        {
            byte[] binary = new byte[listPlayRecord.Count];

            for (int i = 0; i < listPlayRecord.Count; i++)
            {
                if (listPlayRecord[i].Unlocked == false)
                {
                    binary[i] = 0x00;
                }
                else
                {
                    binary[i] = 0x01;
                }
            }

            return binary;
        }
    }
}
