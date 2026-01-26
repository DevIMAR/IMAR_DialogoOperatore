using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class PopupTimbratureViewModel : PopupViewModelBase
    {
        public PopupTimbratureViewModel(
            IDialogoOperatoreObserver dialogoOperatoreObserver)
            :base(dialogoOperatoreObserver)
        {
        }
    }
}
