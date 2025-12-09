using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore.Interfaces.Observers;
using System.Windows.Input;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class TaskPopupViewModel : PopupViewModelBase
    {
        private readonly ITaskCompilerObserver _taskCompilerObserver;

        private bool _isVisible;
        private string _categoriaErroreSelezionata;

        public List<string> CategorieErrori => new List<string>() { Costanti.TASK_QUANTITA_ERRATA, Costanti.TASK_CHIUSURA_A_SALDO_ERRATA, Costanti.TASK_TIMBRATURA_ERRATA, Costanti.TASK_ALTRO };

        public ICommand InviaTaskCommand { get; }

        override public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                CategoriaErroreSelezionata = null;

                _taskCompilerObserver.IsPopupVisible = _isVisible;

                OnNotifyStateChanged();
            }
        }
        public string CategoriaErroreSelezionata
        {
            get { return _categoriaErroreSelezionata; }
            set
            {
                _categoriaErroreSelezionata = value;
                _taskCompilerObserver.CategoriaErroreSelezionata = _categoriaErroreSelezionata;

                OnNotifyStateChanged();
            }
        }

        public TaskPopupViewModel(
            InviaTaskCommand inviaTaskCommand,
            ITaskCompilerObserver taskCompilerObserver,
            IDialogoOperatoreObserver dialogoOperatoreObserver)
            :base(dialogoOperatoreObserver)
        {
            _taskCompilerObserver = taskCompilerObserver;

            InviaTaskCommand = inviaTaskCommand;
        }
    }
}
