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

        public string FacebookImage => $"{WebSiteUrl}images/fb_logo.png";
        
        public string DefaultPublicationImage => $"{WebSiteUrl}images/news.jpg";

        public string RssFeedUrl => $"{WebSiteUrl}rss";
        
        public string SupportEmail { get; set; }

        public string FbAppId { get; set; }
        
        public string CognitiveServicesTextAnalyticsKey { get; set; }
        
        #region Current

        public static Settings Current { get; private set; }
        
        public static void Initialize(IConfiguration configuration)
        {
            Current = new Settings
            {
                ConnectionString = configuration.GetConnectionString("DefaultConnection"),
                WebSiteUrl = configuration["WebSiteUrl"],
                WebSiteTitle = configuration["WebSiteTitle"],                
                CognitiveServicesTextAnalyticsKey = configuration["CS-Text-Analytics-Key"],                
                DefaultDescription = WebUtility.HtmlDecode(configuration["DefaultDescription"]),
                DefaultKeywords = WebUtility.HtmlDecode(configuration["DefaultKeywords"]),
                SupportEmail = "dncuug@agi.net.ua",
                FbAppId = "112150392810181",
            };
        }

        #endregion
    }
}