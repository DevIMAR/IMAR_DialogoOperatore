using Azure;
using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using IMAR_DialogoOperatore.Application.Interfaces.ViewModels;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Commands
{
    public class InizioFinePausaCommand : CommandBase
    {
        private readonly InfoOperatoreViewModel _infoOperatoreViewModel;
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
        private readonly IInterruzioneAttivitaHelper _interruzioneLavoroUtility;
        private readonly IJmesApiClient _jmesApiClient;
        private readonly IOperatoriService _operatoriService;
        private readonly IAutoLogoutUtility _autoLogoutUtility;

        public InizioFinePausaCommand(
            InfoOperatoreViewModel infoOperatoreViewModel,
            IDialogoOperatoreObserver dialogoOperatoreObserver,
            IInterruzioneAttivitaHelper interruzioneLavoroUtility,
            IJmesApiClient jmesApiClient,
            IAttivitaService attivitaService,
            IOperatoriService operatoriService,
            IAutoLogoutUtility autoLogoutUtility)
        {
            _infoOperatoreViewModel = infoOperatoreViewModel;
            _dialogoOperatoreObserver = dialogoOperatoreObserver;
            _interruzioneLavoroUtility = interruzioneLavoroUtility;
            _jmesApiClient = jmesApiClient;
            _operatoriService = operatoriService;
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
            if (parameter is not bool)
                return false;

            bool isInPausa = (bool)parameter;

            return _dialogoOperatoreObserver.OperatoreSelezionato != null
                    && !_dialogoOperatoreObserver.AreTastiBloccati
                    && _dialogoOperatoreObserver.OperatoreSelezionato.Stato != Costanti.ASSENTE
                    && isInPausa
                    && base.CanExecute(parameter);
        }

        public override async void Execute(object? parameter)
        {
            if (_dialogoOperatoreObserver.OperatoreSelezionato.Stato == Costanti.IN_PAUSA)
            {
                await FinePausa();
            }
            else
            {
                await InizioPausa();
            }
        }

        private async Task FinePausa()
        {
            _dialogoOperatoreObserver.IsLoaderVisibile = true;
            await Task.Delay(1);
            _jmesApiClient.MesBreakEnd(_dialogoOperatoreObserver.OperatoreSelezionato.Badge.ToString());
            _dialogoOperatoreObserver.IsLoaderVisibile = false;

            _dialogoOperatoreObserver.OperatoreSelezionato.Stato = Costanti.PRESENTE;

            _dialogoOperatoreObserver.OperatoreSelezionato = new OperatoreViewModel(_operatoriService.OttieniOperatore(_dialogoOperatoreObserver.OperatoreSelezionato.Badge));
        }

        private async Task InizioPausa()
        {
            _dialogoOperatoreObserver.AreTastiBloccati = true;

            await AvanzaAttivitaOperatore(_dialogoOperatoreObserver.OperatoreSelezionato);

            if (_dialogoOperatoreObserver.IsOperazioneAnnullata)
            {
                _dialogoOperatoreObserver.AreTastiBloccati = false;
                return;
            }

            _dialogoOperatoreObserver.IsLoaderVisibile = true;
            await Task.Delay(1);
            _jmesApiClient.MesBreakStart(_dialogoOperatoreObserver.OperatoreSelezionato.Badge.ToString());
            _dialogoOperatoreObserver.IsLoaderVisibile = false;

            _dialogoOperatoreObserver.OperatoreSelezionato = new OperatoreViewModel(_operatoriService.OttieniOperatore(_dialogoOperatoreObserver.OperatoreSelezionato.Badge));
            _dialogoOperatoreObserver.OperatoreSelezionato.Stato = Costanti.IN_PAUSA;

            _autoLogoutUtility.StartLogoutTimer(5);
        }

        private async Task AvanzaAttivitaOperatore(IOperatoreViewModel operatore)
        {
            IAttivitaViewModel attivita;

            for (int i = 0; i < operatore.AttivitaAperte.Count; i++)
            {
                attivita = new AttivitaViewModel(operatore.AttivitaAperte[i]);

                if (attivita.Causale == Costanti.IN_ATTREZZAGGIO)
                    continue;

                await _interruzioneLavoroUtility.GestisciInterruzioneAttivita(attivita, false);

                if (_dialogoOperatoreObserver.IsOperazioneAnnullata)
                    break;
            }
        }

        public override void Dispose()
        {
            _autoLogoutUtility.OnLogoutTriggered -= AutoLogoutUtility_OnLogoutTriggered;
        }
    }
}
