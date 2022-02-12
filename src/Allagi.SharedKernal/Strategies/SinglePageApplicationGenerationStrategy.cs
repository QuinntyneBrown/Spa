using Allagi.SharedKernal;
using Spa.Core.Builders;
using Spa.Core.Models;
using Spa.Core.Services;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using static System.Text.Json.JsonSerializer;

namespace Spa.Core.Strategies
{

    public class SinglePageApplicationGenerationStrategy: ISinglePageApplicationGenerationStrategy
    {
        private readonly ICommandService _commandService;
        private readonly IFileSystem _fileSystem;
        private readonly IPackageJsonService _packageJsonService;

        public SinglePageApplicationGenerationStrategy(ICommandService commandService, IFileSystem fileSystem, IPackageJsonService packageJsonService)
        {
            _commandService = commandService;
            _fileSystem = fileSystem;
            _packageJsonService = packageJsonService;
        }

        public bool CanHandle(Settings settings)
        {
            return true;
        }

        public void Create(Settings settings, string rootName, string prefix, string directory)
        {
            var spaFullName = $"{rootName}.App";

            var spaName = "App";

            var srcDirectory = string.Empty;

            if (settings == null)
            {
                settings = new Settings();

                settings.Prefix = prefix;

                settings.SolutionName = rootName;

                settings.RootDirectory = $"{directory}{Path.DirectorySeparatorChar}{rootName}";

                if (!Directory.Exists(settings.RootDirectory))
                {
                    _commandService.Start($"mkdir {settings.RootDirectory}");
                }

                srcDirectory = $"{settings.RootDirectory}{Path.DirectorySeparatorChar}{settings.SourceFolder}";

                if (!Directory.Exists(srcDirectory))
                {
                    _commandService.Start($"mkdir {srcDirectory}");
                }
            }


            settings.AddApp($"{settings.RootDirectory}{Path.DirectorySeparatorChar}{settings.SourceFolder}{Path.DirectorySeparatorChar}{spaFullName}", _fileSystem);


            var json = Serialize(settings, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            _fileSystem.WriteAllLines($"{settings.RootDirectory}{Path.DirectorySeparatorChar}{CoreConstants.SettingsFileName}", new List<string> { json }.ToArray());

            srcDirectory =  $"{settings.RootDirectory}{Path.DirectorySeparatorChar}{settings.SourceFolder}";

            _commandService.Start($"ng new {spaName} --prefix={settings.Prefix} --style=scss --strict=false --routing", srcDirectory);

            _commandService.Start($"spa swagger-gen", srcDirectory);

            var appDirectory = $"{srcDirectory}{Path.DirectorySeparatorChar}{spaName}";

            _packageJsonService.AddGenerateModelsNpmScript($"{appDirectory}{Path.DirectorySeparatorChar}package.json");

            _commandService.Start("npm i --save-dev ng-swagger-gen", appDirectory);

            var angularJson = new AngularJsonProvider().Get(appDirectory);

            new BarrelGenerator(_commandService, _fileSystem, appDirectory)
                .Add("core")
                .Add("shared")
                .Add("api")
                .Build();

            _commandService.Start("mkdir scss", angularJson.SrcDirectory);

            _fileSystem.WriteAllText(angularJson.Styles, "@use '/src/scss/index';");

            _fileSystem.WriteAllText($"{angularJson.ScssDirectory}{Path.DirectorySeparatorChar}index.scss", string.Empty);

            var spaDirectory = $"{srcDirectory}{Path.DirectorySeparatorChar}{spaName}{Path.DirectorySeparatorChar}";

            _commandService.Start("spa breakpoints", spaDirectory);

            _commandService.Start("spa constructor", spaDirectory);

            _commandService.Start("spa destroyable", spaDirectory);

            _commandService.Start("spa combine", spaDirectory);

            _commandService.Start("spa constants", spaDirectory);

            _commandService.Start("spa app-module", spaDirectory);

            _commandService.Start("spa swagger-gen", spaDirectory);

            _commandService.Start("ng add @angular/material", angularJson.RootDirectory);

            _commandService.Start("spa .", angularJson.ApiDirectory);

            _commandService.Start($"ren {spaName} {spaFullName}", $"{srcDirectory}{Path.DirectorySeparatorChar}");

            _commandService.Start("code .", $"{srcDirectory}{Path.DirectorySeparatorChar}{spaFullName}");
        }
    }
}
