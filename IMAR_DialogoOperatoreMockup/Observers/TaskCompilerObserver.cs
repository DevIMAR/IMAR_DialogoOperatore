using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Observers
{
    public class TaskCompilerObserver : ObserverBase, ITaskCompilerObserver
    {
        private bool _isPopupVisible;
        private string _categoriaErroreSelezionata;
        private string _note;
        private TimbraturaAttivitaViewModel? _eventoSelezionato;

        public bool IsPopupVisible 
        { 
            get { return _isPopupVisible; } 
            set
            {
                _isPopupVisible = value;
                InvokeAsync(OnIsPopupVisibleChanged);
            }
        }
        public string CategoriaErroreSelezionata
        {
            get { return _categoriaErroreSelezionata; }
            set
            {
                _categoriaErroreSelezionata = value;
                InvokeAsync(OnCategoriaErroreSelezionataChanged);
            }
        }
        public string Note
        {
            get { return _note; }
            set
            {
                _note = value;
                InvokeAsync(OnNoteChanged);
            }
        }
        public TimbraturaAttivitaViewModel? EventoSelezionato 
        { 
            get { return _eventoSelezionato; } 
            set
            {
                _eventoSelezionato = value;
                InvokeAsync(OnEventoSelezionatoChanged);
            }
        }

        public event Action OnIsPopupVisibleChanged;
        public event Action OnCategoriaErroreSelezionataChanged;
        public event Action OnNoteChanged;
        public event Action OnEventoSelezionatoChanged;
    }
}
