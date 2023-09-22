using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InazumaElevenSaveEditor.InazumaEleven.Logic;

namespace InazumaElevenSaveEditor.InazumaEleven.INZ
{
    public interface IPlayerFiles
    {
        string Extension { get; }

        string GameNameCode { get; }

        string NewFile(Player player);

        Player NewPlayer(string path);
    }
}
