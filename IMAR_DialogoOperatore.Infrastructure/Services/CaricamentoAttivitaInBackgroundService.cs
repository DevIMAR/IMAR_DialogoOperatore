using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using IMAR_DialogoOperatore.Application.Interfaces.UoW;
using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using IMAR_DialogoOperatore.Domain.Models;
using Microsoft.Extensions.DependencyInjection;

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
        private readonly ILoggingService _loggingService;

        private List<string> _odpDatiMonitor;
        private IList<Attivita>? _attivitaAperte;
        private IList<stdMesIndTsk>? _attivitaIndirette;
        private Dictionary<(string, string), DateTime> _calFlOdpCache;

        public CaricamentoAttivitaInBackgroundService(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _attivitaAperte = new List<Attivita>();
            _attivitaIndirette = new List<stdMesIndTsk>();
            _calFlOdpCache = new Dictionary<(string, string), DateTime>();

            using var scope = _serviceProvider.CreateScope();
            _jmesApiClient = scope.ServiceProvider.GetRequiredService<IJmesApiClient>();
            _as400Repository = scope.ServiceProvider.GetRequiredService<IAs400Repository>();
            _loggingService = scope.ServiceProvider.GetRequiredService<ILoggingService>();

            // Aggiorna attività ogni 10 secondi
            _timer = new Timer(UpdateAttivita, null, TimeSpan.Zero, TimeSpan.FromSeconds(15));

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

                    var batchAttivita = _as400Repository.ExecuteQuery<Attivita>(@"WITH
                                                                                    NONCONTABILIZZATE AS (
                                                                                        SELECT
                                                                                            NRTSKJM,
                                                                                            SUM(QTVERJM) AS TOT_QTA_NC,
                                                                                            SUM(QTSCAJM) AS TOT_SCARTO_NC,
                                                                                            SUM(QTNCOJM) AS TOT_NONCONF_NC,
                                                                                            MAX(SAACCJM) AS MAX_SAACCJM
                                                                                        FROM IMA90DAT.JMRILM00F
                                                                                        WHERE QCONTJM = ''
                                                                                        GROUP BY NRTSKJM
                                                                                    ),
                                                                                    FASE_PREC AS (
                                                                                        SELECT
                                                                                            p1.NRBLCI,
                                                                                            p1.ORPRCI,
                                                                                            p1.CDFACI,
                                                                                            prec.NRBLCI AS NRBLCI_PREV,
                                                                                            prec.TIRECI AS TIRECI_PREV,
                                                                                            prec.QPROCI AS QPROCI_PREV,
                                                                                            nc_prec.TOT_QTA_NC AS QTA_NC_PREV,
                                                                                            nc_prec.MAX_SAACCJM AS SAACCJM_PREV,
                                                                                            ROW_NUMBER() OVER (PARTITION BY p1.NRBLCI ORDER BY prec.CDFACI DESC) AS RN
                                                                                        FROM IMA90DAT.PCIMP00F p1
                                                                                        INNER JOIN IMA90DAT.PCIMP00F prec
                                                                                            ON prec.ORPRCI = p1.ORPRCI
                                                                                            AND prec.CDFACI < p1.CDFACI
                                                                                        LEFT JOIN NONCONTABILIZZATE nc_prec
                                                                                            ON nc_prec.NRTSKJM = prec.NRBLCI
                                                                                        WHERE p1.ORPRCI IN (" + batchOdp + @")
                                                                                    )
                                                                                    SELECT
                                                                                        pf2.NRBLCI AS BOLLA,
                                                                                        pf2.ORPRCI AS ODP,
                                                                                        pf2.CDARCI AS ARTICOLO,
                                                                                        TRIM(mf.DSARMA) AS DESCRIZIONEARTICOLO,
                                                                                        pf2.CDFACI AS FASE,
                                                                                        pf2.DSFACI AS DESCRIZIONEFASE,
                                                                                        CASE
                                                                                            WHEN fp.NRBLCI_PREV IS NULL THEN pf2.QORDCI
                                                                                            WHEN fp.TIRECI_PREV = 'S' OR COALESCE(fp.SAACCJM_PREV, '') = 'S'
                                                                                                THEN COALESCE(fp.QPROCI_PREV, 0) + COALESCE(fp.QTA_NC_PREV, 0)
                                                                                            ELSE pf2.QORDCI
                                                                                        END AS QUANTITAORDINE,
                                                                                        COALESCE(ra.TOT_QTA_NC, 0) AS QUANTITAPRODOTTANONCONTABILIZZATA,
                                                                                        pf2.QPROCI AS QUANTITAPRODOTTACONTABILIZZATA,
                                                                                        COALESCE(ra.TOT_SCARTO_NC, 0) AS QUANTITASCARTATANONCONTABILIZZATA,
                                                                                        pf2.QSCACI AS QUANTITASCARTATACONTABILIZZATA,
                                                                                        COALESCE(ra.TOT_NONCONF_NC, 0) AS QTANONCONFORMENONCONTABILIZZATA,
                                                                                        pf2.QRESCI AS QTANONCONFORMECONTABILIZZATA,
                                                                                        CASE
                                                                                            WHEN ra.MAX_SAACCJM IS NULL THEN pf2.TIRECI
                                                                                            ELSE ra.MAX_SAACCJM
                                                                                        END AS SALDOACCONTO   
                                                                                    FROM IMA90DAT.PCIMP00F pf2
                                                                                    JOIN IMA90DAT.MGART00F mf ON pf2.CDARCI = mf.CDARMA
                                                                                    LEFT JOIN NONCONTABILIZZATE ra ON ra.NRTSKJM = pf2.NRBLCI
                                                                                    LEFT JOIN FASE_PREC fp ON fp.NRBLCI = pf2.NRBLCI AND fp.RN = 1
                                                                                    WHERE pf2.ORPRCI IN (" + batchOdp + ")");

                    _lockCalFlOdp.EnterReadLock();
                    try
                    {
                        foreach (var attivita in batchAttivita)
                        {
                            if (_calFlOdpCache.TryGetValue((attivita.Odp, attivita.Fase), out var dataSchedulata))
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
                _loggingService.LogError("Errore nell'aggiornamento delle attività in background", ex);
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
                using var scope = _serviceProvider.CreateScope();
                var imarSchedulatoreUoW = scope.ServiceProvider.GetRequiredService<IImarSchedulatoreUoW>();

                var calFlOdpData = imarSchedulatoreUoW.CalFlOdpRepository
                    .Get()
                    .GroupBy(c => new { c.ODP, c.FASE })
                    .ToDictionary(g => (g.Key.ODP, g.Key.FASE.ToString().PadLeft(3, '0')), g => g.Min(x => x.GIORNO));

                _lockCalFlOdp.EnterWriteLock();
                try
                {
                    _calFlOdpCache = calFlOdpData;
                }
                finally
                {
                    _lockCalFlOdp.ExitWriteLock();
                }
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Errore nell'aggiornamento cache CalFlOdp", ex);
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
