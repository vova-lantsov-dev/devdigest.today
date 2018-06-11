using System.Text.Encodings.Web;
using System.Text.Unicode;
using Core;
using Core.Managers;
using DAL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.WebEncoders;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Serilog;
using Serilog.Core;
using LogLevel = Core.Logging.LogLevel;

namespace WebSite
{
    public class Registry
    {
        private readonly Settings _settings;
        private readonly Core.Logging.ILogger _logger;
        private readonly IHostingEnvironment _hostingEnvironment;
        
        public Registry(IHostingEnvironment hostingEnvironment, Settings settingst)
        {
            _hostingEnvironment = hostingEnvironment;
            _settings = settingst;
            _logger = new Core.Logging.SerilogLoggerWrapper(CreateLogger(_hostingEnvironment));
        }

        public IServiceCollection Register(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddSingleton<Settings>(_ => _settings);
            services.AddSingleton<Core.Logging.ILogger>(_ => _logger);
            
            services.AddScoped<DatabaseContext>(_ => new DatabaseContext(_settings.ConnectionString));
            
            services.AddScoped(typeof(TelegramCrosspostManager));
            services.AddScoped(typeof(FacebookCrosspostManager));
            
            services.AddScoped(typeof(ILocalizationManager), typeof(LocalizationManager));
            services.AddScoped(typeof(IPublicationManager), typeof(PublicationManager));
            services.AddScoped(typeof(IUserManager), typeof(UserManager));
            services.AddScoped(typeof(IVacancyManager), typeof(VacancyManager));
            
            services.Configure<WebEncoderOptions>(options =>
            {
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
            });
            
            _logger.Write(LogLevel.Info, "DI container initialized");

            return services;
        }

        private static Logger CreateLogger(IHostingEnvironment env)
        {
            var storageAccount = new CloudStorageAccount(new StorageCredentials("", ""), true);

            var path = $"{env.ContentRootPath}/logs/log-.log";

            return new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(path)
                //.WriteTo.AzureTableStorage(storageAccount)
                .CreateLogger();
        }
    }
}