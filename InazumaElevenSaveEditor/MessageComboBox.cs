using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace InazumaElevenSaveEditor
{
    public partial class MessageComboBox : Form
    {
        public MessageComboBox(string formText, string labelText, string buttonText, ComboBox playerComboBox)
        {
            InitializeComponent();

            this.Text = formText;
            sentenceText.Text = labelText;
            confirmButton.Text = buttonText;

            nameBox.Sorted = playerComboBox.Sorted;
            nameBox.Items.AddRange(playerComboBox.Items.Cast<Object>().ToArray());
            nameBox.SelectedIndex = 0;
        }
    }
}
