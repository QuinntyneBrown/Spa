using Spa.Core.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using static System.Text.Json.JsonSerializer;


namespace Spa.Core.Models
{
    public class Settings
    {
        public bool IsRoot { get; set; } = true;
        public int IdFormat { get; set; }
        public int IdDotNetType { get; set; }
        public string Prefix { get; set; } = "app";
        public bool IsMicroserviceArchitecture { get; set; } = true;
        public List<string> Projects { get; set; } = new List<string>();
        public string RootDirectory { get; set; }
        public string SolutionName { get; set; }
        public string SolutionFileName { get; set; }
        public string ApiFullPath { get; set; }
        public string RootNamespace { get; set; }
        public string DomainNamespace { get; set; }
        public string ApplicationNamespace { get; set; }
        public string InfrastructureNamespace { get; set; }
        public string ApiNamespace { get; set; }
        public string DomainDirectory { get; set; }
        public string ApplicationDirectory { get; set; }
        public string InfrastructureDirectory { get; set; }
        public string ApiDirectory { get; set; }
        public string UnitTestsDirectory { get; set; }
        public string TestingDirectory { get; set; }
        public List<string> AppDirectories { get; set; } = new List<string>();
        public string BuildingBlocksCoreNamespace { get; set; } = "BuildingBlocks.Core";
        public string BuildingBlocksEventStoreNamespace { get; set; } = "BuildingBlocks.EventStore";
        public string SourceFolder { get; set; } = "src";
        public string TestFolder { get; set; } = "tests";
        public string DbContextName { get; set; }
        public int? Port { get; set; } = 5000;
        public int? SslPort { get; set; } = 5001;
        public List<string> Plugins { get; set; }
        public List<dynamic> Entities { get; set; } = new List<dynamic>();
        public List<dynamic> Resources { get; set; } = new List<dynamic>();
        public List<ClientApplicationModel> ClientApplications { get; private set; } = new List<ClientApplicationModel>();  
        public void AddApp(string directory, IFileSystem fileSystem)
        {
            if (!AppDirectories.Contains(directory))
            {
                AppDirectories = AppDirectories.Concat(new string[1] { directory }).ToList();
            }


            if (!Directory.Exists(RootDirectory))
            {
                Directory.CreateDirectory(RootDirectory);
            }

            fileSystem.WriteAllLines($"{RootDirectory}{Path.DirectorySeparatorChar}clisettings.json", new string[1] {
                    Serialize(this, new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            WriteIndented = true
                        })
                });
        }

        public Settings()
        {

        }

    }
}
