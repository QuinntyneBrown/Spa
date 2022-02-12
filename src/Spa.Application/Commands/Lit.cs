using CommandLine;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Core.Features
{
    // use this to create a lit-html SPA
    // https://github.com/QuinntyneBrown/Paris
    internal class Lit
    {
        [Verb("lit")]
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
