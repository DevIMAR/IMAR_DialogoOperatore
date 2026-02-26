using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.DTOs;
using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Application.Interfaces.Services.External;
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

        private bool _attendeSegnalazione;

        public InviaTaskCommand(
            IImarApiClient imarApiClient,
            ITaskCompilerHelper taskCompilerHelper,
            ITaskCompilerObserver taskCompilerObserver,
            IAvanzamentoObserver avanzamentoObserver,
            ISegnalazioneObserver segnalazioneObserver,
            ISegnalazioniDifformitaService segnalazioniDifformitaService,
            IDialogoOperatoreObserver dialogoOperatoreObserver,
            IPopupObserver popupObserver)
        {
            _imarApiClient = imarApiClient;
            _taskCompilerHelper = taskCompilerHelper;
            _taskCompilerObserver = taskCompilerObserver;
            _avanzamentoObserver = avanzamentoObserver;
            _segnalazioneObserver = segnalazioneObserver;
            _segnalazioniDifformitaService = segnalazioniDifformitaService;
            _dialogoOperatoreObserver = dialogoOperatoreObserver;
            _popupObserver = popupObserver;

            _taskCompilerObserver.OnCategoriaErroreSelezionataChanged += CanExecuteEvaluator;
            _taskCompilerObserver.OnNoteChanged += CanExecuteEvaluator;
            _popupObserver.OnIsConfermatoChanged += PopupObserver_OnIsConfermatoChanged;
        }

        private void CanExecuteEvaluator()
        {
            CanExecute(null);
        }

        public override bool CanExecute(object? parameter)
        {
            return _taskCompilerObserver.CategoriaErroreSelezionata != null &&
                   !(_taskCompilerObserver.CategoriaErroreSelezionata == Costanti.TASK_CHIUSURA_A_SALDO_ERRATA &&
                   string.IsNullOrWhiteSpace(_taskCompilerObserver.Note));
        }

        public override async void Execute(object? parameter)
        {
            if (_avanzamentoObserver.QuantitaScartata != null && _avanzamentoObserver.QuantitaScartata > 0)
            {
                _segnalazioneObserver.AttivitaPerSegnalazione = _taskCompilerObserver.EventoSelezionato;
                _attendeSegnalazione = true;
                _popupObserver.TestoPopup = null;
                _popupObserver.IsPopupVisible = true;
            }
            else
            {
                await CompilaEdInviaTask();
            }
        }

        private async void PopupObserver_OnIsConfermatoChanged()
        {
            if (!_attendeSegnalazione)
                return;

            _attendeSegnalazione = false;

            if (!_popupObserver.IsConfermato)
                return;

            await CreaEdInviaSegnalazioneDifformita();
            await CompilaEdInviaTask();
        }

        private async Task CompilaEdInviaTask()
        {
            _taskCompilerHelper.CompilaTaskAsana();
            await _imarApiClient.SendTaskAsana(_taskCompilerHelper.TaskAsana, "federico.crescenzi@imarsrl.com");
        }

        private async Task CreaEdInviaSegnalazioneDifformita()
        {
            var attivita = _segnalazioneObserver.AttivitaPerSegnalazione;

            CostiArticoloDTO costiArticoloDTO = attivita?.CodiceArticolo != null
                ? await _segnalazioniDifformitaService.GetCostiArticolo(attivita.CodiceArticolo)
                : new CostiArticoloDTO();

            _segnalazioniDifformitaService.InsertSegnalazione(new SegnalazioneDifformita
            {
                OrigineSegnalazione = "I",
                Richiedente = _dialogoOperatoreObserver.OperatoreSelezionato.Badge + " - " + _dialogoOperatoreObserver.OperatoreSelezionato.Cognome + " " + _dialogoOperatoreObserver.OperatoreSelezionato.Nome,
                FaseDifformita = attivita?.CodiceFase,
                DescrizioneFase = attivita?.DescrizioneFase,
                Odp = attivita?.Odp,
                Articolo = attivita?.CodiceArticolo,
                DescrizioneArticolo = attivita?.DescrizioneArticolo,
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
