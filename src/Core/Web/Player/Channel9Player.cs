using System;
using System.Threading.Tasks;

namespace Core.Web.Player
{
    public class Channel9Player : IPlayer
    {
        public Task<string> GetEmbeddedPlayerUrl(Uri uri)
        {
            var url = uri.ToString();

            url = url[^1] == '/' ? url + "player" : url + "/player";

            return Task.FromResult(url);
        }
    }
}