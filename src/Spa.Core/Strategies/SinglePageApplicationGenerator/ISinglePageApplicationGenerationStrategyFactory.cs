using Spa.Core.Models;

namespace Spa.Core.Strategies
{
    public interface ISinglePageApplicationGenerationStrategyFactory
    {
        void CreateFor(Settings settings, string name, string prefix, string directory);
    }
}
