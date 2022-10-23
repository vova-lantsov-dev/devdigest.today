namespace WebSite.ViewModels;

public record ErrorViewModel
{
    public string RequestId { get; init; }
    public int StatusCode { get; init; }
    public string Description { get; init; }
}