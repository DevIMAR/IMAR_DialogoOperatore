namespace IMAR_DialogoOperatore.Application.Interfaces.Services.External
{
    public interface IMorpheusApiService
    {
        string? GetDocumentaleDaArticolo(string articolo);
        string? GetDocumentaleDaOdp(string odp);
    }
}