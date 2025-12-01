using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore.Interfaces.Observers;
using System.Windows.Input;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class FasiNonPianificatePopupViewModel : PopupViewModelBase
    {
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;

        public ICommand ConfermaCommand { get; private set; }

        public string? ModalitaDiRiapertura => _dialogoOperatoreObserver.OperazioneInCorso.Equals(Costanti.INIZIO_LAVORO) ? "il lavoro" : "l'attrezzaggio";
        public string? FaseDaRiaprire => _dialogoOperatoreObserver.AttivitaSelezionata.Odp + " - " + _dialogoOperatoreObserver.AttivitaSelezionata.CodiceFase + ": " + _dialogoOperatoreObserver.AttivitaSelezionata.DescrizioneFase;

        public FasiNonPianificatePopupViewModel(
            CreaFaseNonPianificataCommand confermaCommand,
            IDialogoOperatoreObserver dialogoOperatoreObserver)
            : base(dialogoOperatoreObserver)
        {
            _dialogoOperatoreObserver = dialogoOperatoreObserver;
            ConfermaCommand = confermaCommand;
        }
    }
}
