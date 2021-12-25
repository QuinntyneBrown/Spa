using CommandLine;
using MediatR;
using Spa.Core.Models;
using Spa.Core.Services;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Core.Features
{
    internal class ChooseAFileOrDragItHere
    {
        [Verb("choose-a-file-or-drag-it-here")]
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
                var angularJson = _angularJsonProvider.Get(request.Directory);

                var tokens = new TokensBuilder()
                    .With("Name", (Token)nameof(ChooseAFileOrDragItHere))
                    .With("Directory", (Token)request.Directory)
                    .With("Prefix", (Token)angularJson.Prefix)
                    .Build();


                _commandService.Start(_templateProcessor.Process("ng g c {{ nameSnakeCase }} --skip-import", tokens), request.Directory);

                var componentDirectory = $"{request.Directory}{Path.DirectorySeparatorChar}{_templateProcessor.Process("{{ nameSnakeCase }}", tokens)}";

                var componentTemplate = _templateProcessor.Process(_templateLocator.Get("ChooseAFileOrDragItHere"), tokens);

                var htmlTemplate = _templateProcessor.Process(_templateLocator.Get("ChooseAFileOrDragItHereHtml"), tokens);

                var scssTemplate = _templateProcessor.Process(_templateLocator.Get("ChooseAFileOrDragItHereScss"), tokens);

                _fileSystem.WriteAllLines($@"{componentDirectory}{Path.DirectorySeparatorChar}{_templateProcessor.Process(@"{{ nameSnakeCase }}.component.ts", tokens)}", componentTemplate);

                _fileSystem.WriteAllLines($@"{componentDirectory}{Path.DirectorySeparatorChar}{_templateProcessor.Process(@"{{ nameSnakeCase }}.component.html", tokens)}", htmlTemplate);

                _fileSystem.WriteAllLines($@"{componentDirectory}{Path.DirectorySeparatorChar}{_templateProcessor.Process(@"{{ nameSnakeCase }}.component.scss", tokens)}", scssTemplate);

                _commandService.Start("spa .", componentDirectory);

                _commandService.Start("spa .", request.Directory);

                return Task.FromResult(new Unit());
            }
        }
    }
}
