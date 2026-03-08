using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class CompilatoreTaskViewModel : ViewModelBase
    {
        private readonly ITaskCompilerObserver _taskCompilerObserver;

        private string _note;
        private EventoRaggrupatoViewModel? _eventoRaggrupatoSelezionato;
        private int _oraInizio;
        private int _minutoInizio;
        private int _oraFine;
        private int _minutoFine;

        public bool IsRettificaQuantita => _taskCompilerObserver.IsRettificaQuantita;
        public bool IsTogliSaldo => _taskCompilerObserver.IsTogliSaldo;
        public bool IsCorreggiOrarioInizio => _taskCompilerObserver.IsCorreggiOrarioInizio;
        public bool IsCorreggiOrarioFine => _taskCompilerObserver.IsCorreggiOrarioFine;

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

        public EventoRaggrupatoViewModel? EventoRaggrupatoSelezionato
        {
            get { return _eventoRaggrupatoSelezionato; }
            set
            {
                _eventoRaggrupatoSelezionato = value;
                _taskCompilerObserver.EventoRaggrupatoSelezionato = _eventoRaggrupatoSelezionato;
                PrecompilaOrari();
                OnNotifyStateChanged();
            }
        }

        public int OraInizio
        {
            get { return _oraInizio; }
            set { _oraInizio = value; _taskCompilerObserver.OraInizio = value; OnNotifyStateChanged(); }
        }
        public int MinutoInizio
        {
            get { return _minutoInizio; }
            set { _minutoInizio = value; _taskCompilerObserver.MinutoInizio = value; OnNotifyStateChanged(); }
        }
        public int OraFine
        {
            get { return _oraFine; }
            set { _oraFine = value; _taskCompilerObserver.OraFine = value; OnNotifyStateChanged(); }
        }
        public int MinutoFine
        {
            get { return _minutoFine; }
            set { _minutoFine = value; _taskCompilerObserver.MinutoFine = value; OnNotifyStateChanged(); }
        }

        public CompilatoreTaskViewModel(
            ITaskCompilerObserver taskCompilerObserver)
        {
            _taskCompilerObserver = taskCompilerObserver;

            _taskCompilerObserver.OnCorrezioniChanged += TaskCompilerObserver_OnCorrezioniChanged;
            _taskCompilerObserver.OnIsPopupVisibleChanged += TaskCompilerObserver_OnIsPopupVisibleChanged;
        }

        private void TaskCompilerObserver_OnCorrezioniChanged()
        {
            // Se è stato spuntato "Togli saldo", precompila la nota
            if (_taskCompilerObserver.IsTogliSaldo && string.IsNullOrWhiteSpace(_note))
                Note = "Riportare in acconto";

            OnNotifyStateChanged();
        }

        private void TaskCompilerObserver_OnIsPopupVisibleChanged()
        {
            if (_taskCompilerObserver.IsPopupVisible)
            {
                Note = null;
                _oraInizio = 0; _minutoInizio = 0;
                _oraFine = 0; _minutoFine = 0;
            }
            OnNotifyStateChanged();
        }

        /// <summary>
        /// Pre-compila i campi orario con gli orari dell'evento selezionato.
        /// </summary>
        private void PrecompilaOrari()
        {
            if (_eventoRaggrupatoSelezionato?.OraInizio != null)
            {
                OraInizio = _eventoRaggrupatoSelezionato.OraInizio.Value.Hour;
                MinutoInizio = _eventoRaggrupatoSelezionato.OraInizio.Value.Minute;
            }

            if (_eventoRaggrupatoSelezionato?.OraFine != null)
            {
                OraFine = _eventoRaggrupatoSelezionato.OraFine.Value.Hour;
                MinutoFine = _eventoRaggrupatoSelezionato.OraFine.Value.Minute;
            }
        }
    }
}
