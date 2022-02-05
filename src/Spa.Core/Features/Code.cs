using Allagi.SharedKernal.Services;
using CommandLine;
using MediatR;
using Spa.Core.Events;
using Spa.Core.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Features
{
    internal class Code
    {
        [Verb("code")]
        internal class Request : IRequest<Unit> {
            [Option('p')]
            public string Prefix { get; set; } = "app";
            [Value(0)]
            public string Name { get; set; }
        }

        internal class Handler : IRequestHandler<Request, Unit>
        {
            private readonly IOrchestrationHandler _orchestrationHandler;

            public Handler(IOrchestrationHandler orchestrationHandler)
            {
                _orchestrationHandler = orchestrationHandler;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var angularCodeResult = await _orchestrationHandler.Handle<AngularCodeResult>(new CodeRequest(request.Name, request.Prefix), (tcs) => async message => {
                    switch (message)
                    {
                        case CodeResponse response:
                            tcs.SetResult(response.Code);
                            break;
                    }

                });

                _present("Html", angularCodeResult.Html);

                _present("TypeScript", angularCodeResult.TypeScript);

                _present("Scss", angularCodeResult.Scss);

                return new();
            }

            private void _present(string fileType, string[] code)
            {
                if(code.Length > 0)
                {
                    Console.WriteLine($"'---------------- {fileType}");

                    foreach (var lineOfCode in code)
                    {
                        Console.WriteLine(lineOfCode);
                    }

                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                }
            }
        }
    }
}
