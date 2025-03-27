using IMAR_DialogoOperatore.Interfaces.ViewModels;

namespace IMAR_DialogoOperatore.Interfaces.Observers
{
    public interface ICercaAttivitaObserver
	{
		IEnumerable<IAttivitaViewModel> AttivitaTrovate { get; set; }
		string FaseCercata { get; set; }
		bool IsAttivitaCercata { get; set; }

		event Action OnAttivitaTrovateChanged;
		event Action OnFaseCercataChanged;
		event Action OnIsBottoneCercaPremutoChanged;
	}
}