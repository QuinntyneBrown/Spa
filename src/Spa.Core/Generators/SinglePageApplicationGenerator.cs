using Spa.Core.Models;
using Spa.Core.Strategies;

namespace Spa.Core
{
    public static class SinglePageApplicationGenerator
    {
        public static void Generate(Settings settings, ISinglePageApplicationGenerationStrategyFactory factory, string name, string prefix, string directory, bool minimal)
        {
            factory.CreateFor(settings, name, prefix, directory);
        }
    }
}
