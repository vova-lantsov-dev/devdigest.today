using System;
using System.Threading.Tasks;
using Core.Logging;
using Core.Repositories;
using Tweetinvi;

namespace Core.Managers.Crosspost
{
    public class TwitterCrosspostManager : ICrossPostManager
    {
        private readonly IPublicationRepository _publicationRepository;
        private readonly ILogger _logger;
        
        private const int MaxTweetLength = 280;

        public TwitterCrosspostManager(
            string consumerKey, 
            string consumerSecret, 
            string accessToken,
            string accessTokenSecret,
            IPublicationRepository publicationRepository,
            ILogger logger)
        {
            _publicationRepository = publicationRepository;
            _logger = logger;
            
            Auth.SetUserCredentials(consumerKey, consumerSecret, accessToken, accessTokenSecret);
        }

        public async Task<bool> Send(int categoryId, string comment, string link)
        {
            try
            {
                string categoryName = await _publicationRepository.GetCategoryName(categoryId);
                var tag = $" #devdigest {ConvertToHashTag(categoryName)} ";
                var maxMessageLength = MaxTweetLength - link.Length - tag.Length;
                var message = Substring(comment, maxMessageLength);

                var text = $"{message}{tag}{link}";

                Tweet.PublishTweet(text);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Write(LogLevel.Error, "Error in TwitterCrosspostManager.Send", ex);
                return false;
            }
        }

        private static string ConvertToHashTag(string text) =>
            text
                .Replace(".", "")
                .Replace("#", "")
                .Replace(" ", "_")
                .ToLower();

        private static string Substring(string text, int length)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            return text.Length <= length ? text : $"{text.Substring(0, length - 4)}... ";
        }
    }
}