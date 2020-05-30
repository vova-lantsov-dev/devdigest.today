using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using X.Web.Facebook;

namespace Core.Services.Crosspost
{
    public class FacebookCrosspostService : ICrossPostService
    {
        private readonly ILogger _logger;
        private readonly string _token;
        private readonly string _name;

        public FacebookCrosspostService(string token, string name, ILogger<FacebookCrosspostService> logger)
        {
            _logger = logger;
            _token = token;
            _name = name;
        }

        public async Task Send(string message, Uri link, IReadOnlyCollection<string> tags)
        {

            try
            {
                var facebook = new FacebookClient(_token);

                await facebook.PostOnWall(message, link.ToString());

                _logger.LogInformation($"Message was sent to Facebook page `{_name}`: `{message}` `{link}`");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during send message to Facebook: `{message}` `{link}`");
            }
        }
    }
}