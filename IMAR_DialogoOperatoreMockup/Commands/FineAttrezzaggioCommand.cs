using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.Commands
{
	public class FineAttrezzaggioCommand : CommandBase
	{
		private IDialogoOperatoreObserver _dialogoOperatoreStore;

        public FineAttrezzaggioCommand(
			IDialogoOperatoreObserver dialogoOperatoreStore)
        {
            _dialogoOperatoreStore = dialogoOperatoreStore;
		}

		public override bool CanExecute(object? parameter)
		{
			return _dialogoOperatoreStore.OperatoreSelezionato != null
					&& !_dialogoOperatoreStore.AreTastiBloccati
					&& _dialogoOperatoreStore.OperatoreSelezionato.Stato != Costanti.ASSENTE
					&& _dialogoOperatoreStore.OperatoreSelezionato.Stato != Costanti.IN_PAUSA
					&& _dialogoOperatoreStore.AttivitaSelezionata != null
					&& _dialogoOperatoreStore.AttivitaSelezionata.Causale == Costanti.IN_ATTREZZAGGIO
					&& base.CanExecute(parameter);
		}

		public override void Execute(object? parameter)
		{
			_dialogoOperatoreStore.OperazioneInCorso = Costanti.FINE_ATTREZZAGGIO;
		}
	}
}
