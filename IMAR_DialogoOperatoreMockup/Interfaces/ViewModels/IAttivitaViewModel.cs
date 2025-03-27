using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Interfaces.ViewModels
{
    public interface IAttivitaViewModel
    {
        string? Articolo { get; }
        string? Bolla { get; }
        string Causale { get; set; }
        string? DescrizioneArticolo { get; }
        string? DescrizioneFase { get; }
        string? Fase { get; }
        string? Odp { get; }
        int QuantitaOrdine { get; }
        int QuantitaProdotta { get; set; }
        int QuantitaResidua { get; }
        int QuantitaScartata { get; set; }
        string SaldoAcconto { get; set; }
        double? CodiceJMes { get; }
        Macchina? Macchina { get; }
    }
}