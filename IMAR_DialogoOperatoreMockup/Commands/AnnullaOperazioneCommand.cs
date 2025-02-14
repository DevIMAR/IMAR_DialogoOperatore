using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Commands
{
    public class AnnullaOperazioneCommand : CommandBase
	{
		private readonly InfoOperatoreViewModel _infoOperatoreViewModel;
		private readonly AttivitaGridViewModel _attivitaGridViewModel;
		private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;

		public AnnullaOperazioneCommand(
			InfoOperatoreViewModel infoOperatoreViewModel,
			AttivitaGridViewModel attivitaGridViewModel,
			IDialogoOperatoreObserver dialogoOperatoreObserver)
		{
			_infoOperatoreViewModel = infoOperatoreViewModel;
			_attivitaGridViewModel = attivitaGridViewModel;
			_dialogoOperatoreObserver = dialogoOperatoreObserver;
		}

		public override bool CanExecute(object? parameter) =>
            _dialogoOperatoreObserver.OperatoreSelezionato != null
			&& _dialogoOperatoreObserver.OperatoreSelezionato.Badge != null
            && base.CanExecute(parameter);

		public override void Execute(object? parameter)
		{
			_dialogoOperatoreObserver.IsOperazioneAnnullata = true;

			if (IsCondizioneDiLogout())
			{
				_infoOperatoreViewModel.Badge = null;
				_dialogoOperatoreObserver.IsOperazioneAnnullata = false;
				return;
			}

			if (IsAttivitaDetailsDaChiudere())
				_dialogoOperatoreObserver.OperazioneInCorso = Costanti.NESSUNA;

			_attivitaGridViewModel.AttivitaSelezionata = null;
			_dialogoOperatoreObserver.IsOperazioneAnnullata = false;
        }

		private bool IsCondizioneDiLogout() =>
			_dialogoOperatoreObserver.OperazioneInCorso.Equals(Costanti.NESSUNA)
			&& _dialogoOperatoreObserver.AttivitaSelezionata == null;

		private bool IsAttivitaDetailsDaChiudere() =>
			!_dialogoOperatoreObserver.OperazioneInCorso.Equals(Costanti.NESSUNA);		
	}
}
