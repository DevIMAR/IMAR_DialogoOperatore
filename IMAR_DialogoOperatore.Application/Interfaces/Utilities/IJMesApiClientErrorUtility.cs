namespace IMAR_DialogoOperatore.Application.Interfaces.Utilities
{
    public interface IJMesApiClientErrorUtility
    {
        string? GestioneEventualeErrore(HttpResponseMessage result);
    }
}
