using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Core.Services;

/// <summary>
/// Represent services for posting information to social networks
/// </summary>
public interface ISocialNetworkPostingService
{
    /// <summary>
    /// Post message to social network
    /// </summary>
    /// <param name="title">Message title</param>
    /// <param name="body">Message body</param>
    /// <param name="link"></param>
    /// <returns></returns>
    Task Post(string title, string body, Uri link);
}

public abstract class SocialNetworkPostingService : ISocialNetworkPostingService
{
    private readonly ILogger _logger;

    protected SocialNetworkPostingService(ILogger logger) => _logger = logger;

    public async Task Post(string title, string body, Uri link)
    {
        try
        {
            await PostImplementation(title, body, link);
        }
        catch (Exception ex)
        {
            //Error in one posting service should newer broke whole workflow, so we only collect errors 
            
            _logger.LogError(ex, ex.Message);
        }
    }

    protected abstract Task PostImplementation(string title, string body, Uri link);
    
    /// <summary>
    /// Some social networks do not support formatting title and body,
    /// so we merge title and body to single message   
    /// </summary>
    /// <param name="title"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    protected static string MergeMessage(string title, string body)
    {
        if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(body))
        {
            return string.Empty;            
        }

        if (!string.IsNullOrWhiteSpace(title))
        {
            var symbols = new[] { ".", "?", "!", "..." };
            var point = symbols.Any(title.EndsWith) ? string.Empty : ".";
            
            return $"{title.Trim()}{point} {body}".Trim();
        }

        return body ?? string.Empty;
    }
}