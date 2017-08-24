using System;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace Core
{
    public class Settings
    {
        public string ConnectionString { get; private set; }

        public string WebSiteTitle { get; private set; }

        public string WebSiteUrl { get; private set; }

        public string DefaultDescription { get; private set; }

        public string DefaultKeywords { get; private set; }

        public string FacebookImage
        {
            get { return $"{WebSiteUrl}images/fb_logo.png"; }
        }

        #region Current

        public static Settings Current { get; private set; }
        public Guid PublicationKey { get; set; }

        public static void Initialize(IConfiguration configuration)
        {
            Current = new Settings
            {
                ConnectionString = configuration.GetConnectionString("DefaultConnection"),
                WebSiteUrl = configuration["WebSiteUrl"],
                WebSiteTitle = configuration["WebSiteTitle"],
                DefaultDescription = WebUtility.HtmlDecode(configuration["DefaultDescription"]),
                DefaultKeywords = WebUtility.HtmlDecode(configuration["DefaultKeywords"]),
                PublicationKey = Guid.Parse(configuration["PublicationKey"])
            };
        }

        #endregion
    }
}