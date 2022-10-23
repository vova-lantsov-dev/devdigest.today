using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tweetinvi;
using Tweetinvi.Parameters;

namespace Core.Services.Posting;

public class TwitterPostingService : SocialNetworkPostingService
{
    private static readonly Semaphore Semaphore = new(1, 1);

    private readonly ILogger _logger;
    private readonly string _name;
    private readonly IReadOnlyCollection<string> _defaultTags;
    private readonly TwitterClient _client;

    private const int MaxTweetLength = 186;

    public TwitterPostingService(
        string consumerKey, 
        string consumerSecret,
        string accessToken,
        string accessSecret,
        string name,
        IReadOnlyCollection<string> defaultTags,
        ILogger<TwitterPostingService> logger) 
        : base(logger)
    {
        _logger = logger;
        _defaultTags = defaultTags;
        _name = name;
            
        _client = new TwitterClient(consumerKey, consumerSecret, accessToken, accessSecret);
    }

    protected override async Task PostImplementation(string title, string body, Uri link)
    {
        var tagLine = string.Join(" ", _defaultTags);
        var message = MergeMessage(title, body);
        var maxMessageLength = MaxTweetLength - tagLine.Length - 4;

        var sb = new StringBuilder();
        sb.AppendLine(Substring(message, maxMessageLength));
        sb.AppendLine(tagLine);
        sb.AppendLine();
        sb.AppendLine(link.ToString());

        var text = sb.ToString();

        try
        {
            Semaphore.WaitOne();

            await _client.Tweets.PublishTweetAsync(new PublishTweetParameters(text));

            _logger.LogInformation($"Message was sent to Twitter channel `{_name}`: `{text}`");
        }
        finally
        {
            Semaphore.Release();
        }
    }

    private static string Substring(string text, int length)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        return text.Length <= length ? text : $"{text.Substring(0, length - 4)}... ";
    }
}