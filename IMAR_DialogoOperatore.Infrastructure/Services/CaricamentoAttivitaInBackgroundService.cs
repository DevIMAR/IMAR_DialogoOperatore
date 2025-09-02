using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using IMAR_DialogoOperatore.Application.Interfaces.UoW;
using IMAR_DialogoOperatore.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace IMAR_DialogoOperatore.Infrastructure.Services
{
    public class CaricamentoAttivitaInBackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Timer _timer;
        private readonly Timer _timerDatiDaMonitor;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private readonly IJmesApiClient _jmesApiClient;
        private readonly IAs400Repository _as400Repository;

        private List<string> _odpDatiMonitor;
        private IList<Attivita>? _attivitaAperte;
        private IList<stdMesIndTsk>? _attivitaIndirette;

        public CaricamentoAttivitaInBackgroundService(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _attivitaAperte = new List<Attivita>();
            _attivitaIndirette = new List<stdMesIndTsk>();

            using var scope = _serviceProvider.CreateScope();
            _jmesApiClient = scope.ServiceProvider.GetRequiredService<IJmesApiClient>();
            _as400Repository = scope.ServiceProvider.GetRequiredService<IAs400Repository>();

            _timerDatiDaMonitor = new Timer(UpdateDatiDaMonitor, null, TimeSpan.Zero, TimeSpan.FromMinutes(15));
            _timer = new Timer(UpdateAttivita, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
        }

        private void UpdateAttivita(object? state)
        {
            if (_odpDatiMonitor == null)
                return;

            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                List<Attivita> nuoveAttivita = new List<Attivita>();

                decimal dimensioneBatch = 1000;
                decimal batchCount = Math.Ceiling(_odpDatiMonitor.Count / dimensioneBatch);

                for (int i = 0; i < batchCount; i++)
                {
                    var batchConsiderato = Math.Min((int)dimensioneBatch, _odpDatiMonitor.Count - (int)dimensioneBatch * i);
                    var batchOdp = string.Join(',', _odpDatiMonitor.Slice((int)dimensioneBatch * i, batchConsiderato));

                    var batchAttivita = _as400Repository.ExecuteQuery<Attivita>(@"SELECT
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
                                                                                 WHERE ORPRCI IN (" + batchOdp + @")
                                                                                 GROUP BY NRBLCI, ORPRCI, CDARCI, DSARMA, CDFACI, DSFACI,
                                                                                          pf2.QORDCI, pf2.QPROCI, pf2.QSCACI, pf2.QRESCI");

                    nuoveAttivita.AddRange(batchAttivita);
                }

                var attivitaIndirette = _jmesApiClient.ChiamaQueryGetJmes<stdMesIndTsk>();

                _lock.EnterWriteLock();
                try
                {
                    _attivitaAperte = nuoveAttivita;
                    _attivitaIndirette = attivitaIndirette;
                }
                finally
                {
                    _lock.ExitWriteLock();

                    stopwatch.Stop();
                    Console.WriteLine(stopwatch.Elapsed.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nell'aggiornamento delle attività: {ex.Message}");
            }
        }

        private void UpdateDatiDaMonitor(object? state)
        {
            _lock.EnterWriteLock();
            try
            {
                _odpDatiMonitor = _as400Repository.ExecuteQuery<string>(@"SELECT DISTINCT(ORPRCI)
                                                                          FROM IMA90DAT.PCIMP00F
                                                                          WHERE TIRECI <> 'S'")
                                                  .ToList();
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
                return _attivitaAperte?.ToList() ?? new List<Attivita>();
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
