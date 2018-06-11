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

namespace WebSite
{
    public class Registry
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly Settings _settings;

        public Registry(IHostingEnvironment hostingEnvironment, Settings settingst)
        {
            _hostingEnvironment = hostingEnvironment;
            _settings = settingst;
        }

        public IServiceCollection Register(IServiceCollection services)
        {
            var logger = new Core.Logging.SerilogLoggerWrapper(CreateLogger(_hostingEnvironment));
            services.AddMemoryCache();

            services.AddSingleton<Settings>(_ => _settings);
            services.AddSingleton<Core.Logging.ILogger>(_ => logger);
            
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

            return services;
        }

        private static Logger CreateLogger(IHostingEnvironment env)
        {

            var storageAccount = new CloudStorageAccount(new StorageCredentials("", ""), true);

            var path = $"{env.ContentRootPath}/logs/log-.log";

            return new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(path)
                .WriteTo.AzureTableStorage(storageAccount)
                .CreateLogger();
        }
    }
}