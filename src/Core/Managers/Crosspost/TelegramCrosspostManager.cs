using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Logging;
using Core.Repositories;
using DAL;
using Telegram.Bot;

namespace Core.Managers.Crosspost
{
    public class TelegramCrosspostManager : ICrossPostManager
    {
        private readonly ILogger _logger;
        private readonly ISocialRepository _socialRepository;

        public TelegramCrosspostManager(ISocialRepository socialRepository, ILogger logger)
        {
            _logger = logger;
            _socialRepository = socialRepository;
        }

        public async Task<bool> Send(int categoryId, string comment, string link)
        {
            var channels = await _socialRepository.GetTelegramChannels(categoryId);

            var message = comment + Environment.NewLine + Environment.NewLine + link;
            
            try
            {
                foreach (var channel in channels)
                {
                    var bot = new TelegramBotClient(channel.Token);
                    
                    await bot.SendTextMessageAsync(channel.Name, message);
                    
                    _logger.Write(LogLevel.Info, $"Message was sent to Telegram channel `{channel.Name}`: `{comment}` `{link}` Category: `{categoryId}`");
                }
            }
            catch (Exception ex)
            {
                _logger.Write(LogLevel.Error, $"Error during send message to Telegram: `{comment}` `{link}` Category: `{categoryId}`", ex);
                return false;
            }

            return true;
        }

        public async Task<IReadOnlyCollection<Channel>> GetChannels() => await _socialRepository.GetTelegramChannels();
    }
}