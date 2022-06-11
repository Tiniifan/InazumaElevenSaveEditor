using InazumaElevenSaveEditor.Tools;

namespace InazumaElevenSaveEditor.Formats
{
    public interface IFormat
    {
        string Name { get; }

        string Extension { get; }

        string Description { get; }

        bool CanOpen { get; }

        bool CanSave { get; }

        bool CanBeCompressed { get; }

        IGame Open(DataReader file);
    }
}
