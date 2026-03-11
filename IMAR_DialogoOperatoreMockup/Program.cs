using DevExpress.Blazor;
using IMAR_DialogoOperatore.Components;
using IMAR_DialogoOperatore.Infrastructure;
using IMAR_DialogoOperatore.Application;
using log4net;
using log4net.Config;
using System.Reflection;

namespace IMAR_DialogoOperatore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configurazione Log4Net
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            // Sovrascrivi il path del log da appsettings (così test e produzione usano path diversi)
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var logPath = config["LogSettings:NetworkPath"];
            if (!string.IsNullOrEmpty(logPath))
            {
                // Se siamo in test (hostname contiene "test"), cambia il nome del file log
                var hostname = Environment.MachineName?.ToLower() ?? "";
                var siteName = Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME")?.ToLower()
                    ?? Environment.GetEnvironmentVariable("APP_POOL_ID")?.ToLower() ?? "";

                if (hostname.Contains("035") || siteName.Contains("test"))
                    logPath = logPath.Replace("DialogoOperatore.log", "DialogoOperatore_TEST.log");

                var appender = logRepository.GetAppenders()
                    .OfType<log4net.Appender.RollingFileAppender>()
                    .FirstOrDefault();
                if (appender != null)
                {
                    appender.File = logPath;
                    appender.ActivateOptions();
                }
            }

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddDevExpressBlazor();
            builder.Services.AddServerSideBlazor()
                            .AddCircuitOptions(options => { options.DetailedErrors = true; });

            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddDialogoOperatoreServices();

			var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
