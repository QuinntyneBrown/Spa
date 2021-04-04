using Spa.Core.Services;
using System.IO;

namespace Spa.Core.Builders
{
    public class SpaBuilder
    {
        private readonly ICommandService _commandService;
        private readonly IFileSystem _fileSystem;

        private string _directory = System.Environment.CurrentDirectory;
        private string _name = $"Default";
        private string _resources;
        private bool _hasPublicApp;
        private bool _hasWorkspaceApp;
        private bool _hasLogin;

        public SpaBuilder(ICommandService commandService, IFileSystem fileSystem, string resources = null, string name = null, string directory = null)
        {
            _commandService = commandService;
            _fileSystem = fileSystem;

            _name = string.IsNullOrEmpty(name) ? _name : name;
            _directory = string.IsNullOrEmpty(directory) ? _directory : directory;
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

        public void Build()
        {            
            _commandService.Start($"ng new {_name} --style=scss --strict=false --routing", _directory);

            _commandService.Start("code .", $"{_directory}{Path.DirectorySeparatorChar}{_name}");

            new BarrelBuilder(_commandService, _fileSystem, $"{_directory}{Path.DirectorySeparatorChar}{_name}")
                .Add("core")
                .Add("shared")
                .Add("shell")
                .Build();

            new FrameworkBuilder().Build();

            _commandService.Start($"ng g m not-found", $"{_directory}{Path.DirectorySeparatorChar}{_name}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}app");

            _commandService.Start($"ng g c not-found", $"{_directory}{Path.DirectorySeparatorChar}{_name}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}app{Path.DirectorySeparatorChar}not-found");

            if(_hasLogin)
            {
                _commandService.Start($"ng g m login", $"{_directory}{Path.DirectorySeparatorChar}{_name}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}app");

                _commandService.Start($"ng g c login", $"{_directory}{Path.DirectorySeparatorChar}{_name}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}app{Path.DirectorySeparatorChar}login");

                _commandService.Start($"ng g c login-form", $"{_directory}{Path.DirectorySeparatorChar}{_name}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}app{Path.DirectorySeparatorChar}login-form");
            }

            if (_hasPublicApp)
            {
                string app = "public";

                _commandService.Start($"ng g m {app}", $"{_directory}{Path.DirectorySeparatorChar}{_name}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}app");

                _commandService.Start($"ng g c {app}", $"{_directory}{Path.DirectorySeparatorChar}{_name}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}app{Path.DirectorySeparatorChar}{app}");
            }

            if (_hasWorkspaceApp)
            {
                string app = "workspace";

                _commandService.Start($"ng g m {app}", $"{_directory}{Path.DirectorySeparatorChar}{_name}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}app");

                _commandService.Start($"ng g c {app}", $"{_directory}{Path.DirectorySeparatorChar}{_name}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}app{Path.DirectorySeparatorChar}{app}");
            }

            new AppModuleBuilder().Build();

            if(!string.IsNullOrEmpty(_resources))
            {
                foreach(var resource in _resources.Split(','))
                {
                    new EntityModuleBuilder(resource).Build();
                }
            }

            _commandService.Start("ng serve", $"{_directory}{Path.DirectorySeparatorChar}{_name}");
        }
    }
}
