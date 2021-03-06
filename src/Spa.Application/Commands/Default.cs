using CommandLine;
using MediatR;
using Spa.Core.Events;
using Spa.Core.Services;
using Spa.Core.Strategies;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Core.Features
{
    internal class Default
    {
        [Verb("default")]
        internal class Request : IRequest<Unit>
        {
            [Option('n', Required = false)]
            public string Name { get; set; } = "Default";

            [Option('m', Required = false)]
            public bool Minimal { get; set; } = true;

            [Option('p', Required = false)]
            public string Prefix { get; set; } = "app";

            [Option('d', Required = false)]
            public string Directory { get; set; } = System.Environment.CurrentDirectory;
        }

        internal class Handler : IRequestHandler<Request, Unit>
        {
            private readonly ISettingsProvider _settingsProvider;
            private readonly IMediator _mediator;
            private readonly ISinglePageApplicationGenerationStrategyFactory _factory;
            
            public Handler(
                IMediator mediator,
                ISinglePageApplicationGenerationStrategyFactory singlePageApplicationGenerationStrategyFactory,
                ISettingsProvider settingsProvider)
            {
                _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
                _settingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
                _factory = singlePageApplicationGenerationStrategyFactory ?? throw new ArgumentNullException(nameof(singlePageApplicationGenerationStrategyFactory));
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {

                var settings = _settingsProvider.Get($"{request.Directory}{Path.DirectorySeparatorChar}{request.Name}");

                settings = SettingsGenerator.Create(null, null, request.Name, request.Prefix, request.Directory, request.Minimal);

                SinglePageApplicationGenerator.Generate(settings, _factory, request.Name, request.Prefix, request.Directory, request.Minimal);

                settings = _settingsProvider.Get($"{request.Directory}{Path.DirectorySeparatorChar}{request.Name}");

                await _mediator.Publish(new SinglePageApplicationGenerated(settings));

                return new();
            }
        }
    }
}
