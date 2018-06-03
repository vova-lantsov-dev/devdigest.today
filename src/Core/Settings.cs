using System.Net;
using Microsoft.Extensions.Configuration;

namespace Core
{
    public class Settings
    {
        public Settings(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DefaultConnection");
            WebSiteUrl = configuration["WebSiteUrl"];
            WebSiteTitle = configuration["WebSiteTitle"];
            CognitiveServicesTextAnalyticsKey = configuration["CognitiveServicesTextAnalyticsKey"];
            DefaultDescription = WebUtility.HtmlDecode(configuration["DefaultDescription"]);
            DefaultKeywords = WebUtility.HtmlDecode(configuration["DefaultKeywords"]);
            SupportEmail = "dncuug@agi.net.ua";
            FbAppId = "112150392810181";
        }

        public string ConnectionString { get; }

        public string WebSiteTitle { get; }

        public string WebSiteUrl { get; }

        public string DefaultDescription { get; }

        public string DefaultKeywords { get; }

        public string FacebookImage => $"{WebSiteUrl}images/logo.png";

        public string DefaultPublicationImage => $"{WebSiteUrl}images/news.jpg";

        public string RssFeedUrl => $"{WebSiteUrl}rss";

        public string SupportEmail { get; }

        public string FbAppId { get; }

        public string CognitiveServicesTextAnalyticsKey { get; }

        #region Current
        
        /// <summary>
        /// Settings.Current will be used in views
        /// </summary>
        /// <param name="settings"></param>
        public static void Initialize(Settings settings) => Current = settings;

        public static Settings Current { get; private set; }
        
        #endregion
    }
}