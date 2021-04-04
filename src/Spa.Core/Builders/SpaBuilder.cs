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
        private string _publicDirectory;
        private string _workspaceDirectory; 

        public SpaBuilder(ICommandService commandService, IFileSystem fileSystem, string resources = null, string name = null, string directory = null)
        {
            _commandService = commandService;
            _fileSystem = fileSystem;

            _name = string.IsNullOrEmpty(name) ? _name : name;
            _directory = string.IsNullOrEmpty(directory) ? _directory : directory;
            _resources ??= resources;

        }

        public SpaBuilder WithWorkspaceDirectory(string directory)
        {
            _workspaceDirectory = directory;
            return this;
        }

        public SpaBuilder WithPublicDirectory(string directory)
        {
            _publicDirectory = directory;   
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

            if (string.IsNullOrEmpty(_publicDirectory))
            {

            }

            if (string.IsNullOrEmpty(_workspaceDirectory))
            {

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
