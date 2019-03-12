using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Logging;
using DAL;
using Telegram.Bot;
using X.Web.Facebook;

namespace Core.Managers
{
    public interface ICrossPostManager : IManager
    {
        Task<bool> Send(int categoryId, string comment, string link);
    }

    public class FacebookCrosspostManager : IManager, ICrossPostManager
    {
        private readonly IRepository _repository;

        private readonly ILogger _logger;

        public FacebookCrosspostManager(IRepository repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<bool> Send(int categoryId, string comment, string link)
        {
            var pages = await _repository.GetFacebookPages(categoryId); 

            try
            {
                foreach (var page in pages)
                {
                    var facebook = new FacebookClient(page.Token);
                    await facebook.PostOnWall(comment, link);

                    _logger.Write(LogLevel.Info, $"Message was sent to Facebook page `{page.Name}`: `{comment}` `{link}` Category: `{categoryId}`");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Write(LogLevel.Error, $"Error during send message to Facebook: `{comment}` `{link}` Category: `{categoryId}`", ex);
                return false;
            }
        }

        public async Task<IReadOnlyCollection<DAL.FacebookPage>> GetPages() => await _repository.GetFacebookPages();
    }

    public class TelegramCrosspostManager : IManager, ICrossPostManager
    {
        private readonly ILogger _logger;
        private readonly IRepository _repository;

        public TelegramCrosspostManager(IRepository repository, ILogger logger)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<bool> Send(int categoryId, string comment, string link)
        {
            var channels = await _repository.GetTelegramChannels(categoryId);

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

        public async Task<IReadOnlyCollection<Channel>> GetChannels() => await _repository.GetTelegramChannels();
    }

    public class FakeCrosspostManager : IManager, ICrossPostManager
    {
        private readonly ILogger _logger;

        public FakeCrosspostManager(ILogger logger) => _logger = logger;

        public async Task<bool> Send(int categoryId, string comment, string link)
        {
            _logger.Write(LogLevel.Info, $"{comment} {link} {categoryId}");
            
            return await Task.FromResult(true);
        }
    }
}