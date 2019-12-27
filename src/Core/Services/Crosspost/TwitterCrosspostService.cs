using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Logging;
using Core.Repositories;
using DAL;
using Serilog.Events;
using Tweetinvi;

namespace Core.Services.Crosspost
{
    public class TwitterCrosspostService : ICrossPostService
    {
        private static readonly Semaphore _semaphore = new Semaphore(1, 1);
        
        private readonly ISocialRepository _socialRepository;
        private readonly IPublicationRepository _publicationRepository;
        private readonly ILogger _logger;
        
        private const int MaxTweetLength = 186;

        public TwitterCrosspostService(
            ISocialRepository socialRepository, 
            IPublicationRepository publicationRepository,
            ILogger logger)
        {
            _logger = logger;
            _publicationRepository = publicationRepository;
            _socialRepository = socialRepository;
        }

        public async Task Send(int categoryId,
            string message,
            Uri link,
            [NotNull] IReadOnlyCollection<string> tags)
        {
            var accounts = await _socialRepository.GetTwitterAccountsChannels(categoryId);
           
            var text = await BuildMessage(categoryId, message, link, tags);

            foreach (var account in accounts)
            {
                try
                {
                    _semaphore.WaitOne();

                    var credentials = Auth.SetUserCredentials(
                        account.ConsumerKey,
                        account.ConsumerSecret,
                        account.AccessToken,
                        account.AccessTokenSecret);

                    var publishTweetParameters = Tweet.CreatePublishTweetParameters(text);
                    var tweet = Tweet.PublishTweet(publishTweetParameters);

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

        private async Task<string> BuildMessage(int categoryId, string message, Uri url, IReadOnlyCollection<string> tags)
        {
            var categoryTags = (await _publicationRepository.GetCategoryTags(categoryId)).ToList();
            var tagLine = string.Join(" ", categoryTags.Union(tags).Distinct().ToImmutableList());

            var maxMessageLength = MaxTweetLength - tagLine.Length;

            return $"{Substring(message, maxMessageLength)} {tagLine} {url}";
        }

        private static string Substring(string text, int length)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            return text.Length <= length ? text : $"{text.Substring(0, length - 4)}... ";
        }

        public async Task<IReadOnlyCollection<TwitterAccount>> GetAccounts() => await _socialRepository.GetTwitterAccounts();
    }
}