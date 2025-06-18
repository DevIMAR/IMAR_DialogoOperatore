using IMAR_DialogoOperatore.Application.Interfaces.Clients;
using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Interfaces.Observers;
using System.Windows.Input;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class TaskPopupViewModel : ViewModelBase
    {
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;

        private bool _visible;
        private TaskAsana _taskAsana;
        private string _categoriaErroreSelezionata;

        public List<string> CategorieErrori => new List<string>() { "---", "Quantità prodotta errata", "Quantità scartata errata", "Chiusura a saldo errata", "Timbratura errata" };
        public bool IsDescrizioneErroreAttiva => CategoriaErroreSelezionata != null;

        public ICommand InviaTaskCommand { get; }

        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                InizializzaCampi();

                OnNotifyStateChanged();
            }
        }

        public TaskAsana TaskAsana
        {
            get { return _taskAsana; }
            set
            {
                _taskAsana = value;
                OnNotifyStateChanged();
            }
        }
        public string CategoriaErroreSelezionata
        {
            get { return _categoriaErroreSelezionata; }
            set
            {
                _categoriaErroreSelezionata = value;
                ModificaTestoPrecompilato();

                OnNotifyStateChanged();
            }
        }

        public TaskPopupViewModel(
            IDialogoOperatoreObserver dialogoOperatoreObserver,
            IImarApiClient imarApiClient)
        {
            _dialogoOperatoreObserver = dialogoOperatoreObserver;
            InviaTaskCommand = new InviaTaskCommand(this, imarApiClient);
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

            CategoriaErroreSelezionata = null;
        }

        private void ModificaTestoPrecompilato()
        {
            switch (CategoriaErroreSelezionata)
            {
                case "Quantità prodotta errata":
                    TaskAsana.Name = "Richiesta modifica QUANTITà PRODOTTA errata";
                    TaskAsana.Html_notes = "Bolla: " + _dialogoOperatoreObserver.AttivitaSelezionata?.Bolla + "\n" +
                                           "Quantità prodotta dichiarata: " + _dialogoOperatoreObserver.AttivitaSelezionata?.QuantitaProdotta + "\n" +
                                           "Quantità prodotta reale: \n";
                    break;

                case "Quantità scartata errata":
                    TaskAsana.Name = "Richiesta modifica QUANTITà SCARTATA errata";
                    TaskAsana.Html_notes = "Bolla: " + _dialogoOperatoreObserver.AttivitaSelezionata?.Bolla + "\n" +
                                           "Quantità scartata dichiarata: " + _dialogoOperatoreObserver.AttivitaSelezionata?.QuantitaScartata + "\n" +
                                           "Quantità scartata reale: \n";
                    break;

                case "Chiusura a saldo errata":
                    TaskAsana.Name = "Richiesta modifica CHIUSURA A SALDO errata";
                    TaskAsana.Html_notes = "Bolla: " + _dialogoOperatoreObserver.AttivitaSelezionata?.Bolla + "\n" +
                                           "Quantità prodotta dichiarata: " + _dialogoOperatoreObserver.AttivitaSelezionata?.QuantitaProdotta + "\n" +
                                           "Quantità prodotta reale: \n";
                    break;

                case "Timbratura errata":
                    TaskAsana.Name = "Richiesta modifica TIMBRATURA errata";
                    TaskAsana.Html_notes = "Operatore: " + _dialogoOperatoreObserver.OperatoreSelezionato.Nome + " " + _dialogoOperatoreObserver.OperatoreSelezionato.Cognome + "\n" +
                                           "Badge: " + _dialogoOperatoreObserver.OperatoreSelezionato.Badge + "\n" +
                                           "Tipologia timbratura (entrata/uscita): \n" +
                                           "Orario timbratura (approssimativo): \n";
                    break;

                default:
                    TaskAsana.Html_notes = null;
                    break;
            }
        }
    }
}
