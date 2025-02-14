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

		protected void OnCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, new EventArgs());
		}

        public virtual void Dispose()
        {
        }
	}
}
