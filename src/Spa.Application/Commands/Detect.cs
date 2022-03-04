using CommandLine;
using MediatR;
using Spa.Core.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Commands
{
    internal class Detect
    {
        [Verb("detect")]
        internal class Request : IRequest<Unit> {
            [Value(0)]
            public string TextToDetect { get; set; }
        }

        internal class Handler : IRequestHandler<Request, Unit>
        {
            private readonly ITranslationService _translationService;

            public Handler(ITranslationService translationService)
            {
                _translationService = translationService ?? throw new System.ArgumentNullException(nameof(translationService)); 
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                Console.WriteLine(_translationService.Detect(request.TextToDetect));

                return new();
            }
        }
    }
}
