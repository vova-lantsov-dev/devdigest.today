using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Logging;
using DAL;
using Telegram.Bot;
using X.Web;

namespace Core.Managers
{
    public interface ICrossPostManager : IManager
    {
        Task<bool> Send(int categoryId, string comment, string link);
    }

    public class FacebookCrosspostManager : IManager, ICrossPostManager
    {
        private readonly ILogger _logger;
        private readonly DatabaseContext _database;

        public FacebookCrosspostManager(DatabaseContext database, ILogger logger)
        {
            _database = database;
            _logger = logger;
        }

        public async Task<bool> Send(int categoryId, string comment, string link)
        {
            var pages = _database.FacebookPage.Where(o => o.CategoryId == categoryId).ToList();

            try
            {
                foreach (var page in pages)
                {
                    var facebook = new Facebook(page.Token);
                    await facebook.PostOnWall(comment, link);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

    public class TelegramCrosspostManager : IManager, ICrossPostManager
    {
        private readonly ILogger _logger;
        private readonly DatabaseContext _database;

        public TelegramCrosspostManager(DatabaseContext database, ILogger logger)
        {
            _database = database;
            _logger = logger;
        }

        public IEnumerable<Channel> GetTelegramChannels() => _database.Channel.ToList();

        public async Task<bool> Send(int categoryId, string comment, string link)
        {
            var channels = _database.Channel.Where(o => o.CategoryId == categoryId).ToList();

            var message = comment + Environment.NewLine + Environment.NewLine + link;

            foreach (var channel in channels)
            {
                var bot = new TelegramBotClient(channel.Token);
                await bot.SendTextMessageAsync(channel.Name, message);
            }

            return true;
        }
    }
}