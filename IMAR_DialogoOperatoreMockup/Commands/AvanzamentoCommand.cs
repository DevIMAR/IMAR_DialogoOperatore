using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.Commands
{
	public class AvanzamentoCommand : CommandBase
	{
		private IDialogoOperatoreObserver _dialogoOperatoreStore;

		public AvanzamentoCommand(IDialogoOperatoreObserver dialogoOperatoreStore)
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
			_dialogoOperatoreStore.OperazioneInCorso = Costanti.AVANZAMENTO;
		}
	}
}
