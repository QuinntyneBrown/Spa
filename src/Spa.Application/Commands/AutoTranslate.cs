using CommandLine;
using MediatR;
using Newtonsoft.Json.Linq;
using Spa.Core.Models;
using Spa.Core.Services;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Core.Features
{
    internal class AutoTranslate
    {
        [Verb("auto-translate")]
        internal class Request : IRequest<Unit>
        {
            [Value(0)]
            public string Section { get; set; }

            [Value(1)]
            public string English { get; set; }

            [Option('p',"placeholder", Required = false)]
            public bool Placeholder { get; set; }

            [Option('d', Required = false)]
            public string Directory { get; set; } = System.Environment.CurrentDirectory;
        }

        internal class Handler : IRequestHandler<Request, Unit>
        {
            private readonly IAngularJsonProvider _angularJsonProvider;
            private readonly ITranslationService _translationService;

            public Handler(IAngularJsonProvider angularJsonProvider, ITranslationService translationService)
            {
                _angularJsonProvider = angularJsonProvider;
                _translationService = translationService;
            }
            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                AngularJson angularJson = _angularJsonProvider.Get(request.Directory);

                AddOrUpdateTranslation("en.json", request.Section, request.English, request.English);

                AddOrUpdateTranslation("fr.json", request.Section, request.English, request.Placeholder ? $"{request.English} -FR" : _translationService.Translate(request.English));

                void AddOrUpdateTranslation(string filename, string section, string key, string translation)
                {
                    section = ((Token)section).KebobCase;

                    key = ((Token)key).KebobCase;

                    key = key.Replace("/", "_");

                    key = key.Replace(",", "");

                    key = key.Replace("'", "");

                    key = key.Replace("?", "");

                    var translationsFilePath = $"{angularJson.TranslationsDirectory}{Path.DirectorySeparatorChar}{filename}";

                    var translationJson = File.ReadAllText(translationsFilePath);

                    JObject translationsJsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(translationJson);

                    if (translationsJsonObject[section] == null)
                    {
                        translationsJsonObject[section] = new JObject();
                    }

                    JObject translationsSectionJsonObject = translationsJsonObject[section] as JObject;

                    if (translationsSectionJsonObject.GetValue(key) == null)
                    {
                        translationsSectionJsonObject.Add(key, translation);
                    }
                    else
                    {
                        translationsSectionJsonObject[key] = translation;
                    }

                    translationsJsonObject[section] = translationsSectionJsonObject;

                    File.WriteAllText(translationsFilePath, Newtonsoft.Json.JsonConvert.SerializeObject(translationsJsonObject, Newtonsoft.Json.Formatting.Indented));

                    if (filename == "fr.json")
                    {
                        Console.Write("{{ ");

                        Console.Write($"\"{section}.{key}\" | translate");

                        Console.Write(" }}");
                    }
                }


                return new();
            }
        }
    }
}
