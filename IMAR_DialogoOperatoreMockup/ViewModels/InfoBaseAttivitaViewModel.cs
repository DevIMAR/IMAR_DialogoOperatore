using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Interfaces.ViewModels;
using System.Windows.Input;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class InfoBaseAttivitaViewModel : ViewModelBase
    {
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
        private readonly ICercaAttivitaObserver _cercaAttivitaObserver;

        private IAttivitaViewModel? _attivitaSelezionata;
        private string? _bolla;
        private string? _odp;
        private IEnumerable<string>? _fasiPerAttivita;
        private string _faseSelezionata;

        public ICercaAttivitaHelper CercaAttivitaHelper { get; private set; }
        public ICommand ApriListaIndirette { get; private set; }

        public string? Articolo => _attivitaSelezionata != null ? _attivitaSelezionata.Articolo : string.Empty;
        public string? DescrizioneArticolo => _attivitaSelezionata != null ? _attivitaSelezionata.DescrizioneArticolo : string.Empty;
        public string? DescrizioneFase => _attivitaSelezionata != null ? _attivitaSelezionata.DescrizioneFase : string.Empty;
        public bool IsAttivitaSelezionata => _dialogoOperatoreObserver.AttivitaSelezionata?.Bolla == Bolla &&
                                             _dialogoOperatoreObserver.AttivitaSelezionata?.Odp == Odp;

        public string? Bolla
        {
            get { return _bolla; }
            set
            {
                if (_bolla == value)
                    return;

                _bolla = value;

                OnNotifyStateChanged();
            }
        }
        public string? Odp
        {
            get { return _odp; }
            set
            {
                if (_odp == value)
                    return;

                _odp = value;

                OnNotifyStateChanged();
            }
        }
        public IEnumerable<string>? FasiPerAttivita
        {
            get { return _fasiPerAttivita; }
            private set
            {
                _fasiPerAttivita = value;
                OnNotifyStateChanged();
            }
        }
        public string FaseSelezionata
        {
            get { return _faseSelezionata; }
            set
            {
                _faseSelezionata = value;

                CercaAttivitaHelper.CercaAttivitaDaFase(value);

                OnNotifyStateChanged();
            }
        }

        public InfoBaseAttivitaViewModel(
            IDialogoOperatoreObserver dialogoOperatoreObserver,
            ICercaAttivitaObserver cercaAttivitaObserver,
            ICercaAttivitaHelper cercaAttivitaHelper,
            MostraIndiretteCommand mostraIndiretteCommand)
        {
            _dialogoOperatoreObserver = dialogoOperatoreObserver;
            _cercaAttivitaObserver = cercaAttivitaObserver;

            CercaAttivitaHelper = cercaAttivitaHelper;
            ApriListaIndirette = mostraIndiretteCommand;

            _dialogoOperatoreObserver.OnIsDettaglioAttivitaOpenChanged += DialogoOperatoreObserver_OnIsDettaglioAttivitaOpenChanged;
            _dialogoOperatoreObserver.OnAttivitaSelezionataChanged += DialogoOperatoreStore_OnAttivitaSelezionataChanged;
            _cercaAttivitaObserver.OnAttivitaTrovateChanged += CercaAttivitaStore_OnAttivitaTrovateChanged;
        }

        private void DialogoOperatoreObserver_OnIsDettaglioAttivitaOpenChanged()
        {
            Bolla = null;
            Odp = null;
            _cercaAttivitaObserver.FaseCercata = string.Empty;
        }

        private void CercaAttivitaStore_OnAttivitaTrovateChanged()
        {
            IEnumerable<IAttivitaViewModel>? attivitaTrovate = _cercaAttivitaObserver.AttivitaTrovate;

            if (!attivitaTrovate.Any())
                return;

            FasiPerAttivita = _cercaAttivitaObserver.AttivitaTrovate.Select(x => x.Fase);
        }

        private void DialogoOperatoreStore_OnAttivitaSelezionataChanged()
        {
            _attivitaSelezionata = _dialogoOperatoreObserver.AttivitaSelezionata;

            if (_attivitaSelezionata == null)
            {
                _fasiPerAttivita = new List<string>();
                _faseSelezionata = null;

                return;
            }

            if (!_cercaAttivitaObserver.IsAttivitaCercata)
                _fasiPerAttivita = new List<string> { _attivitaSelezionata.Fase };

            _faseSelezionata = _attivitaSelezionata.Fase;
            _odp = _attivitaSelezionata.Odp ?? string.Empty;
            _bolla = _attivitaSelezionata.Bolla ?? string.Empty;

            OnNotifyStateChanged();
        }

        public override void Dispose()
        {
            _dialogoOperatoreObserver.OnIsDettaglioAttivitaOpenChanged -= DialogoOperatoreObserver_OnIsDettaglioAttivitaOpenChanged;
            _dialogoOperatoreObserver.OnAttivitaSelezionataChanged -= DialogoOperatoreStore_OnAttivitaSelezionataChanged;
            _cercaAttivitaObserver.OnAttivitaTrovateChanged -= CercaAttivitaStore_OnAttivitaTrovateChanged;
        }
    }
}
