using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Core.Logging;
using DAL;
using Serilog.Events;
using Telegram.Bot;

namespace Core.Services.Crosspost
{
    public class TelegramCrosspostService : ICrossPostService
    {
        private readonly Core.Logging.ILogger _logger;
        private readonly string _token;
        private readonly string _name;

        public TelegramCrosspostService(string token, string name, ILogger logger)
        {
            _logger = logger;
            _token = token;
            _name = name;
        }

        public async Task Send(string message, Uri link, IReadOnlyCollection<string> tags)
        {
            var sb = new StringBuilder();

            sb.Append(message);
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append(link);
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append(string.Join(" ", tags));

            try
            {

                var bot = new TelegramBotClient(_token);

                await bot.SendTextMessageAsync(_name, sb.ToString());

                _logger.Write(LogEventLevel.Information, $"Message was sent to Telegram channel `{_name}`: `{sb}`");

            }
            catch (Exception ex)
            {
                _logger.Write(LogEventLevel.Error, $"Error during send message to Telegram: `{sb}`", ex);
            }
        }
    }
}