using CommandLine;
using MediatR;
using Spa.Core.Models;
using Spa.Core.Services;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Core.Features
{
    internal class Api
    {
        [Verb("api")]
        internal class Request : IRequest<Unit>
        {
            [Option('d')]
            public string Directory { get; set; } = System.Environment.CurrentDirectory;
        }

        internal class Handler : IRequestHandler<Request, Unit>
        {
            private readonly ISettingsProvider _settingsProvder;
            private readonly ICommandService _commandService;

            public Handler(ICommandService commandService, ISettingsProvider settingsProvider)
            {
                _settingsProvder = settingsProvider;
                _commandService = commandService;
            }
            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                Settings settings = _settingsProvder.Get(request.Directory);

                foreach (var appDirectory in settings.AppDirectories)
                {
                    string apiDirectory = $"{appDirectory}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}app{Path.DirectorySeparatorChar}@api";
                    string coreDirectory = $"{appDirectory}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}app{Path.DirectorySeparatorChar}@core";
                    string modelsDirectory = $"{apiDirectory}{Path.DirectorySeparatorChar}models";
                    string servicesDirectory = $"{apiDirectory}{Path.DirectorySeparatorChar}services";

                    if (!Directory.Exists(modelsDirectory))
                    {
                        _commandService.Start($"mkdir {modelsDirectory}");
                    }

                    if (!Directory.Exists(servicesDirectory))
                    {
                        _commandService.Start($"mkdir {servicesDirectory}");
                    }

                    foreach (var resource in settings.Resources)
                    {
                        _commandService.Start($"spa type {resource}", modelsDirectory);
                        _commandService.Start($"spa service {resource}", servicesDirectory);
                    }

                    _commandService.Start($"spa .", modelsDirectory);
                    _commandService.Start($"spa .", servicesDirectory);
                }


                return new();
            }
        }
    }
}
