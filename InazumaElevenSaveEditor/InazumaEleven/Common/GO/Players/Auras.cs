using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InazumaElevenSaveEditor.InazumaEleven.Logic;

namespace InazumaElevenSaveEditor.InazumaEleven.Common.GO
{
    public static class Auras
    {
        public static Dictionary<UInt32, Player> Cs = new Dictionary<UInt32, Player>
        {
            {0x2B004EF0, new Player("T-rex aura", null, Element.Wood(), Gender.Unknown(), new List<UInt32>(){ 0xE65FE66C, 0x83648354, 0x00000000, 0x00000000}, new List<int>(){115, 48, 61, 36, 103, 50, 35, 82, 48, 91}, 50)},
            {0xB2091F4A, new Player("Nobunaga Oda's aura", Position.Forward(), Element.Wood(), Gender.Boy(), new List<UInt32>(){ 0xC9D9F94E, 0x796CD3F3, 0x00000000, 0x00000000}, new List<int>(){107, 82, 86, 51, 96, 42, 55, 46, 68, 77}, 50)},
            {0xC50E2FDC, new Player("Joan of Arc's aura", Position.Defender(), Element.Fire(), Gender.Girl(), new List<UInt32>(){ 0x4A2ED143, 0x0906277C, 0x00000000, 0x00000000}, new List<int>(){80, 111, 31, 62, 58, 91, 78, 34, 75, 42}, 50)},
            {0x5B6ABA7F, new Player("Liu Bei's aura", Position.Goalkeeper(), Element.Wood(), null, new List<UInt32>(){ 0x51771728, 0x9762B2DF, 0x00000000, 0x00000000}, new List<int>(){101, 85, 69, 48, 60, 84, 56, 43, 78, 67}, 50)},
            {0x2C6D8AE9, new Player("Zhuge Liang's aura", Position.Midfielder(), Element.Wind(), Gender.Girl(), new List<UInt32>(){ 0x8B856F43, 0xEEBE0A7B, 0x00000000, 0x00000000}, new List<int>(){78, 106, 53, 79, 59, 63, 91, 40, 33, 41}, 50)},
            {0xB564DB53, new Player("Soji Okita's aura", Position.Forward(), Element.Fire(), Gender.Boy(), new List<UInt32>(){ 0x658B0E6F, 0x900F76C6, 0x00000000, 0x00000000}, new List<int>(){93, 91, 99, 78, 77, 58, 64, 32, 38, 30}, 50)},
            {0xC263EBC5, new Player("Sakamoto's aura", Position.Midfielder(), Element.Earth(), Gender.Boy(), new List<UInt32>(){ 0x3448AC79, 0x1770D224, 0x00000000, 0x00000000}, new List<int>(){99, 90, 38, 97, 46, 55, 46, 64, 63, 82}, 50)},
            {0x52DCF654, new Player("Big's aura", Position.Forward(), Element.Earth(), Gender.Unknown(), new List<UInt32>(){ 0x0B70B795, 0xE7084650, 0x00000000, 0x00000000}, new List<int>(){140, 51, 84, 49, 81, 54, 43, 100, 56, 71}, 50)},
            {0x25DBC6C2, new Player("Pa's aura", Position.Defender(), Element.Wind(), Gender.Unknown(), new List<UInt32>(){ 0xBD3B812E, 0x0E6BE365, 0x00000000, 0x00000000}, new List<int>(){137, 70, 49, 38, 86, 84, 51, 100, 76, 64}, 50)},
            {0x451C4F27, new Player("Queen's aura", Position.Defender(), Element.Wood(), Gender.Unknown(), new List<UInt32>(){ 0x3A4425CC, 0x7E0117EA, 0x00000000, 0x00000000}, new List<int>(){122, 75, 54, 36, 99, 85, 44, 83, 70, 108}, 50)},
            {0x321B7FB1, new Player("King's aura", Position.Forward(), Element.Earth(), Gender.Boy(), new List<UInt32>(){ 0x02D5821A, 0x00000000, 0x00000000, 0x00000000}, new List<int>(){87, 79, 70, 42, 86, 63, 42, 82, 40, 77}, 50)},
            {0xAB122E0B, new Player("Hurricane Zeta's aura", null, Element.Wind(), Gender.Unknown(), new List<UInt32>(){ 0x72AC0F31, 0x99B93AED, 0x00000000, 0x00000000}, new List<int>(){116, 63, 93, 85, 37, 65, 67, 81, 48, 34}, 50)},
            {0xDC151E9D, new Player("S gene aura", null, Element.Earth(), Gender.Unknown(), new List<UInt32>(){ 0xFF44D72D, 0xE7084650, 0x00000000, 0x00000000}, new List<int>(){90, 100, 87, 81, 65, 74, 74, 54, 56, 60}, 50)},
            {0x42718B3E, new Player("W gene aura", null, Element.Fire(), Gender.Unknown(), new List<UInt32>(){ 0x27D79862, 0xE7084650, 0x00000000, 0x00000000}, new List<int>(){131, 98, 123, 87, 98, 66, 70, 94, 43, 62}, 50)},
            {0x3576BBA8, new Player("V gene aura", null, Element.Earth(), Gender.Unknown(), new List<UInt32>(){ 0x49CB99B5, 0xE7084650, 0x00000000, 0x00000000}, new List<int>(){112, 119, 96, 123, 98, 98, 96, 53, 62, 79}, 50)},
            {0xAC7FEA12, new Player("Black rose aura", null, Element.Wind(), Gender.Unknown(), new List<UInt32>(){ 0x7BFDFB37, 0xEEBE0A7B, 0x00000000, 0x00000000}, new List<int>(){90, 134, 99, 111, 63, 115, 117, 47, 84, 113}, 50)},
            {0xDB78DA84, new Player("Cao Cao's aura", Position.Defender(), Element.Earth(), Gender.Boy(), new List<UInt32>(){ 0x7818B3C1, 0x0E6BE365, 0x00000000, 0x00000000}, new List<int>(){91, 94, 40, 37, 84, 90, 30, 100, 62, 40}, 50)},
        };
    }
}
