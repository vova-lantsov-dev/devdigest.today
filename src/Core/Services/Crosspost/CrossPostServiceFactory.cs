using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Core.Services.Crosspost
{
    public class CrossPostServiceFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        public CrossPostServiceFactory(ILoggerFactory loggerFactory) =>
            _loggerFactory = loggerFactory;

        public ICrossPostService CreateTwitterService(
            string consumerKey,
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
                _loggerFactory.CreateLogger<TwitterCrosspostService>());

        public ICrossPostService CreateTelegramService(string token, string name) =>
            new TelegramCrosspostService(token, name, _loggerFactory.CreateLogger<TelegramCrosspostService>());

        public ICrossPostService CreateFacebookService(string token, string name) =>
            new FacebookCrosspostService(token, name, _loggerFactory.CreateLogger<FacebookCrosspostService>());

        public ICrossPostService CreateFakeService() =>
            new FakeCrosspostService(_loggerFactory.CreateLogger<FakeCrosspostService>());
    }
}