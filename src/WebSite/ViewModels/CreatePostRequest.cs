namespace WebSite.ViewModels;

public record CreatePostRequest
{
    public Uri Link { get; set; } = null;
    public Guid Key { get; set; } = Guid.Empty;
    public int CategoryId { get; set; } = -1;
    public string Title { get; set; } = "";
    public string Comment { get; set; } = "";
    public string TitleUa { get; set; } = "";
    public string CommentUa { get; set; } = "";
}