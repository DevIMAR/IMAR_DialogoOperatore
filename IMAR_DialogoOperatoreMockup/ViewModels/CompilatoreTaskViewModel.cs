using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class CompilatoreTaskViewModel : ViewModelBase
    {
        private readonly ITaskCompilerObserver _taskCompilerObserver;

        private string _note;
        private TimbraturaAttivitaViewModel? _eventoSelezionato;

        public string CategoriaErroreSelezionata => _taskCompilerObserver.CategoriaErroreSelezionata;
        public bool IsDescrizioneErroreAttiva => _taskCompilerObserver.CategoriaErroreSelezionata != null &&
                                                 _taskCompilerObserver.CategoriaErroreSelezionata != Costanti.TASK_CHIUSURA_A_SALDO_ERRATA;
        public bool ShowTimbratureInGriglia => string.IsNullOrWhiteSpace(_taskCompilerObserver.CategoriaErroreSelezionata) ||
                                                !_taskCompilerObserver.CategoriaErroreSelezionata.ToLower().Contains("attività");

        public string Note
        {
            get { return _note; }
            set 
            { 
                _note = value;
                _taskCompilerObserver.Note = _note;

                OnNotifyStateChanged();
            }
        }
        public TimbraturaAttivitaViewModel? EventoSelezionato
        {
            get { return _eventoSelezionato; }
            set
            {
                _eventoSelezionato = value;
                _taskCompilerObserver.EventoSelezionato = _eventoSelezionato;

                OnNotifyStateChanged();
            }
        }

        public CompilatoreTaskViewModel(
            ITaskCompilerObserver taskCompilerObserver)
        {
            _taskCompilerObserver = taskCompilerObserver;

            _taskCompilerObserver.OnCategoriaErroreSelezionataChanged += TaskCompilerObserver_OnCategoriaErroreSelezionataChanged;
        }

        private void TaskCompilerObserver_OnCategoriaErroreSelezionataChanged()
        {
            ModificaTestoPrecompilato();
            OnNotifyStateChanged();
        }

        private void ModificaTestoPrecompilato()
        {
            switch (_taskCompilerObserver.CategoriaErroreSelezionata)
            {
                case Costanti.TASK_CHIUSURA_A_SALDO_ERRATA:
                    Note = "Ho chiuso a saldo per sbaglio. Riportare in acconto";
                    break;

                default:
                    Note = null;
                    break;
            }

            OnNotifyStateChanged();
        }
    }
}
