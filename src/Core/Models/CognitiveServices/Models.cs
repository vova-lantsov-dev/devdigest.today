using System.Collections.Generic;

namespace Core.CognitiveServices
{
    public class Query
    {
        public IEnumerable<RequestDocument> Documents { get; set; }
    }

    public class RequestDocument
    {
        public RequestDocument(string id, string text)
        {
            Id = id;
            Text = text;
        }

        public string Id { get; set; }
        public string Text { get; set; }
    }

    public class DetectedLanguage
    {
        public string Name { get; set; }
        public string Iso6391Name { get; set; }
        public double Score { get; set; }
    }

    public class ResponseDocument
    {
        public string Id { get; set; }
        public IList<DetectedLanguage> DetectedLanguages { get; set; }
    }

    public class Response
    {
        public IList<ResponseDocument> Documents { get; set; }
        public IList<object> Errors { get; set; }
    }
}