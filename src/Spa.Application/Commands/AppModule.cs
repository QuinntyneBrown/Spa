using CommandLine;
using MediatR;
using Spa.Core.Services;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Core.Features
{
    internal class AppModule
    {
        [Verb("app-module")]
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

            public Handler(
                IFileSystem fileSystem,
                ITemplateLocator templateLocator,
                IAngularJsonProvider angularJsonProvider
                )
            {
                _fileSystem = fileSystem;
                _templateLocator = templateLocator;
                _angularJsonProvider = angularJsonProvider;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var template = _templateLocator.Get("AppModule");

                var angularJson = _angularJsonProvider.Get(request.Directory);

                _fileSystem.WriteAllLines($@"{angularJson.AppDirectory}{Path.DirectorySeparatorChar}app.module.ts", template);

                return new();
            }
        }
    }
}
