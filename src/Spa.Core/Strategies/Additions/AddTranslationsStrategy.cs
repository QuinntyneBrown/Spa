using Spa.Core.Models;
using Spa.Core.Services;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Spa.Core.Strategies.Additions
{

    public class AddTranslationsStrategy: IAdditionsStrategy
    {
        private readonly ICommandService _commandService;
        private readonly IFileSystem _fileSystem;

        private readonly List<ImportModel> _importModels = new List<ImportModel>()
        {
            new ImportModel("HttpClient","@angular/common/http"),
            new ImportModel("TranslateLoader","@ngx-translate/core"),
            new ImportModel("TranslateModule ","@ngx-translate/core"),
            new ImportModel("TranslateHttpLoader ","@ngx-translate/http-loader")
        };

        public AddTranslationsStrategy(ICommandService commandService, IFileSystem fileSystem)
        {
            _commandService = commandService;
            _fileSystem = fileSystem;
        }

        public void Add(SinglePageApplicationModel model)
        {
            _commandService.Start("npm i @ngx-translate/core", model.Directory);

            _commandService.Start("npm i @ngx-translate/http-loader", model.Directory);

            _addImports(model);

            _addHttpLoaderFactory(model);

            _registerTranslateModule(model);

            var i18nDirectory = $"{model.SrcDirectory}{Path.DirectorySeparatorChar}assets{Path.DirectorySeparatorChar}i18n";

            _fileSystem.CreateDirectory(i18nDirectory);

            foreach(var lang in model.SupportedLanguages)
            {
                _fileSystem.WriteAllText($"{i18nDirectory}{Path.DirectorySeparatorChar}{lang}.json", "{}");
            }

            _setInitialLanguageInAppComponent(model);
        }

        private void _addImports(SinglePageApplicationModel model)
        {
            var appModulePath = $"{model.AppDirectory}{Path.DirectorySeparatorChar}app.module.ts";

            var lines = _fileSystem.ReadAllLines(appModulePath);

            var newLines = new List<string>();

            var added = false;

            foreach (var line in lines)
            {
                if(!line.StartsWith("import") && !added)
                {
                    foreach(var importModel in _importModels)
                    {
                        newLines.Add(new StringBuilder()
                            .Append("import { ")
                            .Append(importModel.Name)
                            .Append(" }")
                            .Append($" from '{importModel.ModuleName}';")
                            .ToString());
                    }

                    added = true;
                }

                newLines.Add(line);

            }

            _fileSystem.WriteAllLines(appModulePath, newLines.ToArray());
        }

        private void _addHttpLoaderFactory(SinglePageApplicationModel model)
        {
            var appModulePath = $"{model.AppDirectory}{Path.DirectorySeparatorChar}app.module.ts";

            var lines = _fileSystem.ReadAllLines(appModulePath);

            var newLines = new List<string>();

            var added = false;

            foreach (var line in lines)
            {
                if (!line.StartsWith("import") && !added)
                {
                    foreach (var newLine in new string[3] {
                        "export function HttpLoaderFactory(httpClient: HttpClient) {",
                        "  return new TranslateHttpLoader(httpClient);",
                        "}"
                    })
                    {
                        newLines.Add(newLine);
                    }

                    added = true;
                }

                newLines.Add(line);

            }

            _fileSystem.WriteAllLines(appModulePath, newLines.ToArray());
        }

        private void _registerTranslateModule(SinglePageApplicationModel model)
        {
            var appModulePath = $"{model.AppDirectory}{Path.DirectorySeparatorChar}app.module.ts";

            var lines = _fileSystem.ReadAllLines(appModulePath);

            var newLines = new List<string>();

            var added = false;

            foreach (var line in lines)
            {
                if (line.Contains("imports: [") && !added)
                {
                    newLines.Add(line);

                    foreach (var newLine in new string[7]
                    {
                        "    TranslateModule.forRoot({",
                        "       loader: {",
                        "        provide: TranslateLoader,",
                        "        useFactory: HttpLoaderFactory,",
                        "        deps: [HttpClient]",
                        "      }",
                        "    }),"
                    })
                    {
                        newLines.Add(newLine);
                    }

                    added = true;
                } else
                {
                    newLines.Add(line);
                }
            }

            _fileSystem.WriteAllLines(appModulePath, newLines.ToArray());
        }

        private void _setInitialLanguageInAppComponent(SinglePageApplicationModel model)
        {
            var appComponentPath = $"{model.AppDirectory}{Path.DirectorySeparatorChar}app.component.ts";

            var createConstructor = !_fileSystem.ReadAllText(appComponentPath).Contains("constructor");

            var ctor = new string[4]
            {
                "  constructor(private readonly _translateService: TranslateService) {",
                $"    _translateService.setDefaultLang(\"{model.DefaultLanguage}\");",
                $"    _translateService.use(localStorage.getItem(\"currentLanguage\") || \"{model.DefaultLanguage}\");",
                "  }"
            };

            var lines = _fileSystem.ReadAllLines(appComponentPath);

            var newLines = new List<string>();

            var importAdded = false;

            var constructorUpdated = false;

            foreach (var line in lines)
            {
                if (!line.StartsWith("import") && !importAdded)
                {
                    newLines.Add("import { TranslateService } from '@ngx-translate/core';");

                    importAdded = true;
                }

                newLines.Add(line);

                if (line.StartsWith("export class AppComponent {") && !constructorUpdated && createConstructor)
                {
                    foreach(var newLine in ctor)
                    {
                        newLines.Add(newLine);
                    }

                    constructorUpdated = true;
                }

                if (line.Contains("constructor") && !constructorUpdated && !createConstructor)
                {
                    newLines.RemoveAt(newLines.Count - 1);

                    var newLine = line.Replace("constructor(", "constructor(private readonly _translateService: TranslateService, ");

                    newLines.Add(newLine);

                    newLines.Add($"    _translateService.setDefaultLang(\"{model.DefaultLanguage}\");");
                    newLines.Add($"    _translateService.use(localStorage.getItem(\"currentLanguage\") || \"{model.DefaultLanguage}\");");


                    constructorUpdated = true;
                }
            }

            _fileSystem.WriteAllLines(appComponentPath, newLines.ToArray());
        }

    }



    public class TranslateModuleRegistration
    {
        public string[] Create() => new string[7]
        {
            "    TranslateModule.forRoot({",
            "       loader: {",
            "        provide: TranslateLoader,",
            "        useFactory: HttpLoaderFactory,",
            "        deps: [HttpClient]",
            "      }",
            "    }),"
        };
    }
}
