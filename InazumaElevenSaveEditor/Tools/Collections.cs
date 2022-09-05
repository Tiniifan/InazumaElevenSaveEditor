using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.Threading.Tasks;

namespace InazumaElevenSaveEditor.Tools
{
    // Extended Class for System.Collections.Generic 

    public static class Collections
    {
        public static double SumIf<T>(this IEnumerable<T> source, Func<T, bool> predicate, Func<T, double> valueSelector)
        {
            return source.Where(predicate)
                         .Sum(valueSelector);
        }
    }
}
