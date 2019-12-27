using System.Collections.Generic;
using Core.Logging;

namespace Core.Services.Crosspost
{
    public class CrossPostServiceFactory
    {
        private readonly ILogger _logger;

        public CrossPostServiceFactory(ILogger logger)
        {
            _logger = logger;
        }

        public ICrossPostService CreateTwitterService(string consumerKey,
            string consumerSecret,
            string accessToken,
            string accessTokenSecret,
            string name, 
            IReadOnlyCollection<string> defaultTags) =>
            new TwitterCrosspostService(
                consumerKey,
                consumerSecret,
                accessToken,
                accessTokenSecret,
                name,
                defaultTags,
                _logger);

        public ICrossPostService CreateTelegramService(string token, string name) =>
            new TelegramCrosspostService(token, name, _logger);

        public ICrossPostService CreateFacebookService(string token, string name) =>
            new FacebookCrosspostService(token, name, _logger);

        public ICrossPostService CreateFakeService() =>
            new FakeCrosspostService(_logger);
    }
}