using CommandLine;
using MediatR;
using Spa.Core.Services;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Core.Features
{
    internal class Breakpoints
    {
        [Verb("breakpoints")]
        internal class Request : IRequest<Unit>
        {
            [Option('d', Required = false)]
            public string Directory { get; set; } = System.Environment.CurrentDirectory;
        }

        internal class Handler : IRequestHandler<Request, Unit>
        {
            private readonly IFileSystem _fileSystem;
            private readonly ITemplateLocator _templateLocator;
            private readonly IAngularJsonProvider _angularJsonProvider;
            private readonly ICommandService _commandService;

            public Handler(
                IFileSystem fileSystem,
                ITemplateLocator templateLocator,
                IAngularJsonProvider angularJsonProvider,
                ICommandService commandService
                )
            {
                _fileSystem = fileSystem;
                _templateLocator = templateLocator;
                _angularJsonProvider = angularJsonProvider;
                _commandService = commandService;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var template = _templateLocator.Get("Breakpoints");

                var angularJson = _angularJsonProvider.Get(request.Directory);

                _fileSystem.WriteAllLines($@"{angularJson.ScssDirectory}{Path.DirectorySeparatorChar}breakpoints.scss", template);

                _commandService.Start("spa . -s", angularJson.ScssDirectory);

                return new();
            }
        }
    }
}
