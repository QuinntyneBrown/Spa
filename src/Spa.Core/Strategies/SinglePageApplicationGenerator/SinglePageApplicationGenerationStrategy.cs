using Allagi.SharedKernal;
using Spa.Core.Builders;
using Spa.Core.Models;
using Spa.Core.Services;
using Spa.Core.Strategies.GlobalGeneration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using static System.Text.Json.JsonSerializer;

namespace Spa.Core.Strategies
{

    public class SinglePageApplicationGenerationStrategy: ISinglePageApplicationGenerationStrategy
    {
        private readonly ICommandService _commandService;
        private readonly IFileSystem _fileSystem;
        private readonly IPackageJsonService _packageJsonService;
        private readonly ISettingsGenerationStrategyFactory _settingsGenerationStrategyFactory;
        private readonly ITemplateLocator _templateLocator;
        private readonly ITemplateProcessor _templateProcessor;
        
        public SinglePageApplicationGenerationStrategy(ICommandService commandService, IFileSystem fileSystem, IPackageJsonService packageJsonService, ITemplateLocator templateLocator, ITemplateProcessor templateProcessor)
        {
            _commandService = commandService;
            _fileSystem = fileSystem;
            _packageJsonService = packageJsonService;
            _settingsGenerationStrategyFactory = new SettingsGenerationStrategyFactory(commandService, fileSystem);
            _templateLocator = templateLocator;
            _templateProcessor = templateProcessor;
            
        }

        public bool CanHandle(Settings settings)
        {
            return true;
        }

        public void Create(Settings settings, string rootName, string prefix, string directory)
        {
            settings = SettingsGenerator.Create(settings, _settingsGenerationStrategyFactory, rootName, prefix, directory);

            var appName = $"{rootName}.App";

            var temporaryName = "App";

            var srcDirectory =  $"{settings.RootDirectory}{Path.DirectorySeparatorChar}{settings.SourceFolder}";

            _commandService.Start($"ng new {temporaryName} --prefix={settings.Prefix} --style=scss --strict=false --routing", srcDirectory);

            _commandService.Start($"ren {temporaryName} {appName}", $"{srcDirectory}{Path.DirectorySeparatorChar}");

            new IndexHtmlFileGenerationStrategy(_fileSystem).Generate(settings);

            new RootAppComponentGenerationStrategy(_fileSystem, _templateLocator, _templateProcessor).Generate(settings);
            
            _commandService.Start($"spa swagger-gen", srcDirectory);

            var appDirectory = $"{srcDirectory}{Path.DirectorySeparatorChar}{appName}";

            _packageJsonService.AddGenerateModelsNpmScript($"{appDirectory}{Path.DirectorySeparatorChar}package.json");

            _commandService.Start("npm i --save-dev ng-swagger-gen", appDirectory);

            var angularJson = new AngularJsonProvider().Get(appDirectory);

            new BarrelGenerationStrategy(_fileSystem).Create(new SinglePageApplicationModel(settings));

            _commandService.Start("mkdir scss", angularJson.SrcDirectory);

            _fileSystem.WriteAllText(angularJson.Styles, "@use '/src/scss/index';");

            _fileSystem.WriteAllText($"{angularJson.ScssDirectory}{Path.DirectorySeparatorChar}index.scss", string.Empty);

            var spaDirectory = $"{srcDirectory}{Path.DirectorySeparatorChar}{appName}{Path.DirectorySeparatorChar}";

            _commandService.Start("spa constructor", spaDirectory);

            _commandService.Start("spa destroyable", spaDirectory);

            _commandService.Start("spa combine", spaDirectory);

            _commandService.Start("spa constants", spaDirectory);

            _commandService.Start("spa app-module", spaDirectory);

            _commandService.Start("spa swagger-gen", spaDirectory);

            _commandService.Start("ng add @angular/material", angularJson.RootDirectory);

            var rootScssFileFullPath = $"{appDirectory}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}styles.scss";

            var rootScssFile = new RootScssFileModel(_fileSystem.ReadAllLines(rootScssFileFullPath).ToList());

            rootScssFile.EnsureUseStatementsBeginFile();

            File.WriteAllLines(rootScssFileFullPath, rootScssFile.Lines);

            _commandService.Start("spa .", angularJson.ApiDirectory);

            _commandService.Start("code .", $"{srcDirectory}{Path.DirectorySeparatorChar}{appName}");
        }
    }
}
