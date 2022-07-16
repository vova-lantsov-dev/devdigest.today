namespace WebSite.ViewModels;

public record NewPostRequest
{
    public Uri Link { get; set; } = null;
    public Guid Key { get; set; } = Guid.Empty;
    public int CategoryId { get; set; } = -1;
    public string Comment { get; set; } = "";
    public string Tags { get; set; } = "";
}