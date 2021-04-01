using Spa.Core.Services;
using System.IO;

namespace Spa.Core.Builders
{
    public class SpaBuilder
    {
        private readonly ICommandService _commandService;

        private string _directory = System.Environment.CurrentDirectory;
        private string _name = $"Default";

        public SpaBuilder(ICommandService commandService, string name = null, string directory = null)
        {
            _name = string.IsNullOrEmpty(name) ? _name : name;
            _directory = string.IsNullOrEmpty(directory) ? _directory : directory;
            _commandService = commandService;
        }

        public void Build()
        {
            _commandService.Start($"ng new {_name} --style=scss --strict=false --routing", _directory);

            _commandService.Start("code .", $"{_directory}{Path.DirectorySeparatorChar}{_name}");

            _commandService.Start("ng serve", $"{_directory}{Path.DirectorySeparatorChar}{_name}");
        }
    }
}
