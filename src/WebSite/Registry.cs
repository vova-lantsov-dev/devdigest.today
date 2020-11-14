using System.Text.Encodings.Web;
using System.Text.Unicode;
using Core;
using Core.Repositories;
using Core.Services;
using Core.Services.Crosspost;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.WebEncoders;
using MySqlConnector;
using WebSite.AppCode;

namespace WebSite
{
    public class Registry
    {
        private readonly Settings _settings;

        public Registry(string contentRootPath, Settings settings)
        {
            _settings = settings;
        }

        public IServiceCollection Register(IServiceCollection services)
        {
            services
                .AddApplicationInsightsTelemetry()

                .AddMemoryCache()

                .AddEntityFrameworkMySql()

                .AddDbContext<DatabaseContext>(options =>
                {
                    options.UseMySql(_settings.ConnectionString, ServerVersion.FromString("5.7.32-mysql"));
                })

                .AddSingleton(_ => _settings)

                .AddSingleton<CrossPostServiceFactory>()

                .AddScoped<ILocalizationService, LocalizationService>()
                .AddScoped<IPublicationService, PublicationService>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<IVacancyService, VacancyService>()
                .AddScoped<IPublicationRepository, PublicationRepository>()
                .AddScoped<ISettingsRepository, SettingsRepository>()
                .AddScoped<ISocialRepository, SocialRepository>()
                .AddScoped<IPageRepository, PageRepository>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IVacancyRepository, VacancyRepository>()
                .AddScoped<IWebAppPublicationService, WebAppPublicationService>()

                .AddScoped<ILanguageAnalyzerService>(provider =>
                {
                    var logger = provider.GetService<ILogger<LanguageAnalyzerService>>();

                    return new LanguageAnalyzerService(_settings.CognitiveServicesTextAnalyticsKey, logger);
                })

                .Configure<WebEncoderOptions>(options =>
                {
                    options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
                });

            return services;
        }
    }
}