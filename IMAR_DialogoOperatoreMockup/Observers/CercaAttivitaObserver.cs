using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Interfaces.ViewModels;

namespace IMAR_DialogoOperatore.Observers
{
    public class CercaAttivitaObserver : ObserverBase, ICercaAttivitaObserver
	{
		private IEnumerable<IAttivitaViewModel> _attivitaTrovate;
		private bool _isBottoneCercaPremuto;
		private string _faseCercata;

		public IEnumerable<IAttivitaViewModel> AttivitaTrovate
		{
			get { return _attivitaTrovate; }
			set
			{
				_attivitaTrovate = value;
				CallAction(OnAttivitaTrovateChanged);
			}
		}
		public bool IsAttivitaCercata
		{
			get { return _isBottoneCercaPremuto; }
			set
			{
				_isBottoneCercaPremuto = value;
				CallAction(OnIsBottoneCercaPremutoChanged);
			}
		}
		public string FaseCercata
		{
			get { return _faseCercata; }
			set
			{
				_faseCercata = value;
				CallAction(OnFaseCercataChanged);
			}
		}

		public event Action OnAttivitaTrovateChanged;
		public event Action OnIsBottoneCercaPremutoChanged;
		public event Action OnFaseCercataChanged;
	}
}
