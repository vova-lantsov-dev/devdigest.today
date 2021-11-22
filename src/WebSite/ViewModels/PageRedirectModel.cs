using System;

namespace WebSite.ViewModels;

public record PageRedirectModel
{
    public string Title { get; init; }
    public Uri Image { get; init; }
    public string Description { get; init; }
    public string Keywords { get; init; }
    public Uri Url { get; init; }
    public string Content { get; init; }
}