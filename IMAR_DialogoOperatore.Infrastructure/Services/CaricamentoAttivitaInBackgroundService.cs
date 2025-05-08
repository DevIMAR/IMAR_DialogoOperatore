using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using IMAR_DialogoOperatore.Application.Interfaces.UoW;
using IMAR_DialogoOperatore.Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace IMAR_DialogoOperatore.Infrastructure.Services
{
    public class CaricamentoAttivitaInBackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Timer _timer;
        private readonly Timer _timerDatiDaMonitor;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        private string _odpDatiMonitor;
        private IList<Attivita>? _attivitaAperte;
        private IList<stdMesIndTsk>? _attivitaIndirette;

        public CaricamentoAttivitaInBackgroundService(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _attivitaAperte = new List<Attivita>();
            _attivitaIndirette = new List<stdMesIndTsk>();

            _timerDatiDaMonitor = new Timer(UpdateDatiDaMonitor, null, TimeSpan.Zero, TimeSpan.FromMinutes(15));
            _timer = new Timer(UpdateAttivita, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
        }

        private void UpdateAttivita(object? state)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var jmesApiClient = scope.ServiceProvider.GetRequiredService<IJmesApiClient>();
                var as400Repository = scope.ServiceProvider.GetRequiredService<IAs400Repository>();

                var nuoveAttivita = as400Repository.ExecuteQuery<Attivita>(@"SELECT
                                                                             NRBLCI AS BOLLA, ORPRCI AS ODP, CDARCI AS ARTICOLO, trim(DSARMA) AS DESCRIZIONEARTICOLO, 
                                                                             CDFACI AS FASE, DSFACI AS DESCRIZIONEFASE, PF2.QORDCI AS QUANTITAORDINE, 
                                                                             COALESCE(SUM(jf.QTVERJM), 0) AS QUANTITAPRODOTTANONCONTABILIZZATA, 
                                                                             pf2.QPROCI AS QUANTITAPRODOTTACONTABILIZZATA,
                                                                             COALESCE(SUM(jf.QTSCAJM), 0) AS QUNATITASCARTATANONCONTABILIZZATA, 
                                                                             pf2.QSCACI AS QUANTITASCARTATACONTABILIZZATA,
                                                                             COALESCE(SUM(jf.QTNCOJM), 0) AS QTANONCONFORMENONCONTABILIZZATA,
                                                                             pf2.QRESCI AS QTANONCONFORMECONTABILIZZATA,
                                                                             CASE WHEN MAX(SAACCJM) IS NULL THEN max(TIRECI) ELSE  MAX(SAACCJM) end AS SALDOACCONTO
                                                                             FROM IMA90DAT.PCIMP00F pf2 
                                                                             JOIN IMA90DAT.MGART00F mf ON pf2.CDARCI = mf.CDARMA 
                                                                             LEFT JOIN IMA90DAT.JMRILM00F jf ON NRBLCI = NRTSKJM AND jf.QCONTJM = ''
                                                                             WHERE ORPRCI IN (" + _odpDatiMonitor + @")
                                                                             GROUP BY NRBLCI, ORPRCI, CDARCI, DSARMA, 
                                                                             CDFACI, DSFACI, PF2.QORDCI, QPROCI , QSCACI, QRESCI");

                var attivitaIndirette = jmesApiClient.ChiamaQueryGetJmes<stdMesIndTsk>();

                _lock.EnterWriteLock();
                try
                {
                    _attivitaAperte = nuoveAttivita.ToList();
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

        private void UpdateDatiDaMonitor(object? state)
        {
            using var scope = _serviceProvider.CreateScope();
            var imarSchedulatoreUoW = scope.ServiceProvider.GetRequiredService<IImarSchedulatoreUoW>();

            _lock.EnterWriteLock();
            try
            {
                _odpDatiMonitor = "'";
                List<DATIMONITOR> datiMonitor = imarSchedulatoreUoW.DatiMonitorRepository.Get()
                                                .OrderBy(x => x.ODP)
                                                .ThenBy(x => x.FASE)
                                                .ToList();

                foreach (DATIMONITOR dato in datiMonitor)
                {
                    _odpDatiMonitor += dato.ODP + "'";
                    _odpDatiMonitor += dato == datiMonitor.Last() ? string.Empty : ",'";
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public IList<Attivita> GetAttivitaAperte()
        {
            _lock.EnterReadLock();
            try
            {
                return _attivitaAperte?.OrderBy(x => x.Bolla).ToList() ?? new List<Attivita>();
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
