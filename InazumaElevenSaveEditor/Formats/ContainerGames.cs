using System;
using System.Windows.Forms;
using System.Collections.Generic;
using InazumaElevenSaveEditor.Logic;
using InazumaElevenSaveEditor.Tools;

namespace InazumaElevenSaveEditor.Formats
{
    public interface IGame
    {
        DataReader File { get; set; }

        string GameNameCode { get; }

        int MaximumPlayer { get; }

        IDictionary<UInt32, Player> Players { get; set; }

        IDictionary<UInt32, Move> Moves { get; set; }

        IDictionary<UInt32, Avatar> Avatars { get; set; }

        IDictionary<UInt32, Equipment> Equipments { get; set; }

        IDictionary<UInt32, Item> Items { get; set; }

        Save SaveInfo { get; set; }

        Dictionary<UInt32, Player> PlayersInSave { get; set; }

        Dictionary<UInt32, Player> AuraInSave { get; set; }

        List<UInt32> PlayersInSaveSort { get; set; }

        Player GetPlayer(int index);

        void ChangePlayer(Player selectedPlayer, KeyValuePair<UInt32, Player> newPlayer);

        void ChangePlayer(Player selectedPlayer, Player newPlayer);

        void RecruitPlayer(KeyValuePair<UInt32, Player> newPlayer);

        void RecruitPlayer(Player newPlayer);

        void Open();

        void OpenTactics();

        void OpenSaveInfo();

        void OpenPlayRecords();

        void Save(OpenFileDialog initialDirectory);

        void NewMixiMax(Player player, UInt32 playerAura, int mixiMaxMove1, int mixiMaxMove2);

        void UpdateResource();

        (int, int, string, bool) Training(Player player, int newStat, int statIndex);
    }
}
