using Spa.Core.Models;
using Spa.Core.Services;
using System.IO;
using System.Linq;

namespace Spa.Core.Strategies.GlobalGeneration
{
    public interface IRootAppComponentGenerationStrategy
    {
        void Generate(string appDirectory, string prefix);
    }

    public class RootAppComponentGenerationStrategy: IRootAppComponentGenerationStrategy
    {
        private readonly IFileSystem _fileSystem;
        private readonly ITemplateLocator _templateLocator;
        private readonly ITemplateProcessor _templateProcessor;

        public RootAppComponentGenerationStrategy(IFileSystem fileSystem, ITemplateLocator templateLocator, ITemplateProcessor templateProcessor)
        {
            _fileSystem = fileSystem;
            _templateLocator = templateLocator;
            _templateProcessor = templateProcessor;
        }
        public void Generate(string appDirectory, string prefix)
        {
            var tokens = new TokensBuilder()
                .With("Prefix", (Token)prefix)
                .Build();

            //var appDirectory = settings.AppDirectories.First();
            _fileSystem.WriteAllLines($"{appDirectory}{Path.DirectorySeparatorChar}app.component.html", _render("RootAppComponentHtml", tokens));
            _fileSystem.WriteAllLines($"{appDirectory}{Path.DirectorySeparatorChar}app.component.ts", _render("RootAppComponentTs", tokens));
        }

        private string[] _render(string templateName, dynamic tokens)
        {
            var template = _templateLocator.Get(templateName);
            return _templateProcessor.Process(template, tokens);
        }
    }
}
