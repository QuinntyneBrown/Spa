using Spa.Core.Models;

namespace Spa.Core.Strategies
{
    public interface ISettingsGenerationStrategy
    {
        bool CanHandle(Settings model);

        Settings Create(Settings model, string rootName, string prefix, string directory, bool minimal);
    }
}
