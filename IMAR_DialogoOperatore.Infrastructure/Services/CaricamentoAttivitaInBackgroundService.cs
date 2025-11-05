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
        private readonly Timer _timerCalFlOdp;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private readonly ReaderWriterLockSlim _lockCalFlOdp = new ReaderWriterLockSlim();
        private readonly IJmesApiClient _jmesApiClient;
        private readonly IAs400Repository _as400Repository;

        private List<string> _odpDatiMonitor;
        private IList<Attivita>? _attivitaAperte;
        private IList<stdMesIndTsk>? _attivitaIndirette;
        private Dictionary<string, DateTime> _calFlOdpCache;

        public CaricamentoAttivitaInBackgroundService(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _attivitaAperte = new List<Attivita>();
            _attivitaIndirette = new List<stdMesIndTsk>();
            _calFlOdpCache = new Dictionary<string, DateTime>();

            using var scope = _serviceProvider.CreateScope();
            _jmesApiClient = scope.ServiceProvider.GetRequiredService<IJmesApiClient>();
            _as400Repository = scope.ServiceProvider.GetRequiredService<IAs400Repository>();

            // Aggiorna attività ogni 10 secondi
            _timer = new Timer(UpdateAttivita, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

            // Aggiorna cache CAL_FL_ODP ogni 60 secondi
            _timerCalFlOdp = new Timer(UpdateCalFlOdpCache, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
        }

        private void UpdateAttivita(object? state)
        {
            try
            {
                List<Attivita> nuoveAttivita = new List<Attivita>();

                UpdateDatiDaMonitor();
                if (_odpDatiMonitor == null)
                    return;

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

                    // Assegna DataSchedulata da cache CAL_FL_ODP
                    _lockCalFlOdp.EnterReadLock();
                    try
                    {
                        foreach (var attivita in batchAttivita)
                        {
                            if (_calFlOdpCache.TryGetValue(attivita.Odp, out var dataSchedulata))
                            {
                                attivita.DataSchedulata = dataSchedulata;
                            }
                        }
                    }
                    finally
                    {
                        _lockCalFlOdp.ExitReadLock();
                    }

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
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Errore nell'aggiornamento delle attività: {ex.Message}");
            }
        }

        private void UpdateDatiDaMonitor()
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

        private void UpdateCalFlOdpCache(object? state)
        {
            try
            {
                // Crea un nuovo scope per accedere al DbContext
                using var scope = _serviceProvider.CreateScope();
                var imarSchedulatoreUoW = scope.ServiceProvider.GetRequiredService<IImarSchedulatoreUoW>();

                // Recupera tutti i record da CAL_FL_ODP e prende il GIORNO minimo per ogni ODP
                var calFlOdpData = imarSchedulatoreUoW.CalFlOdpRepository
                    .Get()
                    .GroupBy(c => c.ODP)
                    .ToDictionary(g => g.Key, g => g.Min(x => x.GIORNO));

                _lockCalFlOdp.EnterWriteLock();
                try
                {
                    _calFlOdpCache = calFlOdpData;
                }
                finally
                {
                    _lockCalFlOdp.ExitWriteLock();
                }

                Debug.WriteLine($"Cache CAL_FL_ODP aggiornata: {_calFlOdpCache.Count} ODP trovati");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Errore nell'aggiornamento cache CAL_FL_ODP: {ex.Message}");
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
