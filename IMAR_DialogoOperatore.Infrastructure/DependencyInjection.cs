using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Application.Interfaces.Services.External;
using IMAR_DialogoOperatore.Application.Interfaces.UoW;
using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using IMAR_DialogoOperatore.Domain.Imar_Produzione;
using IMAR_DialogoOperatore.Infrastructure.As400;
using IMAR_DialogoOperatore.Infrastructure.Imar_Connect;
using IMAR_DialogoOperatore.Infrastructure.Imar_Produzione;
using IMAR_DialogoOperatore.Infrastructure.Imar_Schedulatore;
using IMAR_DialogoOperatore.Infrastructure.ImarApi;
using IMAR_DialogoOperatore.Infrastructure.JMes;
using IMAR_DialogoOperatore.Infrastructure.Services;
using IMAR_DialogoOperatore.Infrastructure.Utilities;
using IMAR_DialogoOperatore.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IMAR_DialogoOperatore.Infrastructure
{
    public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
		{
			services.AddDbContext<As400Context>();
			services.AddDbContext<ImarConnectContext>();
			services.AddDbContext<ImarProduzioneContext>();
			services.AddDbContext<ImarSchedulatoreContext>();
			services.AddDbContext<SynergyJmesContext>();

			services.AddSingleton<CaricamentoAttivitaInBackgroundService>();

			services.AddScoped<ISynergyJmesUoW, SynergyJmesUoW>();
			services.AddScoped<IImarConnectUoW, ImarConnectUoW>();
			services.AddScoped<IImarProduzioneUoW, ImarProduzioneUoW>();
			services.AddScoped<IImarSchedulatoreUoW, ImarSchedulatoreUoW>();

			services.AddScoped<IAttivitaService, AttivitaService>();
			services.AddScoped<IMorpheusApiService, MorpheusApiService>();
			services.AddScoped<IOperatoreService, OperatoreService>();
			services.AddScoped<IMacchinaService, MacchinaService>();
			services.AddScoped<ISegnalazioniDifformitaService, SegnalazioniDifformitaService>();
			services.AddScoped<ITimbratureService, TimbratureService>();

			services.AddScoped<IImarApiClient, ImarApiClient>();
			services.AddScoped<IJmesApiClient, JmesApiClient>();

			services.AddScoped<IAs400Repository, As400Repository>();

			services.AddScoped<IHttpClientUtility, HttpClientUtility>();
			services.AddScoped<IJSonUtility, JSonUtility>();
			services.AddScoped<IJMesApiClientErrorUtility, JMesApiClientErrorUtility>();
			services.AddScoped<IAutoLogoutUtility, AutoLogoutUtility>();

			return services;
		}
	}
}
