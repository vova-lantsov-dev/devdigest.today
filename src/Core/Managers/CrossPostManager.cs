using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DAL;
using Telegram.Bot.Types.Enums;

namespace Core.Managers
{
    public interface ICrossPostManager
    {
        IEnumerable<Channel> GetTelegramChannels();
        Task<bool> Send(int categoryId, string comment, string link);
        Task<bool> SendToTelegram(int categoryId, string comment, string link);
        Task<bool> SendToFacebook(int categoryId, string comment, string link);
    }

    public class CrossPostManager : ManagerBase, ICrossPostManager
    {
        private readonly DAL.DatabaseContext _database;

        public CrossPostManager(DatabaseContext database)
        {
            _database = database;
        }
        
        public IEnumerable<Channel> GetTelegramChannels()
        {
            return _database.Channel.ToList();
        }

        public async Task<bool> Send(int categoryId, string comment, string link)
        {
            var result = true;

            result &= await SendToTelegram(categoryId, comment, link);
            result &= await SendToFacebook(categoryId, comment, link);

            return result;
        }

        public async Task<bool> SendToTelegram(int categoryId, string comment, string link)
        {
            var channels = _database.Channel.Where(o => o.CategoryId == categoryId).ToList();

            var message = comment + Environment.NewLine + Environment.NewLine + link;

            foreach (var channel in channels)
            {
                var bot = new Telegram.Bot.TelegramBotClient(channel.Token);
                var result = await bot.SendTextMessageAsync(channel.Name, message, ParseMode.Default, false, false);
            }

            return true;
        }

        public async Task<bool> SendToFacebook(int categoryId, string comment, string link)
        {
            var pages = _database.FacebookPage.Where(o => o.CategoryId == categoryId).ToList();

            try
            {
                foreach (var page in pages)
                {
                    var facebook = new X.Web.Facebook(page.Token);
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
}