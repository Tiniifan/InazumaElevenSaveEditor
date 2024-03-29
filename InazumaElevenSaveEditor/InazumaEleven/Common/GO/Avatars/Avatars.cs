﻿using System.Collections.Generic;
using InazumaElevenSaveEditor.InazumaEleven.Logic;

namespace InazumaElevenSaveEditor.InazumaEleven.Common.GO
{
    public static class Avatars
    {
        public static Dictionary<uint, Avatar> Go = new Dictionary<uint, Avatar>
        {
            {0x00000000, new Avatar(" ", true) },
            {0x8B98B1F3, new Avatar("Pegasus", true) },
            {0x1291E049, new Avatar("Lancelot", true) },
            {0x6596D0DF, new Avatar("Maestro", true) },
            {0xFBF2457C, new Avatar("Apollo", true) },
            {0x8CF575EA, new Avatar("Musashi", true) },
            {0x15FC2450, new Avatar("Judge", true) },
            {0x62FB14C6, new Avatar("Leon", true) },
            {0xF2440957, new Avatar("Chione", true) },
            {0x854339C1, new Avatar("Scramjet", true) },
            {0xE584B024, new Avatar("Surtr", true) },
            {0x928380B2, new Avatar("Gigante", true) },
            {0x0B8AD108, new Avatar("Thunderbird", true) },
            {0x7C8DE19E, new Avatar("Poseidon", true) },
            {0xE2E9743D, new Avatar("Trickster", true) },
            {0x95EE44AB, new Avatar("Phantasma", true) },
            {0x0CE71511, new Avatar("Goliath", true) },
            {0x7BE02587, new Avatar("Lot", true) },
            {0xEB5F3816, new Avatar("White Pawn", true) },
            {0x9C580880, new Avatar("White Knight", true) },
            {0xCEA9E3E7, new Avatar("White Bishop", true) },
            {0xB9AED371, new Avatar("White Rook", true) },
            {0x20A782CB, new Avatar("White Queen", true) },
            {0x57A0B25D, new Avatar("White King", true) },
            {0xBEC31768, new Avatar("Atlas", true) },
            {0x27CA46D2, new Avatar("Demogorgon", true) },
            {0x50CD7644, new Avatar("Trigger", true) },
            {0xC0726BD5, new Avatar("Black Butcher", true) },
            {0xB7755B43, new Avatar("White Wyvern", true) },
            {0xA0B5E230, new Avatar("Corvus", true) },
            {0x39BCB38A, new Avatar("Dragoon", true) },
            {0x4EBB831C, new Avatar("Arch Pegasus", true) },
            {0xD0DF16BF, new Avatar("Black Pawn", true) },
            {0xA7D82629, new Avatar("Black Knight", true) },
            {0x3ED17793, new Avatar("Black Bishop", true) },
            {0x49D64705, new Avatar("Black Rook", true) },
            {0xD9695A94, new Avatar("Black Queen", true) },
            {0xAE6E6A02, new Avatar("Black King", true) },
            {0x98F34461, new Avatar("Firebird", true) },
            {0xEFF474F7, new Avatar("Roc", true) },
            {0x76FD254D, new Avatar("Berserker", true) },
            {0x01FA15DB, new Avatar("Behemoth", true) },
            {0x9F9E8078, new Avatar("Jester", true) },
            {0xE899B0EE, new Avatar("Neptune", true) },
            {0x7190E154, new Avatar("Gargantua", true) },
            {0x0697D1C2, new Avatar("Siren", true) },
            {0x9628CC53, new Avatar("Tachyon", true) },
            {0xE12FFCC5, new Avatar("Las Vega", true) },
            {0x81E87520, new Avatar("Koro-koro-Gon", true) },
            {0xF6EF45B6, new Avatar("Majin", true) },
            {0x6FE6140C, new Avatar("Serendipity", true) },
            {0x18E1249A, new Avatar("Sandman", true) },
            {0x8685B139, new Avatar("Pendragon", true) },
            {0xC9C427FE, new Avatar("Arthur", true) },
            {0xD7B2D2A6, new Avatar("Griffin", true) },
        };

