namespace IMAR_DialogoOperatore.Interfaces.Services
{
    /// <summary>
    /// Parametri deep link per apertura diretta con badge, ODP e fase.
    /// Uso: /?badge=1234&odp=12345678&fase=10
    /// </summary>
    public interface IDeepLinkParameters
    {
        int? Badge { get; set; }
        string? Odp { get; set; }
        string? Fase { get; set; }
        bool HasDeepLink { get; }
        void Consume();
    }
}
