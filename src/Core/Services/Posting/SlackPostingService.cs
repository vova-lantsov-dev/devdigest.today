using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Slack.Webhooks;

namespace Core.Services.Posting
{
    public class SlackPostingService: IPostingService
    {
        private readonly ILogger<SlackPostingService> _logger;
        private ISlackClient _client;

        public SlackPostingService(string webHookUrl, ILogger<SlackPostingService> logger)
        {
            _logger = logger;
            _client = new SlackClient(webHookUrl);
        }
        
        public async Task Send(string message, Uri link, IReadOnlyCollection<string> tags)
        {
            try
            {
                var slackMessage = new SlackMessage
                {
                    Channel = "#general",
                    Text = $"{message}\n {link}",
                    IconEmoji = Emoji.Newspaper
                };

                await _client.PostAsync(slackMessage);
                
                _logger.LogInformation($"Message was sent to Slack");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during send message to Slack");
            }
        }
    }
}