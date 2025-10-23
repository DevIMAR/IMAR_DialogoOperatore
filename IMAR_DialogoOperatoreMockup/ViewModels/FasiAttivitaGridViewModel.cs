using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Interfaces.ViewModels;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class FasiAttivitaGridViewModel : ViewModelBase
    {
        private readonly ICercaAttivitaObserver _cercaAttivitaObserver;

        private IEnumerable<IAttivitaViewModel>? _attivitaConStessoOdp;
        private object? _faseSelezionata;

		public IEnumerable<IAttivitaViewModel>? AttivitaConStessoOdp
        {
			get { return _attivitaConStessoOdp; }
			set 
			{ 
				_attivitaConStessoOdp = value; 
				OnNotifyStateChanged();
			}
		}
        public object? FaseSelezionata
        {
            get { return _faseSelezionata; }
            set 
            { 
                _faseSelezionata = value;
                OnNotifyStateChanged();
            }
        }


        public FasiAttivitaGridViewModel(
            ICercaAttivitaObserver cercaAttivitaObserver)
        {
            _cercaAttivitaObserver = cercaAttivitaObserver;

            AttivitaConStessoOdp = _cercaAttivitaObserver.AttivitaTrovate;

            _cercaAttivitaObserver.OnAttivitaTrovateChanged += CercaAttivitaObserver_OnAttivitaTrovateChanged;
        }

        private void CercaAttivitaObserver_OnAttivitaTrovateChanged()
        {
            AttivitaConStessoOdp = _cercaAttivitaObserver.AttivitaTrovate;
        }
    }
}
