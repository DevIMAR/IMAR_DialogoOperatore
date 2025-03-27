using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.Commands
{
    public class InizioLavoroCommand : CommandBase
	{
		private IDialogoOperatoreObserver _dialogoOperatoreObserver;

		public InizioLavoroCommand(IDialogoOperatoreObserver dialogoOperatoreStore)
        {
            _dialogoOperatoreObserver = dialogoOperatoreStore;
		}

		public override bool CanExecute(object? parameter)
		{
			return _dialogoOperatoreObserver.OperatoreSelezionato != null
                    && !_dialogoOperatoreObserver.IsDettaglioAttivitaOpen
                    && !_dialogoOperatoreObserver.IsUscita
                    && _dialogoOperatoreObserver.OperatoreSelezionato.Stato != Costanti.ASSENTE
					&& _dialogoOperatoreObserver.OperatoreSelezionato.Stato != Costanti.IN_PAUSA
					&& base.CanExecute(parameter);
		}

		public override void Execute(object? parameter)
		{
			if (!(_dialogoOperatoreObserver.OperazioneInCorso == Costanti.AVANZAMENTO || _dialogoOperatoreObserver.OperazioneInCorso == Costanti.INIZIO_ATTREZZAGGIO))
				_dialogoOperatoreObserver.AttivitaSelezionata = null;

			_dialogoOperatoreObserver.OperazioneInCorso = Costanti.INIZIO_LAVORO;
		}
	}
}
