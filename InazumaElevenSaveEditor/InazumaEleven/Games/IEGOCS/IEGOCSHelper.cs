using System.Runtime.InteropServices;

namespace InazumaElevenSaveEditor.InazumaEleven.Games.IEGOCS
{
    public sealed class IEGOCSHelper
    {
        public long NameOffset = 0x3C;

        public long TeamNameOffset = 0x64;

        public long TimeOffset = 0x20;

        public long LinkOffset = 0x3B4;

        public long ChapterOffset = 0x10FC;

        public long MoneyOffset = 0x1C0B0;

        public long PlayRecordsOffset = 0x2EC;

        public long PlayerDataOffset = 0x5B1C;

        public long PlayerIndexOffset = 0x1C4E0;

        public uint MainTeamInfoOffset = 0x1C0C4;

        public uint MainTeamNameOffset = 0x1C494;

        public uint MainTeamPlayersOffset = 0x1C4E0;

        public uint CustomTeamInfoOffset = 0x1C0F4;

        public uint CustomTeamNameOffset = 0x1C304;

        public uint CustomTeamPlayersOffset = 0x1CA60;

        public int MaximumPlayer = 336;

        public bool IsJP;
        private IEGOCSHelper(bool isJP)
        {
            IsJP = isJP;
            if (isJP)
            {
                TeamNameOffset = 0x5C;
                LinkOffset = 0x364;
                ChapterOffset = 0x10AC;
                MoneyOffset = 0x1C060;
                PlayRecordsOffset = 0x29C;
                PlayerDataOffset = 0x5ACC;
                PlayerIndexOffset = 0x1C438;
                ItemBlockGroup1Offset = 0x14A4;
                ItemBlockGroup2Offset = 0x2CB0;
                ItemBlockGroup3Offset = 0x41BC;
                MainTeamInfoOffset = 0x1C074;
                MainTeamNameOffset = 0x1C3F4;
                MainTeamPlayersOffset = 0x1C438;
                CustomTeamInfoOffset = 0x1C0A4;
                CustomTeamNameOffset = 0X1C2B4;
                CustomTeamPlayersOffset = 0x1C9B8;


            }

        }

        private static IEGOCSHelper _instance;

        public static IEGOCSHelper GetInstance(bool isJP)
        {
            _instance = new IEGOCSHelper(isJP);
            return _instance;
        }

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

        public long ItemBlockGroup1Offset = 0x14F4;
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ItemBlockGroup1
        {
            public int Index;
            public uint ID;
            public int Quantity;
        }

        public long ItemBlockGroup2Offset = 0x2D00;
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ItemBlockGroup2
        {
            public int Index;
            public uint ID;
            public int Quantity;
            public int QuantityEquiped;
        }

        public long ItemBlockGroup3Offset = 0x420C;
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