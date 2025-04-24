using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class FormSegnalazioneDifformitaViewModel : ViewModelBase
    {
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
        private readonly IAvanzamentoObserver _avanzamentoObserver;
        private readonly ISegnalazioneObserver _segnalazioneObserver;

        private uint? _quantitaScartata;
        private uint? _quantitaRecuperata;
        private string _categoria;
        private bool _isErroreFaseAttuale;
        private string _descrizioneDifetto;

        public string Bolla => _dialogoOperatoreObserver.AttivitaSelezionata.Bolla;
        public string Odp => _dialogoOperatoreObserver.AttivitaSelezionata.Odp;
        public string Fase => _dialogoOperatoreObserver.AttivitaSelezionata.Fase;
        public string DescrizioneFase => _dialogoOperatoreObserver.AttivitaSelezionata.DescrizioneFase;
        public List<string> ListaCategorie => new List<string> { "Dimensionale", "Finitura", "Materiale", "Strutturale", "Quantitativo", "Errore Disegno/Distinta", "Varie Imballo" };

        public uint? QuantitaScartata
        {
            get { return _quantitaScartata; }
            set 
            {
                if (_quantitaScartata == value || value == null)
                    return;

                _quantitaScartata = value;
                _avanzamentoObserver.QuantitaScartata = (uint)_quantitaScartata;

                OnNotifyStateChanged();
            }
        }
        public uint? QuantitaRecuperata
        {
            get { return _quantitaRecuperata; }
            set 
            {
                _quantitaRecuperata = value;
                _segnalazioneObserver.QuantitaRecuperata = _quantitaRecuperata;

                OnNotifyStateChanged();
            }
        }
        public string Categoria
        {
            get { return _categoria; }
            set 
            {
                _categoria = value;
                _segnalazioneObserver.Categoria = _categoria;

                OnNotifyStateChanged();
            }
        }
        public bool IsErroreFaseAttuale
        {
            get { return _isErroreFaseAttuale; }
            set 
            {
                _isErroreFaseAttuale = value;
                _segnalazioneObserver.IsErroreFaseAttuale = _isErroreFaseAttuale;

                OnNotifyStateChanged();
            }
        }
        public string DescrizioneDifetto
        {
            get { return _descrizioneDifetto; }
            set 
            {
                _descrizioneDifetto = value;
                _segnalazioneObserver.DescrizioneDifetto = _descrizioneDifetto;

                OnNotifyStateChanged();
            }
        }


        public FormSegnalazioneDifformitaViewModel(
            IDialogoOperatoreObserver dialogoOperatoreObserver,
            IAvanzamentoObserver avanzamentoObserver,
            ISegnalazioneObserver segnalazioneObserver)
        {
            _dialogoOperatoreObserver = dialogoOperatoreObserver;
            _avanzamentoObserver = avanzamentoObserver;
            _segnalazioneObserver = segnalazioneObserver;

            QuantitaScartata = _avanzamentoObserver.QuantitaScartata;
            Inizializza();

            _dialogoOperatoreObserver.OnAttivitaSelezionataChanged += DialogoOperatoreObserver_OnAttivitaSelezionataChanged;
            _avanzamentoObserver.OnQuantitaScartataChanged += AvanzamentoObserver_OnQuantitaScartataChanged;
        }

        private void DialogoOperatoreObserver_OnAttivitaSelezionataChanged()
        {
            Inizializza();
        }

        private void Inizializza()
        {
            QuantitaRecuperata = 0;
            Categoria = "Dimensionale";
            IsErroreFaseAttuale = true;
            DescrizioneDifetto = string.Empty;
        }

        private void AvanzamentoObserver_OnQuantitaScartataChanged()
        {
            QuantitaScartata = _avanzamentoObserver.QuantitaScartata;
        }
    }
}
