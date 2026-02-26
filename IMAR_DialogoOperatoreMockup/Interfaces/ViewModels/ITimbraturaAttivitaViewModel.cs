namespace IMAR_DialogoOperatore.Interfaces.ViewModels
{
    public interface ITimbraturaAttivitaViewModel : IDatiAttivitaBase
    {
        double? CodiceJMes { get; set; }
        string Causale { get; set; }
        string CausaleEstesa { get; set; }
        new string? Bolla { get; set; }
        new string? Odp { get; set; }
        new string? CodiceFase { get; set; }
        new string? DescrizioneFase { get; set; }
        new string? CodiceArticolo { get; set; }
        new string? DescrizioneArticolo { get; set; }
        int? QuantitaProdotta { get; set; }
        int? QuantitaScartata { get; set; }
        DateTime? Timestamp { get; set; }
        string SaldoAcconto { get; set; }
    }
}
