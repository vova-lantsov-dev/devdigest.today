using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace WebSite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseApplicationInsights()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                          .AddJsonFile($"/Users/andrew/pub/devdigest.today/appsettings.json", optional: true, reloadOnChange: true)
                          .AddJsonFile($"c:/pub/devdigest.today/appsettings.json", optional: true, reloadOnChange: true)
                          .AddEnvironmentVariables();

                })
                .UseStartup<Startup>()
                .Build();
    }
}
