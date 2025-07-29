namespace IMAR_DialogoOperatore.Observers
{
	public class ObserverBase
	{
		public void CallAction(Action? action)
		{
			action?.Invoke();
        }

        public Task InvokeAsync(Action action)
        {
            return Task.Run(() =>
            {
                var syncContext = SynchronizationContext.Current;
                if (syncContext != null)
                {
                    syncContext.Post(_ => action(), null);
                }
                else
                {
                    CallAction(action);
                }
            });
        }
    }
}
