using CommandLine;
using MediatR;
using Spa.Core.Models;
using Spa.Core.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Core.Features
{
    internal class ListDetail
    {
        [Verb("list-detail")]
        internal class Request : IRequest<Unit>
        {
            [Value(0)]
            public string Entity { get; set; }

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
            private readonly INearestModuleNameProvider _nearestModuleNameProvider;

            public Handler(
                IFileSystem fileSystem,
                ITemplateLocator templateLocator,
                ITemplateProcessor templateProcessor,
                ICommandService commandService,
                IAngularJsonProvider angularJsonProvider,
                INearestModuleNameProvider nearestModuleNameProvider
                )
            {
                _fileSystem = fileSystem;
                _templateProcessor = templateProcessor;
                _templateLocator = templateLocator;
                _commandService = commandService;
                _angularJsonProvider = angularJsonProvider;
                _nearestModuleNameProvider = nearestModuleNameProvider;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var angularJson = _angularJsonProvider.Get(request.Directory);

                var moduleName = _nearestModuleNameProvider.Get(request.Directory);

                _commandService.Start($"spa page {((Token)request.Entity).SnakeCasePlural} {moduleName}");

                _commandService.Start($"spa c {((Token)request.Entity).SnakeCase}-list", angularJson.SharedComponentsDirectory);

                _commandService.Start($"spa c {((Token)request.Entity).SnakeCase}-detail", angularJson.SharedComponentsDirectory);

                return new();
            }
        }
    }
}
