using Spa.Core.Services;
using Xunit;

namespace Spa.UnitTests
{
    public class NearestModuleNameProviderTests
    {
        [Fact]
        public void Get()
        {
            string path = null;

            var provider = new NearestModuleNameProvider();

            var module = provider.Get(path);

            Assert.NotNull(module);
        }
    }
}
