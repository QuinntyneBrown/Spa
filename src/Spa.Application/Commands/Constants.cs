using CommandLine;
using MediatR;
using Spa.Core.Services;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Core.Features
{
    internal class Constants
    {
        [Verb("constants")]
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
                var template = _templateLocator.Get("Constants");

                var angularJson = _angularJsonProvider.Get(request.Directory);

                _fileSystem.WriteAllLines($@"{angularJson.CoreDirectory}{Path.DirectorySeparatorChar}constants.ts", template);

                _commandService.Start("spa .", angularJson.CoreDirectory);

                return new();
            }
        }
    }
}
