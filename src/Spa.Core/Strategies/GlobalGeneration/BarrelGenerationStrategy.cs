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
        protected readonly ICommandService _commandService;

        public BarrelGenerationStrategy(ICommandService commandService, IFileSystem fileSystem, string appDirectory)
        {
            _commandService = commandService;
            _fileSystem = fileSystem;
        }

        private JObject _json(SinglePageApplicationModel model)
        {
            var json = DeserializeObject<JObject>(_fileSystem.ReadAllText(model.TsConfigFullFilePath));

            json["compilerOptions"]["paths"] = JToken.FromObject(model.Barrels.Select(x => x.ToPath()));

            return json;
        }

        public void Create(SinglePageApplicationModel model)
        {
            foreach (var path in model.Barrels.Select(x => x.ToPath()))
            {                
                if (!path.Key.Contains("*"))
                {
                    _commandService.Start($"mkdir {path.Key}", $"{model.AppDirectory}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}app");
                }
            }

            _fileSystem.WriteAllText(model.TsConfigFullFilePath, SerializeObject(_json(model), Formatting.Indented));
        }
    }

    public static class StringExtensions
    {
        public static KeyValuePair<string,List<string>> ToPath(this string value)
            => new KeyValuePair<string, List<string>>($"@{((Token)value).SnakeCase}", new List<string>() { @$"src/app/@{((Token)value).SnakeCase}" });
    }
}
