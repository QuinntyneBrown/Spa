using Spa.Core.Builders;
using Spa.Core.Models;
using Spa.Core.Services;
using System.IO;
using Xunit;

namespace Spa.UnitTests
{
    public class BarrelGenerationStrategyTests
    {
        [Fact]
        public void CreateBarreks()
        {
            var fileSystem = new InMemoryFileSystem();

            fileSystem.WriteAllText($"{Path.DirectorySeparatorChar}tsconfig.json", "{ 'compilerOptions': {  }");

            var sut = new BarrelGenerationStrategy(fileSystem);

            var settings = new Settings();

            settings.AppDirectories.Add("");

            var model = new SinglePageApplicationModel(settings);

            sut.Create(model);

        }
    }
}
