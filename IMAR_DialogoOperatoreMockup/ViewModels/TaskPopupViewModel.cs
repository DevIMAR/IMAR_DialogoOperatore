using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Utilities;
using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore.Interfaces.Observers;
using System.Windows.Input;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class TaskPopupViewModel : ViewModelBase
    {
        private readonly ITaskCompilerObserver _taskCompilerObserver;
        private readonly IAutoLogoutUtility _autoLogoutUtility;

        private bool _visible;
        private string _categoriaErroreSelezionata;

        public List<string> CategorieErrori => new List<string>() { Costanti.TASK_QUANTITA_ERRATA, Costanti.TASK_CHIUSURA_A_SALDO_ERRATA, Costanti.TASK_TIMBRATURA_ERRATA, Costanti.TASK_ALTRO };

        public ICommand InviaTaskCommand { get; }

        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                CategoriaErroreSelezionata = null;

                _taskCompilerObserver.IsPopupVisible = _visible;

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
            IAutoLogoutUtility autoLogoutUtility)
        {
            _taskCompilerObserver = taskCompilerObserver;

            InviaTaskCommand = inviaTaskCommand;
            _autoLogoutUtility = autoLogoutUtility;
        }
    }
}
