using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Application.Interfaces.Services.Activities
{
    public interface IFaseNonPianificataService
    {
        Task<string?> ApriAttrezzaggioFaseNonPianificataAsync(Attivita attivita, Operatore operatore);
        Task<string?> ApriLavoroFaseNonPianificataAsync(Attivita attivita, Operatore operatore);
    }
}
