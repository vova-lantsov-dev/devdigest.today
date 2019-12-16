using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Logging;
using Core.Repositories;
using DAL;
using Serilog.Events;
using Tweetinvi;
using Tweetinvi.Parameters;

namespace Core.Services.Crosspost
{
    public class TwitterCrosspostService : ICrossPostService
    {
        private static readonly Semaphore _semaphore = new Semaphore(1, 1);
        
        private readonly ISocialRepository _socialRepository;
        private readonly ILogger _logger;
        
        private const int MaxTweetLength = 277;

        public TwitterCrosspostService(ISocialRepository socialRepository, ILogger logger)
        {
            _logger = logger;
            _socialRepository = socialRepository;
        }

        public async Task Send(
            int categoryId, 
            string message, 
            string link,  
            IReadOnlyCollection<string> channelTags,
            IReadOnlyCollection<string> commonTags)
        {
            var accounts = await _socialRepository.GetTwitterAccountsChannels(categoryId);

            var tags = string.Join(" ", channelTags.Union(commonTags)
                .Where(o => !string.IsNullOrWhiteSpace(o))
                .Select(o => o.Trim().ToLower())
                .Distinct()
                .ToImmutableList());
            
            var maxMessageLength = MaxTweetLength - tags.Length;
            
            var text = $"{Substring(message, maxMessageLength)} {tags} {link}";

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

                    _logger.Write(LogEventLevel.Information, $"Message was sent to Twitter channel `{account.Name}`: `{text}` Category: `{categoryId}`");
                }
                catch (Exception ex)
                {
                    _logger.Write(LogEventLevel.Error, "Error in TwitterCrosspostService.Send", ex);
                }
                finally
                {
                    _semaphore.Release();    
                }
            }
        }


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