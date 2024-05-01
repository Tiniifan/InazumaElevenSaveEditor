using System.Runtime.InteropServices;

namespace InazumaElevenSaveEditor.InazumaEleven.Games.IEGO
{
    public sealed class IEGOHelper
    {
        public long NameOffset = 0x3C;

        public long TeamNameOffset = 0x64;

        public long TimeOffset = 0x24;

        public long LinkOffset = 0x253;

        public long MoneyOffset = 0x8DDC;

        public long PlayerDataOffset = 0x3488;

        public long PlayerIndexOffset = 0x8EC8;

        public uint MainTeamInfoOffset = 0x8DF0;

        public uint MainTeamPlayersOffset = 0x8EC8;

        public uint CustomTeamInfoOffset = 0x8E20;

        public uint CustomTeamPlayersOffset = 0x9088;

        public int MaximumPlayer = 112;

        public bool IsJP;

        private IEGOHelper(bool isJP) {
            IsJP = isJP;
            if (isJP)
            {
                TeamNameOffset = 0x5C;
                MoneyOffset = 0x8D8C;
                PlayerDataOffset = 0x3438;
                PlayerIndexOffset = 0x8E78;
                ItemBlockGroup1Offset = 0xA10;
                ItemBlockGroup2Offset = 0x161C;
                ItemBlockGroup3Offset = 0x2428;
                MainTeamInfoOffset = 0x8DA0;
                MainTeamPlayersOffset = 0x8E78;
                CustomTeamInfoOffset = 0x8DD0;
                CustomTeamPlayersOffset = 0x9038;
            }

        }

        private static IEGOHelper _instance;

        public static IEGOHelper GetInstance(bool isJP)
        { 
            _instance = new IEGOHelper(isJP);
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
            public byte IsKeyPlayer;
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

        public long ItemBlockGroup1Offset = 0xA60;
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ItemBlockGroup1
        {
            public int Index;
            public uint ID;
            public int Quantity;
        }

        public long ItemBlockGroup2Offset = 0x166C;
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ItemBlockGroup2
        {
            public int Index;
            public uint ID;
            public int Quantity;
            public int QuantityEquiped;
        }

        public long ItemBlockGroup3Offset = 0x2478;
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
