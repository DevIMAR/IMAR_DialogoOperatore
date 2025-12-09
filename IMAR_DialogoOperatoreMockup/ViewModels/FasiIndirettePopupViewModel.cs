using IMAR_DialogoOperatore.Commands;
using IMAR_DialogoOperatore.Interfaces.Observers;
using System.Windows.Input;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class FasiIndirettePopupViewModel : PopupViewModelBase
    {
        public ICommand ConfermaCommand { get; private set; }

        public FasiIndirettePopupViewModel(
            ConfermaCommand confermaCommand,
            IDialogoOperatoreObserver dialogoOperatoreObserver)
            :base(dialogoOperatoreObserver)
        {
            ConfermaCommand = confermaCommand;
        }
    }
}
