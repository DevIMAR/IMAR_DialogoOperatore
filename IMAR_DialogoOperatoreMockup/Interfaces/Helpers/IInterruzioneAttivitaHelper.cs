using IMAR_DialogoOperatore.Application.Interfaces.ViewModels;

namespace IMAR_DialogoOperatore.Interfaces.Helpers
{
	public interface IInterruzioneAttivitaHelper
	{
        Task GestisciInterruzioneAttivita(IAttivitaViewModel attivita, bool isUscita);
	}
}