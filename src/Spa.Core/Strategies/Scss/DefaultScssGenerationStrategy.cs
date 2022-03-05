using Spa.Core.Models;
using Spa.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;

namespace Spa.Core.Strategies.Scss
{

    public class DefaultScssGenerationStrategy: IDefaultScssGenerationStrategy
    {
        private readonly IAngularJsonProvider _angularJsonProvider;
        private readonly INamingConventionConverter _namingConventionConverter;
        private readonly IFileSystem _fileSystem;
        private readonly ICommandService _commandService;
        private readonly ITemplateLocator _templateLocator;
        private readonly ITemplateProcessor _templateProcessor;

        public DefaultScssGenerationStrategy(
            IAngularJsonProvider angularJsonProvider,
            INamingConventionConverter namingConventionConverter,
            IFileSystem fileSystem,
            ICommandService commandService,
            ITemplateLocator templateLocator,
            ITemplateProcessor templateProcessor)
        {
            _angularJsonProvider = angularJsonProvider ?? throw new ArgumentNullException(nameof(angularJsonProvider));
            _namingConventionConverter = namingConventionConverter ?? throw new ArgumentNullException(nameof(namingConventionConverter));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            _templateLocator = templateLocator ?? throw new ArgumentNullException(nameof(templateLocator));
            _templateProcessor = templateProcessor ?? throw new ArgumentNullException(nameof(templateProcessor));
        }

        public void Create(SinglePageApplicationModel model)
        {

            var angularJson = _angularJsonProvider.Get(model.Directory);

            var tokens = new TokensBuilder()
                .With("Prefix", (Token)model.Prefix)
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


        }
    }
}
