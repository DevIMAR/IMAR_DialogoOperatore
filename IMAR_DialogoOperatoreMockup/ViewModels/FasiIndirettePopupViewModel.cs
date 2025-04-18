using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore.Interfaces.Observers;
using System.Windows.Input;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class FasiIndirettePopupViewModel : ViewModelBase
    {
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
        private bool _mostraPopup;

        public ICommand ConfermaCommand { get; private set; }

        public bool MostraPopup
        {
            get { return _mostraPopup; }
            set 
            {
                _mostraPopup = value; 
                OnNotifyStateChanged();
            }
        }

        public FasiIndirettePopupViewModel(
            ConfermaCommand confermaCommand,
            IDialogoOperatoreObserver dialogoOperatoreObserver)
        {
            _dialogoOperatoreObserver = dialogoOperatoreObserver;
            ConfermaCommand = confermaCommand;

            _dialogoOperatoreObserver.OnOperatoreSelezionatoChanged += DialogoOperatoreObserver_OnOperatoreSelezionatoChanged;
        }

        private void DialogoOperatoreObserver_OnOperatoreSelezionatoChanged()
        {
            if (_dialogoOperatoreObserver.OperatoreSelezionato == null)
                MostraPopup = false;
        }
    }
}
