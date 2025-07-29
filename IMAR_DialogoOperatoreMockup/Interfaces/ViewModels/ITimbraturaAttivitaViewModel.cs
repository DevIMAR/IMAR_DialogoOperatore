namespace IMAR_DialogoOperatore.Interfaces.ViewModels
{
    public interface ITimbraturaAttivitaViewModel
    {
        string Causale { get; set; }
        string CausaleEstesa { get; set; }
        string? Bolla { get; set; }
        string? Odp { get; set; }
        string? Fase { get; set; }
        int? QuantitaProdotta { get; set; }
        int? QuantitaScartata { get; set; }
        DateTime Timestamp { get; set; }
    }
}
