using IMAR_DialogoOperatore.Interfaces.ViewModels;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class TimbraturaAttivitaViewModel : ITimbraturaAttivitaViewModel
    {
        public double? CodiceJMes {  get; set; }
        public string Causale { get; set; }
        public string CausaleEstesa { get; set; }
        public string? Bolla { get; set; }
        public string? Odp { get; set; }
        public string? Fase { get; set; }
        public int? QuantitaProdotta { get; set; }
        public int? QuantitaScartata { get; set; }
        public DateTime? Timestamp { get; set; }
		public string SaldoAcconto { get; set; }
	}
}
