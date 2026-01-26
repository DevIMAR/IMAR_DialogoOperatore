using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using IMAR_DialogoOperatore.Infrastructure.Utilities;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;
using Microsoft.Extensions.Configuration;

namespace IMAR_DialogoOperatore.Test.Logging
{
    public class LoggingServiceTest : IDisposable
    {
        private readonly MemoryAppender _memoryAppender;
        private readonly ILoggingService _loggingService;
        private readonly IConfiguration _configuration;

        public LoggingServiceTest()
        {
            // Configura Log4Net con un MemoryAppender per catturare i log in memoria
            _memoryAppender = new MemoryAppender();
            _memoryAppender.Threshold = Level.All;

            // Usa il repository dell'assembly Infrastructure dove risiede LoggingService
            var infrastructureAssembly = typeof(LoggingService).Assembly;
            var hierarchy = (Hierarchy)LogManager.GetRepository(infrastructureAssembly);
            hierarchy.Root.AddAppender(_memoryAppender);
            hierarchy.Root.Level = Level.All;
            hierarchy.Configured = true;

            // Mock della configurazione senza credenziali di rete (per test locali)
            var inMemorySettings = new Dictionary<string, string?>
            {
                { "LogSettings:NetworkPath", "" },
                { "LogSettings:Username", "" },
                { "LogSettings:Password", "" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _loggingService = new LoggingService(_configuration);
        }

        public void Dispose()
        {
            _memoryAppender.Clear();
            var infrastructureAssembly = typeof(LoggingService).Assembly;
            var hierarchy = (Hierarchy)LogManager.GetRepository(infrastructureAssembly);
            hierarchy.Root.RemoveAppender(_memoryAppender);
        }

        [Fact]
        public void LogError_WithMessage_LogsErrorLevel()
        {
            // Arrange
            var message = "Test error message";

            // Act
            _loggingService.LogError(message);

            // Assert
            var events = _memoryAppender.GetEvents();
            Assert.Contains(events, e => e.Level == Level.Error && e.RenderedMessage.Contains(message));
        }

        [Fact]
        public void LogError_WithException_LogsErrorWithException()
        {
            // Arrange
            var message = "Test error with exception";
            var exception = new InvalidOperationException("Test exception");

            // Act
            _loggingService.LogError(message, exception);

            // Assert
            var events = _memoryAppender.GetEvents();
            Assert.Contains(events, e =>
                e.Level == Level.Error &&
                e.RenderedMessage.Contains(message) &&
                e.ExceptionObject != null &&
                e.ExceptionObject.Message == "Test exception");
        }

        [Fact]
        public void LogWarning_LogsWarningLevel()
        {
            // Arrange
            var message = "Test warning message";

            // Act
            _loggingService.LogWarning(message);

            // Assert
            var events = _memoryAppender.GetEvents();
            Assert.Contains(events, e => e.Level == Level.Warn && e.RenderedMessage.Contains(message));
        }

        [Fact]
        public void LogInfo_LogsInfoLevel()
        {
            // Arrange
            var message = "Test info message";

            // Act
            _loggingService.LogInfo(message);

            // Assert
            var events = _memoryAppender.GetEvents();
            Assert.Contains(events, e => e.Level == Level.Info && e.RenderedMessage.Contains(message));
        }

        [Fact]
        public void LogDebug_LogsDebugLevel()
        {
            // Arrange
            var message = "Test debug message";

            // Act
            _loggingService.LogDebug(message);

            // Assert
            var events = _memoryAppender.GetEvents();
            Assert.Contains(events, e => e.Level == Level.Debug && e.RenderedMessage.Contains(message));
        }

        [Fact]
        public void LoggingService_ImplementsIDisposable()
        {
            // Assert
            Assert.True(_loggingService is IDisposable);
        }

        [Fact]
        public void LogError_WithNullException_LogsOnlyMessage()
        {
            // Arrange
            var message = "Error without exception";

            // Act
            _loggingService.LogError(message, null);

            // Assert
            var events = _memoryAppender.GetEvents();
            var logEvent = events.FirstOrDefault(e => e.Level == Level.Error && e.RenderedMessage.Contains(message));
            Assert.NotNull(logEvent);
            Assert.Null(logEvent.ExceptionObject);
        }
    }
}
