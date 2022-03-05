using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spa.Core.Models;
using Spa.Core.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Newtonsoft.Json.JsonConvert;

namespace Spa.Core.Builders
{
    public interface IBarrelGenerationStrategy
    {
        void Create(SinglePageApplicationModel model);
    }

    public class BarrelGenerationStrategy: IBarrelGenerationStrategy
    {
        protected readonly IFileSystem _fileSystem;

        public BarrelGenerationStrategy(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        private JObject _json(SinglePageApplicationModel model)
        {
            var json = DeserializeObject<JObject>(_fileSystem.ReadAllText(model.TsConfigFullFilePath));

            var paths = new Dictionary<string, List<string>>();

            foreach(var path in model.Barrels)
            {
                paths.Add($"@{path}", new List<string>() { @$"src/app/@{path}" });
                paths.Add($"@{path}/*", new List<string>() { @$"src/app/@{path}/*" });
            }
            json["compilerOptions"]["paths"] = JToken.FromObject(paths);

            return json;
        }

        public void Create(SinglePageApplicationModel model)
        {
            foreach (var path in model.Barrels)
            {                
                _fileSystem.CreateDirectory($"{model.Directory}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}app{Path.DirectorySeparatorChar}@{path}");
            }

            _fileSystem.WriteAllText(model.TsConfigFullFilePath, SerializeObject(_json(model), Formatting.Indented));
        }
    }
}
