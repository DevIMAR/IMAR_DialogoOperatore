using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class PopupTimbratureViewModel : ViewModelBase
    {
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;

        private bool _isVisibile;

        public bool IsVisible
        {
            get { return _isVisibile; }
            set
            {
                _isVisibile = value;

                OnNotifyStateChanged();
            }
        }

        public PopupTimbratureViewModel(
            IDialogoOperatoreObserver dialogoOperatoreObserver)
        {
            _dialogoOperatoreObserver = dialogoOperatoreObserver;

            _dialogoOperatoreObserver.OnOperatoreSelezionatoChanged += DialogoOperatoreObserver_OnOperatoreSelezionatoChanged;
        }

        private void DialogoOperatoreObserver_OnOperatoreSelezionatoChanged()
        {
            if (_dialogoOperatoreObserver.OperatoreSelezionato == null)
                IsVisible = false;
        }
    }
}
