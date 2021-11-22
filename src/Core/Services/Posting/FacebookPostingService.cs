using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using X.Web.Facebook;

namespace Core.Services.Posting;

public class FacebookPostingService : IPostingService
{
    private readonly ILogger _logger;
    private readonly string _token;
    private readonly string _name;

    public FacebookPostingService(string token, string name, ILogger<FacebookPostingService> logger)
    {
        _logger = logger;
        _token = token;
        _name = name;
    }

    public async Task Send(string message, Uri link, IReadOnlyCollection<string> tags)
    {

        try
        {
            var facebook = new FacebookClient(_token);

            await facebook.PostOnWall(message, link.ToString());

            _logger.LogInformation($"Message was sent to Facebook page `{_name}`: `{message}` `{link}`");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error during send message to Facebook: `{message}` `{link}`");
        }
    }
}