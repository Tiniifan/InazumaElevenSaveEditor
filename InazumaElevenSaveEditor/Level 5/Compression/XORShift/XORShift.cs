using System;
using System.Linq;
using System.Collections.Generic;
using InazumaElevenSaveEditor.Tools;

namespace InazumaElevenSaveEditor.Level_5.Compression.XORShift
{
    // Level5 Decrypt & Encrypt (XORShift Algorithm) 
    // Original Source from yw_save.py by togenyan
    // Converted to C#

    public class XORShift : ICompression
    {
        private List<int> odd_primes = new List<int> {
        3,
        5,
        7,
        11,
        13,
        17,
        19,
        23,
        29,
        31,
        37,
        41,
        43,
        47,
        53,
        59,
        61,
        67,
        71,
        73,
        79,
        83,
        89,
        97,
        101,
        103,
        107,
        109,
        113,
        127,
        131,
        137,
        139,
        149,
        151,
        157,
        163,
        167,
        173,
        179,
        181,
        191,
        193,
        197,
        199,
        211,
        223,
        227,
        229,
        233,
        239,
        241,
        251,
        257,
        263,
        269,
        271,
        277,
        281,
        283,
        293,
        307,
        311,
        313,
        317,
        331,
        337,
        347,
        349,
        353,
        359,
        367,
        373,
        379,
        383,
        389,
        397,
        401,
        409,
        419,
        421,
        431,
        433,
        439,
        443,
        449,
        457,
        461,
        463,
        467,
        479,
        487,
        491,
        499,
        503,
        509,
        521,
        523,
        541,
        547,
        557,
        563,
        569,
        571,
        577,
        587,
        593,
        599,
        601,
        607,
        613,
        617,
        619,
        631,
        641,
        643,
        647,
        653,
        659,
        661,
        673,
        677,
        683,
        691,
        701,
        709,
        719,
        727,
        733,
        739,
        743,
        751,
        757,
        761,
        769,
        773,
        787,
        797,
        809,
        811,
        821,
        823,
        827,
        829,
        839,
        853,
        857,
        859,
        863,
        877,
        881,
        883,
        887,
        907,
        911,
        919,
        929,
        937,
        941,
        947,
        953,
        967,
        971,
        977,
        983,
        991,
        997,
        1009,
        1013,
        1019,
        1021,
        1031,
        1033,
        1039,
        1049,
        1051,
        1061,
        1063,
        1069,
        1087,
        1091,
        1093,
        1097,
        1103,
        1109,
        1117,
        1123,
        1129,
        1151,
        1153,
        1163,
        1171,
        1181,
        1187,
        1193,
        1201,
        1213,
        1217,
        1223,
        1229,
        1231,
        1237,
        1249,
        1259,
        1277,
        1279,
        1283,
        1289,
        1291,
        1297,
        1301,
        1303,
        1307,
        1319,
        1321,
        1327,
        1361,
        1367,
        1373,
        1381,
        1399,
        1409,
        1423,
        1427,
        1429,
        1433,
        1439,
        1447,
        1451,
        1453,
        1459,
        1471,
        1481,
        1483,
        1487,
        1489,
        1493,
        1499,
        1511,
        1523,
        1531,
        1543,
        1549,
        1553,
        1559,
        1567,
        1571,
        1579,
        1583,
        1597,
        1601,
        1607,
        1609,
        1613,
        1619,
        1621
    };

        private List<long> GenerateStateList(long start, int number)
        {
            List<long> states = new List<long>();
            for (int i = 0; i < number; i++)
            {
                start = start ^ start >> 30;
                start = i + 1 + start * (0x6C078966 - 1) & 0xFFFFFFFF;
                states.Add(start);
            }
            return states;
        }

        private int Shift(List<long> states, int arg)
        {
            long x = states[0];
            long y = states[3];
            states[0] = states[1];
            states[1] = states[2];
            states[2] = states[3];
            x = x ^ x << 11 & 0xFFFFFFFF;
            x = x ^ x >> 8 & 0xFFFFFFFF;
            y = y ^ y >> 19 & 0xFFFFFFFF;
            states[3] = x ^ y;
            if (arg == 0)
            {
                return (int)states[3];
            }
            return Convert.ToInt32(states[3] % arg);
        }

        public byte[] Decompress(byte[] input)
        {
            byte[] output = new byte[input.Length];

            List<int> table = (from x in Enumerable.Range(0, 0x100) select x).ToList();
            long seed = System.BitConverter.ToUInt32(input, input.Length - 4);
            List<long> states = GenerateStateList(seed, 3);
            states.Add(0x03DF95B3);

            // Generate Table
            for (int i = 0; i < 4096; i++)
            {
                int r = Shift(states, 0x10000);
                int r1 = r & 0xFF;
                int r2 = (r >> 8) & 0xFF;
                if (r1 != r2)
                {
                    int a = table[r1];
                    int b = table[r2];
                    int backupA = table[a];
                    int backupB = table[b];
                    table[a] = backupB;
                    table[b] = backupA;
                }
            }

            // Decrypt
            int ka = 0;
            for (int i = 0; i < input.Count() - 8; i++)
            {
                if (i % 0x100 == 0)
                {
                    ka = odd_primes[table[(i & 0xff00) >> 8]];
                }
                int kb = table[ka * (i + 1) & 0xff];
                output[i] = Convert.ToByte(input[i] ^ kb);
            }

            Array.Copy(input, input.Count() - 8, output, input.Count() - 8, 8);

            return output;
        }

        public byte[] Compress(byte[] input)
        {
            byte[] output = Decompress(input);
            byte[] calculateChecksum = new byte[input.Length - 8];

            Array.Copy(output, 0, calculateChecksum, 0, input.Length - 8);
            Array.Copy(new Crc32().ComputeHash(calculateChecksum).Reverse().ToArray(), 0, output, input.Count() - 8, 4);
            Array.Copy(input, input.Count() - 4, output, input.Count() - 4, 4);

            return output;
        }
    }
}
