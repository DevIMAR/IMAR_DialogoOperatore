using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Interfaces.ViewModels;
using System.Windows.Input;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class GestoreAttivitaViewModel : ViewModelBase
	{
		private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
		private readonly ICercaAttivitaObserver _cercaAttivitaObserver;

		public bool IsOpen => IsOpenCondition();
		public string? OperazioneInCorso => _dialogoOperatoreObserver.OperazioneInCorso;

		public ICommand ConfermaCommand { get; set; }
		public ICommand CreaFaseNonPianificataCommand { get; set; }

		public GestoreAttivitaViewModel(
			IDialogoOperatoreObserver dialogoOperatoreObserver,
			ICercaAttivitaObserver cercaAttivitaObserver,
			ConfermaCommand confermaCommand,
			MostraFasiNonPianificatePopupCommand creaFaseNonPianificataCommand)
        {
			_dialogoOperatoreObserver = dialogoOperatoreObserver;
			_cercaAttivitaObserver = cercaAttivitaObserver;

			ConfermaCommand = confermaCommand;
			CreaFaseNonPianificataCommand = creaFaseNonPianificataCommand;


            _dialogoOperatoreObserver.OnAttivitaSelezionataChanged += AttivitaStore_OnAttivitaSelezionataChanged;
			_dialogoOperatoreObserver.OnOperazioneInCorsoChanged += DialogoOperatoreStore_OnOperazioneInCorsoChanged;
            _dialogoOperatoreObserver.OnOperatoreSelezionatoChanged += DialogoOperatoreObserver_OnOperatoreSelezionatoChanged;
		}

        private void DialogoOperatoreStore_OnOperazioneInCorsoChanged()
		{
			if (!(_dialogoOperatoreObserver.OperazioneInCorso == Costanti.AVANZAMENTO || 
					_dialogoOperatoreObserver.OperazioneInCorso == Costanti.INIZIO_LAVORO || 
					_dialogoOperatoreObserver.OperazioneInCorso == Costanti.INIZIO_ATTREZZAGGIO))
				_cercaAttivitaObserver.IsAttivitaCercata = false;

            _dialogoOperatoreObserver.IsDettaglioAttivitaOpen = IsOpen;

            OnNotifyStateChanged();
		}

        private void DialogoOperatoreObserver_OnOperatoreSelezionatoChanged()
        {
            _dialogoOperatoreObserver.IsDettaglioAttivitaOpen = IsOpen;
            OnNotifyStateChanged();
        }

		private void AttivitaStore_OnAttivitaSelezionataChanged()
		{
			_dialogoOperatoreObserver.IsDettaglioAttivitaOpen = IsOpen;
			OnNotifyStateChanged();
		}

		private bool IsOpenCondition()
		{
			IOperatoreViewModel operatore = _dialogoOperatoreObserver.OperatoreSelezionato;
			string operazioneInCorso = _dialogoOperatoreObserver.OperazioneInCorso;
			IAttivitaViewModel attivitaSelezionata = _dialogoOperatoreObserver.AttivitaSelezionata;

			if (operatore == null)
				return false;

			if (operatore.Stato == Costanti.ASSENTE)
				return false;

			if ((operazioneInCorso == null || operazioneInCorso == Costanti.NESSUNA) && attivitaSelezionata == null)
				return false;

			if ((operazioneInCorso != Costanti.INIZIO_LAVORO && operazioneInCorso != Costanti.INIZIO_ATTREZZAGGIO) && operazioneInCorso != Costanti.AVANZAMENTO && attivitaSelezionata == null)
				return false;

			return true;
		}

		public override void Dispose()
		{
			_dialogoOperatoreObserver.OnAttivitaSelezionataChanged -= AttivitaStore_OnAttivitaSelezionataChanged;
			_dialogoOperatoreObserver.OnOperazioneInCorsoChanged -= DialogoOperatoreStore_OnOperazioneInCorsoChanged;
		}
	}
}
