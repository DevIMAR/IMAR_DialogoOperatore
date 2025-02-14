using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using IMAR_DialogoOperatore.Application.Interfaces.ViewModels;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Observers;
using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Commands
{
	public class IngressoUscitaCommand : CommandBase
    {
        private readonly InfoOperatoreViewModel _infoOperatoreViewModel;
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
        private readonly IInterruzioneAttivitaHelper _interruzioneLavoroHelper;
		private readonly IJmesApiClient _jmesApiClient;
        private readonly IAutoLogoutUtility _autoLogoutUtility;

		public IngressoUscitaCommand(
			InfoOperatoreViewModel infoOperatoreViewModel,
			IDialogoOperatoreObserver dialogoOperatoreObserver,
			IInterruzioneAttivitaHelper interruzioneLavoroHelper,
			IJmesApiClient jmesApiClient,
            IAutoLogoutUtility autoLogoutUtility)
        {
			_infoOperatoreViewModel = infoOperatoreViewModel;
            _dialogoOperatoreObserver = dialogoOperatoreObserver;
			_interruzioneLavoroHelper = interruzioneLavoroHelper;
			_jmesApiClient = jmesApiClient;
            _autoLogoutUtility = autoLogoutUtility;

            _autoLogoutUtility.OnLogoutTriggered += AutoLogoutUtility_OnLogoutTriggered;
        }

        private void AutoLogoutUtility_OnLogoutTriggered()
        {
            _infoOperatoreViewModel.Badge = null;
            _dialogoOperatoreObserver.AreTastiBloccati = false;
        }

        public override bool CanExecute(object? parameter)
		{
			return _dialogoOperatoreObserver.OperatoreSelezionato != null
					&& _dialogoOperatoreObserver.OperatoreSelezionato.Badge != null
                    && !_dialogoOperatoreObserver.AreTastiBloccati
					&& _dialogoOperatoreObserver.OperatoreSelezionato.Stato != Costanti.IN_PAUSA;
		}

		public override async void Execute(object? parameter)
        {
            if (_dialogoOperatoreObserver.OperatoreSelezionato.Stato == Costanti.ASSENTE)
            {
                _dialogoOperatoreObserver.IsLoaderVisibile = true;
                await Task.Delay(1);
                _jmesApiClient.MesAutoClock(_dialogoOperatoreObserver.OperatoreSelezionato.Badge.ToString(), true);
                _dialogoOperatoreObserver.IsLoaderVisibile = false;

                _dialogoOperatoreObserver.OperatoreSelezionato.Stato = Costanti.PRESENTE;
            }
			else
			{
				_dialogoOperatoreObserver.AreTastiBloccati = true;

                await ChiudiAttivitaOperatore();

                if (_dialogoOperatoreObserver.IsOperazioneAnnullata)
                {
					_dialogoOperatoreObserver.AreTastiBloccati = false;
					return;
				}

                _dialogoOperatoreObserver.IsLoaderVisibile = true;
                await Task.Delay(1);
                _jmesApiClient.MesAutoClock(_dialogoOperatoreObserver.OperatoreSelezionato.Badge.ToString(), false);
                _dialogoOperatoreObserver.IsLoaderVisibile = false;

                _dialogoOperatoreObserver.OperatoreSelezionato.Stato = Costanti.ASSENTE;

                _autoLogoutUtility.StartLogoutTimer(5);
            }
		}

        private async Task ChiudiAttivitaOperatore()
        {
            try
            {
                IAttivitaViewModel attivita;

                while (_dialogoOperatoreObserver.OperatoreSelezionato.AttivitaAperte.Any())
                {
                    attivita = new AttivitaViewModel(_dialogoOperatoreObserver.OperatoreSelezionato.AttivitaAperte[0]);

                    await _interruzioneLavoroHelper.GestisciInterruzioneAttivita(attivita, true);
                    await Task.Delay(1);

                    if (_dialogoOperatoreObserver.IsOperazioneAnnullata)
                        break;
                }
            }
            catch (Exception ex)
            {
                // Log dell'errore
                Console.WriteLine($"Errore durante la chiusura delle attività: {ex.Message}");
            }
        }

        public override void Dispose()
        {
            _autoLogoutUtility.OnLogoutTriggered -= AutoLogoutUtility_OnLogoutTriggered;
        }
    }
}
