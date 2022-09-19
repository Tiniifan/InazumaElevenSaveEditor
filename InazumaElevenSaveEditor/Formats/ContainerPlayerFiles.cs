using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using InazumaElevenSaveEditor.Logic;
using InazumaElevenSaveEditor.Tools;

namespace InazumaElevenSaveEditor.Formats
{
    public interface IPlayerFiles
    {
        string Extension { get; }

        string GameNameCode { get; }

        string NewFile(Player player);

        Player NewPlayer(string path);
    }
}
