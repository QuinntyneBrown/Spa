using CommandLine;
using MediatR;
using Spa.Core.Models;
using Spa.Core.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Core.Features
{
    internal class Store
    {
        [Verb("store")]
        internal class Request : IRequest<Unit>
        {
            [Value(0)]
            public string EntityName { get; set; }

            [Option('f', Required = false)]
            public bool Force { get; set; } = false;

            [Option('d', Required = false)]
            public string Directory { get; set; } = System.Environment.CurrentDirectory;
        }

        internal class Handler : IRequestHandler<Request, Unit>
        {
            private readonly IFileSystem _fileSystem;
            private readonly ITemplateLocator _templateLocator;
            private readonly ITemplateProcessor _templateProcessor;
            private readonly ICommandService _commandService;

            public Handler(
                IFileSystem fileSystem,
                ITemplateLocator templateLocator,
                ITemplateProcessor templateProcessor,
                ICommandService commandService
                )
            {
                _fileSystem = fileSystem;
                _templateProcessor = templateProcessor;
                _templateLocator = templateLocator;
                _commandService = commandService;
            }
            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var template = _templateLocator.Get("ComponentStoreBuilder");

                var tokens = new TokensBuilder()
                    .With(nameof(request.EntityName), (Token)request.EntityName)
                    .With(nameof(request.Directory), (Token)request.Directory)
                    .Build();

                var result = _templateProcessor.Process(template, tokens);

                var entityNameSnakeCase = _templateProcessor.Process("{{ entityNameSnakeCase }}", tokens);

                var filename = $"{entityNameSnakeCase}.store.ts";

                if (_fileSystem.Exists($@"{request.Directory}/{filename}"))
                {
                    if (request.Force)
                    {
                        System.IO.File.Delete($@"{request.Directory}/{filename}");
                    }
                    else
                    {
                        return new();
                    }
                }

                _fileSystem.WriteAllLines($@"{request.Directory}/{filename}", result);

                return new();
            }
        }
    }
}
