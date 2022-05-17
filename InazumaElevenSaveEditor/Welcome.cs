using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InazumaElevenSaveEditor.Logic;
using InazumaElevenSaveEditor.Common.InazumaElevenGo.Avatars;

namespace NoFarmForMeOpenSource
{
    public partial class Welcome : Form
    {
        private Size InitialWelcomeSize;

        public Welcome()
        {
            InitializeComponent();
            InitialWelcomeSize = this.Size;

            IDictionary<uint, Avatar> test = Avatars.Cs;
            Console.WriteLine(test[0x49E09112].Name);
        }

        private void Welcome_Resize(object sender, EventArgs e)
        {
            // label25.Font = new Font(label25.Font.FontFamily, label25.Font.Size+1, FontStyle.Regular);
        }

        private void Welcome_SizeChanged(object sender, EventArgs e)
        {
        }

        private void Welcome_Layout(object sender, LayoutEventArgs e)
        {
        }

        private void Welcome_Load(object sender, EventArgs e)
        {

        }
    }
}
