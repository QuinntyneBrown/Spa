using CommandLine;
using MediatR;
using Spa.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Core.Features
{
    internal class Index
    {
        [Verb(".")]
        internal class Request : IRequest<Unit>
        {
            [Option('s')]
            public bool Scss { get; set; }

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
                List<string> lines = new();

                foreach (var path in Directory.GetDirectories(request.Directory))
                {
                    var files = Directory.GetFiles(path);

                    var fileNames = Directory.GetFiles(path)
                        .Select(path => Path.GetFileNameWithoutExtension(path));

                    var containsIndex = Directory.GetFiles(path)
                        .Select(path => Path.GetFileNameWithoutExtension(path))
                        .Contains("index");

                    if (!request.Scss && Directory.GetFiles(path)
                        .Select(path => Path.GetFileNameWithoutExtension(path))
                        .Contains("index"))
                    {
                        lines.Add($"export * from './{Path.GetFileNameWithoutExtension(path)}';");
                    }
                }

                if (request.Scss)
                {

                    foreach (var file in Directory.GetFiles(request.Directory, "*.scss"))
                    {
                        if (!file.EndsWith("_index.scss"))
                        {
                            lines.Add($"@use './{Path.GetFileNameWithoutExtension(file)}';");
                        }
                    }

                    _fileSystem.WriteAllLines($"{request.Directory}{Path.DirectorySeparatorChar}_index.scss", lines.ToArray());
                }
                else
                {
                    foreach (var file in Directory.GetFiles(request.Directory, "*.ts"))
                    {
                        if (!file.Contains(".spec.") && !file.EndsWith("index.ts"))
                        {
                            lines.Add($"export * from './{Path.GetFileNameWithoutExtension(file)}';");
                        }
                    }

                    _fileSystem.WriteAllLines($"{request.Directory}{Path.DirectorySeparatorChar}index.ts", lines.ToArray());
                }

                return new();
            }
        }
    }
}
