using System.Runtime.InteropServices;

namespace InazumaElevenSaveEditor.InazumaEleven.Games.IEGOCS
{
    public static class IEGOCSHelper
    {
        public static long NameOffset = 0x3C;

        public static long TeamNameOffset = 0x64;

        public static long TimeOffset = 0x20;

        public static long LinkOffset = 0x3B4;

        public static long ChapterOffset = 0x10FC;

        public static long MoneyOffset = 0x1C0B0;

        public static long PlayRecordsOffset = 0x2EC;

        public static long PlayerDataOffset = 0x5B1C;

        public static long PlayerIndexOffset = 0x1C4E0;

        public static int MaximumPlayer = 336;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MoveBlock
        {
            public uint MoveID;
            public byte MoveLevel;
            public byte MoveUsage;
            public bool MoveLearned;
            public short MoveUnk;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PlayerBlock
        {
            public int Index;
            public uint ID;
            public int Unk1;
            public short GP;
            public short TP;
            public short Freedom;
            public byte Level;
            public byte Unk2;
            public int MiximaxIndex;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x08)]
            public byte[] EmptyBlock2;
            public byte MixiMaxMove1Index;
            public byte MixiMaxMove2Index;
            public byte InvokeStatus;
            public byte Style;
            public short ParticipationPoint;
            public short ScorePoint;
            public int Unk3;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public short[] InvestedPoint;
            public uint AvatarID;
            public byte AvatarLevel;
            public byte AvatarUsage;
            public byte Unk4;
            public byte Unk5;
            public int BootsIndex;
            public int BraceletIndex;
            public int PendantIndex;
            public int GlovesIndex;
            public int Unk6;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public MoveBlock[] Moves;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x64)]
            public byte[] Unk7;
        }

        public static long ItemBlockGroup1Offset = 0x14F4;
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ItemBlockGroup1
        {
            public int Index;
            public uint ID;
            public int Quantity;
        }

        public static long ItemBlockGroup2Offset = 0x2D00;
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ItemBlockGroup2
        {
            public int Index;
            public uint ID;
            public int Quantity;
            public int QuantityEquiped;
        }

        public static long ItemBlockGroup3Offset = 0x420C;
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ItemBlockGroup3
        {
            public int Index;
            public uint ID;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Team
        {
            public int CoachIndex;
            public int FormationIndex;
            public int KitIndex;
            public int EmblemIndex;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
            public byte[] PlayersFormationIndex;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
            public byte[] PlayerKitNumber;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TeamPlayer
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
            public int[] PlayersIndex;
        }
    }
}