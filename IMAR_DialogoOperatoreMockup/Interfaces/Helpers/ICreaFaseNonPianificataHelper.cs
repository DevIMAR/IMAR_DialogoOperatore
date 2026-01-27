using IMAR_DialogoOperatore.Interfaces.ViewModels;

namespace IMAR_DialogoOperatore.Interfaces.Helpers
{
    public interface ICreaFaseNonPianificataHelper
    {
		Task<string?> ApriFaseNonPianificata(IAttivitaViewModel attivita);
    }
}
