using Spa.Core.Services;
using System.IO;

namespace Spa.Core.Builders
{
    public class SpaBuilder
    {
        private readonly ICommandService _commandService;
        private readonly IFileSystem _fileSystem;

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

            new BarrelBuilder(_commandService, _fileSystem, $"{_srcDirectory}{Path.DirectorySeparatorChar}{_name}")
                .Add("core")
                .Add("shared")
                .Add("api")
                .Build();

            new FrameworkBuilder().Build();

            if (_hasLogin)
            {
                _commandService.Start($"ng g m login", $"{_srcDirectory}{Path.DirectorySeparatorChar}{_name}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}app");

                _commandService.Start($"ng g c login", $"{_srcDirectory}{Path.DirectorySeparatorChar}{_name}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}app{Path.DirectorySeparatorChar}login");

                _commandService.Start($"ng g c login-form", $"{_srcDirectory}{Path.DirectorySeparatorChar}{_name}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}app{Path.DirectorySeparatorChar}login-form");
            }

            if (_hasPublicApp)
            {
                string app = "public";

                _commandService.Start($"ng g m {app}", $"{_srcDirectory}{Path.DirectorySeparatorChar}{_name}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}app");

                _commandService.Start($"ng g c {app}", $"{_srcDirectory}{Path.DirectorySeparatorChar}{_name}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}app{Path.DirectorySeparatorChar}{app}");
            }

            if (_hasWorkspaceApp)
            {
                string app = "workspace";

                _commandService.Start($"ng g m {app}", $"{_srcDirectory}{Path.DirectorySeparatorChar}{_name}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}app");

                _commandService.Start($"ng g c {app}", $"{_srcDirectory}{Path.DirectorySeparatorChar}{_name}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}app{Path.DirectorySeparatorChar}{app}");
            }

            new AppModuleBuilder().Build();

            if (!string.IsNullOrEmpty(_resources))
            {
                foreach (var resource in _resources.Split(','))
                {
                    new EntityModuleBuilder(resource).Build();
                }
            }

            _commandService.Start("spa constants", $"{_srcDirectory}{Path.DirectorySeparatorChar}{_name}{Path.DirectorySeparatorChar}");

            _commandService.Start("spa app-module", $"{_srcDirectory}{Path.DirectorySeparatorChar}{_name}{Path.DirectorySeparatorChar}");

            _commandService.Start($"ren {_name} {_solutionName}.{_name}", $"{_srcDirectory}{Path.DirectorySeparatorChar}");

            _commandService.Start("code .", $"{_srcDirectory}{Path.DirectorySeparatorChar}{_solutionName}.{_name}");
        }
    }
}
