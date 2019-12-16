using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Core.Repositories;
using DAL;
using Microsoft.Extensions.Logging;
using Serilog.Events;
using Telegram.Bot;
using ILogger = Core.Logging.ILogger;

namespace Core.Services.Crosspost
{
    public class TelegramCrosspostService : ICrossPostService
    {
        private readonly ILogger _logger;
        private readonly ISocialRepository _socialRepository;

        public TelegramCrosspostService(ISocialRepository socialRepository, ILogger logger)
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
            var channels = await _socialRepository.GetTelegramChannels(categoryId);

            var sb = new StringBuilder();
            
            sb.Append(message);
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append(link);
            sb.Append(Environment.NewLine);
            sb.Append(string.Join(", ", channelTags));
            
            try
            {
                foreach (var channel in channels)
                {
                    var bot = new TelegramBotClient(channel.Token);
                    
                    await bot.SendTextMessageAsync(channel.Name, sb.ToString());
                    
                    _logger.Write(LogEventLevel.Information, $"Message was sent to Telegram channel `{channel.Name}`: `{message}` `{link}` Category: `{categoryId}`");
                }
            }
            catch (Exception ex)
            {
                _logger.Write(LogEventLevel.Error, $"Error during send message to Telegram: `{message}` `{link}` Category: `{categoryId}`", ex);
            }
        }

        public async Task<IReadOnlyCollection<Channel>> GetChannels() => await _socialRepository.GetTelegramChannels();
    }
}