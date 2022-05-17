﻿using System;
using System.Collections.Generic;
using InazumaElevenSaveEditor.Logic;

namespace InazumaElevenSaveEditor.Common.InazumaElevenGo.Avatars
{
    public static class Avatars
    {
        public static IDictionary<UInt32, Avatar> Go = new Dictionary<UInt32, Avatar>
        {
            {0x00000000, new Avatar(" ", true) },
            {0xF3B1988B, new Avatar("Pegasus", true) },
            {0x49E09112, new Avatar("Lancelot", true) },
            {0xDFD09665, new Avatar("Maestro", true) },
            {0x7C45F2FB, new Avatar("Apollo", true) },
            {0xEA75F58C, new Avatar("Musashi", true) },
            {0x5024FC15, new Avatar("Judge", true) },
            {0xC614FB62, new Avatar("Lion", true) },
            {0x570944F2, new Avatar("Chione", true) },
            {0xC1394385, new Avatar("Varius", true) },
            {0x24B084E5, new Avatar("Surtur", true) },
            {0xB2808392, new Avatar("Giant", true) },
            {0x08D18A0B, new Avatar("Falcon", true) },
            {0x9EE18D7C, new Avatar("Poseidon", true) },
            {0x3D74E9E2, new Avatar("Jester", true) },
            {0xAB44EE95, new Avatar("Garas", true) },
            {0x1115E70C, new Avatar("Golia", true) },
            {0x8725E07B, new Avatar("Lot", true) },
            {0x16385FEB, new Avatar("White Pawn", true) },
            {0x8008589C, new Avatar("White Horse", true) },
            {0xE7E3A9CE, new Avatar("White Bisho", true) },
            {0x71D3AEB9, new Avatar("White Tower", true) },
            {0xCB82A720, new Avatar("White Queen", true) },
            {0x5DB2A057, new Avatar("White King", true) },
            {0x6817C3BE, new Avatar("Atlas", true) },
            {0xD246CA27, new Avatar("Demogorgon", true) },
            {0x4476CD50, new Avatar("Shadow Shooter", true) },
            {0xD56B72C0, new Avatar("Black Butcher", true) },
            {0x435B75B7, new Avatar("White Wyvern", true) },
            {0x30E2B5A0, new Avatar("Crow", true) },
            {0x8AB3BC39, new Avatar("Drake", true) },
            {0x1C83BB4E, new Avatar("Pegasus Arc", true) },
            {0xBF16DFD0, new Avatar("Black Pawn", true) },
            {0x2926D8A7, new Avatar("Black Horse", true) },
            {0x9377D13E, new Avatar("Black Bishop", true) },
            {0x0547D649, new Avatar("Black Tower", true) },
            {0x945A69D9, new Avatar("Black Queen", true) },
            {0x026A6EAE, new Avatar("Black King", true) },
            {0x6144F398, new Avatar("Cardinal", true) },
            {0xF774F4EF, new Avatar("Roc", true) },
            {0x4D25FD76, new Avatar("Berserker", true) },
            {0xDB15FA01, new Avatar("Barron", true) },
            {0x78809E9F, new Avatar("Joker", true) },
            {0xEEB099E8, new Avatar("Neptune", true) },
            {0x54E19071, new Avatar("Gargantua", true) },
            {0xC2D19706, new Avatar("Siren", true) },
            {0x53CC2896, new Avatar("Tachyon", true) },
            {0xC5FC2FE1, new Avatar("Black Jack", true) },
            {0x2075E881, new Avatar("Korokoro", true) },
            {0xB645EFF6, new Avatar("Majin", true) },
            {0x0C14E66F, new Avatar("Serendip", true) },
            {0x9A24E118, new Avatar("Sandman", true) },
            {0x39B18586, new Avatar("Pendragon", true) },
            {0xFE27C4C9, new Avatar("Arthur", true) },
            {0xA6D2B2D7, new Avatar("Griffin", true) },
        };

        public static IDictionary<UInt32, Avatar> Cs = new Dictionary<UInt32, Avatar>(Go)
        {
            {0x7516C2DD, new Avatar("Fenix", true) },
            {0x5977CC33, new Avatar("Atena", true) },
            {0xFAE2A8AD, new Avatar("Wheasel", true) },
            {0x6CD2AFDA, new Avatar("Brunilde", true) },
            {0xD683A643, new Avatar("Kykhreides", true) },
            {0x40B3A134, new Avatar("Xuan-Wu", true) },
            {0xD1AE1EA4, new Avatar("Licaone", true) },
            {0x479E19D3, new Avatar("Zodiaco", true) },
            {0xA217DEB3, new Avatar("Amaterasu", true) },
            {0x3427D9C4, new Avatar("Jaguar", true) },
            {0x8E76D05D, new Avatar("Plasma Shadow MF", true) },
            {0x1846D72A, new Avatar("Plasma Shadow DF", true) },
            {0xBBD3B3B4, new Avatar("Plasma Shadow FW", true) },
            {0x2DE3B4C3, new Avatar("Plasma Shadow GK", true) },
            {0x97B2BD5A, new Avatar("Robin", true) },
            {0x0182BA2D, new Avatar("Atlanticus", true) },
            {0x909F05BD, new Avatar("Lovers ♀", true) },
            {0x06AF02CA, new Avatar("Loverette", true) },
            {0x6D0B4634, new Avatar("Evera", true) },
            {0xFB3B4143, new Avatar("Dire Wolf", true) },
            {0x416A48DA, new Avatar("Glenr", true) },
            {0xD75A4FAD, new Avatar("Kukulkan", true) },
            {0x74CF2B33, new Avatar("Eris", true) },
            {0xE2FF2C44, new Avatar("Patron Saint of Swots", true) },
            {0x58AE25DD, new Avatar("Asura", true) },
            {0xCE9E22AA, new Avatar("Metis", true) },
            {0x5F839D3A, new Avatar("Argentia", true) },
            {0xC9B39A4D, new Avatar("Zhuque", true) },
            {0x2C3A5D2D, new Avatar("Quinglong", true) },
            {0xBA0A5A5A, new Avatar("Baihu", true) },
            {0x005B53C3, new Avatar("Raiden", true) },
            {0x966B54B4, new Avatar("Khmet", true) },
            {0x35FE302A, new Avatar("Arges", true) },
            {0xA3CE375D, new Avatar("Daji", true) },
            {0x199F3EC4, new Avatar("Vritra", true) },
            {0x8FAF39B3, new Avatar("Astaroth", true) },
            {0x1EB28623, new Avatar("Lovers ♂", true) },
            {0x88828154, new Avatar("Pegasus Arc Red", true) },
        };

