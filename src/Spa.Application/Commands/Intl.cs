using CommandLine;
using MediatR;
using Spa.Core.Models;
using Spa.Core.Services;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Core.Features
{
    internal class Intl
    {
        [Verb("intl")]
        internal class Request : IRequest<Unit> {
            [Value(0)]
            public string Name { get; set; }

            [Option('d', Required = false)]
            public string Directory { get; set; } = System.Environment.CurrentDirectory;
        }

        internal class Handler : IRequestHandler<Request, Unit>
        {
            private readonly IFileSystem _fileSystem;
            private readonly ITemplateLocator _templateLocator;
            private readonly ITemplateProcessor _templateProcessor;

            public Handler(IFileSystem fileSystem, ITemplateLocator templateLocator, ITemplateProcessor templateProcessor)
            {
                _fileSystem = fileSystem;
                _templateLocator = templateLocator;
                _templateProcessor = templateProcessor;
            }
            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var template = _templateLocator.Get("IntlBuilder");

                var tokens = new TokensBuilder()
                .With(nameof(request.Name), (Token)request.Name)
                .Build();

                var lines = _templateProcessor.Process(template, tokens);

                _fileSystem.WriteAllLines($"{request.Directory}{Path.DirectorySeparatorChar}{((Token)request.Name).SnakeCase}-intl.ts", lines);

                return new();
            }
        }
    }
}
