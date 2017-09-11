using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;

namespace Core.Managers
{
    public class TelegramManager : ManagerBase
    {
        public TelegramManager(string connectionString)
            : base(connectionString)
        {
        }

        public async Task<bool> Send(int categoryId, string comment, string link)
        {
            var channels = _database.Channel.Where(o => o.CategoryId == categoryId).ToList();

            foreach (var channel in channels)
            {
                var message = comment + Environment.NewLine + Environment.NewLine + link;

                var bot = new Telegram.Bot.TelegramBotClient(channel.Token);
                var result = await bot.SendTextMessageAsync(channel.Name, message, ParseMode.Default, false, false);
            }

            return true;
        }
    }
}