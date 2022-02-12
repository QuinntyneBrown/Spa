using Spa.Core.Models;
using Spa.Core.Services;
using System.Collections.Generic;
using System.Linq;

namespace Spa.Core.Strategies
{
    public class SinglePageApplicationGenerationStrategyFactory : ISinglePageApplicationGenerationStrategyFactory
    {
        private readonly IList<ISinglePageApplicationGenerationStrategy> _strategies;

        public SinglePageApplicationGenerationStrategyFactory(ICommandService commandService, IPackageJsonService packageJsonService, IFileSystem fileSystem)
        {
            _strategies = new List<ISinglePageApplicationGenerationStrategy>()
            {
                new SinglePageApplicationGenerationStrategy(commandService,fileSystem,packageJsonService)
            };
        }

        public void CreateFor(Settings settings, string name, string prefix, string directory)
        {
            var strategy = _strategies.Where(x => x.CanHandle(settings)).First();

            strategy.Create(settings, name, prefix, directory);
        }
    }
}
