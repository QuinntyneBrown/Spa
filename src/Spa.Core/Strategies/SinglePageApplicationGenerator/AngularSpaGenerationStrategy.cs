using Spa.Core.Builders;
using Spa.Core.Models;
using Spa.Core.Services;
using Spa.Core.Strategies.Additions;
using Spa.Core.Strategies.GlobalGeneration;
using System;
using System.IO;
using System.Linq;

namespace Spa.Core.Strategies.SinglePageApplicationGenerator
{
    public class AngularSpaGenerationStrategy : ISpaGenerationStrategy
    {
        private readonly ICommandService _commandService;
        private readonly IFileSystem _fileSystem;
        private readonly ITemplateLocator _templateLocator;
        private readonly ITemplateProcessor _templateProcessor;
        private readonly IPackageJsonService _packageJsonService;

        public AngularSpaGenerationStrategy(ICommandService commandService, IFileSystem fileSystem, ITemplateLocator templateLocator, ITemplateProcessor templateProcessor, IPackageJsonService packageJsonService)
        {
            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _templateLocator = templateLocator ?? throw new ArgumentNullException(nameof(templateLocator));
            _templateProcessor = templateProcessor ?? throw new ArgumentNullException(nameof(templateProcessor));
            _packageJsonService = packageJsonService ?? throw new ArgumentNullException(nameof(packageJsonService));
        }

        public void Create(string name, string prefix, string directory)
        {
            int retries = 0;

            string originalName = name;

            while (true)
            {
                var appDirectory = $"{directory}{Path.DirectorySeparatorChar}{name}";

                if (!Directory.Exists(appDirectory))
                {
                    var temporaryName = $"{Guid.NewGuid()}".Replace("-", "");

                    var model = new SinglePageApplicationModel($"{directory}{Path.DirectorySeparatorChar}{name}", prefix);

                    var packageJsonFilePath = $"{model.Directory}{Path.DirectorySeparatorChar}package.json";

                    var angularJsonFilePath = $"{model.Directory}{Path.DirectorySeparatorChar}angular.json";

                    _commandService.Start($"ng new {temporaryName} --prefix={model.Prefix} --style=scss --strict=false --routing", directory);

                    Directory.Move($"{directory}{Path.DirectorySeparatorChar}{temporaryName}", appDirectory);

                    _packageJsonService.SetName(packageJsonFilePath, name);

                    var appComponentPath = $"{model.AppDirectory}{Path.DirectorySeparatorChar}app.component.ts";

                    File.WriteAllText(appComponentPath,File.ReadAllText(appComponentPath).Replace(temporaryName,name));

                    File.WriteAllText(angularJsonFilePath, File.ReadAllText(angularJsonFilePath).Replace(temporaryName, name));

                    new IndexHtmlFileGenerationStrategy(_fileSystem).Generate(model.IndexHtmlPath);

                    new RootAppComponentGenerationStrategy(_fileSystem, _templateLocator, _templateProcessor).Generate(model.Directory, model.Prefix);

                    _commandService.Start($"spa swagger-gen", model.Directory);

                    _packageJsonService.AddGenerateModelsNpmScript(packageJsonFilePath);

                    _commandService.Start("npm i --save-dev ng-swagger-gen", model.Directory);

                    var angularJson = new AngularJsonProvider().Get(model.Directory);

                    new BarrelGenerationStrategy(_fileSystem).Create(model);

                    _commandService.Start("mkdir scss", angularJson.SrcDirectory);

                    _fileSystem.WriteAllText(angularJson.Styles, "@use '/src/scss/index';");

                    _fileSystem.WriteAllText($"{angularJson.ScssDirectory}{Path.DirectorySeparatorChar}index.scss", string.Empty);

                    _commandService.Start("spa constructor", model.Directory);

                    _commandService.Start("spa destroyable", model.Directory);

                    _commandService.Start("spa combine", model.Directory);

                    _commandService.Start("spa constants", model.Directory);

                    _commandService.Start("spa app-module", model.Directory);

                    _commandService.Start("spa swagger-gen", model.Directory);

                    _commandService.Start("spa default-scss", model.Directory);

                    new AddTranslationsStrategy(_commandService, _fileSystem).Add(model);

                    _commandService.Start("ng add @angular/material", angularJson.RootDirectory);

                    var rootScssFileFullPath = $"{model.Directory}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}styles.scss";

                    var rootScssFile = new RootScssFileModel(_fileSystem.ReadAllLines(rootScssFileFullPath).ToList());

                    rootScssFile.EnsureUseStatementsBeginFile();

                    File.WriteAllLines(rootScssFileFullPath, rootScssFile.Lines);

                    _commandService.Start("spa .", angularJson.ApiDirectory);

                    _commandService.Start("code .", $"{model.Directory}");

                    return;
                }

                retries++;

                name = $"{originalName}{retries}";
            }
        }
    }
}
