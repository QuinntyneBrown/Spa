using CommandLine;
using MediatR;
using Spa.Core.Models;
using Spa.Core.Services;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Core.Features
{
    internal class ListDetailDirective
    {
        [Verb("list-detail-directive")]
        internal class Request : IRequest<Unit>
        {
            [Option('d', Required = false)]
            public string Directory { get; set; } = System.Environment.CurrentDirectory;
        }

        internal class Handler : IRequestHandler<Request, Unit>
        {
            private readonly IFileSystem _fileSystem;
            private readonly ITemplateLocator _templateLocator;
            private readonly ITemplateProcessor _templateProcessor;
            private readonly ICommandService _commandService;
            private readonly IAngularJsonProvider _angularJsonProvider;

            public Handler(
                IFileSystem fileSystem,
                ITemplateLocator templateLocator,
                ITemplateProcessor templateProcessor,
                ICommandService commandService,
                IAngularJsonProvider angularJsonProvider
                )
            {
                _fileSystem = fileSystem;
                _templateProcessor = templateProcessor;
                _templateLocator = templateLocator;
                _commandService = commandService;
                _angularJsonProvider = angularJsonProvider;
            }
            public Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {

                var angularJson = _angularJsonProvider.Get(request.Directory);

                var tokens = new TokensBuilder()
                    .With("Prefix", (Token)angularJson.Prefix)
                    .Build();

                var listDetailDirective = _templateProcessor.Process(_templateLocator.Get("ListDetailDirective"), tokens);

                var listDetailDirectiveScss = _templateProcessor.Process(_templateLocator.Get("ListDetailDirectiveScss"), tokens);

                _commandService.Start("spa css-component list-detail-directive");

                _fileSystem.WriteAllLines($"{angularJson.ScssDirectory}{Path.DirectorySeparatorChar}list-detail-directive.scss", listDetailDirectiveScss);

                _commandService.Start("mkdir components", angularJson.SharedDirectory);

                _commandService.Start("mkdir directives", angularJson.SharedDirectory);

                _fileSystem.WriteAllLines($"{angularJson.SharedDirectory}{Path.DirectorySeparatorChar}directives{Path.DirectorySeparatorChar}list-detail.directive.ts", listDetailDirective);

                _commandService.Start("spa .", $"{angularJson.SharedDirectory}{Path.DirectorySeparatorChar}directives");

                _commandService.Start("spa .", angularJson.SharedDirectory);

                return Task.FromResult(new Unit());

            }

            public void WriteFile(string templateName, string filename, Dictionary<string, object> tokens)
            {
                var template = _templateLocator.Get(templateName);

                var result = _templateProcessor.Process(template, tokens);

                var path = _templateProcessor.Process("{{ directory }}//" + filename, tokens);

                _fileSystem.WriteAllLines(path, result);
            }
        }
    }
}
