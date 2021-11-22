using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Core.Web.Player;

public class YouTubePlayer : IPlayer
{
    public Task<string> GetEmbeddedPlayerUrl(Uri uri)
    {
        var query = HttpUtility.ParseQueryString(uri.Query);

        var videoId = query.AllKeys.Contains("v")
            ? query["v"]
            : uri.Segments.Last();

        return Task.FromResult($"https://www.youtube.com/embed/{videoId}?rel=0&amp;showinfo=0");
    }
}