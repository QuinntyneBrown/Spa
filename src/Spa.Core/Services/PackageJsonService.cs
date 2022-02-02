using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spa.Core.Services
{
    public interface IPackageJsonService
    {
        void AddGenerateModelsNpmScript(string packageJsonFilePath);
    }

    public class PackageJsonService: IPackageJsonService
    {
        public void AddGenerateModelsNpmScript(string filePath)
        {
            var key = "generate-models";

            var value = "node node_modules/ng-swagger-gen/ng-swagger-gen --config ng-swagger-gen.json";

            var section = "scripts";

            var json = File.ReadAllText(filePath);

            JObject jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(json);

            JObject scriptsJsonObject = jsonObject[section] as JObject;

            if (scriptsJsonObject.GetValue(key) == null)
            {
                scriptsJsonObject.Add(key, value);
            }
            else
            {
                scriptsJsonObject[key] = value;
            }

            jsonObject[section] = scriptsJsonObject;

            File.WriteAllText(filePath, Newtonsoft.Json.JsonConvert.SerializeObject(jsonObject, Newtonsoft.Json.Formatting.Indented));
        }
    }
}
