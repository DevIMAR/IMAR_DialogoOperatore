using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using IMAR_DialogoOperatore.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace IMAR_DialogoOperatore.Infrastructure.Services
{
    public class CaricamentoAttivitaInBackroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Timer _timer;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        private IList<vrtManNotActive>? _attivitaAperte;
        private IList<stdMesIndTsk>? _attivitaIndirette;

        public CaricamentoAttivitaInBackroundService(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _attivitaAperte = new List<vrtManNotActive>();
            _attivitaIndirette = new List<stdMesIndTsk>();

            _timer = new Timer(UpdateAttivita, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
        }

        private void UpdateAttivita(object? state)
        {
            try
            {
                List<dynamic> obj;
                decimal somma;

                using var scope = _serviceProvider.CreateScope();
                var jmesApiClient = scope.ServiceProvider.GetRequiredService<IJmesApiClient>();
                var as400Repository = scope.ServiceProvider.GetRequiredService<IAs400Repository>();

                var nuoveAttivita = jmesApiClient.ChiamaQueryVirtualJmes<vrtManNotActive>();

                var attivitaIndirette = jmesApiClient.ChiamaQueryGetJmes<stdMesIndTsk>();

                _lock.EnterWriteLock();
                try
                {
                    _attivitaAperte = nuoveAttivita;
                    _attivitaIndirette = attivitaIndirette;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nell'aggiornamento delle attività: {ex.Message}");
            }
        }

        public IList<vrtManNotActive> GetAttivitaAperte()
        {
            _lock.EnterReadLock();
            try
            {
                return _attivitaAperte?.ToList() ?? new List<vrtManNotActive>();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public IList<stdMesIndTsk> GetAttivitaIndirette()
        {
            _lock.EnterReadLock();
            try
            {
                return _attivitaIndirette?.ToList() ?? new List<stdMesIndTsk>();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }
}
