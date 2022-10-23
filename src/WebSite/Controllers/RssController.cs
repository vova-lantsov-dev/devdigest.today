using System.Globalization;
using Core;
using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using X.Web.RSS;
using X.Web.RSS.Enumerators;
using X.Web.RSS.Structure;
using X.Web.RSS.Structure.Validators;

namespace WebSite.Controllers;

public class RssController : Controller
{
    private readonly IPostService _postService;
    private readonly PostUrlBuilder _postUrlBuilder;
    private readonly IMemoryCache _cache;
    private readonly Settings _settings;

    public RssController(IMemoryCache cache, IPostService postService, PostUrlBuilder postUrlBuilder, Settings settings)
    {
        _cache = cache;
        _postService = postService;
        _postUrlBuilder = postUrlBuilder;
        _settings = settings;
    }

    [HttpGet]
    [Route("rss")]
    public async Task<IActionResult> GetRssFeed()
    {
        var key = "rss";
        var xml = _cache.Get(key)?.ToString();

        if (string.IsNullOrEmpty(xml))
        {
            int? categoryId = null;
            var pagedResult = await _postService.GetList(categoryId, 1, 50);
            var lastUpdateDate = pagedResult.Select(o => o.DateTime).DefaultIfEmpty().Max();

            var rss = new RssDocument
            {
                Channel =
                    new RssChannel
                    {
                        Copyright = _settings.WebSiteTitle,
                        Description = _settings.DefaultDescription,
                        SkipDays = new List<Day>(),
                        SkipHours = new List<Hour>(),
                        AtomLink = new RssLink(_settings.RssFeedUrl),
                        Image = new RssImage
                        {
                            Description = _settings.WebSiteTitle,
                            Height = 100,
                            Width = 100,
                            Link = new RssUrl(_settings.FacebookImage),
                            Title = _settings.WebSiteTitle,
                            Url = new RssUrl(_settings.FacebookImage)
                        },
                        Language = new CultureInfo("ru"),
                        LastBuildDate = lastUpdateDate,
                        Link = new RssUrl(_settings.RssFeedUrl),
                        ManagingEditor = new RssPerson("Andrew G.", _settings.SupportEmail),
                        PubDate = lastUpdateDate,
                        Title = _settings.WebSiteTitle,
                        TTL = 10,
                        WebMaster = new RssPerson("Andrew G.", _settings.SupportEmail),
                        Items = new List<RssItem> { }
                    }
            };

            foreach (var p in pagedResult)
            {
                rss.Channel.Items.Add(CreateRssItem(p));
            }

            xml = rss.ToXml();

            _cache.Set(key, xml, TimeSpan.FromMinutes(10));
        }

        return Content(xml, RssDocument.MimeType);
    }

    private RssItem CreateRssItem(Post post)
    {
        Uri shareUrl = _postUrlBuilder.Build(post.Id);
        
        return new RssItem
        {
            Description = post.Description,
            Link = new RssUrl(shareUrl),
            PubDate = post.DateTime,
            Title = post.Title,
            Guid = new RssGuid { IsPermaLink = true, Value = shareUrl.ToString() },
            Source = new RssSource { Url = new RssUrl(shareUrl) }
        };
    }
}