using System.Collections.Generic;
using System.IO;

namespace Spa.Core.Models
{
    public class SinglePageApplicationModel
    {
        public List<string> Barrels { get; set; } = new List<string>
        {
            "api","shared","core"
        };

        public string TsConfigFullFilePath => $"{Directory}{Path.DirectorySeparatorChar}tsconfig.json";
        public string Directory { get; private set; }
        public string Prefix { get; private set; }
        public string SrcDirectory => $"{Directory}{Path.DirectorySeparatorChar}src";
        public string AppDirectory => $"{SrcDirectory}{Path.DirectorySeparatorChar}app";
        public string ScssDirectory => $"{SrcDirectory}{Path.DirectorySeparatorChar}scss";
        public string IndexHtmlPath => $"{SrcDirectory}{Path.DirectorySeparatorChar}index.html";
        public SinglePageApplicationModel(string directory,string prefix = "app")
        {
            Directory = directory;
            Prefix = prefix;
        }
    }
}
