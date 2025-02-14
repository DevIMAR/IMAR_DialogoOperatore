using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using System.Timers;

namespace IMAR_DialogoOperatore.Infrastructure.Utilities
{
    public class AutoLogoutUtility : IAutoLogoutUtility
    {
        private readonly System.Timers.Timer _timer;
        public event Action? OnLogoutTriggered;

        public bool IsActive {  get; set; }

        public AutoLogoutUtility()
        {
            _timer = new System.Timers.Timer();
            _timer.Elapsed += HandleTimerElapsed;
            _timer.AutoReset = false; 

            IsActive = false;
        }

        public void StartLogoutTimer(double durationInSeconds)
        {
            IsActive = true;

            _timer.Stop();
            _timer.Interval = durationInSeconds * 1000;
            _timer.Start();
        }

        public void RestartTimer()
        {
            _timer.Stop();
            _timer.Start();
        }

        private void HandleTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            OnLogoutTriggered?.Invoke();
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}
