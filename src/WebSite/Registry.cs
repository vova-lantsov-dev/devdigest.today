using System.Text.Encodings.Web;
using System.Text.Unicode;
using Core;
using Core.Logging;
using Core.Managers;
using Core.Managers.Crosspost;
using Core.Repositories;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.WebEncoders;
using Serilog;
using Serilog.Core;

namespace WebSite
{
    public class Registry
    {
        private readonly Settings _settings;
        private readonly Core.Logging.ILogger _logger;

        public Registry(string contentRootPath, Settings settings)
        {
            _settings = settings;
            _logger = new SerilogLoggerWrapper(CreateLogger(contentRootPath));
        }

        public IServiceCollection Register(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddEntityFrameworkMySql();
            
            services.AddDbContext<DatabaseContext>(options => options.UseMySql(_settings.ConnectionString));
            
            services.AddSingleton(_ => _settings);
            services.AddSingleton(_ => _logger);
            
            services.AddTransient<TelegramCrosspostManager>();
            services.AddTransient<FacebookCrosspostManager>();
            services.AddTransient<TwitterCrosspostManager>();
            
            services.AddScoped<ILocalizationManager, LocalizationManager>();
            services.AddScoped<IPublicationManager, PublicationManager>();
            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped<IVacancyManager, VacancyManager>();
            
            services.AddScoped<IPublicationRepository, PublicationRepository>();
            services.AddScoped<ISettingsRepository, SettingsRepository>();
            services.AddScoped<ISocialRepository, SocialRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IVacancyRepository, VacancyRepository>();
            
            services.Configure<WebEncoderOptions>(options =>
            {
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
            });
            
            _logger.Write(LogLevel.Info, "DI container initialized");

            return services;
        }

        private static Logger CreateLogger(string contentRootPath)
        {
            var path = $"{contentRootPath}/logs/log-.log";

            return new Serilog.LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(path, rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}