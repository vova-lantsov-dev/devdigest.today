using System;

namespace Core;

public class PostUrlBuilder
{
    private readonly Settings _settings;

    public PostUrlBuilder(Settings settings) => _settings = settings;

    /// <summary>
    /// Build post url
    /// </summary>
    /// <param name="postId"></param>
    /// <returns></returns>
    public Uri Build(int postId) => new($"{_settings.WebSiteUrl}post/{postId}");

    /// <summary>
    /// Build url for redirecting to original web page
    /// </summary>
    /// <param name="postId"></param>
    /// <returns></returns>
    public Uri BuildRedirectUrl(int postId) => new($"{_settings.WebSiteUrl}goto/{postId}");
}