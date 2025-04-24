using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.Observers
{
    public class SegnalazioneObserver : ObserverBase, ISegnalazioneObserver
    {
        private uint? _quantitaRecuperata;
        private string _categoria;
        private bool _isErroreFaseAttuale;
        private string _descrizioneDifetto;
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

        public event Action OnQuantitaRecuperataChanged;
        public event Action OnCategoriaChanged;
        public event Action OnIsErroreFaseAttualeChanged;
        public event Action OnDescrizioneDifettoChanged;
    }
}
