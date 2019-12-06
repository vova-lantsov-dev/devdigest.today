using System.Text.Encodings.Web;
using System.Text.Unicode;
using Core;
using Core.Logging;
using Core.Services;
using Core.Services.Crosspost;
using Core.Repositories;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.WebEncoders;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace WebSite
{
    public class Registry
    {
        private readonly Settings _settings;
        private readonly Core.Logging.ILogger _logger;

        public Registry(string contentRootPath, Settings settings)
        {
            
            _settings = settings;
            _logger = new SerilogLoggerWrapper(new SerilogFactory().CreateLogger(contentRootPath));
        }

        public IServiceCollection Register(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddEntityFrameworkMySql();
            
            services.AddDbContext<DatabaseContext>(options => options.UseMySql(_settings.ConnectionString));
            
            services.AddSingleton(_ => _settings);
            services.AddSingleton(_ => _logger);
            
            services.AddTransient<TelegramCrosspostService>();
            services.AddTransient<FacebookCrosspostService>();
            services.AddTransient<TwitterCrosspostService>();
            
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<IPublicationService, PublicationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IVacancyService, VacancyService>();
            
            services.AddScoped<IPublicationRepository, PublicationRepository>();
            services.AddScoped<ISettingsRepository, SettingsRepository>();
            services.AddScoped<ISocialRepository, SocialRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IVacancyRepository, VacancyRepository>();
            
            services.Configure<WebEncoderOptions>(options =>
            {
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
            });
            
            _logger.Write(LogEventLevel.Information, "DI container initialized");

            return services;
        }

    }
}