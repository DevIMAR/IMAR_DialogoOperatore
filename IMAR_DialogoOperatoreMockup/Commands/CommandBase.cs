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
		protected static async Task SafeExecuteAsync(Func<Task> operation, ILoggingService loggingService, string contesto, string? badge = null)
		{
			try
			{
				await operation();
			}
			catch (Exception ex)
			{
				string contestoConBadge = badge != null ? $"{contesto} [badge={badge}]" : contesto;
				loggingService.LogError($"Errore non gestito in {contestoConBadge}", ex);
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
