using EsportatoreTimbratureTeamSystem.Services;
using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .Build();


var host = Host.CreateDefaultBuilder(args).
                ConfigureServices((context, services) =>
                {
                    services.AddApplicationServices();
                    services.AddInfrastructureServices();

                    services.AddScoped<TimbratureService>();
                    services.AddScoped<EncodingService>();
                    services.AddScoped<FileExportService>();
                })
                .Build();

using var scope = host.Services.CreateScope();
var timbratureService = scope.ServiceProvider.GetRequiredService<TimbratureService>();
var encodingService = scope.ServiceProvider.GetRequiredService<EncodingService>();
var fileExportService = scope.ServiceProvider.GetRequiredService<FileExportService>();

var timbrature = timbratureService.GetTimbrature();
string timbratureCodificate = encodingService.GetTimbratureCodificate(timbrature);
fileExportService.Export(timbratureCodificate);