using System;
using System.Windows.Forms;

namespace NoFarmForMeOpenSource
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(args.Length == 0 ? new Welcome(string.Empty) : new Welcome(args[0]));
        }
    }
}
