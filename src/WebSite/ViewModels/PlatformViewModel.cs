using Core.Models;

namespace WebSite.ViewModels;

public record PlatformViewModel
{
    public IReadOnlyCollection<SocialAccount> Telegram { get; init; }
    public IReadOnlyCollection<SocialAccount> Facebook { get; init; }
    public IReadOnlyCollection<SocialAccount> Twitter { get; init; }
}