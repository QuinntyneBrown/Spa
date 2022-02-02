using Spa.Core.Services;
using System.IO;

namespace Spa.Core.Builders
{
    public class SpaBuilder
    {
        private readonly ICommandService _commandService;
        private readonly IFileSystem _fileSystem;
        private readonly IPackageJsonService _packageJsonService;

        private string _srcDirectory = System.Environment.CurrentDirectory;
        private string _name = $"Default";
        private string _solutionName = "";
        private string _resources;
        private string _prefix = "app";
        private bool _hasPublicApp;
        private bool _hasWorkspaceApp;
        private bool _hasLogin;

        public SpaBuilder(ICommandService commandService, IFileSystem fileSystem, string srcDirectory, string solutionName, string name, string resources = null)
        {
            _commandService = commandService;
            _fileSystem = fileSystem;
            _solutionName = solutionName;
            _name = name;
            _srcDirectory = srcDirectory;
            _resources ??= resources;
            _packageJsonService = new PackageJsonService();

        }

        public SpaBuilder WithWorkspaceApp(bool hasWorkspaceApp = true)
        {
            _hasWorkspaceApp = hasWorkspaceApp;
            return this;
        }

        public SpaBuilder WithPublicApp(bool hasPublicApp = true)
        {
            _hasPublicApp = hasPublicApp;
            return this;
        }

        public SpaBuilder WithPrefix(string prefix)
        {
            _prefix = prefix;
            return this;
        }

        public void Build()
        {
            _commandService.Start($"ng new {_name} --prefix={_prefix} --style=scss --strict=false --routing", _srcDirectory);

            _commandService.Start($"spa swagger-gen", _srcDirectory);

            var appDirectory = $"{_srcDirectory}{Path.DirectorySeparatorChar}{_name}";

            _packageJsonService.AddGenerateModelsNpmScript($"{appDirectory}{Path.DirectorySeparatorChar}package.json");

            _commandService.Start("npm i --save-dev ng-swagger-gen", appDirectory);

            var angularJson = new AngularJsonProvider().Get(appDirectory);

            new BarrelBuilder(_commandService, _fileSystem, appDirectory)
                .Add("core")
                .Add("shared")
                .Add("api")
                .Build();

            _commandService.Start("mkdir scss", angularJson.SrcDirectory);

            _fileSystem.WriteAllText($"{angularJson.ScssDirectory}{Path.DirectorySeparatorChar}_index.scss", string.Empty);

            _fileSystem.WriteAllText(angularJson.Styles, "@use '/src/scss/index';");

            _fileSystem.WriteAllText($"{angularJson.ScssDirectory}{Path.DirectorySeparatorChar}_index.scss", string.Empty);

            _commandService.Start("spa breakpoints", $"{_srcDirectory}{Path.DirectorySeparatorChar}{_name}{Path.DirectorySeparatorChar}");

            _commandService.Start("spa constructor", $"{_srcDirectory}{Path.DirectorySeparatorChar}{_name}{Path.DirectorySeparatorChar}");

            _commandService.Start("spa destroyable", $"{_srcDirectory}{Path.DirectorySeparatorChar}{_name}{Path.DirectorySeparatorChar}");

            _commandService.Start("spa combine", $"{_srcDirectory}{Path.DirectorySeparatorChar}{_name}{Path.DirectorySeparatorChar}");

            _commandService.Start("spa constants", $"{_srcDirectory}{Path.DirectorySeparatorChar}{_name}{Path.DirectorySeparatorChar}");

            _commandService.Start("spa app-module", $"{_srcDirectory}{Path.DirectorySeparatorChar}{_name}{Path.DirectorySeparatorChar}");

            _commandService.Start("spa swagger-gen", $"{_srcDirectory}{Path.DirectorySeparatorChar}{_name}{Path.DirectorySeparatorChar}");

            _commandService.Start("ng add @angular/material", angularJson.RootDirectory);

            _commandService.Start("spa .", angularJson.ApiDirectory);

            _commandService.Start($"ren {_name} {_solutionName}.{_name}", $"{_srcDirectory}{Path.DirectorySeparatorChar}");

            _commandService.Start("code .", $"{_srcDirectory}{Path.DirectorySeparatorChar}{_solutionName}.{_name}");
        }
    }
}
