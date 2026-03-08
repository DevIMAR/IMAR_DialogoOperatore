using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Application.Interfaces.Services.External;
using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using IMAR_DialogoOperatore.Application.DTOs;
using IMAR_DialogoOperatore.Domain.Entities.Imar_Produzione;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.Commands
{
    public class InviaTaskCommand : CommandBase
    {
        private readonly IImarApiClient _imarApiClient;
        private readonly ITaskCompilerHelper _taskCompilerHelper;
        private readonly ITaskCompilerObserver _taskCompilerObserver;
        private readonly IAvanzamentoObserver _avanzamentoObserver;
        private readonly ISegnalazioneObserver _segnalazioneObserver;
        private readonly ISegnalazioniDifformitaService _segnalazioniDifformitaService;
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
        private readonly IPopupObserver _popupObserver;
        private readonly ILoggingService _loggingService;

        private bool _attendeSegnalazione;

        public InviaTaskCommand(
            IImarApiClient imarApiClient,
            ITaskCompilerHelper taskCompilerHelper,
            ITaskCompilerObserver taskCompilerObserver,
            IAvanzamentoObserver avanzamentoObserver,
            ISegnalazioneObserver segnalazioneObserver,
            ISegnalazioniDifformitaService segnalazioniDifformitaService,
            IDialogoOperatoreObserver dialogoOperatoreObserver,
            IPopupObserver popupObserver,
            ILoggingService loggingService)
        {
            _imarApiClient = imarApiClient;
            _taskCompilerHelper = taskCompilerHelper;
            _taskCompilerObserver = taskCompilerObserver;
            _avanzamentoObserver = avanzamentoObserver;
            _segnalazioneObserver = segnalazioneObserver;
            _segnalazioniDifformitaService = segnalazioniDifformitaService;
            _dialogoOperatoreObserver = dialogoOperatoreObserver;
            _popupObserver = popupObserver;
            _loggingService = loggingService;

            _taskCompilerObserver.OnCorrezioniChanged += CanExecuteEvaluator;
            _taskCompilerObserver.OnNoteChanged += CanExecuteEvaluator;
            _taskCompilerObserver.OnEventoRaggrupatoSelezionatoChanged += CanExecuteEvaluator;
            _popupObserver.OnIsConfermatoChanged += PopupObserver_OnIsConfermatoChanged;
        }

        private void CanExecuteEvaluator()
        {
            CanExecute(null);
        }

        public override bool CanExecute(object? parameter)
        {
            // Serve almeno un checkbox spuntato e un evento selezionato
            bool haCorrezione = _taskCompilerObserver.IsRettificaQuantita ||
                                _taskCompilerObserver.IsTogliSaldo ||
                                _taskCompilerObserver.IsCorreggiOrarioInizio ||
                                _taskCompilerObserver.IsCorreggiOrarioFine;

            bool haEvento = _taskCompilerObserver.EventoRaggrupatoSelezionato != null;

            return haCorrezione && haEvento;
        }

        public override async void Execute(object? parameter)
        {
            await SafeExecuteAsync(async () =>
            {
                if (_avanzamentoObserver.QuantitaScartata != null && _avanzamentoObserver.QuantitaScartata > 0
                    && _taskCompilerObserver.IsRettificaQuantita)
                {
                    _attendeSegnalazione = true;
                    _popupObserver.TestoPopup = null;
                    _popupObserver.IsPopupVisible = true;
                }
                else
                {
                    await CompilaEdInviaTask();
                }
            }, _loggingService, "InviaTaskCommand.Execute");
        }

        private async void PopupObserver_OnIsConfermatoChanged()
        {
            await SafeExecuteAsync(async () =>
            {
                if (!_attendeSegnalazione)
                    return;

                _attendeSegnalazione = false;

                if (!_popupObserver.IsConfermato)
                    return;

                await CreaEdInviaSegnalazioneDifformita();
                await CompilaEdInviaTask();
            }, _loggingService, "InviaTaskCommand.PopupObserver_OnIsConfermatoChanged");
        }

        private async Task CompilaEdInviaTask()
        {
            await _taskCompilerHelper.CompilaTaskAsanaAsync();
            await _imarApiClient.SendTaskAsana(_taskCompilerHelper.TaskAsana, "michele.casoli@imarsrl.com");
        }

        private async Task CreaEdInviaSegnalazioneDifformita()
        {
            var evento = _taskCompilerObserver.EventoRaggrupatoSelezionato;

            CostiArticoloDTO costiArticoloDTO = evento?.CodiceArticolo != null
                ? await _segnalazioniDifformitaService.GetCostiArticolo(evento.CodiceArticolo)
                : new CostiArticoloDTO();

            _segnalazioniDifformitaService.InsertSegnalazione(new SegnalazioneDifformita
            {
                OrigineSegnalazione = "I",
                Richiedente = _dialogoOperatoreObserver.OperatoreSelezionato.Badge + " - " + _dialogoOperatoreObserver.OperatoreSelezionato.Cognome + " " + _dialogoOperatoreObserver.OperatoreSelezionato.Nome,
                FaseDifformita = evento?.CodiceFase,
                DescrizioneFase = evento?.DescrizioneFase,
                Odp = evento?.Odp,
                Articolo = evento?.CodiceArticolo,
                DescrizioneArticolo = evento?.DescrizioneArticolo,
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
    }
}
