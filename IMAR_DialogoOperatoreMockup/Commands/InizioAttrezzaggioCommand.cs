using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.Commands
{
    public class InizioAttrezzaggioCommand : CommandBase
	{
		private IDialogoOperatoreObserver _dialogoOperatoreObserver;

		public InizioAttrezzaggioCommand(IDialogoOperatoreObserver dialogoOperatoreStore)
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
			if (!(_dialogoOperatoreObserver.OperazioneInCorso == Costanti.AVANZAMENTO || _dialogoOperatoreObserver.OperazioneInCorso == Costanti.INIZIO_LAVORO))
				_dialogoOperatoreObserver.AttivitaSelezionata = null;

			_dialogoOperatoreObserver.OperazioneInCorso = Costanti.INIZIO_ATTREZZAGGIO;
		}
	}
}
