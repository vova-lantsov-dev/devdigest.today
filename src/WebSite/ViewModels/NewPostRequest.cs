namespace WebSite.ViewModels;

public record NewPostRequest
{
    public Uri Link { get; init; } = null;
    public Guid Key { get; init; } = Guid.Empty;
    public int CategoryId { get; init; } = -1;
    public string Comment { get; init; } = "";
    public string Tags { get; init; } = "";
}