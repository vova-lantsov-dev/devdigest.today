using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Logging;
using Core.Repositories;
using DAL;
using Serilog.Events;
using Tweetinvi;

namespace Core.Managers.Crosspost
{
    public class TwitterCrosspostManager : ICrossPostManager
    {
        private static readonly Semaphore _semaphore = new Semaphore(1, 1);
        
        private readonly IPublicationRepository _publicationRepository;
        private readonly ISocialRepository _socialRepository;
        private readonly ILogger _logger;
        
        private const int MaxTweetLength = 280;

        public TwitterCrosspostManager(
            IPublicationRepository publicationRepository,
            ISocialRepository socialRepository,
            ILogger logger)
        {
            _publicationRepository = publicationRepository;
            _logger = logger;
            _socialRepository = socialRepository;
        }

        public async Task Send(int categoryId, string comment, string link)
        {
            var accounts = await _socialRepository.GetTwitterAccountsChannels(categoryId);
            var categoryName = await _publicationRepository.GetCategoryName(categoryId);

            var tag = $" #devdigest {ConvertToHashTag(categoryName)} ";
            var maxMessageLength = MaxTweetLength - link.Length - tag.Length;
            var message = Substring(comment, maxMessageLength);

            var text = $"{message}{tag}{link}";

            foreach (var account in accounts)
            {
                try
                {
                    _semaphore.WaitOne();
                    
                    Auth.SetUserCredentials(
                        account.ConsumerKey,
                        account.ConsumerSecret,
                        account.AccessToken,
                        account.AccessTokenSecret);

                    Tweet.PublishTweet(text);

                    _logger.Write(LogEventLevel.Information, $"Message was sent to Twitter channel `{account.Name}`: `{comment}` `{link}` Category: `{categoryId}`");
                }
                catch (Exception ex)
                {
                    _logger.Write(LogEventLevel.Error, "Error in TwitterCrosspostManager.Send", ex);
                }
                finally
                {
                    _semaphore.Release();    
                }
            }
        }

        private static string ConvertToHashTag(string text) =>
            "#" + text
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

        public async Task<IReadOnlyCollection<TwitterAccount>> GetAccounts()
        {
            return await _socialRepository.GetTwitterAccounts();
        }
    }
}