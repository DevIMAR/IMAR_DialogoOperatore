using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using System.Windows.Input;

namespace IMAR_DialogoOperatore.Commands
{
	public abstract class CommandBase : ICommand, IDisposable
	{
		public event EventHandler? CanExecuteChanged;

		public virtual bool CanExecute(object? parameter)
		{
			return true;
		}

        public abstract void Execute(object? parameter);

		/// <summary>
		/// Wrappa un'operazione async con try/catch + logging.
		/// Da usare nei metodi async void (Execute, event handler) per evitare crash silenziosi del circuito Blazor.
		/// </summary>
		protected static async Task SafeExecuteAsync(Func<Task> operation, ILoggingService loggingService, string contesto)
		{
			try
			{
				await operation();
			}
			catch (Exception ex)
			{
				loggingService.LogError($"Errore non gestito in {contesto}", ex);
			}
		}

		protected void OnCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, new EventArgs());
		}

        public virtual void Dispose()
        {
        }
	}
}
