using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spa.Core.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Spa.Core.Services
{

    public class TranslationService: ITranslationService
    {
        private static readonly string endpoint = "https://api.cognitive.microsofttranslator.com/";
        private static readonly string location = "eastus2";
        private readonly IConfiguration _configuration;

        public TranslationService(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Detect(string textToDetect)
        {
            string route = "/detect?api-version=3.0";
            object[] body = new object[] { new { Text = textToDetect } };
            var requestBody = JsonConvert.SerializeObject(body);
            var client = new HttpClient();


            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _configuration["AzureTranslatorApiKey"]);
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", location);

            var response = client.PostAsAsync<List<DetectResponseDto>>(endpoint + route, new StringContent(requestBody, Encoding.UTF8, "application/json"))
                .GetAwaiter()
                .GetResult();

            return response.First().Language;
        }

        public void Sanitize(string path, string targetLanguage)
        {
            var json = File.ReadAllText(path);

            JObject jObject = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(json);

            Process(jObject);

            void Process(JObject jObject)
            {
                foreach (var prop in jObject.Children<JProperty>())
                {
                    switch (prop.Value)
                    {
                        case JObject childJObject:
                            Process(childJObject);
                            break;

                        case JValue value:
                            if (Detect($"{value}") != targetLanguage)
                            {
                                prop.Value = Translate($"{value}");
                            }
                            break;
                    }
                }
            }


            File.WriteAllText(path, JsonConvert.SerializeObject(jObject, Formatting.Indented));

        }

        public string Translate(string textToTranslate)
        {
            string route = "/translate?api-version=3.0&from=en&to=fr";
            object[] body = new object[] { new { Text = textToTranslate } };
            var requestBody = JsonConvert.SerializeObject(body);
            var client = new HttpClient();


            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _configuration["AzureTranslatorApiKey"]);
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", location);

            var response = client.PostAsAsync<JArray>(endpoint + route, new StringContent(requestBody, Encoding.UTF8, "application/json"))
                .GetAwaiter()
                .GetResult();

            return $"{response[0]["translations"][0]["text"]}";
        }
    }
}
