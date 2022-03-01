using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spa.Core.Extensions;
using System.Net.Http;
using System.Text;

namespace Spa.Core.Services
{
    public interface ITranslationService
    {
        string Translate(string textToTranslate);
    }

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
