using System.Collections.Generic;
using Core.Models;

namespace WebSite.ViewModels;

public class PlatformViewModel
{
    public IReadOnlyCollection<SocialAccount> Telegram { get; set; }
    public IReadOnlyCollection<SocialAccount> Facebook { get; set; }
    public IReadOnlyCollection<SocialAccount> Twitter { get; set; }
}