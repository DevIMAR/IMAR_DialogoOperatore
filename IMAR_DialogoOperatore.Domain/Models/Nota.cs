namespace IMAR_DialogoOperatore.Domain.Models
{
    public class Nota
    {
        public DateTime DataImmissione { get; set; }
        public string? Operatore { get; set; }
        public required string Odp {  get; set; }
        public required string Fase { get; set; }
        public required string Bolla { get; set; }
        public required string Testo { get; set; }
    }
}
