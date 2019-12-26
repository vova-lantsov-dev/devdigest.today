using System.Text.Encodings.Web;
using System.Text.Unicode;
using Core;
using Core.Logging;
using Core.Repositories;
using Core.Services;
using Core.Services.Crosspost;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.WebEncoders;
using Serilog.Events;
using WebSite.AppCode;

namespace WebSite
{
    public class Registry
    {
        private readonly Settings _settings;
        private readonly ILogger _logger;

        public Registry(string contentRootPath, Settings settings)
        {
            _settings = settings;
            _logger = new SerilogLoggerWrapper(new SerilogFactory().CreateLogger(contentRootPath));
        }

        public IServiceCollection Register(IServiceCollection services)
        {
            services
                .AddApplicationInsightsTelemetry()
                
                .AddMemoryCache()

                .AddEntityFrameworkMySql()

                .AddDbContext<DatabaseContext>(options => options.UseMySql(_settings.ConnectionString))

                .AddSingleton(_ => _settings)
                .AddSingleton(_ => _logger)

                .AddTransient<TelegramCrosspostService>()
                .AddTransient<FacebookCrosspostService>()
                .AddTransient<TwitterCrosspostService>()

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

                .Configure<WebEncoderOptions>(options =>
                {
                    options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
                });
            
            _logger.Write(LogEventLevel.Information, "DI container initialized");

            return services;
        }

    }
}