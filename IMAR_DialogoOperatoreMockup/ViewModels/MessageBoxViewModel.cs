using IMAR_DialogoOperatore.Enums;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.Interfaces.Services;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class MessageBoxViewModel : ViewModelBase
    {
        private readonly IMessageBoxService _messageBoxService;
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;

        public MessageBoxViewModel(
            IMessageBoxService messageBoxService,
            IDialogoOperatoreObserver dialogoOperatoreObserver)
        {
            _messageBoxService = messageBoxService;
            _dialogoOperatoreObserver = dialogoOperatoreObserver;

            _messageBoxService.OnStateChanged += MessageBoxService_OnStateChanged;
            _dialogoOperatoreObserver.OnOperatoreSelezionatoChanged += DialogoOperatoreObserver_OnOperatoreSelezionatoChanged;
        }

        public bool IsVisible => _messageBoxService.IsVisible;
        public string? Title => _messageBoxService.Title;
        public string? Message => _messageBoxService.Message;

        public IEnumerable<ButtonInfo> Buttons => GetButtonsForCurrentOptions();

        public void HandleButtonClick(MessageBoxResult result)
        {
            _messageBoxService.Close(result);
        }

        private void MessageBoxService_OnStateChanged()
        {
            OnNotifyStateChanged();
        }

        private void DialogoOperatoreObserver_OnOperatoreSelezionatoChanged()
        {
            if (IsVisible)
            {
                _messageBoxService.Close(MessageBoxResult.Cancel);
            }
        }

        private IEnumerable<ButtonInfo> GetButtonsForCurrentOptions()
        {
            var buttons = _messageBoxService.CurrentButtons;

            switch (buttons)
            {
                case MessageBoxButtons.Ok:
                    yield return new ButtonInfo { Result = MessageBoxResult.Ok, Text = "Ok", IsOutline = false };
                    break;

                case MessageBoxButtons.OkCancel:
                    yield return new ButtonInfo { Result = MessageBoxResult.Cancel, Text = "Annulla", IsOutline = true };
                    yield return new ButtonInfo { Result = MessageBoxResult.Ok, Text = "Ok", IsOutline = false };
                    break;

                case MessageBoxButtons.YesNo:
                    yield return new ButtonInfo { Result = MessageBoxResult.No, Text = "No", IsOutline = true };
                    yield return new ButtonInfo { Result = MessageBoxResult.Yes, Text = "Sì", IsOutline = false };
                    break;

                case MessageBoxButtons.YesNoCancel:
                    yield return new ButtonInfo { Result = MessageBoxResult.Cancel, Text = "Annulla", IsOutline = true };
                    yield return new ButtonInfo { Result = MessageBoxResult.No, Text = "No", IsOutline = false };
                    yield return new ButtonInfo { Result = MessageBoxResult.Yes, Text = "Sì", IsOutline = false };
                    break;

                case MessageBoxButtons.ConfirmCancel:
                    yield return new ButtonInfo { Result = MessageBoxResult.Cancel, Text = "Annulla", IsOutline = true };
                    yield return new ButtonInfo { Result = MessageBoxResult.Confirm, Text = "Conferma", IsOutline = false };
                    break;
            }
        }

        public override void Dispose()
        {
            _messageBoxService.OnStateChanged -= MessageBoxService_OnStateChanged;
            _dialogoOperatoreObserver.OnOperatoreSelezionatoChanged -= DialogoOperatoreObserver_OnOperatoreSelezionatoChanged;
            base.Dispose();
        }
    }
}
