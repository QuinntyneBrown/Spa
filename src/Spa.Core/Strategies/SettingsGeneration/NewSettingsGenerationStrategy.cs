using Allagi.SharedKernal;
using Spa.Core.Models;
using Spa.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using static System.Text.Json.JsonSerializer;


namespace Spa.Core.Strategies
{
    public class NewSettingsGenerationStrategy : ISettingsGenerationStrategy
    {
        private readonly ICommandService _commandService;
        private readonly IFileSystem _fileSystem;

        public NewSettingsGenerationStrategy(ICommandService commandService, IFileSystem fileSystem)
        {
            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(_fileSystem));
        }

        public bool CanHandle(Settings model)
        {
            return model == null;
        }

        public Settings Create(Settings model, string rootName, string prefix, string directory, bool minimal)
        {
            var spaFullName = $"{rootName}.App";

            model = new Settings();

            model.Prefix = prefix;

            model.SolutionName = rootName;

            model.RootDirectory = $"{directory}{Path.DirectorySeparatorChar}{rootName}";

            if (!Directory.Exists(model.RootDirectory))
            {
                _commandService.Start($"mkdir {model.RootDirectory}");
            }

            var srcDirectory = $"{model.RootDirectory}{Path.DirectorySeparatorChar}{model.SourceFolder}";

            if (!Directory.Exists(srcDirectory))
            {
                _commandService.Start($"mkdir {srcDirectory}");
            }

            model.AddApp($"{model.RootDirectory}{Path.DirectorySeparatorChar}{model.SourceFolder}{Path.DirectorySeparatorChar}{spaFullName}", _fileSystem);


            var json = Serialize(model, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            _fileSystem.WriteAllLines($"{model.RootDirectory}{Path.DirectorySeparatorChar}{CoreConstants.SettingsFileName}", new List<string> { json }.ToArray());

            return model;
        }
    }
}
