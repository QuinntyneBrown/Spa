using CommandLine;
using MediatR;
using Spa.Core.Models;
using Spa.Core.Services;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Core.Features
{
    internal class Table
    {
        [Verb("table")]
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
            private readonly IAngularJsonProvider _angularJsonProvider;
            private readonly ICommandService _commandService;

            public Handler(
                IFileSystem fileSystem,
                ITemplateLocator templateLocator,
                IAngularJsonProvider angularJsonProvider,
                ICommandService commandService,
                ITemplateProcessor templateProcessor
                )
            {
                _fileSystem = fileSystem;
                _templateLocator = templateLocator;
                _angularJsonProvider = angularJsonProvider;
                _commandService = commandService;
                _templateProcessor = templateProcessor;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {

                var tokens = new TokensBuilder()
                    .With("Prefix", (Token)_angularJsonProvider.Get(request.Directory).Prefix)
                    .Build();

                var template = _templateLocator.Get("Table");

                var result = _templateProcessor.Process(template, tokens);

                var angularJson = _angularJsonProvider.Get(request.Directory);

                _fileSystem.WriteAllLines($"{angularJson.ScssDirectory}{Path.DirectorySeparatorChar}table.scss", result);

                _commandService.Start("spa . -s", angularJson.ScssDirectory);

                return new();

            }
        }
    }
}
