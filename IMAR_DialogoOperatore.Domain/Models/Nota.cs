namespace IMAR_DialogoOperatore.Domain.Models
{
    public class Nota
    {
        public decimal DataImmissione { get; set; }
        public string? Operatore { get; set; }
        public string? Odp {  get; set; }
        public string? Fase { get; set; }
        public required decimal Riga { get; set; }
        public required string Testo { get; set; }
        public string? Bolla { get; set; }
    }
}
