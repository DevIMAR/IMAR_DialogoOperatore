namespace IMAR_DialogoOperatore.Interfaces.Observers
{
    public interface ITaskCompilerObserver
    {
        string CategoriaErroreSelezionata { get; set; }
        string Note { get; set; }

        public event Action OnCategoriaErroreSelezionataChanged;
        public event Action OnNoteChanged;
    }
}
