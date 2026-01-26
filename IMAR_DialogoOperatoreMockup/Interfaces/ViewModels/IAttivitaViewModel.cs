using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Interfaces.ViewModels
{
    public interface IAttivitaViewModel : IViewModelBase
    {
        string? CodiceArticolo { get; }
        string? Bolla { get; }
        string Causale { get; set; }
        string? DescrizioneArticolo { get; }
        string? CodiceDescrizioneArticolo { get; }
        string? DescrizioneFase { get; }
        string? CodiceFase { get; }
        string? CodiceDescrizioneFase { get; }
        string? Odp { get; }
        int QuantitaOrdine { get; }
        int QuantitaProdotta { get; }
        int QuantitaProdottaNonContabilizzata { get; set; }
        int QuantitaProdottaContabilizzata { get; set; }
        int QuantitaResidua { get; }
        int QuantitaScartata { get; }
        int QuantitaScartataNonContabilizzata { get; set; }
        int QuantitaScartataContabilizzata { get; set; }
        string SaldoAcconto { get; set; }
        double? CodiceJMes { get; }
        Macchina? MacchinaFittizia { get; }
        Macchina? MacchinaReale { get; }
        string CausaleEstesa { get; }
        DateTime? DataSchedulata { get; }
        DateTime? InizioAttivita { get; }
        DateTime? FineAttivita { get; }
        IEnumerable<Nota> Note { get; set; }
    }
}