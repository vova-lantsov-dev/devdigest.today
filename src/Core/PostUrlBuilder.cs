using System;

namespace Core;

public class PostUrlBuilder
{
    private readonly Settings _settings;

    public PostUrlBuilder(Settings settings) => _settings = settings;

    public Uri Build(int postId) => new($"{_settings.WebSiteUrl}post/{postId}");

    public Uri BuildRedirectUrl(int postId) => new($"{_settings.WebSiteUrl}goto/{postId}");
}