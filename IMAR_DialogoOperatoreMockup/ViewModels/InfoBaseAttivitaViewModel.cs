using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Interfaces.Mappers;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Interfaces.ViewModels;
using System.Windows.Input;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class InfoBaseAttivitaViewModel : ViewModelBase
    {
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
        private readonly ICercaAttivitaObserver _cercaAttivitaObserver;
        private readonly ICercaAttivitaHelper _cercaAttivitaHelper;
        private readonly IAttivitaMapper _attivitaMapper;
        private readonly INotaService _notaService;

        private IAttivitaViewModel? _attivitaSelezionata;
        private string? _bolla;
        private string? _odp;
        private IEnumerable<string>? _fasiPerAttivita;
        private string _faseSelezionata;

        public ICommand ApriListaIndirette { get; private set; }

        public string? CodiceDescrizioneArticolo => _attivitaSelezionata != null ? _attivitaSelezionata.CodiceDescrizioneArticolo : string.Empty;
        public string? DataSchedulata => _attivitaSelezionata != null ? _attivitaSelezionata.DataSchedulata?.ToString("dd/MM/yyyy") : string.Empty;
        public bool IsAttivitaSelezionata => _dialogoOperatoreObserver.AttivitaSelezionata?.Bolla == Bolla &&
                                             _dialogoOperatoreObserver.AttivitaSelezionata?.Odp == Odp;
        public bool IsDataSchedulataLontana => _attivitaSelezionata?.DataSchedulata > DateTime.Today.AddDays(Costanti.LIMITE_GIORNO_VICINO);
        public bool IsAttivitaNonSchedulata => _attivitaSelezionata != null && (DataSchedulata == new DateTime(1, 1, 1).ToString("dd/MM/yyyy") || DataSchedulata == null);

        public string TestoDataLontana => Costanti.TESTO_DATA_LONTANA;
        public string TestoAttivitaNonSchedulata => Costanti.TESTO_ATTIVITA_NON_SCHEDULATA;

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

                _cercaAttivitaHelper.CercaAttivitaDaFase(value.Substring(0, 3));

                OnNotifyStateChanged();
            }
        }

        public InfoBaseAttivitaViewModel(
            IDialogoOperatoreObserver dialogoOperatoreObserver,
            ICercaAttivitaObserver cercaAttivitaObserver,
            ICercaAttivitaHelper cercaAttivitaHelper,
            IAttivitaMapper attivitaMapper,
            MostraIndiretteCommand mostraIndiretteCommand,
            INotaService notaService)
        {
            _dialogoOperatoreObserver = dialogoOperatoreObserver;
            _cercaAttivitaObserver = cercaAttivitaObserver;

            _cercaAttivitaHelper = cercaAttivitaHelper;
            _attivitaMapper = attivitaMapper;
            ApriListaIndirette = mostraIndiretteCommand;

            _notaService = notaService;

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

            FasiPerAttivita = _cercaAttivitaObserver.AttivitaTrovate.Select(x => x.CodiceDescrizioneFase);
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
                _fasiPerAttivita = new List<string> { _attivitaSelezionata.CodiceDescrizioneFase };

            _faseSelezionata = _attivitaSelezionata.CodiceDescrizioneFase;
            _odp = _attivitaSelezionata.Odp ?? string.Empty;
            _bolla = _attivitaSelezionata.Bolla ?? string.Empty;
            GetNoteAttivita();

            OnNotifyStateChanged();
        }

        public void CercaAttivitaDaBolla(string bolla)
        {
            Odp = string.Empty;

            _cercaAttivitaHelper.CercaAttivitaDaBolla(bolla);
        }

        public void CercaAttivitaDaOdp(string odp)
        {
            Bolla = string.Empty;

            _cercaAttivitaHelper.CercaAttivitaDaOdp(odp.Trim());
        }

        public void GetNoteAttivita()
        {
            _dialogoOperatoreObserver.AttivitaSelezionata.Note = _notaService.GetNoteAttivita(_attivitaMapper.AttivitaViewModelToAttivita(_attivitaSelezionata));

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
