namespace IMAR_DialogoOperatore.Application.Interfaces.Utilities
{
    public interface IAutoLogoutUtility
    {
        public bool IsActive { get; set; }

        void StartLogoutTimer(double durationInSeconds);
        void RestartTimer();
        event Action? OnLogoutTriggered;
    }
}
