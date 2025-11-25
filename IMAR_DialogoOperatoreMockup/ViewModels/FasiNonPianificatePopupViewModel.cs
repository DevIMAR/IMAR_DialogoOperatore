using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore.Interfaces.Observers;
using System.Windows.Input;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class FasiNonPianificatePopupViewModel : ViewModelBase
    {
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
        private bool _mostraPopup;

        public ICommand ConfermaCommand { get; private set; }

        public string? ModalitaDiRiapertura => _dialogoOperatoreObserver.OperazioneInCorso.Equals(Costanti.INIZIO_LAVORO) ? "il lavoro" : "l'attrezzaggio";
        public string? FaseDaRiaprire => _dialogoOperatoreObserver.AttivitaSelezionata.Odp + " - " + _dialogoOperatoreObserver.AttivitaSelezionata.CodiceFase + ": " + _dialogoOperatoreObserver.AttivitaSelezionata.DescrizioneFase;

        public bool MostraPopup
        {
            get { return _mostraPopup; }
            set 
            {
                _mostraPopup = value; 
                OnNotifyStateChanged();
            }
        }

        public FasiNonPianificatePopupViewModel(
            CreaFaseNonPianificataCommand confermaCommand,
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
