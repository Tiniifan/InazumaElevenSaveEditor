using System;
using System.Collections.Generic;
using InazumaElevenSaveEditor.Logic;

namespace InazumaElevenSaveEditor.Common.InazumaElevenGo
{
    public static class Auras
    {
        public static IDictionary<UInt32, Player> Cs = new Dictionary<UInt32, Player>
        {
            {0x00000000, new Player("", null, null, null, null, null, 0)},
            {0xF04E002B, new Player("T-rex aura", null, Element.Wood(), Gender.Unknown(), new List<UInt32>(){0x6CE65FE6, 0x54836483, 0x00000000, 0x00000000}, new List<int>(){115, 48, 61, 36, 103, 50, 35, 82, 48, 91}, 50)},
            {0x4A1F09B2, new Player("Nobunaga Oda's aura", Position.Forward(), Element.Wood(), Gender.Boy(), new List<UInt32>(){0x4EF9D9C9, 0xF3D36C79, 0x00000000, 0x00000000}, new List<int>(){107, 82, 86, 51, 96, 42, 55, 46, 68, 77}, 50)},
            {0xDC2F0EC5, new Player("Joan of Arc's aura", Position.Defender(), Element.Fire(), Gender.Girl(), new List<UInt32>(){0x43D12E4A, 0x7C270609, 0x00000000, 0x00000000}, new List<int>(){80, 111, 31, 62, 58, 91, 78, 34, 75, 42}, 50)},
            {0x7FBA6A5B, new Player("Liu Bei's aura", Position.Goalkeeper(), Element.Wood(), null, new List<UInt32>(){0x28177751, 0xDFB26297, 0x00000000, 0x00000000}, new List<int>(){101, 85, 69, 48, 60, 84, 56, 43, 78, 67}, 50)},
            {0xE98A6D2C, new Player("Zhuge Liang's aura", Position.Midfielder(), Element.Wind(), Gender.Girl(), new List<UInt32>(){0x436F858B, 0x7B0ABEEE, 0x00000000, 0x00000000}, new List<int>(){78, 106, 53, 79, 59, 63, 91, 40, 33, 41}, 50)},
            {0x53DB64B5, new Player("Soji Okita's aura", Position.Forward(), Element.Fire(), Gender.Boy(), new List<UInt32>(){0x6F0E8B65, 0xC6760F90, 0x00000000, 0x00000000}, new List<int>(){93, 91, 99, 78, 77, 58, 64, 32, 38, 30}, 50)},
            {0xC5EB63C2, new Player("Sakamoto's aura", Position.Midfielder(), Element.Earth(), Gender.Boy(), new List<UInt32>(){0x79AC4834, 0x24D27017, 0x00000000, 0x00000000}, new List<int>(){99, 90, 38, 97, 46, 55, 46, 64, 63, 82}, 50)},
            {0x54F6DC52, new Player("Big's aura", Position.Forward(), Element.Earth(), Gender.Unknown(), new List<UInt32>(){0x95B7700B, 0x504608E7, 0x00000000, 0x00000000}, new List<int>(){140, 51, 84, 49, 81, 54, 43, 100, 56, 71}, 50)},
            {0xC2C6DB25, new Player("Pa's aura", Position.Defender(), Element.Wind(), Gender.Unknown(), new List<UInt32>(){0x2E813BBD, 0x65E36B0E, 0x00000000, 0x00000000}, new List<int>(){137, 70, 49, 38, 86, 84, 51, 100, 76, 64}, 50)},
            {0x274F1C45, new Player("Queen's aura", Position.Defender(), Element.Wood(), Gender.Unknown(), new List<UInt32>(){0xCC25443A, 0xEA17017E, 0x00000000, 0x00000000}, new List<int>(){122, 75, 54, 36, 99, 85, 44, 83, 70, 108}, 50)},
            {0xB17F1B32, new Player("King's aura", Position.Forward(), Element.Earth(), Gender.Boy(), new List<UInt32>(){0x1A82D502, 0x00000000, 0x00000000, 0x00000000}, new List<int>(){87, 79, 70, 42, 86, 63, 42, 82, 40, 77}, 50)},
            {0x0B2E12AB, new Player("Hurricane Zeta's aura", null, Element.Wind(), Gender.Unknown(), new List<UInt32>(){0x310FAC72, 0xED3AB999, 0x00000000, 0x00000000}, new List<int>(){116, 63, 93, 85, 37, 65, 67, 81, 48, 34}, 50)},
            {0x9D1E15DC, new Player("S gene aura", null, Element.Earth(), Gender.Unknown(), new List<UInt32>(){0x2DD744FF, 0x504608E7, 0x00000000, 0x00000000}, new List<int>(){90, 100, 87, 81, 65, 74, 74, 54, 56, 60}, 50)},
            {0x3E8B7142, new Player("W gene aura", null, Element.Fire(), Gender.Unknown(), new List<UInt32>(){0x6298D727, 0x504608E7, 0x00000000, 0x00000000}, new List<int>(){131, 98, 123, 87, 98, 66, 70, 94, 43, 62}, 50)},
            {0xA8BB7635, new Player("V gene aura", null, Element.Earth(), Gender.Unknown(), new List<UInt32>(){0xB599CB49, 0x504608E7, 0x00000000, 0x00000000}, new List<int>(){112, 119, 96, 123, 98, 98, 96, 53, 62, 79}, 50)},
            {0x12EA7FAC, new Player("Black rose aura", null, Element.Wind(), Gender.Unknown(), new List<UInt32>(){0x37FBFD7B, 0x7B0ABEEE, 0x00000000, 0x00000000}, new List<int>(){90, 134, 99, 111, 63, 115, 117, 47, 84, 113}, 50)},
            {0x84DA78DB, new Player("Cao Cao's aura", Position.Defender(), Element.Earth(), Gender.Boy(), new List<UInt32>(){0xC1B31878, 0x65E36B0E, 0x00000000, 0x00000000}, new List<int>(){91, 94, 40, 37, 84, 90, 30, 100, 62, 40}, 50)},
        };
    }
}
