using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore.Interfaces.Observers;
using System.Windows.Input;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class HeaderToolbarViewModel : ViewModelBase
    {
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;

        public ICommand ShowTaskPopupCommand { get; }
        public ICommand ShowEntrateUscitePauseCommand { get; }

        public HeaderToolbarViewModel(
            ShowTaskPopupCommand showTaskPopupCommand,
            ShowEntrateUscitePauseCommand showEntrateUscitePauseCommand,
            IDialogoOperatoreObserver dialogoOperatoreObserver)
        {
            ShowTaskPopupCommand = showTaskPopupCommand;
            ShowEntrateUscitePauseCommand = showEntrateUscitePauseCommand;
            _dialogoOperatoreObserver = dialogoOperatoreObserver;

            _dialogoOperatoreObserver.OnOperatoreSelezionatoChanged += DialogoOperatoreObserver_OnOperatoreSelezionatoChanged;
        }

        private void DialogoOperatoreObserver_OnOperatoreSelezionatoChanged()
        {
            if (_dialogoOperatoreObserver.OperatoreSelezionato == null)
                ShowTaskPopupCommand.Execute(false);
   
            OnNotifyStateChanged();
        }
    }
}
