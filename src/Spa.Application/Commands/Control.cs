using CommandLine;
using MediatR;
using Spa.Core.Models;
using Spa.Core.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Core.Features
{
    internal class Control
    {
        [Verb("control")]
        internal class Request : IRequest<Unit>
        {
            [Value(0)]
            public string Name { get; set; }

            [Value(1)]
            public string Entity { get; set; }

            [Option('f', Required = false)]
            public bool Flat { get; set; } = false;

            [Option('d', Required = false)]
            public string Directory { get; set; } = System.Environment.CurrentDirectory;
        }

        internal class Handler : IRequestHandler<Request, Unit>
        {
            private readonly IFileSystem _fileSystem;
            private readonly ITemplateLocator _templateLocator;
            private readonly ITemplateProcessor _templateProcessor;
            private readonly ICommandService _commandService;
            private readonly IAngularJsonProvider _angularJsonProvider;

            public Handler(
                IFileSystem fileSystem,
                ITemplateLocator templateLocator,
                ITemplateProcessor templateProcessor,
                ICommandService commandService,
                IAngularJsonProvider angularJsonProvider
                )
            {
                _fileSystem = fileSystem;
                _templateProcessor = templateProcessor;
                _templateLocator = templateLocator;
                _commandService = commandService;
                _angularJsonProvider = angularJsonProvider;
            }
            public Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                try
                {
                    var tokens = new TokensBuilder()
                        .With("Name", (Token)request.Name)
                        .With("Directory", (Token)request.Directory)
                        .With("Prefix", (Token)_angularJsonProvider.Get(request.Directory).Prefix)
                        .Build();

                    if (request.Flat)
                    {
                        _commandService.Start(_templateProcessor.Process("ng g c {{ nameSnakeCase }} --flat", tokens));

                        WriteFile("Control", "{{ nameSnakeCase }}.component.ts", tokens);
                        _commandService.Start(_templateProcessor.Process("rimraf {{ nameSnakeCase }}.module.ts", tokens), request.Directory);
                        _commandService.Start(_templateProcessor.Process("rimraf {{ nameSnakeCase }}.component.spec.ts", tokens), request.Directory);
                    }
                    else
                    {
                        _commandService.Start(_templateProcessor.Process("ng g c {{ nameSnakeCase }} --skip-import", tokens));
                        WriteFile("Control", "{{ nameSnakeCase }}/{{ nameSnakeCase }}.component.ts", tokens);
                        _commandService.Start(_templateProcessor.Process("rimraf {{ nameSnakeCase }}/{{ nameSnakeCase }}.module.ts", tokens), request.Directory);
                        _commandService.Start(_templateProcessor.Process("rimraf {{ nameSnakeCase }}/{{ nameSnakeCase }}.component.spec.ts", tokens), request.Directory);

                    }

                    return Task.FromResult(new Unit());
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);

                    throw e;
                }
            }

            public void WriteFile(string templateName, string filename, Dictionary<string, object> tokens)
            {
                var template = _templateLocator.Get(templateName);

                var result = _templateProcessor.Process(template, tokens);

                var path = _templateProcessor.Process("{{ directory }}//" + filename, tokens);

                _fileSystem.WriteAllLines(path, result);
            }
        }
    }
}
