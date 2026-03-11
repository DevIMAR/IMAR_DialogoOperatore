using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.DTOs;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Application.Interfaces.Services.External;
using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using IMAR_DialogoOperatore.Domain.Entities.Imar_Produzione;
using IMAR_DialogoOperatore.Domain.Models;
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
		private readonly ILoggingService _loggingService;

		public ConfermaCommand(
			IPopupConfermaHelper popupConfermaUtility,
			IConfermaOperazioneHelper confermaOperazioneUtility,
			IDialogoOperatoreObserver dialogoOperatoreObserver,
			IPopupObserver popupObserver,
            IAvanzamentoObserver avanzamentoObserver,
            ISegnalazioneObserver segnalazioneObserver,
			IMacchinaService macchinaService,
            ISegnalazioniDifformitaService segnalazioniDifformitaService,
			ILoggingService loggingService)
		{
			_popupConfermaHelper = popupConfermaUtility;
			_confermaOperazioneHelper = confermaOperazioneUtility;

			_dialogoOperatoreObserver = dialogoOperatoreObserver;
			_popupObserver = popupObserver;
			_avanzamentoObserver = avanzamentoObserver;
            _segnalazioneObserver = segnalazioneObserver;

			_macchinaService = macchinaService;
            _segnalazioniDifformitaService = segnalazioniDifformitaService;
			_loggingService = loggingService;

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
            await SafeExecuteAsync(async () =>
            {
                if (!_popupObserver.IsConfermato)
                    return;

                if (_dialogoOperatoreObserver.OperazioneInCorso == Costanti.NESSUNA)
                    return;

                if (_avanzamentoObserver.QuantitaScartata != null && _avanzamentoObserver.QuantitaScartata != 0)
                {
                    _dialogoOperatoreObserver.IsLoaderVisibile = true;
                    await Task.Delay(1);
                    await CreaEdInviaSegnalazioneDifformita();
                }

                await MostraLoaderEGestisciOperazione();
            }, _loggingService, "ConfermaCommand.PopupStore_OnIsConfermatoChanged",
               _dialogoOperatoreObserver.OperatoreSelezionato?.Badge?.ToString());
        }

        private async Task CreaEdInviaSegnalazioneDifformita()
        {
            var attivita = _segnalazioneObserver.AttivitaPerSegnalazione;

            CostiArticoloDTO costiArticoloDTO = attivita?.CodiceArticolo != null
                ? await _segnalazioniDifformitaService.GetCostiArticolo(attivita.CodiceArticolo)
                : new CostiArticoloDTO();

            // Flusso della fase di apertura NC (dai dati AS400 già in pancia)
            string? flusso = attivita?.Flusso;

            // Se "Errore fase attuale" non è spuntato, FaseDifformita vuota + nota nascosta
            bool isErroreFaseAttuale = _segnalazioneObserver.IsErroreFaseAttuale;
            string descrizione = _segnalazioneObserver.DescrizioneDifetto ?? "";
            if (!isErroreFaseAttuale)
                descrizione = (descrizione + " #Errore in fase precedente o materia prima#").Trim();

            _segnalazioniDifformitaService.InsertSegnalazione(new SegnalazioneDifformita
            {
                OrigineSegnalazione = "I",
                Richiedente = _dialogoOperatoreObserver.OperatoreSelezionato.Badge + " - " + _dialogoOperatoreObserver.OperatoreSelezionato.Cognome + " " + _dialogoOperatoreObserver.OperatoreSelezionato.Nome,
                Flusso = flusso,
                FaseDifformita = isErroreFaseAttuale ? attivita?.CodiceFase : null,
                DescrizioneFase = attivita?.DescrizioneFase,
                Odp = attivita?.Odp,
                Articolo = attivita?.CodiceArticolo,
                DescrizioneArticolo = attivita?.DescrizioneArticolo,
                QtaProdotta = _avanzamentoObserver.QuantitaProdotta,
                QtaDifforme = _avanzamentoObserver.QuantitaScartata,
                CostoGestioneDifformita = 5,
                CostoUnitarioMateriale = Math.Round(costiArticoloDTO.CostoUnitarioMateriale, 2),
                CostoLavorazione = Math.Round(costiArticoloDTO.CostoUnitarioLavorazione, 2),
                DescrizioneDifformita = descrizione,
                CategoriaDifformita = _segnalazioneObserver.Categoria,
                QtaDifformiRecuperati = _segnalazioneObserver.QuantitaRecuperata,
                Sorgente = "DialogoOperatore"
            });
        }

        public override async void Execute(object? parameter)
        {
            await SafeExecuteAsync(async () =>
            {
                _dialogoOperatoreObserver.IsOperazioneGestita = false;

                _segnalazioneObserver.AttivitaPerSegnalazione = _dialogoOperatoreObserver.AttivitaSelezionata;

                string? testoPopup = await _popupConfermaHelper.GetTestoPopupAsync();

                if (testoPopup == string.Empty)
                    await MostraLoaderEGestisciOperazione();
                else
                    MostraPopupConTesto(testoPopup);
            }, _loggingService, "ConfermaCommand.Execute",
               _dialogoOperatoreObserver.OperatoreSelezionato?.Badge?.ToString());
        }

        private async Task MostraLoaderEGestisciOperazione()
        {
            _dialogoOperatoreObserver.IsLoaderVisibile = true;
            await Task.Delay(1);
            await GestioneEsecuzioneOperazioneAsync();
            _dialogoOperatoreObserver.IsLoaderVisibile = false;

            _dialogoOperatoreObserver.IsOperazioneGestita = true;
        }

		private async Task GestioneEsecuzioneOperazioneAsync()
        {
            AssegnaSaldoAccontoAdAttivitaSelezionata();

            await AssegnaMacchinaFittiziaAdOperatoreAsync();

            await EseguiOperazioneOMostraMessaggioAsync();

            _dialogoOperatoreObserver.OperazioneInCorso = Costanti.NESSUNA;
        }

        private void AssegnaSaldoAccontoAdAttivitaSelezionata()
        {
            if (_dialogoOperatoreObserver.AttivitaSelezionata != null)
                _dialogoOperatoreObserver.AttivitaSelezionata.SaldoAcconto = _avanzamentoObserver.SaldoAcconto;
        }

        private async Task AssegnaMacchinaFittiziaAdOperatoreAsync()
        {
            if (!CanAssegnareMacchinaFittiziaAdOperatore())
                return;

            Macchina? macchina = await _macchinaService.GetPrimaMacchinaFittiziaNonUtilizzataAsync();
            if (macchina == null)
            {
                MostraPopupConTesto(Costanti.ERRORE_MACCHINE_FINITE);
                return;
            }
            _dialogoOperatoreObserver.OperatoreSelezionato.MacchineAssegnate.Add(macchina);
        }

        private bool CanAssegnareMacchinaFittiziaAdOperatore()
        {
            return !_dialogoOperatoreObserver.OperatoreSelezionato.MacchineAssegnate.Any() &&
                    (_dialogoOperatoreObserver.OperazioneInCorso == Costanti.INIZIO_ATTREZZAGGIO ||
                     _dialogoOperatoreObserver.OperazioneInCorso == Costanti.INIZIO_LAVORO ||
                     _dialogoOperatoreObserver.OperazioneInCorso == Costanti.AVANZAMENTO);
        }

        private async Task EseguiOperazioneOMostraMessaggioAsync()
        {
            string? result = await _confermaOperazioneHelper.EseguiOperazioneAsync();
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
