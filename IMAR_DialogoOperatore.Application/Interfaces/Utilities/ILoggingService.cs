namespace IMAR_DialogoOperatore.Application.Interfaces.Utilities
{
    public interface ILoggingService
    {
        void LogError(string message, Exception? exception = null);
        void LogWarning(string message);
        void LogInfo(string message);
        void LogDebug(string message);
    }
}
