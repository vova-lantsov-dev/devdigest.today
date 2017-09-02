using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Core
{
    public enum VideoSource
    {
        YouTube,
        Channel9
    }

    public class YouTube
    {
        
        public static bool IsYouTube(string url)
        {
            return url.ToLower().Contains("youtube.com");
        }

        public static string GetEmbededUrl(string videoId)
        {
            return $"https://www.youtube.com/embed/{videoId}?rel=0&amp;showinfo=0";
        }

        public static string GetVideoId(string url)
        {
            var uri = new Uri(url);
            var query = HttpUtility.ParseQueryString(uri.Query);

            if (query.AllKeys.Contains("v"))
            {
                return query["v"];
            }
            else
            {
                return uri.Segments.Last();
            }
        }
    }
}