
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core;
using Core.Managers;
using Core.ViewModels;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using X.PagedList;
using X.Web.MetaExtractor;
using X.Web.Sitemap;

namespace WebSite.Controllers
{
    public class SitemapController : Controller
    {
        private readonly PublicationManager _manager;
        private readonly IMemoryCache _cache;

        public SitemapController(IMemoryCache cache)
        {
            _cache = cache;
            _manager = new PublicationManager(Core.Settings.Current.ConnectionString, cache);
        }

        [HttpGet]
        [Route("sitemap.xml")]
        public async Task<IActionResult> GetSitemap()
        {
            var page = 1;
            var key = "sitemap";
            IPagedList<Publication> pagedResult;

            var xml = _cache.Get(key)?.ToString();

            if (string.IsNullOrEmpty(xml))
            {
                var sitemap = new Sitemap();
                sitemap.Add(CreateUrl(Settings.Current.WebSiteUrl));
                sitemap.Add(CreateUrl(Settings.Current.WebSiteUrl + "partners/"));

                do
                {
                    pagedResult = await _manager.GetPublications(null, page);
                    page++;

                    foreach (var p in pagedResult)
                    {
                        var publication = new PublicationViewModel(p, Settings.Current.WebSiteUrl);
                        sitemap.Add(CreateUrl(publication.ShareUrl));
                    }
                }
                while (pagedResult.HasNextPage);

                 xml = sitemap.ToXml();
                 _cache.Set(key, xml, TimeSpan.FromMinutes(10));
            }

            return Content(xml, Sitemap.MimeType);
        }

        private static Url CreateUrl(string url)
        {
            return new Url
            {
                ChangeFrequency = ChangeFrequency.Daily,
                Location = url,
                Priority = 0.5,
                TimeStamp = DateTime.Now
            };
        }

    }
}
