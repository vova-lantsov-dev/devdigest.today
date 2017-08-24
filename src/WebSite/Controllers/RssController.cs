
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core;
using Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using X.Web.MetaExtractor;
using X.Web.RSS;
using X.Web.RSS.Enumerators;
using X.Web.RSS.Structure;
using X.Web.RSS.Structure.Validators;

namespace WebSite.Controllers
{
    public class RssController : Controller
    {
        private readonly PublicationManager _manager;

        public RssController(IMemoryCache cache)
        {
            _manager = new PublicationManager(Core.Settings.Current.ConnectionString, cache);
        }

        [HttpGet]
        [Route("rss")]
        public async Task<IActionResult> GetRssFeed(string url, Guid key)
        {
            var pagedResult = await _manager.GetPublications(1, 50);
            var lastUpdateDate = pagedResult.Max(o => o.DateTime);

            var rss = new RssDocument
            {
                Channel =
                new RssChannel
                {
                    Copyright = Settings.Current.WebSiteTitle,
                    Description = Settings.Current.DefaultDescription,
                    Image =
                        new RssImage
                        {
                            Description = Settings.Current.WebSiteTitle,
                            Height = 100,
                            Width = 100,
                            Link = new RssUrl(Settings.Current.FacebookImage),
                            Title = Settings.Current.WebSiteTitle,
                            Url = new RssUrl(Settings.Current.FacebookImage)
                        },
                    Language = new CultureInfo("ru"),
                    LastBuildDate = lastUpdateDate,
                    Link = new RssUrl(Settings.Current.RssFeedUrl),
                    ManagingEditor = new RssEmail(Settings.Current.SupportEmail),
                    PubDate = lastUpdateDate,
                    Title = Settings.Current.WebSiteTitle,
                    TTL = 10,
                    WebMaster = new RssEmail(Settings.Current.SupportEmail),
                    Items = new List<RssItem> { }
                }
            };

            foreach (var p in pagedResult)
            {
                rss.Channel.Items.Add(CreateRssItem(p));
            }

            var xml = rss.ToXml();
            return Content(xml, RssDocument.MimeType);

        }

        private RssItem CreateRssItem(DAL.Publication publication)
        {
            var p = new Core.ViewModels.PublicationViewModel(publication, Settings.Current.WebSiteUrl);

            return new RssItem
            {
                Description = p.Description,
                Link = new RssUrl(p.ShareUrl),
                PubDate = p.DateTime,
                Title = p.Title,
                Guid = new RssGuid { IsPermaLink = false, Value = p.Id.ToString() },
                Source = new RssSource { Url = new RssUrl(p.ShareUrl) }
            };
        }

    }
}
