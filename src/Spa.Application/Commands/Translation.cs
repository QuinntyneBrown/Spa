using CommandLine;
using MediatR;
using Newtonsoft.Json.Linq;
using Spa.Core.Models;
using Spa.Core.Services;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Core.Features
{
    internal class Translation
    {
        [Verb("translation")]
        internal class Request : IRequest<Unit> {
            [Value(0)]
            public string Section { get; set; }
            [Value(1)]
            public string Key { get; set; }

            [Value(2)]
            public string English { get; set; }

            [Value(3)]
            public string French { get; set; }

            [Option('d', Required = false)]
            public string Directory { get; set; } = System.Environment.CurrentDirectory;
        }

        internal class Handler : IRequestHandler<Request, Unit>
        {
            private readonly IAngularJsonProvider _angularJsonProvider;

            public Handler(IAngularJsonProvider angularJsonProvider)
            {
                _angularJsonProvider = angularJsonProvider;
            }
            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                AngularJsonFileModel angularJson = _angularJsonProvider.Get(request.Directory);

                AddOrUpdateTranslation("en.json", request.Section, request.Key, request.English);

                AddOrUpdateTranslation("fr.json", request.Section, request.Key, request.French);

                void AddOrUpdateTranslation(string filename, string section, string key, string translation)
                {
                    section = ((Token)section).KebobCase;

                    key = ((Token)key).KebobCase;
                    
                    var translationsFilePath = $"{angularJson.TranslationsDirectory}{Path.DirectorySeparatorChar}{filename}";

                    var translationJson = File.ReadAllText(translationsFilePath);

                    JObject translationsJsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(translationJson);

                    if(translationsJsonObject[section] == null)
                    {
                        translationsJsonObject[section] = new JObject();
                    }

                    JObject translationsSectionJsonObject = translationsJsonObject[section] as JObject;

                    if(translationsSectionJsonObject.GetValue(key) == null)
                    {
                        translationsSectionJsonObject.Add(key, translation);
                    }
                    else
                    {
                        translationsSectionJsonObject[key] = translation;
                    }

                    translationsJsonObject[section] = translationsSectionJsonObject;

                    File.WriteAllText(translationsFilePath, Newtonsoft.Json.JsonConvert.SerializeObject(translationsJsonObject, Newtonsoft.Json.Formatting.Indented));
                }

                return new();
            }
        }
    }
}
