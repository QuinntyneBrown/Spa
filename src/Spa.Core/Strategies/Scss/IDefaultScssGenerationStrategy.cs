using Spa.Core.Models;

namespace Spa.Core.Strategies.Scss
{
    public interface IDefaultScssGenerationStrategy
    {
        void Create(SinglePageApplicationModel model);
    }
}
