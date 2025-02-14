using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Application.Interfaces.Services.External;
using IMAR_DialogoOperatore.Application.Interfaces.UoW;
using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using IMAR_DialogoOperatore.Domain.JMes;
using IMAR_DialogoOperatore.Infrastructure.As400;
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
			services.AddDbContext<SynergyJmesContext>();
			services.AddDbContext<As400Context>();

			services.AddSingleton<CaricamentoAttivitaInBackroundService>();

			services.AddScoped<ISynergyJmesUoW, SynergyJmesUoW>();

			services.AddScoped<IAttivitaService, AttivitaService>();
			services.AddScoped<IMorpheusApiService, MorpheusApiService>();
			services.AddScoped<IOperatoriService, OperatoreService>();
			services.AddScoped<IMacchinaService, MacchinaService>();

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
