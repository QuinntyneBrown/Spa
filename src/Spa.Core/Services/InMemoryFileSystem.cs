using System.Collections.Generic;
using System.IO;

namespace Spa.Core.Services
{
    public class InMemoryFileSystem : IFileSystem
    {
        private Dictionary<string, string> _fileSystem = new Dictionary<string, string>();

        public InMemoryFileSystem()
        {

        }
        public void CreateDirectory(string path)
        {
            
        }

        public void Delete(string path)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteDirectory(string directory)
        {
            throw new System.NotImplementedException();
        }

        public bool Exists(string path)
        {
            return false;
        }

        public bool Exists(string[] paths)
        {
            return false;
        }

        public Stream OpenRead(string path)
        {
            return null;
        }

        public string ParentFolder(string path)
        {
            return "";
        }

        public string[] ReadAllLines(string path)
        {
            return null;
        }

        public string ReadAllText(string path)
        {
            _fileSystem.TryGetValue(path,out var text);

            return text;
        }

        public void WriteAllLines(string path, string[] contents)
        {

        }

        public void WriteAllText(string path, string contents)
        {
            _fileSystem.Remove(path);

            _fileSystem.Add(path, contents);
        }
    }
}
