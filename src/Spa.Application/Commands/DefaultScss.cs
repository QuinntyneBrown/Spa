using CommandLine;
using MediatR;
using Spa.Core.Services;
using Spa.Core.Strategies.Scss;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Commands
{
    internal class DefaultScss
    {
        [Verb("default-scss")]
        internal class Request : IRequest<Unit> {

            [Option('d', Required = false)]
            public string Directory { get; set; } = System.Environment.CurrentDirectory;
        }

        internal class Handler : IRequestHandler<Request, Unit>
        {
            private readonly IDefaultScssGenerationStrategy _defaultScssGenerationStrategy;
            public Handler(
                IAngularJsonProvider angularJsonProvider,
                INamingConventionConverter namingConventionConverter,
                IFileSystem fileSystem,
                ICommandService commandService,
                ITemplateLocator templateLocator,
                ITemplateProcessor templateProcessor)
            {
                _defaultScssGenerationStrategy = new DefaultScssGenerationStrategy(
                    angularJsonProvider,
                    namingConventionConverter,
                    fileSystem,
                    commandService,
                    templateLocator,
                    templateProcessor);
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                _defaultScssGenerationStrategy.Create(new (request.Directory));

                return new();
            }
        }
    }
}
