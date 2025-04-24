using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.DTOs;
using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Application.Interfaces.Services.External;
using IMAR_DialogoOperatore.Domain.Entities.Imar_Produzione;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.Commands
{
    public class ConfermaCommand : CommandBase
	{
		private readonly IPopupConfermaHelper _popupConfermaHelper;
		private readonly IConfermaOperazioneHelper _confermaOperazioneHelper;

		private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
		private readonly IPopupObserver _popupObserver;
		private readonly IAvanzamentoObserver _avanzamentoObserver;
		private readonly ISegnalazioneObserver _segnalazioneObserver;

		private readonly IMacchinaService _macchinaService;
		private readonly ISegnalazioniDifformitaService _segnalazioniDifformitaService;

        private readonly IImarApiClient _imarApiClient;

		public ConfermaCommand(
			IPopupConfermaHelper popupConfermaUtility,
			IConfermaOperazioneHelper confermaOperazioneUtility,
			IDialogoOperatoreObserver dialogoOperatoreObserver,
			IPopupObserver popupObserver,
            IAvanzamentoObserver avanzamentoObserver,
            ISegnalazioneObserver segnalazioneObserver,
			IMacchinaService macchinaService,
            ISegnalazioniDifformitaService segnalazioniDifformitaService,
            IImarApiClient imarApiClient)
		{
			_popupConfermaHelper = popupConfermaUtility;
			_confermaOperazioneHelper = confermaOperazioneUtility;

			_dialogoOperatoreObserver = dialogoOperatoreObserver;
			_popupObserver = popupObserver;
			_avanzamentoObserver = avanzamentoObserver;
            _segnalazioneObserver = segnalazioneObserver;

			_macchinaService = macchinaService;
            _segnalazioniDifformitaService = segnalazioniDifformitaService;

            _imarApiClient = imarApiClient;

			_popupObserver.OnIsConfermatoChanged += PopupStore_OnIsConfermatoChanged;

        }

        public override bool CanExecute(object? parameter)
		{
			return _dialogoOperatoreObserver.OperatoreSelezionato != null
				   && _dialogoOperatoreObserver.OperatoreSelezionato.Stato == Costanti.PRESENTE
				   && _dialogoOperatoreObserver.OperazioneInCorso != Costanti.NESSUNA
				   && _dialogoOperatoreObserver.AttivitaSelezionata != null
				   && base.CanExecute(parameter);
		}

        private async void PopupStore_OnIsConfermatoChanged()
        {
            if (!_popupObserver.IsConfermato)
                return;

            if (_avanzamentoObserver.QuantitaScartata != null && _avanzamentoObserver.QuantitaScartata != 0)
            {
                _dialogoOperatoreObserver.IsLoaderVisibile = true;
                await Task.Delay(1);
                await CreaEdInviaSegnalazioneDifformita();
            }

            await MostraLoaderEGestisciOperazione();
        }

        private async Task CreaEdInviaSegnalazioneDifformita()
        {
            CostiArticoloDTO costiArticoloDTO = await _imarApiClient.GetCostiArticolo(_dialogoOperatoreObserver.AttivitaSelezionata.Articolo);

            _segnalazioniDifformitaService.InsertSegnalazione(new SegnalazioneDifformita
            {
                OrigineSegnalazione = "I",
                Richiedente = _dialogoOperatoreObserver.OperatoreSelezionato.Badge + " - " + _dialogoOperatoreObserver.OperatoreSelezionato.Cognome + " " + _dialogoOperatoreObserver.OperatoreSelezionato.Nome,
                FaseDifformita = _dialogoOperatoreObserver.AttivitaSelezionata.Fase,
                DescrizioneFase = _dialogoOperatoreObserver.AttivitaSelezionata.DescrizioneFase,
                Odp = _dialogoOperatoreObserver.AttivitaSelezionata.Odp,
                Articolo = _dialogoOperatoreObserver.AttivitaSelezionata.Articolo,
                DescrizioneArticolo = _dialogoOperatoreObserver.AttivitaSelezionata.DescrizioneArticolo,
                QtaProdotta = _avanzamentoObserver.QuantitaProdotta,
                QtaDifforme = _avanzamentoObserver.QuantitaScartata,
                CostoGestioneDifformita = 5,
                CostoUnitarioMateriale = Math.Round(costiArticoloDTO.CostoUnitarioMateriale, 2),
                CostoLavorazione = Math.Round(costiArticoloDTO.CostoUnitarioLavorazione, 2),
                DescrizioneDifformita = _segnalazioneObserver.DescrizioneDifetto,
                CategoriaDifformita = _segnalazioneObserver.Categoria,
                QtaDifformiRecuperati = _segnalazioneObserver.QuantitaRecuperata,
                Sorgente = "DialogoOperatore"
            });
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

        private void EseguiOperazioneOMostraMessaggio()
        {
            string? result = _confermaOperazioneHelper.EseguiOperazione();
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
