namespace WebSite.ViewModels;

public record ContentPageViewModel
{
    public string Title { get; init; }
    public string Description { get; init; }
    public string Keywords { get; init; }
    public Uri Url { get; init; }
    public Uri Image { get; init; }
    public string Content { get; init; }
}