using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Interfaces.ViewModels;
using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Commands
{
    public class CreaFaseNonPianificataCommand : CommandBase
    {
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
        private readonly IPopupObserver _popupObserver;
        private readonly ICercaAttivitaObserver _cercaAttivitaObserver;
        private readonly ICreaFaseNonPianificataHelper _creaFaseNonPianificataHelper;
        private readonly IMacchinaService _macchinaService;
        private readonly GestoreFasiNonPianificateViewModel _gestoreFasiNonPianificateViewModel;

        public CreaFaseNonPianificataCommand(
            IDialogoOperatoreObserver dialogoOperatoreObserver,
            IPopupObserver popupObserver,
            ICercaAttivitaObserver cercaAttivitaObserver,
            ICreaFaseNonPianificataHelper creaFaseNonPianificataHelper,
            IMacchinaService macchinaService,
            GestoreFasiNonPianificateViewModel gestoreFasiNonPianificateViewModel)
        {
            _dialogoOperatoreObserver = dialogoOperatoreObserver;
            _popupObserver = popupObserver;
            _cercaAttivitaObserver = cercaAttivitaObserver;

            _creaFaseNonPianificataHelper = creaFaseNonPianificataHelper;

            _macchinaService = macchinaService;

            _gestoreFasiNonPianificateViewModel = gestoreFasiNonPianificateViewModel;
        }

        public override bool CanExecute(object? parameter)
        {
            return _dialogoOperatoreObserver.AttivitaSelezionata != null &&
                   !string.IsNullOrWhiteSpace(_dialogoOperatoreObserver.OperazioneInCorso) &&
                   (_dialogoOperatoreObserver.OperazioneInCorso.Equals(Costanti.INIZIO_ATTREZZAGGIO) ||
                        _dialogoOperatoreObserver.OperazioneInCorso.Equals(Costanti.INIZIO_LAVORO)) &&
                   _gestoreFasiNonPianificateViewModel.FaseDiPartenza != null &&
                   _gestoreFasiNonPianificateViewModel.FaseDiArrivo != null &&
                   _gestoreFasiNonPianificateViewModel.QuantitaRilavorazione > 0 &&
                   Int32.Parse(_gestoreFasiNonPianificateViewModel.FaseDiArrivo.Fase) >= Int32.Parse(_gestoreFasiNonPianificateViewModel.FaseDiPartenza.Fase);
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
            string? result;
            List<IAttivitaViewModel> rangeFasiAttivita = _cercaAttivitaObserver.AttivitaTrovate.OrderBy(a => a.Fase)
                                                                                                      .Where(a => Int32.Parse(a.Fase) >= Int32.Parse(_gestoreFasiNonPianificateViewModel.FaseDiPartenza.Fase) &&
                                                                                                                  Int32.Parse(a.Fase) < Int32.Parse(_gestoreFasiNonPianificateViewModel.FaseDiArrivo.Fase))
                                                                                                      .ToList();
            if (!rangeFasiAttivita.Any())
                rangeFasiAttivita.Add(_gestoreFasiNonPianificateViewModel.FaseDiArrivo);

            foreach (IAttivitaViewModel attivita in rangeFasiAttivita)
            {
                result = _creaFaseNonPianificataHelper.ApriFaseNonPianificata(attivita);
                if (result != null)
                {
                    MostraPopupConTesto(result);
                    break;
                }
            }
        }

        private void MostraPopupConTesto(string testo)
        {
            _popupObserver.TestoPopup = testo;
            _popupObserver.IsPopupVisible = true;
        }
    }
}
