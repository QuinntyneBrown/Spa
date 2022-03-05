using Spa.Core.Models;
using Spa.Core.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Spa.Core.Strategies.GlobalGeneration
{

    public interface IIndexHtmlFileGenerationStrategy
    {
        public void Generate(string indexFileFullPath);

    }
    public class IndexHtmlFileGenerationStrategy : IIndexHtmlFileGenerationStrategy
    {
        private readonly IFileSystem _fileSystem;

        public IndexHtmlFileGenerationStrategy(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void Generate(string indexFileFullPath)
        {
            var lines = new List<string>();

            //var indexFileFullPath = $"{settings.AppDirectories.First()}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}index.html";

            foreach(var line in _fileSystem.ReadAllLines(indexFileFullPath))
            {
                if(line.StartsWith("<body"))
                {
                    if(line.Contains("class"))
                    {
                        foreach (var part in line.Split(" "))
                        {
                            if (part.StartsWith("class"))
                            {

                            }
                        }
                    }
                    else
                    {
                        line.Replace("<body", "<body class=\"root\"");
                    }

                    lines.Add(line);
                }

                else
                {
                    lines.Add(line);
                }
            }

            _fileSystem.WriteAllLines(indexFileFullPath, lines.ToArray());
            
        }
    }
}
