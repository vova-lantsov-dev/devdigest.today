using System.Text.Encodings.Web;
using System.Text.Unicode;
using Core;
using Core.Repositories;
using Core.Services;
using Core.Services.Posting;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.WebEncoders;
using WebSite.AppCode;

var builder = WebApplication.CreateBuilder(args);


builder.Logging
    .AddConsole()
    .AddAzureWebAppDiagnostics();

builder.Configuration
    .AddJsonFile($"appsettings.json", false, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
    .AddJsonFile($"/Users/andrew/pub/dd.settings.json", false, true)
    .AddEnvironmentVariables();

var settings = LoadSettings(builder.Environment, builder.Configuration);
Settings.InitializeCurrentInstance(settings);

// Add services to the container.
builder.Services
    .AddApplicationInsightsTelemetry()

    .AddMemoryCache()

    .AddDbContext<DatabaseContext>(options =>
    {
        options.UseMySql(settings.ConnectionString, ServerVersion.Parse("5.7.32-mysql"));
    })

    .AddSingleton(_ => settings)

    .AddSingleton<PostingServiceFactory>()

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

        return new LanguageAnalyzerService(settings.CognitiveServicesTextAnalyticsKey, logger);
    })
                
    .AddApplicationInsightsTelemetry(settings.ApplicationInsights.InstrumentationKey)

    .Configure<WebEncoderOptions>(options =>
    {
        options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
    })
    .AddControllersWithViews();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static Settings LoadSettings(IWebHostEnvironment hostEnvironment, IConfiguration configuration)
{
    var settings = new Settings();

    configuration.Bind(settings);
    settings.ConnectionString = configuration.GetConnectionString("DefaultConnection");
    settings.WebRootPath = hostEnvironment.WebRootPath;
            
    return settings;
}