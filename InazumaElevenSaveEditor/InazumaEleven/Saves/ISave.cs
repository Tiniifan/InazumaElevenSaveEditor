using System.Windows.Forms;
using InazumaElevenSaveEditor.Tools;
using InazumaElevenSaveEditor.InazumaEleven.Games;

namespace InazumaElevenSaveEditor.InazumaEleven.Saves
{
    public interface ISave
    {
        string Name { get; }

        string Extension { get; }

        string Description { get; }

        IGame Game { get; set; }

        void Open(BinaryDataReader reader);

        void Save(OpenFileDialog initialDirectory);
    }
}
