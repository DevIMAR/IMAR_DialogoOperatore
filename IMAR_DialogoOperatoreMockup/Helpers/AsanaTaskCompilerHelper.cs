using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Services;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Helpers
{
    public class AsanaTaskCompilerHelper : ITaskCompilerHelper
    {
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
        private readonly ITaskCompilerObserver _taskCompilerObserver;
        private readonly IAvanzamentoObserver _avanzamentoObserver;
        private readonly IUtenteService _utenteService;

        public TaskAsana TaskAsana { get; private set; }

        public AsanaTaskCompilerHelper(
            IDialogoOperatoreObserver dialogoOperatoreObserver,
            ITaskCompilerObserver taskCompilerObserver,
            IAvanzamentoObserver avanzamentoObserver,
            IUtenteService utenteService)
        {
            _dialogoOperatoreObserver = dialogoOperatoreObserver;
            _taskCompilerObserver = taskCompilerObserver;
            _avanzamentoObserver = avanzamentoObserver;
            _utenteService = utenteService;
        }

        public async Task CompilaTaskAsanaAsync()
        {
            InizializzaCampi();

            var evento = _taskCompilerObserver.EventoRaggrupatoSelezionato;
            List<string> nomiCorrezioni = new List<string>();

            // Riga 1: Bolla | ODP | Fase
            if (evento != null)
            {
                TaskAsana.Html_notes += "Bolla: " + evento.Bolla + " | Odp: " + evento.Odp + " | Fase: " + evento.CodiceFase + "\n";

                // Riga 2: Orari originali
                string orari = "";
                if (evento.OraInizio != null)
                    orari += "Inizio: " + evento.OraInizio.Value.ToString("HH:mm");
                if (evento.OraFine != null)
                    orari += (orari.Length > 0 ? " | " : "") + "Fine: " + evento.OraFine.Value.ToString("HH:mm");
                if (orari.Length > 0)
                    TaskAsana.Html_notes += orari + "\n";

                TaskAsana.Html_notes += "\n";
            }

            // Rettifica quantità
            if (_taskCompilerObserver.IsRettificaQuantita)
            {
                nomiCorrezioni.Add("Rettifica quantità");
                int qtOriginale = evento?.QuantitaProdotta ?? 0;
                int qtNuova = (int)(_avanzamentoObserver.QuantitaProdotta ?? 0);
                int scOriginale = evento?.QuantitaScartata ?? 0;
                int scNuova = (int)(_avanzamentoObserver.QuantitaScartata ?? 0);
                string saldoAcconto = _avanzamentoObserver.SaldoAcconto == Costanti.SALDO ? "Saldo" : "Acconto";

                TaskAsana.Html_notes += "RETTIFICA QUANTITÀ: ";
                TaskAsana.Html_notes += "Prodotta: " + qtOriginale + " → " + qtNuova;
                TaskAsana.Html_notes += " | Scartata: " + scOriginale + " → " + scNuova;
                TaskAsana.Html_notes += " | " + saldoAcconto + "\n";
            }

            // Togli saldo
            if (_taskCompilerObserver.IsTogliSaldo)
            {
                nomiCorrezioni.Add("Togli saldo");
                TaskAsana.Html_notes += "TOGLI SALDO: Riportare in acconto\n";
            }

            // Correggi orario inizio
            if (_taskCompilerObserver.IsCorreggiOrarioInizio)
            {
                nomiCorrezioni.Add("Correggi orario inizio");
                string orarioOriginale = evento?.OraInizio != null ? evento.OraInizio.Value.ToString("HH:mm") : "--:--";
                string nuovoOrario = _taskCompilerObserver.OraInizio.ToString().PadLeft(2, '0') + ":" +
                                     _taskCompilerObserver.MinutoInizio.ToString().PadLeft(2, '0');
                TaskAsana.Html_notes += "CORREGGI ORARIO INIZIO: " + orarioOriginale + " → " + nuovoOrario + "\n";
            }

            // Correggi orario fine
            if (_taskCompilerObserver.IsCorreggiOrarioFine)
            {
                nomiCorrezioni.Add("Correggi orario fine");
                string orarioOriginale = evento?.OraFine != null ? evento.OraFine.Value.ToString("HH:mm") : "--:--";
                string nuovoOrario = _taskCompilerObserver.OraFine.ToString().PadLeft(2, '0') + ":" +
                                     _taskCompilerObserver.MinutoFine.ToString().PadLeft(2, '0');
                TaskAsana.Html_notes += "CORREGGI ORARIO FINE: " + orarioOriginale + " → " + nuovoOrario + "\n";
            }

            // Elimina attività
            if (_taskCompilerObserver.IsEliminaAttivita)
            {
                nomiCorrezioni.Add("Elimina attività");
                TaskAsana.Html_notes += "RICHIESTA ELIMINAZIONE ATTIVITA\n";
            }

            // Titolo task: ODP FASE - Richiesta: correzioni
            string prefisso = evento != null ? evento.Odp + " " + evento.CodiceFase + " - " : "";
            TaskAsana.Name = prefisso + "Richiesta: " + string.Join(" + ", nomiCorrezioni);

            // Note utente
            if (!string.IsNullOrWhiteSpace(_taskCompilerObserver.Note))
                TaskAsana.Html_notes += "\nNote: " + _taskCompilerObserver.Note + "\n";

            // Firma operatore
            var operatore = _dialogoOperatoreObserver.OperatoreSelezionato;
            string firmaOperatore = operatore.Nome + " " + operatore.Cognome + " - " + operatore.Badge;
            TaskAsana.Html_notes += "\n" + firmaOperatore;

            // Follower automatico: se l'operatore ha un account Asana, aggiungilo come follower
            string badgePadded = operatore.Badge?.ToString().PadLeft(4, '0') ?? "";
            string? idAsanaOperatore = await _utenteService.GetIdAsanaByBadgeAsync(badgePadded);
            if (!string.IsNullOrEmpty(idAsanaOperatore))
                TaskAsana.Followers.Add(idAsanaOperatore);
        }

        private void InizializzaCampi()
        {
            TaskAsana = new TaskAsana()
            {
                Workspace = "10278744064308",
                Projects = new List<string>() { "1210581212376329" },
                Followers = new List<string>() { "michele.casoli@imarsrl.com" },
                Tags = new List<string>(),
                Assignee = "luca.marangoni@imarsrl.com",
                Html_notes = string.Empty
            };
        }
    }
}
