using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.Commands
{
	public class InizioAttrezzaggioCommand : CommandBase
	{
		private IDialogoOperatoreObserver _dialogoOperatoreStore;

		public InizioAttrezzaggioCommand(IDialogoOperatoreObserver dialogoOperatoreStore)
        {
            _dialogoOperatoreStore = dialogoOperatoreStore;
		}

		public override bool CanExecute(object? parameter)
		{
			return _dialogoOperatoreStore.OperatoreSelezionato != null
					&& !_dialogoOperatoreStore.AreTastiBloccati
					&& _dialogoOperatoreStore.OperatoreSelezionato.Stato != Costanti.ASSENTE
					&& _dialogoOperatoreStore.OperatoreSelezionato.Stato != Costanti.IN_PAUSA
					&& base.CanExecute(parameter);
		}

		public override void Execute(object? parameter)
		{
			if (!(_dialogoOperatoreStore.OperazioneInCorso == Costanti.AVANZAMENTO || _dialogoOperatoreStore.OperazioneInCorso == Costanti.INIZIO_LAVORO))
				_dialogoOperatoreStore.AttivitaSelezionata = null;

			_dialogoOperatoreStore.OperazioneInCorso = Costanti.INIZIO_ATTREZZAGGIO;
		}
	}
}
