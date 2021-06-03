using CommandLine;
using MediatR;
using Spa.Core.Builders;
using Spa.Core.Services;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Core.Features
{
    internal class Default
    {
        [Verb("default")]
        internal class Request : IRequest<Unit> {
            [Option('n', Required = false)]
            public string Name { get; set; } = "Default";

            [Option('d', Required = false)]
            public string Directory { get; set; } = System.Environment.CurrentDirectory;
            [Option('p', Required = false)]
            public string PublicDirectory { get; set; }
            [Option('w', Required = false)]
            public string WorkspaceDirectory { get; set; }
        }

        internal class Handler : IRequestHandler<Request, Unit>
        {
            private readonly ICommandService _commandService;
            private readonly IFileSystem _fileSystem;

            public Handler(ICommandService commandService, IFileSystem fileSystem)
            {
                _commandService = commandService;
                _fileSystem = fileSystem;
            }
            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var solutionName = request.Name.Split('.').First();
                var appName = request.Name.Split('.').Length == 1 ? "App" : request.Name.Split('.').Last();

                var solutionDirectory = $"{request.Directory}{Path.DirectorySeparatorChar}{solutionName}";

                if (!Directory.Exists(solutionDirectory))
                {
                    _commandService.Start($"mkdir {solutionDirectory}");
                }

                var srcDirectory = $"{solutionDirectory}{Path.DirectorySeparatorChar}src";

                if (!Directory.Exists(srcDirectory))
                {
                    _commandService.Start($"mkdir {srcDirectory}");
                }

                new SpaBuilder(_commandService, _fileSystem, srcDirectory, solutionName, appName)
                    .Build();

                return new();
            }
        }
    }
}
