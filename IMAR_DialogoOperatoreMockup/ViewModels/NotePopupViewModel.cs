using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class NotePopupViewModel : PopupViewModelBase
    {
        public NotePopupViewModel(
            IDialogoOperatoreObserver dialogoOperatoreObserver)
            :base(dialogoOperatoreObserver) 
        {
            
        }
    }
}
