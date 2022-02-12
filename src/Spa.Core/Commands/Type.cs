using CommandLine;
using MediatR;
using Spa.Core.Models;
using Spa.Core.Services;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Core.Features
{
    internal class Type
    {
        [Verb("type")]
        internal class Request : IRequest<Unit>
        {
            [Value(0)]
            public string EntityName { get; set; }

            [Option('f', Required = false)]
            public bool Force { get; set; } = false;

            [Option('d', Required = false)]
            public string Directory { get; set; } = System.Environment.CurrentDirectory;
        }

        internal class Handler : IRequestHandler<Request, Unit>
        {
            private readonly IFileSystem _fileWriter;
            private readonly ITemplateProcessor _templateProcessor;            
            private readonly INamingConventionConverter _namingConventionConverter;
            private readonly ISettingsProvider _settingsProvider;

            public Handler(
                IFileSystem fileWriter,
                ITemplateProcessor templateProcessor,
                INamingConventionConverter namingConventionConverter,
                ISettingsProvider settingsProvider
                )
            {
                _fileWriter = fileWriter;
                _templateProcessor = templateProcessor;
                _namingConventionConverter = namingConventionConverter;
                _settingsProvider = settingsProvider;
            }

            public Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                Settings settings = _settingsProvider.Get(request.Directory);

                var tokens = new TokensBuilder()
                    .With(nameof(request.EntityName), (Token)request.EntityName)
                    .With(nameof(request.Directory), (Token)request.Directory)
                    .Build();

                var result = new List<string>()
                {
                    _templateProcessor.Process("export type {{ entityNamePascalCase }} = {",tokens)
                };

                var modelsDirectory = $"{settings.DomainDirectory}{Path.DirectorySeparatorChar}Models";

                foreach(var path in Directory.GetFiles(modelsDirectory,$"{((Token)request.EntityName).PascalCase}.cs", SearchOption.AllDirectories))
                {
                    foreach (var line in File.ReadAllLines(path))
                    {
                        if (line.Contains("public string") || line.Contains("public Guid"))
                        {
                            result.Add($"    {_namingConventionConverter.Convert(NamingConvention.CamelCase, line.Split(' ')[10])}: string,");
                        }

                        if (line.Contains("public int") || line.Contains("public double") || line.Contains("public decimal"))
                        {
                            result.Add($"    {_namingConventionConverter.Convert(NamingConvention.CamelCase, line.Split(' ')[10])}: number,");
                        }

                        if (line.Contains("public bool"))
                        {
                            result.Add($"    {_namingConventionConverter.Convert(NamingConvention.CamelCase, line.Split(' ')[10])}: boolean,");
                        }
                    }
                }

                result.Add("};");

                var filePath = _templateProcessor.Process("{{ directory }}//{{ entityNameSnakeCase }}.ts", tokens);

                if(System.IO.File.Exists(filePath) && request.Force == false)
                {
                    return Task.FromResult(new Unit());
                }

                _fileWriter.WriteAllLines(filePath, result.ToArray());

                return Task.FromResult(new Unit());
            }
        }
    }
}
