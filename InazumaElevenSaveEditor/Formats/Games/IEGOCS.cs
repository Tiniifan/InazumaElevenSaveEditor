using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using InazumaElevenSaveEditor.Tools;
using InazumaElevenSaveEditor.Logic;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace InazumaElevenSaveEditor.Formats.Games
{
    public class CS : ContainerGames
    {
        public DataReader File { get; set; }

        public string GameNameCode => "IEGOCS";

        public int PlayerBlock => 260;

        public Dictionary<string, Player> Players { get; set; }

        public Dictionary<string, Move> Techniques { get; set; }

        public Dictionary<string, Avatar> Avatars { get; set; }

        public Dictionary<string, Equipment> Equipments { get; set; }

        public Dictionary<string, Item> Items { get; set; }

        public Save SaveInfo { get; set; }

        public List<Player> PlayersInSave { get; set; }

        public List<string> IndexSort { get; set; }

        public int NumberPlayer { get; set; }

        public int CurrentPlayer { get; set; }

        public List<Player> AuraInSave { get; set; }

        public Dictionary<string, Player> GetAllPlayers()
        {
            Dictionary<string, Player> Players = new Dictionary<string, Player>();
            return Players;
        }

        public Dictionary<string, Move> GetAllTechniques()
        {
            Dictionary<string, Move> Techniques = new Dictionary<string, Move>();
            return Techniques;
        }

        public Dictionary<string, Avatar> GetAllAvatars()
        {
            Dictionary<string, Avatar> Avatars = new Dictionary<string, Avatar>();
            return Avatars;
        }

        public Dictionary<string, Equipment> GetAllEquipments()
        {
            Dictionary<string, Equipment> Equipments = new Dictionary<string, Equipment>();
            return Equipments;
        }

        public Dictionary<string, Item> GetAllItems()
        {
            Dictionary<string, Item> Items = new Dictionary<string, Item>();
            return Items;
        }

        public CS() 
        { 

        }

        public CS(DataReader file) 
        { 
            File = file; 
        }

        public void GetPlayer(Player player, Control form)
        {

        }

        public void GetStat(Player player, Control form)
        {
        }

        public string ConvertPlayerToString(Player player, bool clipboard)
        {
            return "";
        }

        public void Open()
        {

        }

        public void Save(OpenFileDialog initialDirectory)
        {

        }

        public Player ReadPlayer(Player player)
        {
            return new Player();
        }

        public void NewStat(Player player, NumericUpDown upStat)
        {

        }
    }
}
