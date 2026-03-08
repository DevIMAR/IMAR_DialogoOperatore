using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Observers
{
    public class TaskCompilerObserver : ObserverBase, ITaskCompilerObserver
    {
        private bool _isPopupVisible;
        private string _note;
        private bool _isRettificaQuantita;
        private bool _isTogliSaldo;
        private bool _isCorreggiOrarioInizio;
        private bool _isCorreggiOrarioFine;
        private EventoRaggrupatoViewModel? _eventoRaggrupatoSelezionato;
        private int _oraInizio;
        private int _minutoInizio;
        private int _oraFine;
        private int _minutoFine;

        public bool IsPopupVisible
        {
            get { return _isPopupVisible; }
            set
            {
                _isPopupVisible = value;
                InvokeAsync(OnIsPopupVisibleChanged);
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

        public bool IsRettificaQuantita
        {
            get { return _isRettificaQuantita; }
            set
            {
                _isRettificaQuantita = value;
                InvokeAsync(OnCorrezioniChanged);
            }
        }

        public bool IsTogliSaldo
        {
            get { return _isTogliSaldo; }
            set
            {
                _isTogliSaldo = value;
                InvokeAsync(OnCorrezioniChanged);
            }
        }

        public bool IsCorreggiOrarioInizio
        {
            get { return _isCorreggiOrarioInizio; }
            set
            {
                _isCorreggiOrarioInizio = value;
                InvokeAsync(OnCorrezioniChanged);
            }
        }

        public bool IsCorreggiOrarioFine
        {
            get { return _isCorreggiOrarioFine; }
            set
            {
                _isCorreggiOrarioFine = value;
                InvokeAsync(OnCorrezioniChanged);
            }
        }

        public EventoRaggrupatoViewModel? EventoRaggrupatoSelezionato
        {
            get { return _eventoRaggrupatoSelezionato; }
            set
            {
                _eventoRaggrupatoSelezionato = value;
                InvokeAsync(OnEventoRaggrupatoSelezionatoChanged);
            }
        }

        public int OraInizio { get { return _oraInizio; } set { _oraInizio = value; } }
        public int MinutoInizio { get { return _minutoInizio; } set { _minutoInizio = value; } }
        public int OraFine { get { return _oraFine; } set { _oraFine = value; } }
        public int MinutoFine { get { return _minutoFine; } set { _minutoFine = value; } }

        public event Action OnIsPopupVisibleChanged;
        public event Action OnCorrezioniChanged;
        public event Action OnNoteChanged;
        public event Action OnEventoRaggrupatoSelezionatoChanged;
    }
}
