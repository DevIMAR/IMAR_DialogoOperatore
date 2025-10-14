using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Commands
{
    public class CreaFaseNonPianificataCommand : CommandBase
    {
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
        private readonly IPopupObserver _popupObserver;
        private readonly ICreaFaseNonPianificataHelper _creaFaseNonPianificataHelper;
        private readonly IMacchinaService _macchinaService;

        public CreaFaseNonPianificataCommand(
            IDialogoOperatoreObserver dialogoOperatoreObserver,
            IPopupObserver popupObserver,
            ICreaFaseNonPianificataHelper creaFaseNonPianificataHelper,
            IMacchinaService macchinaService)
        {
            _dialogoOperatoreObserver = dialogoOperatoreObserver;
            _popupObserver = popupObserver;
            _creaFaseNonPianificataHelper = creaFaseNonPianificataHelper;
            _macchinaService = macchinaService;
        }

        public override bool CanExecute(object? parameter)
        {
            return _dialogoOperatoreObserver.AttivitaSelezionata != null &&
                   !string.IsNullOrWhiteSpace(_dialogoOperatoreObserver.OperazioneInCorso) &&
                   (_dialogoOperatoreObserver.OperazioneInCorso.Equals(Costanti.INIZIO_ATTREZZAGGIO) ||
                        _dialogoOperatoreObserver.OperazioneInCorso.Equals(Costanti.INIZIO_LAVORO));
        }

        public override async void Execute(object? parameter)
        {
            _dialogoOperatoreObserver.IsLoaderVisibile = true;
            await Task.Delay(1);

            AssegnaMacchinaFittiziaAdOperatore();
            EseguiOperazioneOMostraMessaggio();

            _dialogoOperatoreObserver.IsOperazioneGestita = true;
            _dialogoOperatoreObserver.IsLoaderVisibile = false;
        }

        private void AssegnaMacchinaFittiziaAdOperatore()
        {
            if (_dialogoOperatoreObserver.OperatoreSelezionato.MacchinaAssegnata != null)
                return;

            _dialogoOperatoreObserver.OperatoreSelezionato.MacchinaAssegnata = _macchinaService.GetPrimaMacchinaFittiziaNonUtilizzata();
            if (_dialogoOperatoreObserver.OperatoreSelezionato.MacchinaAssegnata == null)
                MostraPopupConTesto(Costanti.ERRORE_MACCHINE_FINITE);
        }

        private void EseguiOperazioneOMostraMessaggio()
        {

            string? result = _creaFaseNonPianificataHelper.ApriFaseNonPianificata();
            if (result != null)
                MostraPopupConTesto(result);
        }

        private void MostraPopupConTesto(string testo)
        {
            _popupObserver.TestoPopup = testo;
            _popupObserver.IsPopupVisible = true;
        }
    }
}
