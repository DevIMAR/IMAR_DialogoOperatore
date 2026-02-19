using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Helpers
{
    public class AsanaTaskCompilerHelper : ITaskCompilerHelper
    {
        private readonly InfoBaseAttivitaViewModel _infoBaseAttivitaViewModel;
        private readonly AvanzamentoAttivitaViewModel _avanzamentoAttivitaViewModel;
        private readonly InfoTaskOperatoreViewModel _infoTaskOperatoreViewModel;
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
        private readonly ITaskCompilerObserver _taskCompilerObserver;

        public TaskAsana TaskAsana { get; private set; }

        public AsanaTaskCompilerHelper(
            InfoBaseAttivitaViewModel infoBaseAttivitaViewModel,
            AvanzamentoAttivitaViewModel avanzamentoAttivitaViewModel,
            InfoTaskOperatoreViewModel infoTaskOperatoreViewModel,
            IDialogoOperatoreObserver dialogoOperatoreObserver,
            ITaskCompilerObserver taskCompilerObserver)
        {
            _infoBaseAttivitaViewModel = infoBaseAttivitaViewModel;
            _avanzamentoAttivitaViewModel = avanzamentoAttivitaViewModel;
            _infoTaskOperatoreViewModel = infoTaskOperatoreViewModel;
            _dialogoOperatoreObserver = dialogoOperatoreObserver;
            _taskCompilerObserver = taskCompilerObserver;
        }

        public void CompilaTaskAsana()
        {
            InizializzaCampi();

            if (_taskCompilerObserver.EventoSelezionato != null)
            {
                if (_taskCompilerObserver.EventoSelezionato.CodiceJMes != null)
                    TaskAsana.Html_notes += "Id: " + _taskCompilerObserver.EventoSelezionato.CodiceJMes + "\n";

                TaskAsana.Html_notes += "Bolla: " + _taskCompilerObserver.EventoSelezionato.Bolla + "\n" +
                                       "Odp: " + _taskCompilerObserver.EventoSelezionato.Odp + "\n" +
                                       "Fase: " + _taskCompilerObserver.EventoSelezionato.Fase + "\n";
            }

            switch (_taskCompilerObserver.CategoriaErroreSelezionata)
            {
                case Costanti.TASK_QUANTITA_ERRATA:
                    string saldoAcconto = _avanzamentoAttivitaViewModel.IsFaseCompletabile ? "Saldo" : "Acconto";

                    TaskAsana.Name = "Richiesta modifica QUANTITà DICHIARATA errata";
                    TaskAsana.Html_notes += "Quantità prodotta: " + _avanzamentoAttivitaViewModel.QuantitaProdotta + "\n" +
                                           "Quantità scartata: " + _avanzamentoAttivitaViewModel.QuantitaScartata + "\n" +
                                           "Saldo/acconto: " + saldoAcconto + "\n";
                    break;

                case Costanti.TASK_CHIUSURA_A_SALDO_ERRATA:
                    TaskAsana.Name = "Richiesta modifica CHIUSURA A SALDO errata";
                    break;

                case Costanti.TASK_TIMBRATURA_ERRATA:
                    TaskAsana.Name = "Richiesta modifica ORARIO TIMBRATURA errata";
                    TaskAsana.Html_notes += "Operatore: " + _infoTaskOperatoreViewModel.NomeCognomeOperatore + "\n" +
                                           "Causale: " + _taskCompilerObserver.EventoSelezionato.CausaleEstesa + "\n" +
                                           "Orario da correggere: " + _taskCompilerObserver.EventoSelezionato.Timestamp + "\n" +
                                           "Nuovo orario: " + _infoTaskOperatoreViewModel.OraDaDichiarare.ToString().PadLeft(2, '0') + ":" + _infoTaskOperatoreViewModel.MinutoDaDichiarare.ToString().PadLeft(2, '0') + "\n";
                    break;

                case Costanti.TASK_ALTRO:
                    TaskAsana.Name = "Richiesta per altro errore: leggere descrizione";
                    break;

                default:
                    TaskAsana.Html_notes = null;
                    break;
            }

            if (TaskAsana.Html_notes != null)
                    TaskAsana.Html_notes += "Note: " + _taskCompilerObserver.Note + "\n";

            string firmaOperatore = _dialogoOperatoreObserver.OperatoreSelezionato.Nome + " " + _dialogoOperatoreObserver.OperatoreSelezionato.Cognome + " - " + _infoTaskOperatoreViewModel.BadgeOperatore;
            TaskAsana.Html_notes += "\n\n" + firmaOperatore;
        }

        private void InizializzaCampi()
        {
            TaskAsana = new TaskAsana()
            {
                Workspace = "10278744064308",
                Projects = new List<string>() { "1210581212376329" },
                Followers = new List<string>(),
                Tags = new List<string>(),
                Assignee = "luca.marangoni@imarsrl.com", //"produzione@imarsrl.com"
                Html_notes = string.Empty
            };
        }
    }
}
