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
    internal class Resolve
    {
        [Verb("resolve")]
        internal class Request : IRequest<Unit>
        {
            [Value(0)]
            public string Path { get; set; }

            [Option('d', Required = false)]
            public string Directory { get; set; } = System.Environment.CurrentDirectory;
        }

        internal class Handler : IRequestHandler<Request, Unit>
        {
            private readonly ITranslationAuditService _translationAuditService;
            private readonly IFileSystem _fileSystem;
            private readonly ICommandService _commandService;
            public Handler(ITranslationAuditService translationAuditService, IFileSystem fileSystem, ICommandService commandService)
            {
                _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
                _translationAuditService = translationAuditService ?? throw new ArgumentNullException(nameof(translationAuditService));
                _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var directory = $"{request.Directory}{Path.DirectorySeparatorChar}{request.Path}";

                var result = _translationAuditService.AuditDirectory(directory);

                foreach(var path in result.Select(x => x.Key))
                {
                    _commandService.Start($"code {path}");

                    Console.WriteLine("Hit any key to open the next file...");

                    Console.ReadKey();
                }

                _fileSystem.WriteAllLines($"{directory}{Path.DirectorySeparatorChar}translate-audit-results.txt", result.Select(x => $"{x.Key}:{x.Value.Count}").ToArray());

                return new();
            }
        }
    }
}
