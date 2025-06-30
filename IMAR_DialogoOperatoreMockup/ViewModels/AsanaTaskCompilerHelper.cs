using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.ViewModels
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

            switch (_taskCompilerObserver.CategoriaErroreSelezionata)
            {
                case Costanti.TASK_QUANTITA_ERRATA:
                    string saldoAcconto = _avanzamentoAttivitaViewModel.IsFaseCompletabile ? "Saldo" : "Acconto";

                    TaskAsana.Name = "Richiesta modifica QUANTITà DICHIARATA errata";
                    TaskAsana.Html_notes = "Bolla: " + _infoBaseAttivitaViewModel.Bolla + "\n" +
                                           "Odp: " + _infoBaseAttivitaViewModel.Odp + "\n" +
                                           "Fase: " + _infoBaseAttivitaViewModel.FaseSelezionata + "\n" +
                                           "Quantità prodotta: " + _avanzamentoAttivitaViewModel.QuantitaProdotta + "\n" +
                                           "Quantità scartata: " + _avanzamentoAttivitaViewModel.QuantitaScartata + "\n" +
                                           "Saldo/acconto: " + saldoAcconto + "\n" +
                                           "Note: " + _taskCompilerObserver.Note + "\n";
                    break;

                case Costanti.TASK_CHIUSURA_A_SALDO_ERRATA:
                    TaskAsana.Name = "Richiesta modifica CHIUSURA A SALDO errata";
                    TaskAsana.Html_notes = "Bolla: " + _infoBaseAttivitaViewModel.Bolla + "\n" +
                                           "Odp: " + _infoBaseAttivitaViewModel.Odp + "\n" +
                                           "Fase: " + _infoBaseAttivitaViewModel.FaseSelezionata + "\n" +
                                           "Note: " + _taskCompilerObserver.Note + "\n";
                    break;

                case Costanti.TASK_TIMBRATURA_ERRATA:
                    TaskAsana.Name = "Richiesta modifica TIMBRATURA errata";
                    TaskAsana.Html_notes = "Operatore: " + _infoTaskOperatoreViewModel.NomeCognomeOperatore + "\n" +
                                           "Badge: " + _infoTaskOperatoreViewModel.BadgeOperatore + "\n" +
                                           "Tipologia timbratura: " + _infoTaskOperatoreViewModel.TipologiaTimbraturaErrataSelezionata + "\n" +
                                           "Orario da correggere: " + _infoTaskOperatoreViewModel.OrarioDaCorreggere + "\n" +
                                           "Orario reale: " + _infoTaskOperatoreViewModel.OrarioDaDichiarare + "\n" +
                                           "Note: " + _taskCompilerObserver.Note + "\n";
                    break;

                case Costanti.TASK_ALTRO:
                    TaskAsana.Name = "Richiesta per altro errore: leggere descrizione";
                    TaskAsana.Html_notes = "Note: " + _taskCompilerObserver.Note + "\n";
                    break;

                default:
                    TaskAsana.Html_notes = null;
                    break;
            }

            string firmaOperatore = _dialogoOperatoreObserver.OperatoreSelezionato.Nome + " " + _dialogoOperatoreObserver.OperatoreSelezionato.Cognome;
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
                Assignee = "federico.crescenzi@imarsrl.com" //"produzione@imarsrl.com"
            };
        }
    }
}
