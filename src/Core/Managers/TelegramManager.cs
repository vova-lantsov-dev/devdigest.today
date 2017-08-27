using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;

namespace Core.Managers
{
    public class TelegramManager : IManager
    {
        private string _token;
        private string _channelId;

        public TelegramManager(string token, string channelId)
        {
            _token = token;
            _channelId = channelId;
        }

        public async Task<bool> Send(string comment, string link)
        {
            var message = comment + Environment.NewLine + Environment.NewLine + link;

            var bot = new Telegram.Bot.TelegramBotClient(_token);
            var result = await bot.SendTextMessageAsync(_channelId, message, ParseMode.Default, false, false);
            return true;
        }
    }
}