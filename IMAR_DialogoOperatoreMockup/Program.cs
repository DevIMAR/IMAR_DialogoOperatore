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

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddDevExpressBlazor(configure => configure.BootstrapVersion = BootstrapVersion.v5);
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
