using IMAR_DialogoOperatore.Application.DTOs;

namespace IMAR_DialogoOperatore.Application.Interfaces.Utilities
{
    public interface IJMesApiClientErrorUtility
    {
        Task<(string? errore, JMesResultDto? dati)> GestioneEventualeErroreAsync(HttpResponseMessage result);
        string? GestioneEventualeErrore(JMesResultDto? jsonData);
    }
}
