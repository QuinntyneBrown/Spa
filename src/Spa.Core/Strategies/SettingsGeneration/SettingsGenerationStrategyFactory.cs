using Spa.Core.Models;
using Spa.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Spa.Core.Strategies
{
    public interface ISettingsGenerationStrategyFactory
    {
        Settings CreateFor(Settings model, string rootName, string prefix, string directory);
    }

    public class SettingsGenerationStrategyFactory: ISettingsGenerationStrategyFactory
    {
        private readonly IList<ISettingsGenerationStrategy> _strategies;


        public SettingsGenerationStrategyFactory(ICommandService commandService, IFileSystem fileSystem)
        {
            _strategies = new List<ISettingsGenerationStrategy>()
            {
                new NewSettingsGenerationStrategy(commandService,fileSystem),
                new ExistingSettingsGenerationStrategy(commandService,fileSystem)
            };
        }

        public Settings CreateFor(Settings model, string rootName, string prefix, string directory)
        {
            var strategy = _strategies.Where(x => x.CanHandle(model)).FirstOrDefault();

            if (strategy == null)
            {
                throw new InvalidOperationException("Cannot find a strategy for generation for the type ");
            }

            return strategy.Create(model, rootName, prefix, directory);
        }
    }
}
