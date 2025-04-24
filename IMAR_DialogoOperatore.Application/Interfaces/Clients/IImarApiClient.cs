using IMAR_DialogoOperatore.Application.DTOs;

namespace IMAR_DialogoOperatore.Application.Interfaces.Clients
{
    public interface IImarApiClient
    {
        Task<CostiArticoloDTO> GetCostiArticolo(string codiceArticolo);
    }
}
