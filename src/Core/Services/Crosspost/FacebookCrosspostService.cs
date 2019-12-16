using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Logging;
using Core.Repositories;
using Serilog.Events;
using X.Web.Facebook;

namespace Core.Services.Crosspost
{
    public class FacebookCrosspostService : ICrossPostService
    {
        private readonly ISocialRepository _repository;
        private readonly ILogger _logger;

        public FacebookCrosspostService(ISocialRepository repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Send(
            int categoryId, 
            string message, 
            string link,
            IReadOnlyCollection<string> channelTags, 
            IReadOnlyCollection<string> tags)
        {
            var pages = await _repository.GetFacebookPages(categoryId); 

            try
            {
                foreach (var page in pages)
                {
                    var facebook = new FacebookClient(page.Token);
                    
                    await facebook.PostOnWall(message, link);

                    _logger.Write(LogEventLevel.Information, $"Message was sent to Facebook page `{page.Name}`: `{message}` `{link}` Category: `{categoryId}`");
                }

            }
            catch (Exception ex)
            {
                _logger.Write(LogEventLevel.Error, $"Error during send message to Facebook: `{message}` `{link}` Category: `{categoryId}`", ex);
            }
        }

        public async Task<IReadOnlyCollection<DAL.FacebookPage>> GetPages() => await _repository.GetFacebookPages();
    }
}