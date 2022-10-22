using X.Text;

namespace WebSite.ViewModels;

public record PostViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public DateTime DateTime { get; set; }
    public string EmbeddedPlayerCode { get; set; }
    public Uri Url { get; set; }
    public Uri ShareUrl { get; set; }
    public string Keywords => TextHelper.GetKeywords(Description, 10);
    public CategoryViewModel Category { get; set; }
    public int ViewsCount { get; set; }

    
    public string GetShortDescription() => TextHelper.Substring(Description, 256, "...");
}