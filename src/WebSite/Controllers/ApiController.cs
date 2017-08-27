
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core;
using Core.Managers;
using Core.ViewModels;
using Core.VIewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using X.Web.MetaExtractor;

namespace WebSite.Controllers
{
    public class ApiController : Controller
    {
        private readonly PublicationManager _publicationManager;
        private readonly UserManager _userManager;
        private readonly TelegramManager _telegramManager;

        public ApiController(IMemoryCache cache)
        {
            _publicationManager = new PublicationManager(Settings.Current.ConnectionString, cache);
            _userManager = new UserManager(Settings.Current.ConnectionString);
            _telegramManager = new TelegramManager(Settings.Current.TelegramToken, Settings.Current.TelegramChannelId);
        }

        [HttpGet]
        [Route("api/categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = _publicationManager.GetCategories().Select(o => new
            {
                o.Id,
                o.Name
            }).ToList();

            return Ok(categories);
        }

        [HttpPost]
        [Route("api/publications/new")]
        public async Task<IActionResult> AddPublicaton(NewPostRequest request)
        {
            DAL.User user = _userManager.GetBySecretKey(request.Key);

            if (user == null)
            {
                return StatusCode((int)HttpStatusCode.Forbidden);
            }

            var extractor = new X.Web.MetaExtractor.Extractor();

            var metadata = await extractor.Extract(new Uri(request.Link));


            var publication = new DAL.Publication
            {
                Title = metadata.Title,
                Description = metadata.Description,
                Link = metadata.Url,
                Image = metadata.Image.FirstOrDefault(),
                Type = metadata.Type,
                DateTime = DateTime.Now,
                UserId = user.Id,
                CategoryId = request.CategoryId,
                Comment = request.Comment
            };

            publication = await _publicationManager.Save(publication);

            if (publication != null)
            {
                var model = new PublicationViewModel(publication, Settings.Current.WebSiteUrl);

                await _telegramManager.Send(request.CategoryId, request.Comment, request.Link);

                return Created(new Uri($"{Core.Settings.Current.WebSiteUrl}p    ost/{publication.Id}"), model);
            }

            return StatusCode((int)HttpStatusCode.BadRequest);
        }
    }
}
