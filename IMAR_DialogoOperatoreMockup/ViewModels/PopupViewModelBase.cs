using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class PopupViewModelBase : ViewModelBase
    {
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;

        private bool _isVisibile;

        virtual public bool IsVisible
        {
            get { return _isVisibile; }
            set
            {
                _isVisibile = value;

                OnNotifyStateChanged();
            }
        }

        public PopupViewModelBase(
            IDialogoOperatoreObserver dialogoOperatoreObserver)
        {
            _dialogoOperatoreObserver = dialogoOperatoreObserver;

            _dialogoOperatoreObserver.OnOperatoreSelezionatoChanged += DialogoOperatoreObserver_OnOperatoreSelezionatoChanged;
        }

        public virtual void DialogoOperatoreObserver_OnOperatoreSelezionatoChanged()
        {
            if (_dialogoOperatoreObserver.OperatoreSelezionato == null)
                IsVisible = false;
        }
    }
}
