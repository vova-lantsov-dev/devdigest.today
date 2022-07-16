using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Core.Services.Posting;

public class TelegramPostingService : IPostingService
{
    private readonly ILogger _logger;
    private readonly string _token;
    private readonly string _name;

    public TelegramPostingService(string token, string name, ILogger<TelegramPostingService> logger)
    {
        _logger = logger;
        _token = token;
        _name = name;
    }

    public async Task Send(string title, string body, Uri link, IReadOnlyCollection<string> tags)
    {
        var channelLink = $"https://t.me/{_name.Replace("@", "")}";
        
        var sb = new StringBuilder();

        sb.Append($"{FormatMessage(title, body)}");
        sb.Append(Environment.NewLine);
        sb.Append(Environment.NewLine);
        sb.Append($"🔗 {link}");
        sb.Append(Environment.NewLine);
        sb.Append(Environment.NewLine);
        sb.Append($"👉🏻 <a href=\"{channelLink}\">Подписаться на канал</a>" );

        try
        {
            var bot = new TelegramBotClient(_token);

            await bot.SendTextMessageAsync(_name, sb.ToString(), ParseMode.Html);

            _logger.LogInformation($"Message was sent to Telegram channel `{_name}`: `{sb}`");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error during send message to Telegram: `{sb}`");
        }
    }

    private static string FormatMessage(string title,  string body)
    {
        if (!string.IsNullOrWhiteSpace(body))
        {
            return $"⚡ <b>{title}</b>. {Environment.NewLine}{Environment.NewLine}{body}";
        }
        
        return $"⚡ <b>{title}</b>";
    }

    private static string FormatTags(IReadOnlyCollection<string> tags)
    {
        var list = tags.Select(o => $"#{o.Replace("#", "")}").ToList();
        
        return string.Join(" ", list);
    }
}