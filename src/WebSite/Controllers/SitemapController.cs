
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
        private readonly PublicationManager _publicationManager;
        private readonly VacancyManager _vacancyManager;
        private readonly IMemoryCache _cache;

        public SitemapController(IMemoryCache cache)
        {
            _cache = cache;
            _publicationManager = new PublicationManager(Core.Settings.Current.ConnectionString, cache);
            _vacancyManager = new VacancyManager(Core.Settings.Current.ConnectionString, cache);
        }

        [HttpGet]
        [Route("sitemap.xml")]
        public async Task<IActionResult> GetSitemap()
        {
            var page = 1;
            var key = "sitemap";


            var xml = _cache.Get(key)?.ToString();

            if (string.IsNullOrEmpty(xml))
            {
                IPagedList<Publication> publications;
                IPagedList<Vacancy> vacancies;

                var sitemap = new Sitemap();
                sitemap.Add(CreateUrl(Settings.Current.WebSiteUrl));
                sitemap.Add(CreateUrl(Settings.Current.WebSiteUrl + "partners/"));

                page = 1;

                do
                {
                    publications = await _publicationManager.GetPublications(null, page);
                    page++;

                    foreach (var p in publications)
                    {
                        var publication = new PublicationViewModel(p, Settings.Current.WebSiteUrl);
                        sitemap.Add(CreateUrl(publication.ShareUrl));
                    }
                }
                while (publications.HasNextPage);

                page = 1;

                do
                {
                    vacancies = await _vacancyManager.GetVacancies(page);
                    page++;

                    foreach (var v in vacancies)
                    {
                        var vacancy = new VacancyViewModel(v, Settings.Current.WebSiteUrl);
                        sitemap.Add(CreateUrl(vacancy.ShareUrl));
                    }
                }
                while (vacancies.HasNextPage);

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
