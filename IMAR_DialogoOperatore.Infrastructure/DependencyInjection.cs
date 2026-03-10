using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using IMAR_DialogoOperatore.Application.Interfaces.Services;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Application.Interfaces.Services.External;
using IMAR_DialogoOperatore.Application.Interfaces.UoW;
using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using IMAR_DialogoOperatore.Application.Services;
using IMAR_DialogoOperatore.Infrastructure.As400;
using IMAR_DialogoOperatore.Infrastructure.Imar_Connect;
using IMAR_DialogoOperatore.Infrastructure.Imar_Produzione;
using IMAR_DialogoOperatore.Infrastructure.Imar_Schedulatore;
using IMAR_DialogoOperatore.Infrastructure.ImarApi;
using IMAR_DialogoOperatore.Infrastructure.JMes;
using IMAR_DialogoOperatore.Infrastructure.Services;
using IMAR_DialogoOperatore.Infrastructure.Utilities;
using IMAR_DialogoOperatore.Services;
using log4net;
using log4net.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace IMAR_DialogoOperatore.Infrastructure
{
    public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<ImarConnectContext>(options =>
				options.UseSqlServer(configuration.GetConnectionString("ImarConnect")));
			services.AddDbContext<ImarProduzioneContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ImarProduzione")));
			services.AddDbContext<ImarSchedulatoreContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ImarSchedulatore")));
			services.AddDbContext<SynergyJmesContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("SynergyJmes")));
		
			services.AddScoped<As400Context>();

			services.AddSingleton<CalFlOdpCacheService>();
			services.AddSingleton<CaricamentoAttivitaInBackgroundService>();

			services.AddScoped<ISynergyJmesUoW, SynergyJmesUoW>();
			services.AddScoped<IImarConnectUoW, ImarConnectUoW>();
			services.AddScoped<IImarProduzioneUoW, ImarProduzioneUoW>();
			services.AddScoped<IImarSchedulatoreUoW, ImarSchedulatoreUoW>();

			services.AddScoped<IAttivitaService, AttivitaService>();
			services.AddScoped<IFaseNonPianificataService, FaseNonPianificataService>();
			services.AddScoped<IForzaturaService, ForzaturaService>();
			services.AddScoped<IMorpheusApiService, MorpheusApiService>();
			services.AddScoped<IOperatoreService, OperatoreService>();
			services.AddScoped<IMacchinaService, MacchinaService>();
			services.AddScoped<INotaService, NotaService>();
			services.AddScoped<ISegnalazioniDifformitaService, SegnalazioniDifformitaService>();
			services.AddScoped<ITimbratureService, TimbratureService>();
			services.AddScoped<IUtenteService, UtenteService>();

			// HttpClient per IMAR API con autenticazione Basic pre-configurata
			services.AddHttpClient("ImarApi", (sp, client) =>
			{
				var config = sp.GetRequiredService<IConfiguration>();
				client.BaseAddress = new Uri(config["ImarApi:BaseUrl"]!);
				var credentials = Convert.ToBase64String(
					Encoding.ASCII.GetBytes($"{config["ImarApi:Username"]}:{config["ImarApi:Password"]}"));
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
			});
			services.AddScoped<IImarApiClient, ImarApiClient>();
			services.AddScoped<IJmesApiClient, JmesApiClient>();

			services.AddScoped<IAs400Repository, As400Repository>();

			services.AddScoped<IHttpClientUtility, HttpClientUtility>();
			services.AddScoped<IJSonUtility, JSonUtility>();
			services.AddScoped<IJMesApiClientErrorUtility, JMesApiClientErrorUtility>();
			services.AddScoped<IAutoLogoutUtility, AutoLogoutUtility>();
			services.AddSingleton<ILoggingService, LoggingService>();

			return services;
		}
	}
}
