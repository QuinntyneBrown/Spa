using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spa.Core.Services;
using System.Collections.Generic;
using System.IO;
using static Newtonsoft.Json.JsonConvert;

namespace Spa.Core.Builders
{
    public class BarrelBuilder
    {
        private static readonly string TsConfigFilename = "tsconfig.json";

        private readonly IFileSystem _fileSystem;
        private readonly ICommandService _commandService;

        private string _source;
        private Dictionary<string, List<string>> _paths;
        private readonly string _directory = System.Environment.CurrentDirectory;
        public BarrelBuilder(ICommandService commandService, IFileSystem fileSystem, string directory = null)
        {
            _commandService = commandService;
            _fileSystem = fileSystem;

            _directory = string.IsNullOrEmpty(directory) ? _directory : directory;
            _source = $"{_directory}{Path.DirectorySeparatorChar}{TsConfigFilename}";
            _paths = new();
        }

        public BarrelBuilder LoadTsConfig(string path)
        {
            _source = path;

            return this;
        }

        public BarrelBuilder Add(string path)
        {
            _paths.Add($"@{path}", new List<string>() { @$"src/app/@{path}" });

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
            _commandService.Start("mkdir @core", $"{_directory}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}app");

            _commandService.Start("mkdir @shared", $"{_directory}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}app");

            _commandService.Start("mkdir @shell", $"{_directory}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}app");

            _fileSystem.WriteAllText($"{_directory}{Path.DirectorySeparatorChar}{TsConfigFilename}", SerializeObject(Json, Formatting.Indented));
        }
    }
}
