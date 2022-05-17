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

        int PlayerBlock { get; }

        Dictionary<string, Player> Players { get; set; }

        Dictionary<string, Move> Techniques { get; set; }

        Dictionary<string, Avatar> Avatars { get; set; }

        Dictionary<string, Equipment> Equipments { get; set; }

        Dictionary<string, Item> Items { get; set; }
        Save SaveInfo { get; set; }

        List<Player> PlayersInSave { get; set; }

        List<string> IndexSort { get; set; }

        int NumberPlayer { get; set; }

        int CurrentPlayer { get; set; }

        Dictionary<string, Player> GetAllPlayers();

        Dictionary<string, Move> GetAllTechniques();

        Dictionary<string, Avatar> GetAllAvatars();

        Dictionary<string, Equipment> GetAllEquipments();

        Dictionary<string, Item> GetAllItems();

        void GetPlayer(Player player, Control form);

        string ConvertPlayerToString(Player player, bool clipboard);

        void Open();

        void Save(OpenFileDialog initialDirectory);

        Player ReadPlayer(Player player);

        void NewStat(Player player, NumericUpDown upStat);
    }
}
