namespace Spa.Core.Strategies.SinglePageApplicationGenerator
{
    public interface ISpaGenerationStrategy
    {
        void Create(string name, string prefix, string directory);
    }
}
