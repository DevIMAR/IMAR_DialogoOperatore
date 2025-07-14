namespace IMAR_DialogoOperatore.ViewModels
{
	public class ViewModelBase : IDisposable
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
