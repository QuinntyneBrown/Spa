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
                try
                {
                    var angularJson = _angularJsonProvider.Get(request.Directory);

                    var moduleName = _nearestModuleNameProvider.Get(request.Directory);

                    var entityToken = (Token)request.Entity;

                    var tokens = new TokensBuilder()
                        .With("prefix", (Token)angularJson.Prefix)
                        .With("module", (Token)moduleName)
                        .With("entityName", entityToken)
                        .With("directory", (Token)request.Directory)
                        .Build();

                    _commandService.Start($"spa page {((Token)request.Entity).SnakeCasePlural} {moduleName}", request.Directory);

                    var listDetalHtml = _templateProcessor.Process(_templateLocator.Get("ListDetailHtml"), tokens);

                    var listDetailComponent = _templateProcessor.Process(_templateLocator.Get("ListDetail"), tokens);

                    _fileSystem.WriteAllLines($@"{request.Directory}{Path.DirectorySeparatorChar}{entityToken.SnakeCasePlural}{Path.DirectorySeparatorChar}{entityToken.SnakeCasePlural}.component.html", listDetalHtml);

                    _fileSystem.WriteAllLines($@"{request.Directory}{Path.DirectorySeparatorChar}{entityToken.SnakeCasePlural}{Path.DirectorySeparatorChar}{entityToken.SnakeCasePlural}.component.ts", listDetailComponent);

                    _commandService.Start($"spa c {((Token)request.Entity).SnakeCase}-list", angularJson.SharedComponentsDirectory);

                    _commandService.Start($"spa c {((Token)request.Entity).SnakeCase}-detail", angularJson.SharedComponentsDirectory);

                    return new();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
    }
}
