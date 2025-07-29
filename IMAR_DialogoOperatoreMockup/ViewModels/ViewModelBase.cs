using IMAR_DialogoOperatore.Interfaces.ViewModels;

namespace IMAR_DialogoOperatore.ViewModels
{
	public class ViewModelBase : IDisposable, IViewModelBase
    {
        public void OnNotifyStateChanged()
		{
			NotifyStateChanged?.Invoke();
		}

		public virtual void Dispose()
		{
		}

        public event Action NotifyStateChanged;
	}
}
