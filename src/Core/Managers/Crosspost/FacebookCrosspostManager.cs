using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Logging;
using Core.Repositories;
using X.Web.Facebook;

namespace Core.Managers.Crosspost
{
    public class FacebookCrosspostManager : ICrossPostManager
    {
        private readonly ISocialRepository _repository;
        private readonly ILogger _logger;

        public FacebookCrosspostManager(ISocialRepository repository, ILogger logger)
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
}