using System.IO;
using System.Linq;
using static System.Environment;

namespace Spa.Core.Services
{
    public interface INearestModuleNameProvider
    {
        string Get(string directory = null);
    }
    public class NearestModuleNameProvider : INearestModuleNameProvider
    {
        public string Get(string directory = null)
        {
            directory ??= CurrentDirectory;

            var module = Directory.GetFiles(directory, "*.module.ts")
                .Where(x => !x.Contains("routing"))
                .FirstOrDefault();

            if (module != null)
            {
                var name = directory.Split(Path.DirectorySeparatorChar).Last().Split('.').First();

                return name;
            }

            var parts = directory.Split(Path.DirectorySeparatorChar);

            return Get(string.Join(Path.DirectorySeparatorChar, parts.Take(parts.Length - 1)));
        }
    }
}
