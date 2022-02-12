using CommandLine;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Core.Features
{
    internal class Models
    {
        [Verb("models")]
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
