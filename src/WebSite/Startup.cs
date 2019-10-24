using System;
using Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace WebSite
{
    public class Startup
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly Settings _settings;

        public Startup(IWebHostEnvironment hostEnvironment, IConfiguration configuration)
        {
            Configuration = configuration;
            
            _hostEnvironment = hostEnvironment;
            _settings = new Settings(configuration);

            Settings.Initialize(_settings);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            new Registry(_hostEnvironment.ContentRootPath, _settings)
                .Register(services)
                .AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseExceptionHandler("/error/info");
                app.UseStatusCodePagesWithReExecute("/error/info");
                
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}