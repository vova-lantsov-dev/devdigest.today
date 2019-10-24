using System;
using System.Linq;
using Core.Logging;
using Core.Models.CognitiveServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using Serilog.Events;

namespace Core.Services
{
    public class LanguageAnalyzerService
    {
        private readonly IRestClient _client;
        private readonly ILogger _logger;

        public LanguageAnalyzerService(string key, ILogger logger)
        {
            _logger = logger;

            _client = new RestClient("https://westeurope.api.cognitive.microsoft.com/text/analytics/v2.0");

            _client.AddDefaultHeader("Ocp-Apim-Subscription-Key", key);
            _client.AddDefaultHeader("Content-Type", "application/json");
            _client.AddDefaultHeader("Accept", "application/json");
        }

        public string GetTextLanguage(string text)
        {
            var request = new RestRequest("languages", Method.POST) {RequestFormat = DataFormat.Json};

            var body = new Query
            {
                Documents = new[] {new RequestDocument(Guid.NewGuid().ToString(), text)}
            };

            var json = JsonConvert.SerializeObject(body, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            request.AddParameter("application/json", json, ParameterType.RequestBody);

            try
            {
                var response = _client.Execute(request);
                var apiResponse = JsonConvert.DeserializeObject<Response>(response.Content);

                var language = apiResponse.Documents?
                    .Select(o => o.DetectedLanguages.FirstOrDefault())
                    .Select(o => o?.Iso6391Name?.Trim()?.ToLower())
                    .FirstOrDefault();

                return language ?? string.Empty;
            }
            catch(Exception ex)
            {
                _logger.Write(LogEventLevel.Error, $"Error in `{nameof(GetTextLanguage)}` method", ex);
                
                return string.Empty;
            }
        }
    }
}