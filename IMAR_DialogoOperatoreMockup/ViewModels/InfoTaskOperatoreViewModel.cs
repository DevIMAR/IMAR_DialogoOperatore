using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class InfoTaskOperatoreViewModel : ViewModelBase
    {
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;

        private string _tipologiaTimbraturaErrataSelezionata;
        private TimeSpan _orarioDaCorreggere;
        private TimeSpan _orarioDaDichiarare;

        public string NomeCognomeOperatore => _dialogoOperatoreObserver.OperatoreSelezionato.Nome + " " + _dialogoOperatoreObserver.OperatoreSelezionato.Cognome;
        public int BadgeOperatore => (int)_dialogoOperatoreObserver.OperatoreSelezionato.Badge;
        public List<string> TipologieTimbraturaErrata => new List<string> { "Entrata", "Uscita", "Inizio pausa", "Fine pausa", "Inizio lavoro", "Fine lavoro", "Inizio attrezzaggio", "Fine attrezzaggio" };

        public string TipologiaTimbraturaErrataSelezionata
        {
            get { return _tipologiaTimbraturaErrataSelezionata; }
            set 
            { 
                _tipologiaTimbraturaErrataSelezionata = value; 
                OnNotifyStateChanged();
            }
        }
        public TimeSpan OrarioDaCorreggere
        {
            get { return _orarioDaCorreggere; }
            set 
            { 
                _orarioDaCorreggere = value; 
                OnNotifyStateChanged();
            }
        }
        public TimeSpan OrarioDaDichiarare
        {
            get { return _orarioDaDichiarare; }
            set 
            {
                _orarioDaDichiarare = value; 
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
