using System;
using System.Threading.Tasks;
using Core;
using Core.Services;
using Core.ViewModels;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using X.PagedList;
using X.Web.Sitemap;

namespace WebSite.Controllers
{
    public class SitemapController : Controller
    {
        private readonly IPublicationService _publicationService;
        private readonly IVacancyService _vacancyService;
        private readonly IMemoryCache _cache;
        private readonly Settings _settings;
        
        public SitemapController(
            IMemoryCache cache, 
            IVacancyService vacancyService, 
            IPublicationService publicationService, 
            Settings settings)
        {
            _cache = cache;
            _vacancyService = vacancyService;
            _publicationService = publicationService;
            _settings = settings;
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
                sitemap.Add(CreateUrl(_settings.WebSiteUrl));
                sitemap.Add(CreateUrl(_settings.WebSiteUrl + "partners/"));

                page = 1;

                do
                {
                    publications = await _publicationService.GetPublications(null, page);
                    page++;

                    foreach (var p in publications)
                    {
                        var publication = new PublicationViewModel(p, _settings.WebSiteUrl);
                        sitemap.Add(CreateUrl(publication.ShareUrl));
                    }
                }
                while (publications.HasNextPage);

                page = 1;

                do
                {
                    vacancies = await _vacancyService.GetVacancies(page);
                    page++;

                    foreach (var v in vacancies)
                    {
                        var vacancy = new VacancyViewModel(v, _settings.WebSiteUrl);
                        sitemap.Add(CreateUrl(vacancy.ShareUrl));
                    }
                }
                while (vacancies.HasNextPage);

                xml = sitemap.ToXml();
                _cache.Set(key, xml, TimeSpan.FromMinutes(10));
            }

            
            return Content(xml, "application/xml");
        }

        private static Url CreateUrl(string url) => new Url
        {
            ChangeFrequency = ChangeFrequency.Daily,
            Location = url,
            Priority = 0.5,
            TimeStamp = DateTime.Now
        };
    }
}
