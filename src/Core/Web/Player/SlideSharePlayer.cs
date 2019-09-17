using System;
using System.Threading.Tasks;

namespace Core.Web.Player
{
    public class SlideSharePlayer : IPlayer
    {
        public Task<string> GetEmbeddedPlayerUrl(Uri uri) => throw new NotImplementedException();
    }
}