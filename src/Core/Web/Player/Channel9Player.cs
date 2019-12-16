using System;
using System.Threading.Tasks;

namespace Core.Web.Player
{
    public class Channel9Player : IPlayer
    {
        public async Task<string> GetEmbeddedPlayerUrl(Uri uri)
        {
            var url = uri.ToString();

            url = url[^1] == '/' ? url + "player" : url + "/player";

            return url;
        }
    }
}