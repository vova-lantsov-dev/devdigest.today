using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Slack.Webhooks;
using Slack.Webhooks.Blocks;
using Slack.Webhooks.Elements;

namespace Core.Services.Posting;

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
                IconEmoji = Emoji.Newspaper,
                Text = message,
                Blocks = new List<Block>
                {
                    new Header()
                    {
                        Text = new TextObject($"Новости")
                    },
                    new Section
                    {
                        Text = new TextObject($":newspaper: {message}")
                        {
                            Emoji = true,
                            Type = TextObject.TextType.PlainText
                        }
                    },
                    new Divider(),
                    new Section
                    {
                        Text = new TextObject($"Узнать подробности: <{link}>")
                        {
                            Type = TextObject.TextType.Markdown, Verbatim = false
                        }
                    }
                },
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