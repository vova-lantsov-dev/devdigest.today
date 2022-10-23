using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Core.Services.Posting;

public class PostingServiceFactory
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly Settings _settings;

    public PostingServiceFactory(ILoggerFactory loggerFactory, Settings settings)
    {
        _loggerFactory = loggerFactory;
        _settings = settings;
    }

    public ISocialNetworkPostingService CreateTwitterService(
        string consumerKey,
        string consumerSecret,
        string accessToken,
        string accessTokenSecret,
        string name,
        IReadOnlyCollection<string> defaultTags) =>
        new TwitterPostingService(
            consumerKey,
            consumerSecret,
            accessToken,
            accessTokenSecret,
            name,
            defaultTags,
            _loggerFactory.CreateLogger<TwitterPostingService>());

    public ISocialNetworkPostingService CreateTelegramService(string name) =>
        new TelegramPostingService(_settings.TelegramToken, name, _loggerFactory.CreateLogger<TelegramPostingService>());

    public ISocialNetworkPostingService CreateFacebookService(string token, string name) =>
        new FacebookPostingService(token, name, _loggerFactory.CreateLogger<FacebookPostingService>());
        
    public ISocialNetworkPostingService CreateSlackService(string webHookUr) =>
        new SlackPostingService(webHookUr, _loggerFactory.CreateLogger<SlackPostingService>());

    public ISocialNetworkPostingService CreateFakeService() =>
        new FakePostingService(_loggerFactory.CreateLogger<FakePostingService>());
}