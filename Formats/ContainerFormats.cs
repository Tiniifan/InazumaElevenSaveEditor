using InazumaElevenSaveEditor.Tools;

namespace InazumaElevenSaveEditor.Formats
{
    public interface SaveFormat
    {
        string Name { get; }

        string Extension { get; }

        string Description { get; }

        bool CanOpen { get; }

        bool CanSave { get; }

        bool CanBeCompressed { get; }

        ContainerGames Open(DataReader file);
    }
}
