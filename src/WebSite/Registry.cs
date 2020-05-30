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
using WebSite.AppCode;

namespace WebSite
{
    public class Registry
    {
        private readonly Settings _settings;
        //private readonly ILogger _logger;

        public Registry(string contentRootPath, Settings settings)
        {
            _settings = settings;
            //_logger = new SerilogLoggerWrapper(new SerilogFactory().CreateLogger(contentRootPath));
        }

        public IServiceCollection Register(IServiceCollection services)
        {
            services
                .AddApplicationInsightsTelemetry()

                .AddMemoryCache()

                .AddEntityFrameworkMySql()

                .AddDbContext<DatabaseContext>(options => options.UseMySql(_settings.ConnectionString))

                .AddSingleton(_ => _settings)

                .AddSingleton<CrossPostServiceFactory>()

                .AddScoped<ILocalizationService, LocalizationService>()
                .AddScoped<IPublicationService, PublicationService>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<IVacancyService, VacancyService>()

                .AddScoped<IPublicationRepository, PublicationRepository>()
                .AddScoped<ISettingsRepository, SettingsRepository>()
                .AddScoped<ISocialRepository, SocialRepository>()
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