        public static IDictionary<UInt32, Avatar> Galaxy = new Dictionary<UInt32, Avatar>(Cs)
        {
            {0xB06D08DB, new Avatar("Horse", false) },
            {0x0A3C0142, new Avatar("Wolf", false) },
            {0x9C0C0635, new Avatar("Hawk", false) },
            {0x3F9962AB, new Avatar("Fox", false) },
            {0xA9A965DC, new Avatar("Lion (Totem)", false) },
            {0x13F86C45, new Avatar("Dohma", false) },
            {0x85C86B32, new Avatar("Guuma", false) },
            {0x14D5D4A2, new Avatar("Owl", false) },
            {0x82E5D3D5, new Avatar("Dolphanus", false) },
            {0x676C14B5, new Avatar("Delpinus", false) },
            {0xF15C13C2, new Avatar("Buffalo", false) },
            {0x4B0D1A5B, new Avatar("Peackock", false) },
            {0xDD3D1D2C, new Avatar("Mhammut", false) },
            {0x7EA879B2, new Avatar("Capricorn", false) },
            {0xE8987EC5, new Avatar("Bear", false) },
            {0x52C9775C, new Avatar("Badger", false) },
            {0xC4F9702B, new Avatar("Gandoran", false) },
            {0x55E4CFBB, new Avatar("Phoenikias", false) },
            {0xC3D4C8CC, new Avatar("Garyuu", false) },
            {0xA43F399E, new Avatar("Gouryuu", false) },
            {0x320F3EE9, new Avatar("Doruuga", false) },
            {0x885E3770, new Avatar("Gusfii", false) },
            {0x1E6E3007, new Avatar("Kosfii", false) },
            {0xBDFB5499, new Avatar("Belion", false) },
            {0x2BCB53EE, new Avatar("Angidra", false) },
            {0x919A5A77, new Avatar("Ixaal", false) },
            {0x07AA5D00, new Avatar("Begiran", false) },
            {0x96B7E290, new Avatar("Redio", false) },
            {0x0087E5E7, new Avatar("Kulupe", false) },
            {0xE50E2287, new Avatar("Giriras", false) },
            {0x733E25F0, new Avatar("Gryphbang", false) },
            {0xC96F2C69, new Avatar("Univolt", false) },
            {0x5F5F2B1E, new Avatar("Flamey", false) },
            {0xFCCA4F80, new Avatar("Magnatas", false) },
            {0x6AFA48F7, new Avatar("Dragnova", false) },
            {0xD0AB416E, new Avatar("Goliger", false) },
            {0x469B4619, new Avatar("Dandor", false) },
            {0xD786F989, new Avatar("Saranarja ", false) },
            {0x41B6FEFE, new Avatar("White Wolf", false) },
            {0x229863C8, new Avatar("Hakutouwashi", false) },
            {0xB4A864BF, new Avatar("Pelion", false) },
            {0x0EF96D26, new Avatar("Vairaas", false) },
            {0x98C96A51, new Avatar("Pamerio", false) },
            {0x3B5C0ECF, new Avatar("Brakias", false) },
            {0xAD6C09B8, new Avatar("Tsukinowaguma", false) },
            {0x173D0021, new Avatar("Zuma", false) },
            {0x810D0756, new Avatar("Mimizuku", false) },
            {0x1010B8C6, new Avatar("Gyorom", false) },
            {0x8620BFB1, new Avatar("Jinryuu", false) },
            {0x63A978D1, new Avatar("Dolmega", false) },
            {0xF5997FA6, new Avatar("Dalphonus", false) },
            {0x4FC8763F, new Avatar("Mossfii", false) },
            {0xD9F87148, new Avatar("Grimron", false) },
            {0x7A6D15D6, new Avatar("Cerbegira", false) },
            {0x560C1B38, new Avatar("Pegasus (Totem)", false) },
            {0xEC5D12A1, new Avatar("Red Pegasus (Totem)", false) },
        };
    }
}