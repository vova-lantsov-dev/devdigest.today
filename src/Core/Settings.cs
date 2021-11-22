using System;

namespace Core;

public class Settings
{
    public string ConnectionString { get; set; }

    public string WebSiteTitle { get; set; }

    public Uri WebSiteUrl { get; set; }

    public string DefaultDescription { get; set; }

    public string DefaultKeywords { get; set; }

    public Uri FacebookImage => new Uri($"{WebSiteUrl}images/logo.png");

    public string DefaultPublicationImage => $"{WebSiteUrl}images/news.jpg";

    public string RssFeedUrl => $"{WebSiteUrl}rss";

    public string SupportEmail { get; set; }

    public string FbAppId { get; set; }

    public string TwitterAccountName { get; set; }

    public string CognitiveServicesTextAnalyticsKey { get; set; }

    public string Version { get; set; }
        
    public ApplicationInsightsSetting ApplicationInsights { get; set; }
        
    public string WebRootPath { get; set; }
        
    public string FacebookPageUrl { get; set; }

    #region Current

    /// <summary>
    /// Settings.Current will be used in views
    /// </summary>
    /// <param name="settings"></param>
    public static void InitializeCurrentInstance(Settings settings) => Current = settings;

    public static Settings Current { get; private set; }

    #endregion
}

public class ApplicationInsightsSetting
{
    public string InstrumentationKey { get; set; }
}