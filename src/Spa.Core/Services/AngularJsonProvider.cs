using Spa.Core.Models;
using System.IO;
using System.Linq;
using static System.Environment;
using static System.IO.Path;
using static System.Text.Json.JsonSerializer;

namespace Spa.Core.Services
{
    public interface IAngularJsonProvider
    {
        AngularJsonFileModel Get(string directory = null);
    }
    public class AngularJsonProvider : IAngularJsonProvider
    {
        public AngularJsonFileModel Get(string directory = null)
        {
            directory ??= CurrentDirectory;

            var parts = directory.Split(DirectorySeparatorChar);

            for (var i = 1; i <= parts.Length; i++)
            {
                var path = $"{string.Join(DirectorySeparatorChar, parts.Take(i))}{DirectorySeparatorChar}angular.json";

                if (File.Exists(path))
                {
                    var angularJson = Deserialize<AngularJsonFileModel>(File.ReadAllText(path), new()
                    {
                        PropertyNameCaseInsensitive = true,
                    });

                    angularJson.RootDirectory = new FileInfo(path).Directory.FullName;

                    return angularJson;
                }

                i++;
            }

            return AngularJsonFileModel.Empty;

        }
    }
}
