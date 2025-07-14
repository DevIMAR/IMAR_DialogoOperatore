using DevExpress.Blazor;

namespace IMAR_DialogoOperatore.Utilities
{
    public class ToastDisplayerUtility
    {
        private readonly IToastNotificationService _toastNotificationService;
        public ToastDisplayerUtility(
            IToastNotificationService toastNotificationService)
        {
            _toastNotificationService = toastNotificationService;
        }

        public void ShowGreenToast(string title, string message) => ShowRenderedToast(title, message, ToastRenderStyle.Success);
        public void ShowYellowToast(string title, string message) => ShowRenderedToast(title, message, ToastRenderStyle.Warning);
        public void ShowRedToast(string title, string message) => ShowRenderedToast(title, message, ToastRenderStyle.Danger);

        public void ShowRenderedToast(string title, string message, ToastRenderStyle toastRenderStyle)
        {
            _toastNotificationService.ShowToast(new ToastOptions
            {
                Title = title,
                Text = message,
                RenderStyle = toastRenderStyle,
                ThemeMode = ToastThemeMode.Saturated,
                ShowIcon = false
            });
        }
    }
}
