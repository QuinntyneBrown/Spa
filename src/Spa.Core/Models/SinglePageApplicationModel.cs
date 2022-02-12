using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Spa.Core.Models
{
    public class SinglePageApplicationModel
    {
        public List<string> Barrels { get; set; } = new List<string>
        {
            "api","shared","core"
        };

        public string TsConfigFullFilePath { get; set; }

        public string AppDirectory { get; set; }
        public string ScssDirectory { get; set; }

        public SinglePageApplicationModel(Settings settings)
        {
            AppDirectory = settings.AppDirectories.First();

            TsConfigFullFilePath = $"{AppDirectory}{Path.DirectorySeparatorChar}tsconfig.json";
        }
    }
}
