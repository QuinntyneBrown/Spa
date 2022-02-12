/*using Spa.Core.Models;
using Spa.Core.Services;
using System.IO;
using Xunit;
using static System.Text.Json.JsonSerializer;

namespace Spa.UnitTests
{
    public class AngularJsonTests
    {
        [Fact]
        public void Deserialize()
        {
            string path = null;

            var angularJson = Deserialize<AngularJson>(File.ReadAllText(path), new()
            {
                PropertyNameCaseInsensitive = true,
            });

            Assert.NotNull(angularJson);
        }

        [Fact]
        public void DeserializeUsingProvider()
        {
            string path = null;

            var angularJson = new AngularJsonProvider().Get(path);

            Assert.NotNull(angularJson);
        }
    }
}
*/