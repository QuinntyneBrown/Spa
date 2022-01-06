using CommandLine;
using MediatR;
using Spa.Core.Services;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Core.Features
{
    internal class CssComponent
    {
        [Verb("css-component")]
        internal class Request : IRequest<Unit>
        {
            [Value(0)]
            public string Name { get; set; }

            [Option('d', Required = false)]
            public string Directory { get; set; } = System.Environment.CurrentDirectory;
        }

        internal class Handler : IRequestHandler<Request, Unit>
        {
            private readonly IAngularJsonProvider _angularJsonProvider;
            private readonly ICommandService _commandService;
            private readonly IFileSystem _fileSystem;
            private readonly INamingConventionConverter _namingConventionConverter;
            public Handler(
                IAngularJsonProvider angularJsonProvider,
                ICommandService commandService,
                IFileSystem fileSystem,
                INamingConventionConverter namingConventionConverter)
            {
                _angularJsonProvider = angularJsonProvider;
                _commandService = commandService;
                _fileSystem = fileSystem;
                _namingConventionConverter = namingConventionConverter;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var angularJson = _angularJsonProvider.Get(request.Directory);

                var nameSnakeCase = _namingConventionConverter.Convert(NamingConvention.SnakeCase, request.Name);

                _fileSystem.WriteAllLines($"{angularJson.ScssDirectory}{Path.DirectorySeparatorChar}_{nameSnakeCase}.scss", new string[3] {
                    $".{angularJson.Prefix}-{nameSnakeCase}" + " {",
                    "",
                    "}"
                });

                _commandService.Start("spa . -s", angularJson.ScssDirectory);

                return new();
            }
        }
    }
}
