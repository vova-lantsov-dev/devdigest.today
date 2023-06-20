using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Core.Services.Posting;

public class TelegramPostingService : SocialNetworkPostingService
{
    private readonly ILogger _logger;
    private readonly string _token;
    private readonly string _name;

    public TelegramPostingService(string token, string name, ILogger<TelegramPostingService> logger) 
        : base(logger)
    {
        _logger = logger;
        _token = token;
        _name = name;
    }

    protected override async Task PostImplementation(string title, string body, Uri link)
    {
        var channelLink = $"https://t.me/{_name.Replace("@", "")}";
        var chatLink = "https://t.me/dotnet_chat";

        var sb = new StringBuilder();

        sb.Append($"{FormatMessage(title, body)}");
        sb.Append(Environment.NewLine);
        sb.Append(Environment.NewLine);
        sb.Append($"🔗 {link}");
        sb.Append(Environment.NewLine);
        sb.Append(Environment.NewLine);
        sb.Append($"👉🏻 <a href=\"{channelLink}\">Наш канал</a> | 💬 <a href=\"{chatLink}\">Наш чат</a>");


        var bot = new TelegramBotClient(_token);

        await bot.SendTextMessageAsync(_name, sb.ToString(), parseMode: ParseMode.Html);

        _logger.LogInformation($"Message was sent to Telegram channel `{_name}`: `{sb}`");
    }

    private static string FormatMessage(string title, string body)
    {
        var icon = GetIcon(body);
        
        if (!string.IsNullOrWhiteSpace(body))
        {
            return $"{icon} <b>{title}</b>{Environment.NewLine}{Environment.NewLine}{body}";
        }

        return $"{icon} {title}";
    }

    /// <summary>
    /// Chose right icon for the post
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    private static string GetIcon(string body)
    {
        if (body.Contains("devdigest.today/post"))
        {
            //We usually use /post links for videos
            
            return "🎥";
        }
        
        return "⚡️";
    }
}