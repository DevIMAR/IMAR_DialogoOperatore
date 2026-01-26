using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using log4net;
using Microsoft.Extensions.Configuration;

namespace IMAR_DialogoOperatore.Infrastructure.Utilities
{
    public class LoggingService : ILoggingService, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(LoggingService));
        private readonly NetworkShareConnector? _networkConnector;
        private bool _disposed;

        public LoggingService(IConfiguration configuration)
        {
            var logSettings = configuration.GetSection("LogSettings");
            var networkPath = logSettings["NetworkPath"];
            var username = logSettings["Username"];
            var password = logSettings["Password"];

            if (!string.IsNullOrEmpty(networkPath) &&
                !string.IsNullOrEmpty(username) &&
                !string.IsNullOrEmpty(password))
            {
                try
                {
                    _networkConnector = new NetworkShareConnector(networkPath, username, password);
                }
                catch (Exception ex)
                {
                    // Log locale in caso di fallimento connessione alla share
                    _log.Error($"Impossibile connettersi alla share di rete: {networkPath}", ex);
                }
            }
        }

        public void LogError(string message, Exception? exception = null)
        {
            if (exception != null)
                _log.Error(message, exception);
            else
                _log.Error(message);
        }

        public void LogWarning(string message)
        {
            _log.Warn(message);
        }

        public void LogInfo(string message)
        {
            _log.Info(message);
        }

        public void LogDebug(string message)
        {
            _log.Debug(message);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _networkConnector?.Dispose();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
