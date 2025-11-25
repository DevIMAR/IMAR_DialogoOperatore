using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Interfaces.ViewModels;
using IMAR_DialogoOperatore.Utilities;
using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Commands
{
    public class InizioFinePausaCommand : CommandBase
    {
        private readonly InfoOperatoreViewModel _infoOperatoreViewModel;
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
        private readonly IInterruzioneAttivitaHelper _interruzioneAttivitaHelper;
        private readonly IJmesApiClient _jmesApiClient;
        private readonly IOperatoreService _operatoriService;
        private readonly IAutoLogoutUtility _autoLogoutUtility;
        private readonly ToastDisplayerUtility _toastDisplayerUtility;

        public InizioFinePausaCommand(
            InfoOperatoreViewModel infoOperatoreViewModel,
            IDialogoOperatoreObserver dialogoOperatoreObserver,
            IInterruzioneAttivitaHelper interruzioneLavoroUtility,
            IJmesApiClient jmesApiClient,
            IAttivitaService attivitaService,
            IOperatoreService operatoriService,
            IAutoLogoutUtility autoLogoutUtility,
            ToastDisplayerUtility toastDisplayerUtility)
        {
            _infoOperatoreViewModel = infoOperatoreViewModel;
            _dialogoOperatoreObserver = dialogoOperatoreObserver;
            _interruzioneAttivitaHelper = interruzioneLavoroUtility;
            _jmesApiClient = jmesApiClient;
            _operatoriService = operatoriService;
            _autoLogoutUtility = autoLogoutUtility;
            _toastDisplayerUtility = toastDisplayerUtility;

            _autoLogoutUtility.OnLogoutTriggered += AutoLogoutUtility_OnLogoutTriggered;
        }

        private void AutoLogoutUtility_OnLogoutTriggered()
        {
            _infoOperatoreViewModel.Badge = null;
            _dialogoOperatoreObserver.IsUscita = false;
        }

        public override bool CanExecute(object? parameter)
        {
            if (parameter is not bool)
                return false;

            bool isInPausa = (bool)parameter;

            return _dialogoOperatoreObserver.OperatoreSelezionato != null
                    && !_dialogoOperatoreObserver.IsDettaglioAttivitaOpen
                    && !_dialogoOperatoreObserver.IsUscita
                    && _dialogoOperatoreObserver.OperatoreSelezionato.Stato != Costanti.ASSENTE
                    && isInPausa
                    && base.CanExecute(parameter);
        }

        public override async void Execute(object? parameter)
        {
            if (_dialogoOperatoreObserver.OperatoreSelezionato.Stato == Costanti.IN_PAUSA)
                await FinePausa();
            else
                await InizioPausa();
        }

        private async Task FinePausa()
        {
            _dialogoOperatoreObserver.IsLoaderVisibile = true;
            await Task.Delay(1);
            _jmesApiClient.MesBreakEnd(_dialogoOperatoreObserver.OperatoreSelezionato.Badge.ToString());
            _dialogoOperatoreObserver.OperatoreSelezionato = new OperatoreViewModel(_operatoriService.OttieniOperatore(_dialogoOperatoreObserver.OperatoreSelezionato.Badge));
            _dialogoOperatoreObserver.IsLoaderVisibile = false;

            _dialogoOperatoreObserver.OperatoreSelezionato.Stato = Costanti.PRESENTE;
            _toastDisplayerUtility.ShowGreenToast("Rientro", $"Bentornato {_dialogoOperatoreObserver.OperatoreSelezionato.Nome}!");
        }

        private async Task InizioPausa()
        {
            _dialogoOperatoreObserver.IsUscita = true;

            await AvanzaAttivitaOperatore();

            if (_dialogoOperatoreObserver.IsOperazioneAnnullata)
            {
                _dialogoOperatoreObserver.IsUscita = false;
                return;
            }

            _dialogoOperatoreObserver.IsLoaderVisibile = true;
            await Task.Delay(1);
            _jmesApiClient.MesBreakStart(_dialogoOperatoreObserver.OperatoreSelezionato.Badge.ToString());
            _dialogoOperatoreObserver.OperatoreSelezionato = new OperatoreViewModel(_operatoriService.OttieniOperatore(_dialogoOperatoreObserver.OperatoreSelezionato.Badge));
            _dialogoOperatoreObserver.IsLoaderVisibile = false;

            _dialogoOperatoreObserver.OperatoreSelezionato.Stato = Costanti.IN_PAUSA;
            _toastDisplayerUtility.ShowYellowToast("Pausa", $"Buona pausa {_dialogoOperatoreObserver.OperatoreSelezionato.Nome}!");

            _autoLogoutUtility.StartLogoutTimer(2);
        }

        private async Task AvanzaAttivitaOperatore()
        {
            IAttivitaViewModel attivita;
            IOperatoreViewModel operatore = _dialogoOperatoreObserver.OperatoreSelezionato;

            for (int i = 0; i < operatore.AttivitaAperte.Count; i++)
            {
                attivita = new AttivitaViewModel(operatore.AttivitaAperte[i]);

                if (attivita.Causale == Costanti.IN_ATTREZZAGGIO)
                    continue;

                await _interruzioneAttivitaHelper.GestisciInterruzioneAttivita(attivita, false);

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
