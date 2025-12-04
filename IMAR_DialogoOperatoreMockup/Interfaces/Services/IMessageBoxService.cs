using IMAR_DialogoOperatore.Enums;

namespace IMAR_DialogoOperatore.Interfaces.Services
{
    public interface IMessageBoxService
    {
        /// <summary>
        /// Mostra un message box non bloccante. La risposta viene gestita tramite callback.
        /// </summary>
        void Show(string message, string? title = null,
                  MessageBoxButtons buttons = MessageBoxButtons.Ok,
                  Action<MessageBoxResult>? onResult = null);

        /// <summary>
        /// Mostra un message box modale (bloccante) e attende la risposta dell'utente.
        /// </summary>
        Task<MessageBoxResult> ShowModalAsync(string message, string? title = null,
                                               MessageBoxButtons buttons = MessageBoxButtons.Ok);

        /// <summary>
        /// Chiude il message box attualmente visualizzato con il risultato specificato.
        /// </summary>
        void Close(MessageBoxResult result);

        /// <summary>
        /// Evento scatenato quando lo stato del message box cambia (per binding UI).
        /// </summary>
        event Action? OnStateChanged;

        bool IsVisible { get; }
        string? Title { get; }
        string? Message { get; }
        MessageBoxButtons CurrentButtons { get; }
    }
}
