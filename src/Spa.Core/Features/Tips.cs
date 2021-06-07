using CommandLine;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Core.Features
{
    internal class Tips
    {
        [Verb("tips", HelpText = "Display Tips")]
        internal class Request : IRequest<Unit> {

        }

        internal class Handler : IRequestHandler<Request, Unit>
        {
            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                Console.WriteLine("Generate Angular Module and Lazy Loading Code");
                Console.WriteLine("ng g m --module=app.module --route=route your-module-name");
                return new();
            }
        }
    }
}
