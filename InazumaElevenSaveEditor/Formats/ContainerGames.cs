using System;
using System.Windows.Forms;
using System.Collections.Generic;
using InazumaElevenSaveEditor.Logic;
using InazumaElevenSaveEditor.Tools;

namespace InazumaElevenSaveEditor.Formats
{
    public interface ContainerGames
    {
        DataReader File { get; set; }

        string GameNameCode { get; }

        IDictionary<UInt32, Player> Players { get; set; }

        IDictionary<UInt32, Move> Moves { get; set; }

        IDictionary<UInt32, Avatar> Avatars { get; set; }

        IDictionary<UInt32, Equipment> Equipments { get; set; }

        IDictionary<UInt32, Item> Items { get; set; }

        Save SaveInfo { get; set; }

        Dictionary<UInt32, Player> PlayersInSave { get; set; }

        List<UInt32> PlayersInSaveSort { get; set; }

        Player GetPlayer(int index);

        string ConvertPlayerToString(Player player, bool clipboard);

        void Open();

        void OpenTactics();

        void Save(OpenFileDialog initialDirectory);

        Player ReadPlayer(Player player);

        void NewStat(Player player, NumericUpDown upStat);
    }
}
