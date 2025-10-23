using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Interfaces.ViewModels;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class GestoreFasiNonPianificateViewModel : ViewModelBase
    {
		private readonly ICercaAttivitaObserver _cercaAttivitaObserver;
		private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;

		private IAttivitaViewModel? _faseDiPartenza;
		private IAttivitaViewModel? _faseDiArrivo;
		private int? _quantitaRilavorazione;

		public FasiAttivitaGridViewModel FasiDiPartenzaGridViewModel { get; }
		public FasiAttivitaGridViewModel FasiDiArrivoGridViewModel { get; }
		public int? MaxQuantita => _cercaAttivitaObserver.AttivitaTrovate.FirstOrDefault()?.QuantitaProdotta + 
									_cercaAttivitaObserver.AttivitaTrovate.FirstOrDefault()?.QuantitaScartata;


        public IAttivitaViewModel? FaseDiPartenza
        {
			get { return _faseDiPartenza; }
			set 
			{ 
				_faseDiPartenza = value;
                OnNotifyStateChanged();
            }
		}
		public IAttivitaViewModel? FaseDiArrivo
        {
			get { return _faseDiArrivo; }
			set 
			{
                _faseDiArrivo = value;
                OnNotifyStateChanged();
            }
		}
		public int? QuantitaRilavorazione
        {
			get { return _quantitaRilavorazione; }
			set 
			{
				_quantitaRilavorazione = value; 
				OnNotifyStateChanged();
			}
		}

        public GestoreFasiNonPianificateViewModel(
            ICercaAttivitaObserver cercaAttivitaObserver,
			IDialogoOperatoreObserver dialogoOperatoreObserver)
        {
            _cercaAttivitaObserver = cercaAttivitaObserver;
			_dialogoOperatoreObserver = dialogoOperatoreObserver;

			FasiDiPartenzaGridViewModel = new FasiAttivitaGridViewModel(cercaAttivitaObserver);
			FasiDiArrivoGridViewModel = new FasiAttivitaGridViewModel(cercaAttivitaObserver);

			FaseDiPartenza = null;
			FaseDiArrivo = null;

            _cercaAttivitaObserver.OnAttivitaTrovateChanged += CercaAttivitaObserver_OnAttivitaTrovateChanged;
        }

		public void Initialize()
        {
            FaseDiPartenza = _cercaAttivitaObserver.AttivitaTrovate.FirstOrDefault();
            FaseDiArrivo = _cercaAttivitaObserver.AttivitaTrovate.SingleOrDefault(x => x.Bolla == _dialogoOperatoreObserver.AttivitaSelezionata?.Bolla);
            QuantitaRilavorazione = null;
        }

        private void CercaAttivitaObserver_OnAttivitaTrovateChanged()
        {
			FaseDiPartenza = _cercaAttivitaObserver.AttivitaTrovate.FirstOrDefault();
			FaseDiArrivo = _dialogoOperatoreObserver.AttivitaSelezionata;
        }
    }
}
