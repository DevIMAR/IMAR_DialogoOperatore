using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Interfaces.Observers
{
    public interface ITaskCompilerObserver
    {
        bool IsPopupVisible { get; set; }
        string Note { get; set; }

        // Checkbox flags per le correzioni selezionate
        bool IsRettificaQuantita { get; set; }
        bool IsTogliSaldo { get; set; }
        bool IsCorreggiOrarioInizio { get; set; }
        bool IsCorreggiOrarioFine { get; set; }
        bool IsEliminaAttivita { get; set; }

        // Evento raggruppato selezionato nella grid
        EventoRaggrupatoViewModel? EventoRaggrupatoSelezionato { get; set; }

        // Campi orario correzione
        int OraInizio { get; set; }
        int MinutoInizio { get; set; }
        int OraFine { get; set; }
        int MinutoFine { get; set; }

        public event Action OnIsPopupVisibleChanged;
        public event Action OnCorrezioniChanged;
        public event Action OnNoteChanged;
        public event Action OnEventoRaggrupatoSelezionatoChanged;
    }
}
