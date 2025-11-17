using IMAR_DialogoOperatore.Application.DTOs;

namespace IMAR_DialogoOperatore.Application.Interfaces.Utilities
{
    public interface IJMesApiClientErrorUtility
    {
        string? GestioneEventualeErrore(HttpResponseMessage result);
        string? GestioneEventualeErrore(JMesResultDto? jsonData);
    }
}
