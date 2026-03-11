using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using IMAR_DialogoOperatore.Domain.Entities.As400;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Infrastructure.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace IMAR_DialogoOperatore.Infrastructure.Services
{
	public class CaricamentoAttivitaInBackgroundService : IDisposable
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly CalFlOdpCacheService _calFlOdpCacheService;
		private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

		private readonly CancellationTokenSource _cts = new();
		private readonly SemaphoreSlim _attivitaSemaphore = new(1, 1);

		private List<string> _odpDatiMonitor;
		private IList<Attivita>? _attivitaAperte;
		private IList<stdMesIndTsk>? _attivitaIndirette;

		/// <summary>
		/// Indica se il primo caricamento dati è completato.
		/// Finché è false, la UI deve mostrare il loader.
		/// </summary>
		public bool IsReady { get; private set; }

		public CaricamentoAttivitaInBackgroundService(
			IServiceProvider serviceProvider,
			CalFlOdpCacheService calFlOdpCacheService)
		{
			_serviceProvider = serviceProvider;
			_calFlOdpCacheService = calFlOdpCacheService;
			_attivitaAperte = new List<Attivita>();
			_attivitaIndirette = new List<stdMesIndTsk>();

			_ = Task.Run(() => RunAttivitaLoopAsync(_cts.Token));
		}

		private async Task RunAttivitaLoopAsync(CancellationToken cancellationToken)
		{
			// Prima esecuzione immediata
			await UpdateAttivitaAsync();
			IsReady = true;

			using var timer = new PeriodicTimer(TimeSpan.FromSeconds(20));
			while (await timer.WaitForNextTickAsync(cancellationToken))
			{
				await UpdateAttivitaAsync();
			}
		}

		private async Task UpdateAttivitaAsync()
		{
			if (!await _attivitaSemaphore.WaitAsync(0))
				return;

			try
			{
				var sw = Stopwatch.StartNew();

				using var scope = _serviceProvider.CreateScope();
				var jmesApiClient = scope.ServiceProvider.GetRequiredService<IJmesApiClient>();
				var as400Repository = scope.ServiceProvider.GetRequiredService<IAs400Repository>();
				var loggingService = scope.ServiceProvider.GetRequiredService<ILoggingService>();

				UpdateDatiDaMonitor(as400Repository);
				loggingService.LogInfo($"  [TIMING] UpdateDatiDaMonitor: {sw.ElapsedMilliseconds}ms ({_odpDatiMonitor?.Count ?? 0} ODP)");
				if (_odpDatiMonitor == null)
					return;

				var nuoveAttivita = CaricaAttivitaDaAs400InBatch(as400Repository, loggingService);
				var attivitaIndirette = await CaricaAttivitaIndiretteAsync(jmesApiClient, loggingService);

				AggiornaCacheAttivita(nuoveAttivita, attivitaIndirette);

				loggingService.LogInfo($"CaricamentoAttivitaInBackgroundService.UpdateAttivitaAsync completato in {sw.ElapsedMilliseconds}ms ({nuoveAttivita.Count} attività)");
			}
			catch (Exception ex)
			{
				try
				{
					using var scope = _serviceProvider.CreateScope();
					var loggingService = scope.ServiceProvider.GetRequiredService<ILoggingService>();
					loggingService.LogError("Errore nell'aggiornamento delle attività in background", ex);
				}
				catch { }
			}
			finally
			{
				_attivitaSemaphore.Release();
			}
		}

		private List<Attivita> CaricaAttivitaDaAs400InBatch(IAs400Repository as400Repository, ILoggingService loggingService)
		{
			List<Attivita> nuoveAttivita = new List<Attivita>();

			decimal dimensioneBatch = 1000;
			decimal batchCount = Math.Ceiling(_odpDatiMonitor.Count / dimensioneBatch);

			for (int i = 0; i < batchCount; i++)
			{
				var batchConsiderato = Math.Min((int)dimensioneBatch, _odpDatiMonitor.Count - (int)dimensioneBatch * i);
				var batchOdp = string.Join(',', _odpDatiMonitor.Slice((int)dimensioneBatch * i, batchConsiderato));

				var swBatch = Stopwatch.StartNew();
				var batchAttivita = EseguiQueryAttivitaAs400(as400Repository, batchOdp);
				loggingService.LogInfo($"  [TIMING] Batch {i + 1}/{(int)batchCount} query AS400: {swBatch.ElapsedMilliseconds}ms ({batchAttivita.Count} attività, {batchConsiderato} ODP)");

				swBatch.Restart();
				QuantitaOrdineCalculator.Calcola(batchAttivita);
				loggingService.LogInfo($"  [TIMING] Batch {i + 1}/{(int)batchCount} calcolo QuantitaOrdine C#: {swBatch.ElapsedMilliseconds}ms");

				ArricchisciConDateSchedulate(batchAttivita);
				nuoveAttivita.AddRange(batchAttivita);
			}

			return nuoveAttivita;
		}

		// NB: batchOdp è costruito da ODP provenienti da query AS400 interna, non da input utente.
		// ODBC non supporta array parameters per IN(), quindi la concatenazione è accettabile qui.
		private List<Attivita> EseguiQueryAttivitaAs400(IAs400Repository as400Repository, string batchOdp)
		{
			return as400Repository.ExecuteQuery<Attivita>(@"WITH
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
																			)
																			SELECT
																				pf2.NRBLCI AS BOLLA,
																				pf2.ORPRCI AS ODP,
																				pf2.CDARCI AS ARTICOLO,
																				TRIM(mf.DSARMA) AS DESCRIZIONEARTICOLO,
																				pf2.CDFACI AS FASE,
																				pf2.DSFACI AS DESCRIZIONEFASE,
																				pf2.QORDCI AS QUANTITAORDINEORIGINALE,
																				0 AS QUANTITAORDINE,
																				COALESCE(ra.TOT_QTA_NC, 0) AS QUANTITAPRODOTTANONCONTABILIZZATA,
																				pf2.QPROCI AS QUANTITAPRODOTTACONTABILIZZATA,
																				COALESCE(ra.TOT_SCARTO_NC, 0) AS QUANTITASCARTATANONCONTABILIZZATA,
																				pf2.QSCACI AS QUANTITASCARTATACONTABILIZZATA,
																				COALESCE(ra.TOT_NONCONF_NC, 0) AS QTANONCONFORMENONCONTABILIZZATA,
																				pf2.QRESCI AS QTANONCONFORMECONTABILIZZATA,
																				pf2.TIRECI AS TIPORICEVIMENTO,
																				CASE WHEN ra.MAX_SAACCJM IS NULL THEN pf2.TIRECI ELSE ra.MAX_SAACCJM END AS SALDOACCONTO,
																				ra.MAX_SAACCJM AS SALDOACCONTOJMES,
																				pf2.FLSFCI AS ISNONPIANIFICATA,
																			CASE WHEN TRIM(pf2.CDRICI) = '' THEN '09' ELSE TRIM(LEFT(pf2.CDRICI, 2)) END AS FLUSSO
																			FROM IMA90DAT.PCIMP00F pf2
																			JOIN IMA90DAT.MGART00F mf ON pf2.CDARCI = mf.CDARMA
																			LEFT JOIN NONCONTABILIZZATE ra ON ra.NRTSKJM = pf2.NRBLCI
																			WHERE pf2.ORPRCI IN(" + batchOdp + ")").ToList();
		}

		private void ArricchisciConDateSchedulate(List<Attivita> attivita)
		{
			foreach (var a in attivita)
			{
				var dataSchedulata = _calFlOdpCacheService.GetDataSchedulata(a.Odp, a.Fase);
				if (dataSchedulata.HasValue)
					a.DataSchedulata = dataSchedulata.Value;
			}
		}

		private async Task<IList<stdMesIndTsk>?> CaricaAttivitaIndiretteAsync(IJmesApiClient jmesApiClient, ILoggingService loggingService)
		{
			var sw = Stopwatch.StartNew();
			var attivitaIndirette = await jmesApiClient.ChiamaQueryGetJmesAsync<stdMesIndTsk>();
			loggingService.LogInfo($"  [TIMING] Attività indirette JMes: {sw.ElapsedMilliseconds}ms");
			return attivitaIndirette;
		}

		private void AggiornaCacheAttivita(List<Attivita> nuoveAttivita, IList<stdMesIndTsk>? attivitaIndirette)
		{
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

		private void UpdateDatiDaMonitor(IAs400Repository as400Repository)
		{
			_lock.EnterWriteLock();
			try
			{
				_odpDatiMonitor = as400Repository.ExecuteQuery<string>(@"SELECT DISTINCT(ORPRCI)
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

		public void Dispose()
		{
			_cts.Cancel();
			_cts.Dispose();
			_lock.Dispose();
			_attivitaSemaphore.Dispose();
		}
	}
}
