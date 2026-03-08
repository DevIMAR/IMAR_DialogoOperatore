using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using IMAR_DialogoOperatore.Enums;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Interfaces.Services;

namespace IMAR_DialogoOperatore.Commands
{
    public class CreaFaseNonPianificataCommand : CommandBase
    {
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
        private readonly IPopupObserver _popupObserver;
        private readonly ICreaFaseNonPianificataHelper _creaFaseNonPianificataHelper;
        private readonly IMacchinaService _macchinaService;
        private readonly IMessageBoxService _messageBoxService;
        private readonly ILoggingService _loggingService;

        public CreaFaseNonPianificataCommand(
            IDialogoOperatoreObserver dialogoOperatoreObserver,
            IPopupObserver popupObserver,
            ICreaFaseNonPianificataHelper creaFaseNonPianificataHelper,
            IMacchinaService macchinaService,
            IMessageBoxService messageBoxService,
            ILoggingService loggingService)
        {
            _dialogoOperatoreObserver = dialogoOperatoreObserver;
            _popupObserver = popupObserver;

            _creaFaseNonPianificataHelper = creaFaseNonPianificataHelper;

            _macchinaService = macchinaService;
            _messageBoxService = messageBoxService;
            _loggingService = loggingService;
        }

        public override bool CanExecute(object? parameter)
        {
            return _dialogoOperatoreObserver.AttivitaSelezionata != null &&
                   _dialogoOperatoreObserver.AttivitaSelezionata.SaldoAcconto == Costanti.SALDO &&
                   !string.IsNullOrWhiteSpace(_dialogoOperatoreObserver.OperazioneInCorso) &&
                   (_dialogoOperatoreObserver.OperazioneInCorso.Equals(Costanti.INIZIO_ATTREZZAGGIO) ||
                        _dialogoOperatoreObserver.OperazioneInCorso.Equals(Costanti.INIZIO_LAVORO));
        }

        public override async void Execute(object? parameter)
        {
            await SafeExecuteAsync(async () =>
            {
                _dialogoOperatoreObserver.IsLoaderVisibile = true;
                await Task.Delay(1);

                await AssegnaMacchinaFittiziaAdOperatoreAsync();
                await EseguiOperazioneOMostraMessaggio();

                _dialogoOperatoreObserver.IsOperazioneGestita = true;
                _dialogoOperatoreObserver.IsLoaderVisibile = false;
            }, _loggingService, "CreaFaseNonPianificataCommand.Execute");
        }

        private async Task AssegnaMacchinaFittiziaAdOperatoreAsync()
        {
            if (_dialogoOperatoreObserver.OperatoreSelezionato.MacchineAssegnate.Any())
                return;

            Domain.Models.Macchina? macchina = await _macchinaService.GetPrimaMacchinaFittiziaNonUtilizzataAsync();
            if (macchina == null)
            {
                MostraPopupConTesto(Costanti.ERRORE_MACCHINE_FINITE);
                return;
            }
            _dialogoOperatoreObserver.OperatoreSelezionato.MacchineAssegnate.Add(macchina);
        }

        private async Task EseguiOperazioneOMostraMessaggio()
        {
            string? testo;

            testo = await _creaFaseNonPianificataHelper.ApriFaseNonPianificata(_dialogoOperatoreObserver.AttivitaSelezionata);
            if (testo != null)
                await _messageBoxService.ShowModalAsync(testo, "Creazione fase non pianificata", MessageBoxButtons.Ok);
        }

        private void MostraPopupConTesto(string testo)
        {
            _popupObserver.TestoPopup = testo;
            _popupObserver.IsPopupVisible = true;
        }
    }
}
