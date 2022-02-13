using CommandLine;
using MediatR;
using Spa.Core.Builders;
using Spa.Core.Models;
using Spa.Core.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Core.Features
{
    internal class Barrel
    {
        [Verb("barrel")]
        internal class Request : IRequest<Unit>
        {
            [Value(0)]
            public string Name { get; set; }

            [Option('d', Required = false)]
            public string Directory { get; set; } = System.Environment.CurrentDirectory;
        }

        internal class Handler : IRequestHandler<Request, Unit>
        {
            private readonly IFileSystem _fileSystem;
            private readonly ICommandService _commandService;
            private readonly ISettingsProvider _settingsProvider;

            public Handler(
                IFileSystem fileSystem,
                ICommandService commandService,
                ISettingsProvider settingsProvider)
            {
                _fileSystem = fileSystem;
                _commandService = commandService;
                _settingsProvider = settingsProvider;
            }
            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                Settings settings = _settingsProvider.Get(request.Directory);

                new BarrelGenerationStrategy(_fileSystem).Create(new SinglePageApplicationModel(settings));

                return new();
            }
        }
    }
}
