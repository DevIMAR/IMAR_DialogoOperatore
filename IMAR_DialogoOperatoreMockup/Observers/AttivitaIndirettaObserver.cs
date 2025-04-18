using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.Observers
{
    public class AttivitaIndirettaObserver : ObserverBase, IAttivitaIndirettaObserver
    {
        private bool _isAttivitaIndiretta;

        public bool IsAttivitaIndiretta 
        { 
            get { return _isAttivitaIndiretta; }
            set
            {
                _isAttivitaIndiretta = value;
                CallAction(OnIsAttivitaIndirettaChanged);
            }
        }

        public event Action OnIsAttivitaIndirettaChanged;
    }
}
