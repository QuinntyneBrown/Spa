using CommandLine;
using MediatR;
using Spa.Core.Services;
using System;
using System.Collections.Generic;
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
                List<string> lines = new();

                foreach(var file in Directory.GetFiles(request.Directory,"*.ts"))
                {
                    if(!file.Contains(".spec.") && !file.EndsWith("index.ts"))
                    {
                        lines.Add($"export * from './{Path.GetFileNameWithoutExtension(file)}';");
                    }
                }

                _fileSystem.WriteAllLines($"{request.Directory}{Path.DirectorySeparatorChar}index.ts", lines.ToArray());

                return new();
            }
        }
    }
}
