namespace IMAR_DialogoOperatore.Interfaces.ViewModels
{
    public interface IViewModelBase
    {
        void OnNotifyStateChanged();

        event Action NotifyStateChanged;
    }
}
