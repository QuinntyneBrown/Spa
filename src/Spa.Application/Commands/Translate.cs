using CommandLine;
using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Commands
{
    internal class Translate
    {
        [Verb("translate")]
        internal class Request : IRequest<Unit> {
            [Value(0)]
            public string TextToTranslate { get; set; }
        }

        internal class Handler : IRequestHandler<Request, Unit>
        {            
            private static readonly string endpoint = "https://api.cognitive.microsofttranslator.com/";
            private static readonly string location = "eastus2";
            private readonly IConfiguration _configuration;
            public Handler(
                IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                string route = "/translate?api-version=3.0&from=en&to=fr";                
                object[] body = new object[] { new { Text = request.TextToTranslate } };
                var requestBody = JsonConvert.SerializeObject(body);
                var client = new HttpClient();
                var httpRequest = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(endpoint + route),
                    Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
                };
                httpRequest.Headers.Add("Ocp-Apim-Subscription-Key", _configuration["AzureTranslatorApiKey"]);
                httpRequest.Headers.Add("Ocp-Apim-Subscription-Region", location);

                HttpResponseMessage response = client.SendAsync(httpRequest).GetAwaiter().GetResult();
                string result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Console.WriteLine(result);

                return Task.FromResult(new Unit());
            }
        }
    }
}
