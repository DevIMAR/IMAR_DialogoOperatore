namespace IMAR_DialogoOperatore.Interfaces.ViewModels
{
    public interface INotaViewModel
    {
        string? Operatore { get; }
        DateTime DataImmissione { get; }
        string? Fase { get; }
        string? Odp { get; }
        decimal Riga { get; }
        string? Bolla { get; }
        string? Testo { get; set; }
    }
}