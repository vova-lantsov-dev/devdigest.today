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

    public IPostingService CreateTwitterService(
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

    public IPostingService CreateTelegramService(string name) =>
        new TelegramPostingService(_settings.TelegramToken, name, _loggerFactory.CreateLogger<TelegramPostingService>());

    public IPostingService CreateFacebookService(string token, string name) =>
        new FacebookPostingService(token, name, _loggerFactory.CreateLogger<FacebookPostingService>());
        
    public IPostingService CreateSlackService(string webHookUr) =>
        new SlackPostingService(webHookUr, _loggerFactory.CreateLogger<SlackPostingService>());

    public IPostingService CreateFakeService() =>
        new FakePostingService(_loggerFactory.CreateLogger<FakePostingService>());
}