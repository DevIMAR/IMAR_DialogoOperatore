using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Interfaces.Observers
{
    public interface ITaskCompilerObserver
    {
        bool IsPopupVisible { get; set; }
        string CategoriaErroreSelezionata { get; set; }
        string Note { get; set; }
        TimbraturaAttivitaViewModel? EventoSelezionato { get; set; }

        public event Action OnIsPopupVisibleChanged;
        public event Action OnCategoriaErroreSelezionataChanged;
        public event Action OnNoteChanged;
        public event Action OnEventoSelezionatoChanged;
    }
}
