using IMAR_DialogoOperatore.Enums;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class ButtonInfo
    {
        public MessageBoxResult Result { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsOutline { get; set; }
    }
}
