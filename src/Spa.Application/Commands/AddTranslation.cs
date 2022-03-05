using CommandLine;
using MediatR;
using Spa.Core.Models;
using Spa.Core.Services;
using Spa.Core.Strategies.Additions;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Commands
{
    internal class AddTranslation
    {
        [Verb("add-translation")]
        internal class Request : IRequest<Unit> {
            [Option('d', Required = false)]
            public string Directory { get; set; } = System.Environment.CurrentDirectory;
        }

        internal class Handler : IRequestHandler<Request, Unit>
        {

            private readonly AddTranslationsStrategy _addTranslationsStrategy;
            public Handler(ICommandService commandService, IFileSystem fileSystem)
            {
                _addTranslationsStrategy = new AddTranslationsStrategy(commandService, fileSystem);
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                _addTranslationsStrategy.Add(new SinglePageApplicationModel(request.Directory));

                return new();
            }
        }
    }
}
