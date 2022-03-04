using CommandLine;
using MediatR;
using Spa.Core.Models;
using Spa.Core.Services;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Commands
{
    internal class Sanitize
    {
        [Verb("sanitize")]
        internal class Request : IRequest<Unit> {

            [Option('d', Required = false)]
            public string Directory { get; set; }
        }

        internal class Handler : IRequestHandler<Request, Unit>
        {
            private readonly IAngularJsonProvider _angularJsonProvider;
            private readonly ITranslationService _translationService;

            public Handler(
                IAngularJsonProvider angularJsonProvider, 
                ITranslationService translationService)
            {
                _angularJsonProvider = angularJsonProvider;
                _translationService = translationService;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                AngularJson angularJson = _angularJsonProvider.Get(request.Directory);

                var translationsFilePath = $"{angularJson.TranslationsDirectory}{Path.DirectorySeparatorChar}fr.json";

                _translationService.Sanitize(translationsFilePath, "fr");

                return new();
            }
        }
    }
}
