using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using X.Web.Facebook;

namespace Core.Services.Posting;

public class FacebookPostingService : SocialNetworkPostingService
{
    private readonly ILogger _logger;
    private readonly string _token;
    private readonly string _name;

    public FacebookPostingService(string token, string name, ILogger<FacebookPostingService> logger) 
        : base(logger)
    {
        _logger = logger;
        _token = token;
        _name = name;
    }

    protected override async Task PostImplementation(string title, string body, Uri link)
    {
        var message = MergeMessage(title, body);
        var facebook = new FacebookClient(_token);

        await facebook.PostOnWall(message, link.ToString());

        _logger.LogInformation($"Message was sent to Facebook page `{_name}`: `{message}` `{link}`");
    }
}