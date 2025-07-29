using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class InfoTaskOperatoreViewModel : ViewModelBase
    {
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;

        private int _oraDaDichiarare;
        private int _minutoDaDichiarare;

        public string NomeCognomeOperatore => _dialogoOperatoreObserver.OperatoreSelezionato.Nome + " " + _dialogoOperatoreObserver.OperatoreSelezionato.Cognome;
        public int BadgeOperatore => (int)_dialogoOperatoreObserver.OperatoreSelezionato.Badge;

        public int OraDaDichiarare
        {
            get { return _oraDaDichiarare; }
            set 
            {
                _oraDaDichiarare = value; 
                OnNotifyStateChanged();
            }
        }
        public int MinutoDaDichiarare
        {
            get { return _minutoDaDichiarare; }
            set 
            {
                _minutoDaDichiarare = value; 
                OnNotifyStateChanged();
            }
        }

        public InfoTaskOperatoreViewModel(
            IDialogoOperatoreObserver dialogoOperatoreObserver)
        {
            _dialogoOperatoreObserver = dialogoOperatoreObserver;
        }
    }
}
