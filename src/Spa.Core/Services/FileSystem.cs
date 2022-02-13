using System.IO;
using System.Linq;

namespace Spa.Core.Services
{
    public class FileSystem : IFileSystem
    {
        public bool Exists(string path)
            => File.Exists(path);

        public bool Exists(string[] paths)
            => paths.Any(x => Exists(x));

        public Stream OpenRead(string path)
            => File.OpenRead(path);

        public string ReadAllText(string path)
            => File.ReadAllText(path);

        public string[] ReadAllLines(string path)
            => File.ReadAllLines(path);
        public void WriteAllLines(string path, string[] contents)
        {            
            File.WriteAllLines(path, contents);
        }

        public void WriteAllText(string path, string contents)
        {
            File.WriteAllText(path, contents);
        }

        public string ParentFolder(string path)
        {
            var directories = path.Split(Path.DirectorySeparatorChar);

            string parentFolderPath = string.Join($"{Path.DirectorySeparatorChar}", directories.ToList()
                .Take(directories.Length - 1));

            return parentFolderPath;
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }
    }
}
