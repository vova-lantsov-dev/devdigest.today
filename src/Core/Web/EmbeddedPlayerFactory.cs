using System;
using System.Linq;
using System.Web;
using Core.Web.Player;

namespace Core.Web
{
    public static class EmbeddedPlayerFactory
    {
        /// <summary>
        /// Create a player which corresponding to the link source
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static IPlayer CreatePlayer(Uri url)
        {
            var host = url.Host.ToLower().Replace("www.", "");

            switch (host)
            {
                case "youtube.com": return new YouTubePlayer();
                case "channel9.msdn.com": return new Channel9Player();
                //case "slideshare.net": return new SlideSharePlayer();
                //case "vimeo.net": return new VimeoPlayer();
                default: return null;
            }
        }
    }
}