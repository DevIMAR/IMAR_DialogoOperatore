using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Interfaces.ViewModels;

namespace IMAR_DialogoOperatore.Observers
{
    public class SegnalazioneObserver : ObserverBase, ISegnalazioneObserver
    {
        private uint? _quantitaRecuperata;
        private string _categoria;
        private bool _isErroreFaseAttuale;
        private string _descrizioneDifetto;
        private IDatiAttivitaBase? _attivitaPerSegnalazione;
        private bool _isPopupVisible;
        private bool _isConfermato;

        public uint? QuantitaRecuperata
        {
            get { return _quantitaRecuperata; }
            set
            {
                _quantitaRecuperata = value;
                CallAction(OnQuantitaRecuperataChanged);
            }
        }
        public string Categoria
        {
            get { return _categoria; }
            set
            {
                _categoria = value;
                CallAction(OnCategoriaChanged);
            }
        }
        public bool IsErroreFaseAttuale
        {
            get { return _isErroreFaseAttuale; }
            set
            {
                _isErroreFaseAttuale = value;
                CallAction(OnIsErroreFaseAttualeChanged);
            }
        }
        public string DescrizioneDifetto
        {
            get { return _descrizioneDifetto; }
            set
            {
                _descrizioneDifetto = value;
                CallAction(OnDescrizioneDifettoChanged);
            }
        }
        public IDatiAttivitaBase? AttivitaPerSegnalazione
        {
            get { return _attivitaPerSegnalazione; }
            set
            {
                _attivitaPerSegnalazione = value;
                CallAction(OnAttivitaPerSegnalazioneChanged);
            }
        }
        public bool IsPopupVisible
        {
            get { return _isPopupVisible; }
            set
            {
                _isPopupVisible = value;
                CallAction(OnIsPopupVisibleChanged);
            }
        }
        public bool IsConfermato
        {
            get { return _isConfermato; }
            set
            {
                _isConfermato = value;
                CallAction(OnIsConfermatoChanged);
            }
        }

        public event Action OnQuantitaRecuperataChanged;
        public event Action OnCategoriaChanged;
        public event Action OnIsErroreFaseAttualeChanged;
        public event Action OnDescrizioneDifettoChanged;
        public event Action OnAttivitaPerSegnalazioneChanged;
        public event Action OnIsPopupVisibleChanged;
        public event Action OnIsConfermatoChanged;
    }
}
