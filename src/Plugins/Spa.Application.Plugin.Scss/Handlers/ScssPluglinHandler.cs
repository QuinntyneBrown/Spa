using MediatR;
using Spa.Core.Events;
using Spa.Core.Models;
using Spa.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Application.Plugin.Scss.Handlers
{
    public class ScssPluglinHandler : INotificationHandler<SinglePageApplicationGenerated>
    {
        private readonly IAngularJsonProvider _angularJsonProvider;
        private readonly INamingConventionConverter _namingConventionConverter;
        private readonly IFileSystem _fileSystem;
        private readonly ICommandService _commandService;
        private readonly ITemplateLocator _templateLocator;
        private readonly ITemplateProcessor _templateProcessor;

        public ScssPluglinHandler(
            IAngularJsonProvider angularJsonProvider,
            INamingConventionConverter namingConventionConverter,
            IFileSystem fileSystem,
            ICommandService commandService,
            ITemplateLocator templateLocator,
            ITemplateProcessor templateProcessor)
        {
            _angularJsonProvider = angularJsonProvider;
            _namingConventionConverter = namingConventionConverter;
            _fileSystem = fileSystem;
            _commandService = commandService;
            _templateLocator = templateLocator;
            _templateProcessor = templateProcessor;
        }

        public Task Handle(SinglePageApplicationGenerated notification, CancellationToken cancellationToken)
        {

            var angularJson = _angularJsonProvider.Get(notification.Settings.AppDirectories.First());

            var tokens = new TokensBuilder()
                .With("Prefix", (Token)notification.Settings.Prefix)
                .Build();

            foreach (var name in new List<string>() {
                "Actions",
                "Brand",
                "Breakpoints",
                "Buttons",
                "Dialogs",
                "Field",
                "FormFields",
                "Header",
                "Label",
                "ListDetailDirective",
                "Pills",
                "RouterLinkActive",
                "Table",
                "Textarea",
                "Title",
                "TitleBar",
                "Variables"
            })
            {
                var nameSnakeCase = _namingConventionConverter.Convert(NamingConvention.SnakeCase, name);

                var template = _templateLocator.Get(name);

                var results = _templateProcessor.Process(template, tokens);

                _fileSystem.WriteAllLines($"{angularJson.ScssDirectory}{Path.DirectorySeparatorChar}_{nameSnakeCase}.scss", results);
            }

            _commandService.Start("spa . -s", angularJson.ScssDirectory);

            return Task.CompletedTask;
        }
    }
}
