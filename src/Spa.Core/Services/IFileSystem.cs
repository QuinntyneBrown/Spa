using System.IO;

namespace Spa.Core.Services
{
    public interface IFileSystem
    {
        string ReadAllText(string path);
        string[] ReadAllLines(string path);
        Stream OpenRead(string path);
        bool Exists(string path);
        bool Exists(string[] paths);
        void WriteAllLines(string path, string[] contents);
        void WriteAllText(string path, string contents);
        string ParentFolder(string path);
    }
}