using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
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
        private readonly ICercaAttivitaHelper _cercaAttivitaHelper;
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

        public string TestoDataLontana => Costanti.TESTO_DATA_LONTANA;

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
                GetNoteAttivita();

                OnNotifyStateChanged();
            }
        }

        public InfoBaseAttivitaViewModel(
            IDialogoOperatoreObserver dialogoOperatoreObserver,
            ICercaAttivitaObserver cercaAttivitaObserver,
            ICercaAttivitaHelper cercaAttivitaHelper,
            MostraIndiretteCommand mostraIndiretteCommand,
            INotaService notaService)
        {
            _dialogoOperatoreObserver = dialogoOperatoreObserver;
            _cercaAttivitaObserver = cercaAttivitaObserver;

            _cercaAttivitaHelper = cercaAttivitaHelper;
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

            OnNotifyStateChanged();
        }

        public void CercaAttivitaDaBolla(string bolla)
        {
            Odp = string.Empty;

            _cercaAttivitaHelper.CercaAttivitaDaBolla(bolla);
            GetNoteAttivita();
        }

        public void CercaAttivitaDaOdp(string odp)
        {
            Bolla = string.Empty;

            _cercaAttivitaHelper.CercaAttivitaDaOdp(odp.Trim());
            GetNoteAttivita();
        }

        public void GetNoteAttivita()
        {
            IAttivitaViewModel? attivita = _dialogoOperatoreObserver.AttivitaSelezionata;

            if (attivita == null)
                return;

            if (attivita.Bolla.Contains("AI"))
                _dialogoOperatoreObserver.AttivitaSelezionata.Note = _notaService.GetNoteDaBolla(attivita.Bolla);
            else
                _dialogoOperatoreObserver.AttivitaSelezionata.Note = _notaService.GetNoteDaOdpFase(attivita.Odp, attivita.CodiceFase);

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
