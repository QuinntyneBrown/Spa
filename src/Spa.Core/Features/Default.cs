using CommandLine;
using MediatR;
using Spa.Core.Builders;
using Spa.Core.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Core.Features
{
    internal class Default
    {
        [Verb("default")]
        internal class Request : IRequest<Unit> {
            [Value(0)]
            public string Name { get; set; } = "Default";

            [Option('d', Required = false)]
            public string Directory { get; set; } = System.Environment.CurrentDirectory;
        }

        internal class Handler : IRequestHandler<Request, Unit>
        {
            private readonly ICommandService _commandService;

            public Handler(ICommandService commandService)
            {
                _commandService = commandService;
            }
            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                new SpaBuilder(_commandService).Build();

                return new();
            }
        }
    }
}
