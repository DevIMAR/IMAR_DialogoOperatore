using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.Commands
{
    public class FineLavoroCommand : CommandBase
	{
		private IDialogoOperatoreObserver _dialogoOperatoreObserver;

        public FineLavoroCommand(IDialogoOperatoreObserver dialogoOperatoreStore)
        {
             _dialogoOperatoreObserver = dialogoOperatoreStore;
		}

		public override bool CanExecute(object? parameter)
		{
			return _dialogoOperatoreObserver.OperatoreSelezionato != null
            && !_dialogoOperatoreObserver.IsUscita
                    && _dialogoOperatoreObserver.OperatoreSelezionato.Stato != Costanti.ASSENTE
					&& _dialogoOperatoreObserver.OperatoreSelezionato.Stato != Costanti.IN_PAUSA
					&& _dialogoOperatoreObserver.AttivitaSelezionata != null
					&& _dialogoOperatoreObserver.AttivitaSelezionata.Causale == Costanti.IN_LAVORO
					&& base.CanExecute(parameter);
		}

		public override void Execute(object? parameter)
		{
			_dialogoOperatoreObserver.OperazioneInCorso = Costanti.FINE_LAVORO;
		}
	}
}
