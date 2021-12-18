using CommandLine;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Core.Features
{
    internal class Component
    {
        [Verb("component")]
        internal class Request : IRequest<Unit> {

        }

        internal class Handler : IRequestHandler<Request, Unit>
        {
            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                return new();
            }
        }
    }
}
