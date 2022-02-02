using CommandLine;
using MediatR;
using Spa.Core.Services;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Core.Features
{
    internal class SwaggerGen
    {
        [Verb("swagger-gen")]
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
                var template = _templateLocator.Get("SwaggerGenJsonBuilder");

                var angularJson = _angularJsonProvider.Get(request.Directory);

                var path = $@"{angularJson.RootDirectory}{Path.DirectorySeparatorChar}ng-swagger-gen.json";

                Console.WriteLine(path);

                _fileSystem.WriteAllLines(path, template);

                return new();
            }
        }
    }
}
