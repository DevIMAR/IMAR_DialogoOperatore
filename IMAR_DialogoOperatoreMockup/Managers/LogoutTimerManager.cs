using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.Managers
{
    public class LogoutTimerManager : IDisposable
    {
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
        private readonly ITaskCompilerObserver _taskCompilerObserver;
        private readonly IAutoLogoutUtility _autoLogoutUtility;

        public LogoutTimerManager(
            IDialogoOperatoreObserver dialogoOperatoreObserver,
            ITaskCompilerObserver taskCompilerObserver,
            IAutoLogoutUtility autoLogoutUtility)
        {
            _dialogoOperatoreObserver = dialogoOperatoreObserver;
            _taskCompilerObserver = taskCompilerObserver;

            _autoLogoutUtility = autoLogoutUtility;

            _dialogoOperatoreObserver.OnOperatoreSelezionatoChanged += DialogoOperatoreObserver_OnOperatoreSelezionatoChanged;
            _dialogoOperatoreObserver.OnIsDettaglioAttivitaOpenChanged += DialogoOperatoreObserver_OnIsDettaglioAttivitaOpenChanged;
            _taskCompilerObserver.OnIsPopupVisibleChanged += TaskCompilerObserver_OnIsPopupVisibleChanged;
        }

        private void DialogoOperatoreObserver_OnOperatoreSelezionatoChanged()
        {
            if (_dialogoOperatoreObserver.OperatoreSelezionato != null)
                _autoLogoutUtility.StartLogoutTimer(30);
        }

        private void DialogoOperatoreObserver_OnIsDettaglioAttivitaOpenChanged()
        {
            if (_dialogoOperatoreObserver.IsDettaglioAttivitaOpen)
                _autoLogoutUtility.StartLogoutTimer(300);
            else
                _autoLogoutUtility.StartLogoutTimer(30);
        }

        private void TaskCompilerObserver_OnIsPopupVisibleChanged()
        {
            if (_taskCompilerObserver.IsPopupVisible)
                _autoLogoutUtility.StartLogoutTimer(300);
            else
                if (!_dialogoOperatoreObserver.IsDettaglioAttivitaOpen)
                    _autoLogoutUtility.StartLogoutTimer(30);
        }

        public void Dispose()
        {
            _dialogoOperatoreObserver.OnOperatoreSelezionatoChanged -= DialogoOperatoreObserver_OnOperatoreSelezionatoChanged;
            _dialogoOperatoreObserver.OnIsDettaglioAttivitaOpenChanged -= DialogoOperatoreObserver_OnIsDettaglioAttivitaOpenChanged;
            _taskCompilerObserver.OnIsPopupVisibleChanged -= TaskCompilerObserver_OnIsPopupVisibleChanged;
        }
    }
}
