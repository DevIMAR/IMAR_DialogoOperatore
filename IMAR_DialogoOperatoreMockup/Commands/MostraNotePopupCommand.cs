using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Commands
{
    public class MostraNotePopupCommand : CommandBase
    {
        private readonly NotePopupViewModel _notePopupViewModel;

        public MostraNotePopupCommand(
            NotePopupViewModel notePopupViewModel)
        {
            _notePopupViewModel = notePopupViewModel;
        }

        public override bool CanExecute(object? parameter)
        {
            return base.CanExecute(parameter);
        }

        public override void Execute(object? parameter)
        {
            _notePopupViewModel.IsVisible = true;
        }
    }
}
