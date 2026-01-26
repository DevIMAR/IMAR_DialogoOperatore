using IMAR_DialogoOperatore.Infrastructure.Utilities;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace IMAR_DialogoOperatore.Test.Logging
{
    /// <summary>
    /// Test di integrazione per verificare la configurazione completa del logging.
    /// Questi test verificano la connessione alla share di rete e la scrittura dei log.
    /// </summary>
    public class LoggingIntegrationTest : IDisposable
    {
        private readonly IConfiguration _configuration;
        private LoggingService? _loggingService;

        public LoggingIntegrationTest()
        {
            // Carica la configurazione reale da appsettings.json
            _configuration = new ConfigurationBuilder()
                .SetBasePath(GetProjectBasePath())
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            // Configura Log4Net
            var logRepository = LogManager.GetRepository(Assembly.GetExecutingAssembly());
            var configPath = Path.Combine(GetProjectBasePath(), "log4net.config");
            if (File.Exists(configPath))
            {
                XmlConfigurator.Configure(logRepository, new FileInfo(configPath));
            }
        }

        private static string GetProjectBasePath()
        {
            var currentDir = Directory.GetCurrentDirectory();
            var projectDir = Directory.GetParent(currentDir);
            while (projectDir != null && !File.Exists(Path.Combine(projectDir.FullName, "IMAR_DialogoOperatoreMockup", "appsettings.json")))
            {
                projectDir = projectDir.Parent;
            }
            return projectDir != null
                ? Path.Combine(projectDir.FullName, "IMAR_DialogoOperatoreMockup")
                : currentDir;
        }

        public void Dispose()
        {
            _loggingService?.Dispose();
        }

        [Fact]
        public void LoggingService_CanBeCreated_WithConfiguration()
        {
            // Act
            _loggingService = new LoggingService(_configuration);

            // Assert
            Assert.NotNull(_loggingService);
        }

        [Fact]
        public void LogSettings_AreConfigured_InAppSettings()
        {
            // Act
            var logSettings = _configuration.GetSection("LogSettings");
            var networkPath = logSettings["NetworkPath"];

            // Assert
            Assert.NotNull(logSettings);
            Assert.False(string.IsNullOrEmpty(networkPath), "NetworkPath should be configured in appsettings.json");
        }

        [Fact]
        public void NetworkShareConnector_CanConnect_WithValidCredentials()
        {
            // Arrange
            var logSettings = _configuration.GetSection("LogSettings");
            var networkPath = logSettings["NetworkPath"];
            var username = logSettings["Username"];
            var password = logSettings["Password"];

            // Skip test if credentials are not configured
            if (string.IsNullOrEmpty(networkPath) ||
                string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(password))
            {
                return; // Skip silently
            }

            // Estrai solo il percorso della cartella (senza il nome del file)
            var directoryPath = Path.GetDirectoryName(networkPath);
            if (string.IsNullOrEmpty(directoryPath))
            {
                return;
            }

            // Act & Assert
            NetworkShareConnector? connector = null;
            var exception = Record.Exception(() =>
            {
                connector = new NetworkShareConnector(directoryPath, username, password);
            });

            // Assert
            Assert.Null(exception);
            connector?.Dispose();
        }

        [Fact]
        public void LoggingService_CanWriteLog_ToNetworkShare()
        {
            // Arrange
            var logSettings = _configuration.GetSection("LogSettings");
            var networkPath = logSettings["NetworkPath"];
            var username = logSettings["Username"];
            var password = logSettings["Password"];

            // Skip test if credentials are not configured
            if (string.IsNullOrEmpty(networkPath) ||
                string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(password))
            {
                return; // Skip silently
            }

            _loggingService = new LoggingService(_configuration);
            var testMessage = $"[TEST] Log di verifica - {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

            // Act
            var exception = Record.Exception(() =>
            {
                _loggingService.LogInfo(testMessage);
                _loggingService.LogWarning($"{testMessage} - Warning");
                _loggingService.LogError($"{testMessage} - Error");
                _loggingService.LogError($"{testMessage} - Error with exception", new Exception("Test exception"));
            });

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void LoggingService_HandlesNetworkShareConnectionFailure_Gracefully()
        {
            // Arrange - Configurazione con credenziali errate
            var invalidSettings = new Dictionary<string, string?>
            {
                { "LogSettings:NetworkPath", "\\\\invalid-server\\invalid-share" },
                { "LogSettings:Username", "invalid-user" },
                { "LogSettings:Password", "invalid-password" }
            };

            var invalidConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(invalidSettings)
                .Build();

            // Act - Il servizio non deve lanciare eccezioni anche con credenziali errate
            var exception = Record.Exception(() =>
            {
                _loggingService = new LoggingService(invalidConfig);
            });

            // Assert - Il servizio viene creato anche se la connessione fallisce
            Assert.Null(exception);
            Assert.NotNull(_loggingService);
        }

        [Fact]
        public void LoggingService_WorksWithoutNetworkConfiguration()
        {
            // Arrange - Configurazione senza credenziali di rete
            var emptySettings = new Dictionary<string, string?>
            {
                { "LogSettings:NetworkPath", "" },
                { "LogSettings:Username", "" },
                { "LogSettings:Password", "" }
            };

            var emptyConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(emptySettings)
                .Build();

            // Act
            _loggingService = new LoggingService(emptyConfig);
            var exception = Record.Exception(() =>
            {
                _loggingService.LogInfo("Test log without network share");
            });

            // Assert
            Assert.Null(exception);
        }
    }
}
