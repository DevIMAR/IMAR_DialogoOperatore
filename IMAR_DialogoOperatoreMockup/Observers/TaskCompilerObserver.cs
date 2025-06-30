using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.Observers
{
    public class TaskCompilerObserver : ObserverBase, ITaskCompilerObserver
    {
        private string _categoriaErroreSelezionata;
        private string _note;

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

        public event Action OnCategoriaErroreSelezionataChanged;
        public event Action OnNoteChanged;
    }
}
