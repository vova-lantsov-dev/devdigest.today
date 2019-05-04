using System;
using System.Threading.Tasks;

namespace Core.Web
{
    public interface IPlayer
    {
        /// <summary>
        /// Return url which can be used inserted into iframe which will load player
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        Task<string> GetEmbeddedPlayerUrl(Uri uri);
    }
}