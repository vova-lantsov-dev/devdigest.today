using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface ICrossPostService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="message"></param>
        /// <param name="link"></param>
        /// <param name="channelTags">
        /// Tags which should be used to define channel name and project name.
        /// For example: `#devdigest #azure` tags in twitter channel, because
        /// twitter account aggregate all messages from different channels. 
        /// </param>
        /// <param name="commonTags">
        /// Tags which should be added to all all posts 
        /// </param>
        /// <returns></returns>
        Task Send(
            int categoryId, 
            string message, 
            string link, 
            IReadOnlyCollection<string> channelTags,
            IReadOnlyCollection<string> commonTags);
    }
}