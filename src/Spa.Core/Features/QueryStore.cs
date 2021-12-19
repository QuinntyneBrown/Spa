using CommandLine;
using MediatR;
using Spa.Core.Models;
using Spa.Core.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Core.Features
{
    internal class QueryStore
    {
        [Verb("query-store")]
        internal class Request : IRequest<Unit>
        {
            [Value(0)]
            public string EntityName { get; set; }

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
                var template = _templateLocator.Get("QueryStore");

                var tokens = new TokensBuilder()
                    .With(nameof(request.EntityName), (Token)request.EntityName)
                    .With(nameof(request.Directory), (Token)request.Directory)
                    .Build();

                var result = _templateProcessor.Process(template, tokens);

                var entityNameSnakeCase = _templateProcessor.Process("{{ entityNameSnakeCase }}", tokens);

                var filename = $"{entityNameSnakeCase}.store.ts";

                _fileSystem.WriteAllLines($@"{request.Directory}/{filename}", result);

                return new();
            }
        }
    }
}
