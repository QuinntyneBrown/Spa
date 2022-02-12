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
            private readonly IAngularJsonProvider _angularJsonProvider;

            public Handler(ICommandService commandService, ISettingsProvider settingsProvider, IAngularJsonProvider angularJsonProvider)
            {
                _settingsProvder = settingsProvider;
                _commandService = commandService;
                _angularJsonProvider = angularJsonProvider;
            }
            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                Settings settings = _settingsProvder.Get(request.Directory);

                AngularJson angularJson = _angularJsonProvider.Get(request.Directory);

                if (!Directory.Exists(angularJson.ModelsDirectory))
                {
                    _commandService.Start($"mkdir {angularJson.ModelsDirectory}");
                }

                if (!Directory.Exists(angularJson.ServicesDirectory))
                {
                    _commandService.Start($"mkdir {angularJson.ServicesDirectory}");
                }

                foreach (var resource in settings.Resources)
                {
                    _commandService.Start($"spa type {resource}", angularJson.ModelsDirectory);
                    _commandService.Start($"spa service {resource}", angularJson.ServicesDirectory);
                }

                _commandService.Start($"spa .", angularJson.ModelsDirectory);
                _commandService.Start($"spa .", angularJson.ServicesDirectory);

                _commandService.Start("spa .", angularJson.ApiDirectory);

                return new();
            }
        }
    }
}
