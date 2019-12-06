namespace Core
{
    public class Settings
    {
        public string ConnectionString { get; set; }

        public string WebSiteTitle { get; set; }

        public string WebSiteUrl { get; set; }

        public string DefaultDescription { get; set; }

        public string DefaultKeywords { get; set; }

        public string FacebookImage => $"{WebSiteUrl}images/logo.png";

        public string DefaultPublicationImage => $"{WebSiteUrl}images/news.jpg";

        public string RssFeedUrl => $"{WebSiteUrl}rss";

        public string SupportEmail { get; set; }

        public string FbAppId { get; set; }

        public string CognitiveServicesTextAnalyticsKey { get; set; }

        public string Version { get; set; }

        public TwitterSettings Twitter { get; set; }

        #region Current

        /// <summary>
        /// Settings.Current will be used in views
        /// </summary>
        /// <param name="settings"></param>
        public static void InitializeCurrentInstance(Settings settings) => Current = settings;

        public static Settings Current { get; private set; }

        #endregion

        public class TwitterSettings
        {
            public string ConsumerKey { get; set; }
            public string ConsumerSecret { get; set; }
            public string AccessTokenSecret { get; set; }
            public string AccessToken { get; set; }
        }
    }
}