        public static Dictionary<uint, Avatar> Cs = new Dictionary<uint, Avatar>(Go)
        {
            {0xDDC21675, new Avatar("Feng-Huang", true) },
            {0x33CC7759, new Avatar("Athene", true) },
            {0xADA8E2FA, new Avatar("Sickle Weasel", true) },
            {0xDAAFD26C, new Avatar("Brynhildr", true) },
            {0x43A683D6, new Avatar("Cychr", true) },
            {0x34A1B340, new Avatar("Xuan-Wu", true) },
            {0xA41EAED1, new Avatar("Lycaon", true) },
            {0xD3199E47, new Avatar("Zodiac", true) },
            {0xB3DE17A2, new Avatar("Amaterasu", true) },
            {0xC4D92734, new Avatar("Jagwarrior", true) },
            {0x5DD0768E, new Avatar("Plasma Shadow AT", true) },
            {0x2AD74618, new Avatar("Plasma Shadow DF", true) },
            {0xB4B3D3BB, new Avatar("Plasma Shadow SH", true) },
            {0xC3B4E32D, new Avatar("Plasma Shadow GK", true) },
            {0x5ABDB297, new Avatar("Sir Rabby", true) },
            {0x2DBA8201, new Avatar("Atlanticus", true) },
            {0xBD059F90, new Avatar("The Lovers♀", true) },
            {0xCA02AF06, new Avatar("Loverette", true) },
            {0x34460B6D, new Avatar("Evera", true) },
            {0x43413BFB, new Avatar("Dire Wolf", true) },
            {0xDA486A41, new Avatar("Glenr", true) },
            {0xAD4F5AD7, new Avatar("Kukulkan", true) },
            {0x332BCF74, new Avatar("Eris", true) },
            {0x442CFFE2, new Avatar("The Stationer", true) },
            {0xDD25AE58, new Avatar("Asura", true) },
            {0xAA229ECE, new Avatar("Metis", true) },
            {0x3A9D835F, new Avatar("Argentia", true) },
            {0x4D9AB3C9, new Avatar("Zhuque", true) },
            {0x2D5D3A2C, new Avatar("Quinglong", true) },
            {0x5A5A0ABA, new Avatar("Baihu", true) },
            {0xC3535B00, new Avatar("Raiden", true) },
            {0xB4546B96, new Avatar("Khmet", true) },
            {0x2A30FE35, new Avatar("Arges", true) },
            {0x5D37CEA3, new Avatar("Daji", true) },
            {0xC43E9F19, new Avatar("Vritra", true) },
            {0xB339AF8F, new Avatar("Ashtaroth", true) },
            {0x2386B21E, new Avatar("The Lovers♂", true) },
            {0x54818288, new Avatar("Arch Pegasus Red", true) },
        };

        public static Dictionary<uint, Avatar> Galaxy = new Dictionary<uint, Avatar>(Cs)
        {
            {0xDB086DB0, new Avatar("Horse", false) },
            {0x42013C0A, new Avatar("Wolf", false) },
            {0x35060C9C, new Avatar("Falcon", false) },
            {0xAB62993F, new Avatar("Fox", false) },
            {0xDC65A9A9, new Avatar("Lion (Totem)", false) },
            {0x456CF813, new Avatar("Dohma", false) },
            {0x326BC885, new Avatar("Guuma", false) },
            {0xA2D4D514, new Avatar("Owl", false) },
            {0xD5D3E582, new Avatar("Dolphanus", false) },
            {0xB5146C67, new Avatar("Delpinus", false) },
            {0xC2135CF1, new Avatar("Buffalo", false) },
            {0x5B1A0D4B, new Avatar("Peacock", false) },
            {0x2C1D3DDD, new Avatar("Mammoth", false) },
            {0xB279A87E, new Avatar("Gazelle", false) },
            {0xC57E98E8, new Avatar("Grizzly Bear", false) },
            {0x5C77C952, new Avatar("Ratel", false) },
            {0x2B70F9C4, new Avatar("Gandoran", false) },
            {0xBBCFE455, new Avatar("Phoenikias", false) },
            {0xCCC8D4C3, new Avatar("Garyuu", false) },
            {0x9E393FA4, new Avatar("Gouryuu", false) },
            {0xE93E0F32, new Avatar("Doruuga", false) },
            {0x70375E88, new Avatar("Gusfii", false) },
            {0x07306E1E, new Avatar("Mossfii", false) },
            {0x9954FBBD, new Avatar("Belion", false) },
            {0xEE53CB2B, new Avatar("Angidra", false) },
            {0x775A9A91, new Avatar("Ixaal", false) },
            {0x005DAA07, new Avatar("Begiran", false) },
            {0x90E2B796, new Avatar("Redio", false) },
            {0xE7E58700, new Avatar("Kulupe", false) },
            {0x87220EE5, new Avatar("Giriras", false) },
            {0xF0253E73, new Avatar("Grifbang", false) },
            {0x692C6FC9, new Avatar("Univolt", false) },
            {0x1E2B5F5F, new Avatar("Flamey", false) },
            {0x804FCAFC, new Avatar("Magnatas", false) },
            {0xF748FA6A, new Avatar("Dragnova", false) },
            {0x6E41ABD0, new Avatar("Goliger", false) },
            {0x19469B46, new Avatar("Dandor", false) },
            {0x89F986D7, new Avatar("Saranarja ", false) },
            {0xFEFEB641, new Avatar("Silver Wolf", false) },
            {0xC8639822, new Avatar("Bald Eagle", false) },
            {0xBF64A8B4, new Avatar("Pelion", false) },
            {0x266DF90E, new Avatar("Vairaas", false) },
            {0x516AC998, new Avatar("Pamerio", false) },
            {0xCF0E5C3B, new Avatar("Brakias", false) },
            {0xB8096CAD, new Avatar("Black Bear", false) },
            {0x21003D17, new Avatar("Zuuma", false) },
            {0x56070D81, new Avatar("Horned Owl", false) },
            {0xC6B81010, new Avatar("Gyorom", false) },
            {0xB1BF2086, new Avatar("Jinryuu", false) },
            {0xD178A963, new Avatar("Dolmega", false) },
            {0xA67F99F5, new Avatar("Dalphonus", false) },
            {0x3F76C84F, new Avatar("Kosfii", false) },
            {0x4871F8D9, new Avatar("Grimron", false) },
            {0xD6156D7A, new Avatar("Cerbegira", false) },
            {0x381B0C56, new Avatar("Pegasus (Totem)", false) },
            {0xA1125DEC, new Avatar("Red Pegasus (Totem)", false) },
        };
    }
}
