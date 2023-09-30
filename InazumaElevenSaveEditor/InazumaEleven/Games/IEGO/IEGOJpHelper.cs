using System.Runtime.InteropServices;

namespace InazumaElevenSaveEditor.InazumaEleven.Games.IEGO
{
    public class IEGOJpHelper
    {
        public static long NameOffset = 0x3C;

        public static long TeamNameOffset = 0x5C;

        public static long TimeOffset = 0x24;
        //change link
        public static long LinkOffset = 0x253;

        public static long MoneyOffset = 0x8D8C;

        public static long PlayerDataOffset = 0x3438;

        public static long PlayerIndexOffset = 0x8E78;

        public static int MaximumPlayer = 112;

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
            public byte Unk3;
            public byte Unk4;
            public byte CanInvoke;
            public byte Unk5;
            public short ParticipationPoint;
            public short ScorePoint;
            public int Unk6;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public short[] InvestedPoint;
            public uint AvatarID;
            public byte AvatarLevel;
            public byte AvatarUsage;
            public byte Unk7;
            public byte Unk8;
            public int BootsIndex;
            public int BraceletIndex;
            public int PendantIndex;
            public int GlovesIndex;
            public int Unk9;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public MoveBlock[] Moves;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x38)]
            public byte[] Unk10;
        }

        public static long ItemBlockGroup1Offset = 0xA10;
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ItemBlockGroup1
        {
            public int Index;
            public uint ID;
            public int Quantity;
        }

        public static long ItemBlockGroup2Offset = 0x161C;
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ItemBlockGroup2
        {
            public int Index;
            public uint ID;
            public int Quantity;
            public int QuantityEquiped;
        }

        public static long ItemBlockGroup3Offset = 0x2428;
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
