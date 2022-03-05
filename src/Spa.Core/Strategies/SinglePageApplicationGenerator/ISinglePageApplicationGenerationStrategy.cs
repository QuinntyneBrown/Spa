using Spa.Core.Models;

namespace Spa.Core.Strategies
{
    public interface ISinglePageApplicationGenerationStrategy
    {
        void Create(Settings settings, string name, string prefix, string directory, bool minimal);
        bool CanHandle(Settings settings);
    }
}
