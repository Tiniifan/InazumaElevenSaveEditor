using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace InazumaElevenSaveEditor.Tools
{
    public class ResourceReader
    {
        public string Path;

        public List<string> Content;

        public ResourceReader()
        {

        }

        public ResourceReader(string file)
        {
            this.Path = file;
            BinaryReader reader = new BinaryReader(GetResourceStream());
            string text = new StreamReader(GetResourceStream()).ReadToEnd();
            Content = Regex.Split(text, "\r\n|\r|\n").ToList();
        }

        public Stream GetResourceStream()
        {
            string resourcePath = Path;
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            List<string> source = new List<string>(executingAssembly.GetManifestResourceNames());
            resourcePath = source.FirstOrDefault((string r) => r.Contains(resourcePath));
            if (resourcePath == null)
            {
                throw new FileNotFoundException("Resource not found");
            }
            return executingAssembly.GetManifestResourceStream(resourcePath);
        }
    }
}
