using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Core
{
    public enum PlayerSoure
    {
        YouTube,
        Channel9,
        SlideShare
    }

    public class EmbededPlayer
    {
        private Uri _uri;

        public PlayerSoure Source { get; private set; }

        public EmbededPlayer(string url)
        {
            var source = GetPlayerSoure(url);

            if (!source.HasValue)
            {
                throw new Exception("Incorrect player source");
            }

            Source = source.Value;
            _uri = new Uri(url);
        }

        public static PlayerSoure? GetPlayerSoure(string url)
        {
            var uri = new Uri(url);
            var host = uri.Host.ToLower();

            switch (host)
            {
                case "youtube.com":
                case "www.youtube.com":
                    return PlayerSoure.YouTube;
                case "channel9.msdn.com":
                case "www.channel9.msdn.com":
                    return PlayerSoure.Channel9;
                case "slideshare.net":
                case "www.slideshare.net":
                    //return PlayerSoure.SlideShare;  //Will be implementad later
                default:
                    return null;
            }
        }

        /// <summary>
        /// Return link for iframe with embeded player
        /// </summary>
        /// <returns></returns>
        public string Render()
        {
            switch (Source)
            {
                case PlayerSoure.Channel9:
                    return RenderChannel9(_uri);
                case PlayerSoure.YouTube:
                    return RenderYouTube(_uri);
                case PlayerSoure.SlideShare:
                    return RenderSlideShare(_uri);
                default:
                    throw new Exception();
            }
        }

        private string RenderSlideShare(Uri uri)
        {
            throw new NotImplementedException();
        }

        private string RenderYouTube(Uri uri)
        {
            var query = HttpUtility.ParseQueryString(uri.Query);

            var videoId = query.AllKeys.Contains("v")
                ? query["v"]
                : uri.Segments.Last();

            return $"https://www.youtube.com/embed/{videoId}?rel=0&amp;showinfo=0";
        }

        private string RenderChannel9(Uri uri)
        {
            var url = uri.ToString();

            url = url[url.Length - 1] == '/'
                ? url + "player"
                : url + "/player";

            return url;
        }
    }
}