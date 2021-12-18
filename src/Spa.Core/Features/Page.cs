using CommandLine;
using MediatR;
using Spa.Core.Models;
using Spa.Core.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Core.Features
{
    internal class Page
    {
        [Verb("page")]
        internal class Request : IRequest<Unit>
        {
            [Value(0)]
            public string Name { get; set; }

            [Value(1)]
            public string Module { get; set; }

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
                var tokens = new TokensBuilder()
                    .With("Name", (Token)request.Name)
                    .With("Directory", (Token)request.Directory)
                    .With("Prefix", (Token)_angularJsonProvider.Get(request.Directory).Prefix)
                    .With("Module", (Token)request.Module)
                    .Build();

                _commandService.Start(_templateProcessor.Process("ng g m {{ nameSnakeCase }} --module={{ moduleSnakeCase }}.module --route={{ nameSnakeCase }}", tokens), request.Directory);

                return Task.FromResult(new Unit());
            }
        }
    }
}
