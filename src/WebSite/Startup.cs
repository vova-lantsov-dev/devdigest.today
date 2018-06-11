using System;
using Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.WebEncoders;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Core.Managers;
using DAL;
using Microsoft.Extensions.Caching.Memory;

namespace WebSite
{
    public class Startup
    {
        private readonly Settings _settings;
        
        public IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _settings = new Settings(configuration);
            
            Settings.Initialize(_settings);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddMemoryCache();

            services.AddSingleton<Settings>(_ => _settings);
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
