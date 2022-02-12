using Spa.Core.Models;
using Spa.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spa.Core.Strategies.Scss
{
    public interface IScssComponentGenerationStrategy
    {
        void Create(string name, string directory);
    }

    public class ScssComponentGenerationStrategy: IScssComponentGenerationStrategy
    {
        private readonly ITemplateLocator _templateLocator;
        private readonly IAngularJsonProvider _angularJsonProvider;
        private readonly IFileSystem _fileSystem;
        private readonly ICommandService _commandService;

        public ScssComponentGenerationStrategy(ITemplateLocator templateLocator, IAngularJsonProvider angularJsonProvider, IFileSystem fileSystem, ICommandService commandService)
        {
            _templateLocator = templateLocator;
            _angularJsonProvider = angularJsonProvider;
            _fileSystem = fileSystem;
            _commandService = commandService;
        }

        public void Create(string name, string directory)
        {
            var nameToken = (Token)name;

            var template = _templateLocator.Get(nameToken.PascalCase);

            var angularJson = _angularJsonProvider.Get(directory);

            _fileSystem.WriteAllLines($@"{angularJson.ScssDirectory}{Path.DirectorySeparatorChar}{nameToken.SnakeCase}.scss", template);

            _commandService.Start("spa . -s", angularJson.ScssDirectory);
        }
    }
}
