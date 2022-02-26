using System.IO;

namespace Spa.Core.Services
{
    public interface IFileSystem
    {
        string ReadAllText(string path);
        Stream OpenRead(string path);
        bool Exists(string path);
        bool Exists(string[] paths);
        void WriteAllLines(string path, string[] contents);
        string ParentFolder(string path);
        void CreateDirectory(string directory);
        void Delete(string path);
        void DeleteDirectory(string directory);
        void WriteAllText(string path, string contents);
        string[] ReadAllLines(string path);
    }
}