using CommandLine;
using MediatR;
using Spa.Core.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Features
{
    internal class Form
    {
        [Verb("form")]
        internal class Request : IRequest<Unit> {
            [Value(0)]
            public string Path { get; set; }

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
                return new();
            }
        }
    }
}
