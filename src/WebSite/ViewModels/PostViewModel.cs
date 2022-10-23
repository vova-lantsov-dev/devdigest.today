using X.Text;

namespace WebSite.ViewModels;

public record PostViewModel
{
    public int Id { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public string Image { get; init; }
    public DateTime DateTime { get; init; }
    public string EmbeddedPlayerCode { get; init; }
    public Uri Url { get; init; }
    public Uri ShareUrl { get; init; }
    public string Keywords => TextHelper.GetKeywords(Description, 10);
    public CategoryViewModel Category { get; init; }
    public int ViewsCount { get; init; }

    
    public string GetShortDescription() => TextHelper.Substring(Description, 256, "...");
}