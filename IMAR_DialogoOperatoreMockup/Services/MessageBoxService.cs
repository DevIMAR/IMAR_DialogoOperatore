using IMAR_DialogoOperatore.Enums;
using IMAR_DialogoOperatore.Interfaces.Services;

namespace IMAR_DialogoOperatore.Services
{
    public class MessageBoxService : IMessageBoxService
    {
        private TaskCompletionSource<MessageBoxResult>? _modalTcs;
        private Action<MessageBoxResult>? _callback;
        private bool _isVisible;
        private string? _title;
        private string? _message;
        private MessageBoxButtons _currentButtons;

        public event Action? OnStateChanged;

        public bool IsVisible => _isVisible;
        public string? Title => _title;
        public string? Message => _message;
        public MessageBoxButtons CurrentButtons => _currentButtons;

        public void Show(string message, string? title = null,
                         MessageBoxButtons buttons = MessageBoxButtons.Ok,
                         Action<MessageBoxResult>? onResult = null)
        {
            _message = message;
            _title = title;
            _currentButtons = buttons;
            _callback = onResult;
            _modalTcs = null;
            _isVisible = true;
            OnStateChanged?.Invoke();
        }

        public Task<MessageBoxResult> ShowModalAsync(string message, string? title = null,
                                                      MessageBoxButtons buttons = MessageBoxButtons.Ok)
        {
            _message = message;
            _title = title;
            _currentButtons = buttons;
            _callback = null;
            _modalTcs = new TaskCompletionSource<MessageBoxResult>();
            _isVisible = true;
            OnStateChanged?.Invoke();
            return _modalTcs.Task;
        }

        public void Close(MessageBoxResult result)
        {
            _isVisible = false;
            _message = null;
            _title = null;
            OnStateChanged?.Invoke();

            // Completa il task modale se presente
            if (_modalTcs != null)
            {
                _modalTcs.TrySetResult(result);
                _modalTcs = null;
            }

            // Invoca il callback se presente
            _callback?.Invoke(result);
            _callback = null;
        }
    }
}
