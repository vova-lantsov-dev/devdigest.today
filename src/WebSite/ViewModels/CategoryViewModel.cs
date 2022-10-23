namespace WebSite.ViewModels;

public record CategoryViewModel
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string CssClass => $"category-{Id}";
}