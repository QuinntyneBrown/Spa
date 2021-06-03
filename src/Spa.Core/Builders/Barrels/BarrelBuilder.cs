using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spa.Core.Models;
using Spa.Core.Services;
using System.Collections.Generic;
using System.IO;
using static Newtonsoft.Json.JsonConvert;

namespace Spa.Core.Builders
{
    public class BarrelBuilder
    {
        protected static readonly string TsConfigFilename = "tsconfig.json";
        protected readonly IFileSystem _fileSystem;
        protected readonly ICommandService _commandService;

        protected string _source;
        protected Dictionary<string, List<string>> _paths;
        protected readonly string _appDirectory;

        public BarrelBuilder(ICommandService commandService, IFileSystem fileSystem, string appDirectory)
        {
            _commandService = commandService;
            _fileSystem = fileSystem;
            _appDirectory = appDirectory;
            _source = $"{_appDirectory}{Path.DirectorySeparatorChar}{TsConfigFilename}";
            _paths = new();
        }

        public BarrelBuilder LoadTsConfig(string path)
        {
            _source = path;

            return this;
        }

        public BarrelBuilder Add(string path)
        {

            _paths.Add($"@{((Token)path).SnakeCase}", new List<string>() { @$"src/app/@{((Token)path).SnakeCase}" });
            _paths.Add($"@{((Token)path).SnakeCase}/*", new List<string>() { @$"src/app/@{((Token)path).SnakeCase}/*" });   

            return this;
        }

        public JObject Json
        {
            get
            {
                var json = DeserializeObject<JObject>(_fileSystem.ReadAllText(_source));

                var existingPaths = json["compilerOptions"]["paths"]?.ToObject<Dictionary<string, List<string>>>();

                if(existingPaths != null)
                {
                    foreach(var path in existingPaths)
                    {
                        if(!_paths.ContainsKey(path.Key))
                        {
                            _paths.Add(path.Key, path.Value);
                        }
                    }
                }

                json["compilerOptions"]["paths"] = JToken.FromObject(_paths);

                return json;
            }
        }

        public void Build()
        {
            foreach(var path in _paths)
            {
                if (!path.Key.Contains("*"))
                {
                    _commandService.Start($"mkdir {path.Key}", $"{_appDirectory}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}app");
                }
            }
            
            _fileSystem.WriteAllText($"{_source}", SerializeObject(Json, Formatting.Indented));
        }
    }
}
