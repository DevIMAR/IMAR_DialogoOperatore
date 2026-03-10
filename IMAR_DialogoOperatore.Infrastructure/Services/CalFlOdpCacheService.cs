using IMAR_DialogoOperatore.Application.Interfaces.UoW;
using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace IMAR_DialogoOperatore.Infrastructure.Services
{
	public class CalFlOdpCacheService : IDisposable
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly ReaderWriterLockSlim _lock = new();
		private readonly CancellationTokenSource _cts = new();
		private readonly SemaphoreSlim _semaphore = new(1, 1);
		private Dictionary<(string, string), DateTime> _cache = new();

		public CalFlOdpCacheService(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
			_ = Task.Run(() => RunLoopAsync(_cts.Token));
		}

		private async Task RunLoopAsync(CancellationToken cancellationToken)
		{
			UpdateCache();

			using var timer = new PeriodicTimer(TimeSpan.FromSeconds(60));
			while (await timer.WaitForNextTickAsync(cancellationToken))
			{
				UpdateCache();
			}
		}

		private void UpdateCache()
		{
			if (!_semaphore.Wait(0))
				return;

			try
			{
				using var scope = _serviceProvider.CreateScope();
				var imarSchedulatoreUoW = scope.ServiceProvider.GetRequiredService<IImarSchedulatoreUoW>();
				var loggingService = scope.ServiceProvider.GetRequiredService<ILoggingService>();

				var sw = Stopwatch.StartNew();

				var calFlOdpData = imarSchedulatoreUoW.CalFlOdpRepository
					.Get()
					.GroupBy(c => new { c.ODP, c.FASE })
					.Select(g => new { g.Key.ODP, g.Key.FASE, MinGiorno = g.Min(x => x.GIORNO) })
					.AsEnumerable()
					.ToDictionary(x => (x.ODP, x.FASE.ToString().PadLeft(3, '0')), x => x.MinGiorno);

				_lock.EnterWriteLock();
				try
				{
					_cache = calFlOdpData;
				}
				finally
				{
					_lock.ExitWriteLock();
				}

				loggingService.LogInfo($"CalFlOdpCacheService.UpdateCache completato in {sw.ElapsedMilliseconds}ms");
			}
			catch (Exception ex)
			{
				try
				{
					using var scope = _serviceProvider.CreateScope();
					var loggingService = scope.ServiceProvider.GetRequiredService<ILoggingService>();
					loggingService.LogError("Errore nell'aggiornamento cache CalFlOdp", ex);
				}
				catch { }
			}
			finally
			{
				_semaphore.Release();
			}
		}

		public DateTime? GetDataSchedulata(string odp, string fase)
		{
			_lock.EnterReadLock();
			try
			{
				return _cache.TryGetValue((odp, fase), out var data) ? data : null;
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
			_semaphore.Dispose();
		}
	}
}
