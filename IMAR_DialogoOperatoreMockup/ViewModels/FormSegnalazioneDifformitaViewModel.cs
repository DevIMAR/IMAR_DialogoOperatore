using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class FormSegnalazioneDifformitaViewModel : ViewModelBase
    {
        private readonly IAvanzamentoObserver _avanzamentoObserver;
        private readonly ISegnalazioneObserver _segnalazioneObserver;

        private uint? _quantitaScartata;
        private uint? _quantitaRecuperata;
        private string _categoria;
        private bool _isErroreFaseAttuale;
        private string _descrizioneDifetto;

        public string? Bolla => _segnalazioneObserver.AttivitaPerSegnalazione?.Bolla;
        public string? Odp => _segnalazioneObserver.AttivitaPerSegnalazione?.Odp;
        public string? Fase => _segnalazioneObserver.AttivitaPerSegnalazione?.CodiceFase;
        public string? DescrizioneFase => _segnalazioneObserver.AttivitaPerSegnalazione?.DescrizioneFase;
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
            IAvanzamentoObserver avanzamentoObserver,
            ISegnalazioneObserver segnalazioneObserver)
        {
            _avanzamentoObserver = avanzamentoObserver;
            _segnalazioneObserver = segnalazioneObserver;

            QuantitaScartata = _avanzamentoObserver.QuantitaScartata;
            Inizializza();

            _segnalazioneObserver.OnAttivitaPerSegnalazioneChanged += SegnalazioneObserver_OnAttivitaPerSegnalazioneChanged;
            _avanzamentoObserver.OnQuantitaScartataChanged += AvanzamentoObserver_OnQuantitaScartataChanged;
        }

        private void SegnalazioneObserver_OnAttivitaPerSegnalazioneChanged()
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
