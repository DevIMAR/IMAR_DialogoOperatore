using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Interfaces.ViewModels;
using System.Windows.Input;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class GestoreAttivitaViewModel : ViewModelBase
	{
		private readonly IDialogoOperatoreObserver _dialogoOperatoreStore;
		private readonly ICercaAttivitaObserver _cercaAttivitaStore;

		public bool IsOpen => IsOpenCondition();
		public string? OperazioneInCorso => _dialogoOperatoreStore.OperazioneInCorso;

		public ICommand ConfermaCommand { get; set; }

		public GestoreAttivitaViewModel(
			IDialogoOperatoreObserver dialogoOperatoreStore,
			ICercaAttivitaObserver cercaAttivitaStore,
			ConfermaCommand confermaCommand)
        {
			_dialogoOperatoreStore = dialogoOperatoreStore;
			_cercaAttivitaStore = cercaAttivitaStore;
			ConfermaCommand = confermaCommand;

			_dialogoOperatoreStore.OnAttivitaSelezionataChanged += AttivitaStore_OnAttivitaSelezionataChanged;
			_dialogoOperatoreStore.OnOperazioneInCorsoChanged += DialogoOperatoreStore_OnOperazioneInCorsoChanged;
		}

		private void DialogoOperatoreStore_OnOperazioneInCorsoChanged()
		{
			if (!(_dialogoOperatoreStore.OperazioneInCorso == Costanti.AVANZAMENTO || 
					_dialogoOperatoreStore.OperazioneInCorso == Costanti.INIZIO_LAVORO || 
					_dialogoOperatoreStore.OperazioneInCorso == Costanti.INIZIO_ATTREZZAGGIO))
				_cercaAttivitaStore.IsAttivitaCercata = false;

            _dialogoOperatoreStore.IsDettaglioAttivitaOpen = IsOpen;

            OnNotifyStateChanged();
		}

		private void AttivitaStore_OnAttivitaSelezionataChanged()
		{
			_dialogoOperatoreStore.IsDettaglioAttivitaOpen = IsOpen;
			OnNotifyStateChanged();
		}

		private bool IsOpenCondition()
		{
			IOperatoreViewModel operatore = _dialogoOperatoreStore.OperatoreSelezionato;
			string operazioneInCorso = _dialogoOperatoreStore.OperazioneInCorso;
			IAttivitaViewModel attivitaSelezionata = _dialogoOperatoreStore.AttivitaSelezionata;

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
			_dialogoOperatoreStore.OnAttivitaSelezionataChanged -= AttivitaStore_OnAttivitaSelezionataChanged;
			_dialogoOperatoreStore.OnOperazioneInCorsoChanged -= DialogoOperatoreStore_OnOperazioneInCorsoChanged;
		}
	}
}
