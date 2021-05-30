using CommandLine;
using MediatR;
using Spa.Core.Services;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static System.Array;

namespace Spa.Core.Features
{
    internal class Index
    {
        [Verb(".")]
        internal class Request : IRequest<Unit>
        {
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

                _fileSystem.WriteAllLines($"{request.Directory}{Path.DirectorySeparatorChar}index.ts", Empty<string>());

                return new();
            }
        }
    }
}
