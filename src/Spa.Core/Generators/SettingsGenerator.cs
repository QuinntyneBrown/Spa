using Spa.Core.Models;
using Spa.Core.Strategies;

namespace Spa.Core
{
    public static class SettingsGenerator
    {
        public static Settings Create(Settings model, ISettingsGenerationStrategyFactory factory, string rootName, string prefix, string directory, bool minimal) {
            
            return factory.CreateFor(model, rootName, prefix, directory, minimal);
        }
    }
}
