using CommandLine;
using MediatR;
using Spa.Core.Services;
using Spa.Core.Strategies.SinglePageApplicationGenerator;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Commands
{
    internal class CreateSinglePageApplication
    {
        [Verb("create")]
        internal class Request : IRequest<Unit> {
            [Value(0)]
            public string Name { get; set; } = "App";

            [Option('p', "prefix", Required = false)]
            public string Prefix { get; set; } = "app";

            [Option('d', Required = false)]
            public string Directory { get; set; } = Environment.CurrentDirectory;
        }

        internal class Handler : IRequestHandler<Request, Unit>
        {
            private readonly ISpaGenerationStrategy _spaGenerationStrategy;
            public Handler(ICommandService commandService, IFileSystem fileSystem, ITemplateLocator templateLocator, ITemplateProcessor templateProcessor, IPackageJsonService packageJsonService)
            {
                _spaGenerationStrategy = new AngularSpaGenerationStrategy(
                    commandService,
                    fileSystem,
                    templateLocator,
                    templateProcessor,
                    packageJsonService);
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                _spaGenerationStrategy.Create(request.Name, request.Prefix, request.Directory);

                return new();
            }
        }
    }
}
