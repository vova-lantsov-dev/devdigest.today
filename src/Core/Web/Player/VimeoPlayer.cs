using System;
using System.Threading.Tasks;

namespace Core.Web.Player;

public class VimeoPlayer : IPlayer
{
    public Task<string> GetEmbeddedPlayerUrl(Uri uri) => throw new NotImplementedException();
}