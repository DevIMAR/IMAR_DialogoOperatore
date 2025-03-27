using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.Commands
{
    public class AvanzamentoCommand : CommandBase
	{
		private IDialogoOperatoreObserver _dialogoOperatoreObserver;

		public AvanzamentoCommand(IDialogoOperatoreObserver dialogoOperatoreStore)
		{
			_dialogoOperatoreObserver = dialogoOperatoreStore;
		}

		public override bool CanExecute(object? parameter)
		{
			return _dialogoOperatoreObserver.OperatoreSelezionato != null
					&& !_dialogoOperatoreObserver.IsUscita
                    && _dialogoOperatoreObserver.OperatoreSelezionato.Stato != Costanti.ASSENTE
					&& _dialogoOperatoreObserver.OperatoreSelezionato.Stato != Costanti.IN_PAUSA
					&& base.CanExecute(parameter);
		}

		public override void Execute(object? parameter)
		{
			_dialogoOperatoreObserver.OperazioneInCorso = Costanti.AVANZAMENTO;
		}
	}
}
