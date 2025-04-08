using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.Commands
{
    public class ConfermaCommand : CommandBase
	{
		private readonly IPopupConfermaHelper _popupConfermaHelper;
		private readonly IConfermaOperazioneHelper _confermaOperazioneHelper;
		private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
		private readonly IPopupObserver _popupStore;
		private readonly IAvanzamentoObserver _avanzamentoObserver;
		private readonly IMacchinaService _macchinaService;

		public ConfermaCommand(
			IPopupConfermaHelper popupConfermaUtility,
			IConfermaOperazioneHelper confermaOperazioneUtility,
			IDialogoOperatoreObserver dialogoOperatoreObserver,
			IPopupObserver popupObserver,
            IAvanzamentoObserver avanzamentoObserver,
			IMacchinaService macchinaService)
		{
			_popupConfermaHelper = popupConfermaUtility;
			_confermaOperazioneHelper = confermaOperazioneUtility;
			_dialogoOperatoreObserver = dialogoOperatoreObserver;
			_popupStore = popupObserver;
			_avanzamentoObserver = avanzamentoObserver;
			_macchinaService = macchinaService;

			_popupStore.OnIsConfermatoChanged += PopupStore_OnIsConfermatoChanged;
		}

		public override bool CanExecute(object? parameter)
		{
			return _dialogoOperatoreObserver.OperatoreSelezionato != null
				   && _dialogoOperatoreObserver.OperatoreSelezionato.Stato != Costanti.ASSENTE
				   && _dialogoOperatoreObserver.OperatoreSelezionato.Stato != Costanti.IN_PAUSA
				   && _dialogoOperatoreObserver.OperazioneInCorso != Costanti.NESSUNA
				   && _dialogoOperatoreObserver.AttivitaSelezionata != null
				   && base.CanExecute(parameter);
		}

		private async void PopupStore_OnIsConfermatoChanged()
        {
            if (!_popupStore.IsConfermato)
                return;

            await MostraLoaderEGestisciOperazione();
        }

        public override async void Execute(object? parameter)
        {
			_dialogoOperatoreObserver.IsOperazioneGestita = false;

            string? testoPopup = _popupConfermaHelper.GetTestoPopup();

			if (testoPopup == string.Empty)
                await MostraLoaderEGestisciOperazione();
            else
				MostraPopupConTesto(testoPopup);
        }

        private async Task MostraLoaderEGestisciOperazione()
        {
            _dialogoOperatoreObserver.IsLoaderVisibile = true;
            await Task.Delay(1);
            GestioneEsecuzioneOperazione();
            _dialogoOperatoreObserver.IsLoaderVisibile = false;


            _dialogoOperatoreObserver.IsOperazioneGestita = true;
        }

		private void GestioneEsecuzioneOperazione()
        {
            AssegnaSaldoAccontoAdAttivitaSelezionata();

            AssegnaMacchinaFittiziaAdOperatore();

            EseguiOperazioneOMostraMessaggio();

            _dialogoOperatoreObserver.OperazioneInCorso = Costanti.NESSUNA;
        }

        private void AssegnaSaldoAccontoAdAttivitaSelezionata()
        {
            if (_dialogoOperatoreObserver.AttivitaSelezionata != null)
                _dialogoOperatoreObserver.AttivitaSelezionata.SaldoAcconto = _avanzamentoObserver.SaldoAcconto;
        }

        private void EseguiOperazioneOMostraMessaggio()
        {
            string? result = _confermaOperazioneHelper.EseguiOperazione();
            if (result != null)
                MostraPopupConTesto(result);
        }

        private void AssegnaMacchinaFittiziaAdOperatore()
        {
            if (!CanAssegnareMacchinaFittiziaAdOperatore())
                return;

            _dialogoOperatoreObserver.OperatoreSelezionato.MacchinaAssegnata = _macchinaService.GetPrimaMacchinaFittiziaNonUtilizzata();
            if (_dialogoOperatoreObserver.OperatoreSelezionato.MacchinaAssegnata == null)
                MostraPopupConTesto("!!! NON CI SONO MACCHINE DISPONIBILI!!!\nCONTATTARE UFFICIO IT");
        }

        private bool CanAssegnareMacchinaFittiziaAdOperatore()
        {
            return (_dialogoOperatoreObserver.OperazioneInCorso == Costanti.INIZIO_ATTREZZAGGIO ||
                    _dialogoOperatoreObserver.OperazioneInCorso == Costanti.INIZIO_LAVORO) &&
                    _dialogoOperatoreObserver.OperatoreSelezionato.MacchinaAssegnata == null;
        }

        private void MostraPopupConTesto(string testo)
		{
			_popupStore.TestoPopup = testo;
			_popupStore.IsPopupVisible = true;
		}
	}
}
