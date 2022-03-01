using CommandLine;
using MediatR;
using Spa.Core.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Commands
{
    internal class Audit
    {
        [Verb("audit")]
        internal class Request : IRequest<Unit> {
            [Value(0)]
            public string Path { get; set; }

            [Option('d', Required = false)]
            public string Directory { get; set; } = System.Environment.CurrentDirectory;
        }

        internal class Handler : IRequestHandler<Request, Unit>
        {
            private readonly ITranslationAuditService _translationAuditService;
            private readonly IFileSystem _fileSystem;
            public Handler(ITranslationAuditService translationAuditService, IFileSystem fileSystem)
            {
                _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
                _translationAuditService = translationAuditService ?? throw new ArgumentNullException(nameof(translationAuditService));
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var directory = $"{request.Directory}{Path.DirectorySeparatorChar}{request.Path}";

                var result = _translationAuditService.AuditDirectory(directory);

                _fileSystem.WriteAllLines($"{directory}{Path.DirectorySeparatorChar}translate-audit-results.txt", result.Select(x => $"{x.Key}:{x.Value.Count}").ToArray());

                return new();
            }
        }
    }
}
