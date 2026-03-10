using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore.Interfaces.Observers;
using System.Windows.Input;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class TaskPopupViewModel : PopupViewModelBase
    {
        private readonly ITaskCompilerObserver _taskCompilerObserver;

        private bool _isVisible;
        private bool _isRettificaQuantita;
        private bool _isTogliSaldo;
        private bool _isCorreggiOrarioInizio;
        private bool _isCorreggiOrarioFine;

        public ICommand InviaTaskCommand { get; }

        override public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                if (_isVisible)
                    ResetCorrezioni();

                _taskCompilerObserver.IsPopupVisible = _isVisible;

                OnNotifyStateChanged();
            }
        }

        public bool IsRettificaQuantita
        {
            get { return _isRettificaQuantita; }
            set
            {
                _isRettificaQuantita = value;
                _taskCompilerObserver.IsRettificaQuantita = value;
                OnNotifyStateChanged();
            }
        }

        public bool IsTogliSaldo
        {
            get { return _isTogliSaldo; }
            set
            {
                _isTogliSaldo = value;
                _taskCompilerObserver.IsTogliSaldo = value;
                OnNotifyStateChanged();
            }
        }

        public bool IsCorreggiOrarioInizio
        {
            get { return _isCorreggiOrarioInizio; }
            set
            {
                _isCorreggiOrarioInizio = value;
                _taskCompilerObserver.IsCorreggiOrarioInizio = value;
                OnNotifyStateChanged();
            }
        }

        public bool IsCorreggiOrarioFine
        {
            get { return _isCorreggiOrarioFine; }
            set
            {
                _isCorreggiOrarioFine = value;
                _taskCompilerObserver.IsCorreggiOrarioFine = value;
                OnNotifyStateChanged();
            }
        }

        public bool HasAlmenoUnaCorrezione => _isRettificaQuantita || _isTogliSaldo || _isCorreggiOrarioInizio || _isCorreggiOrarioFine;

        public TaskPopupViewModel(
            InviaTaskCommand inviaTaskCommand,
            ITaskCompilerObserver taskCompilerObserver,
            IDialogoOperatoreObserver dialogoOperatoreObserver)
            :base(dialogoOperatoreObserver)
        {
            _taskCompilerObserver = taskCompilerObserver;

            InviaTaskCommand = inviaTaskCommand;
        }

        private void ResetCorrezioni()
        {
            _isRettificaQuantita = false;
            _isTogliSaldo = false;
            _isCorreggiOrarioInizio = false;
            _isCorreggiOrarioFine = false;
            _taskCompilerObserver.IsRettificaQuantita = false;
            _taskCompilerObserver.IsTogliSaldo = false;
            _taskCompilerObserver.IsCorreggiOrarioInizio = false;
            _taskCompilerObserver.IsCorreggiOrarioFine = false;
        }
    }
}
