namespace Core.Models;

public record SocialAccount
{
    public string Title { get; init; }
    public string Description { get; init; }
    public string Logo { get; init; }
    public string Url { get; init; }
}