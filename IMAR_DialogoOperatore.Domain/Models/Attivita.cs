namespace IMAR_DialogoOperatore.Domain.Models
{
	public class Attivita
	{
		public string Causale { get; set; }
		public string CausaleEstesa { get; set; }
		public string Bolla { get; set; }
		public string Odp { get; set; }
		public string Articolo { get; set; }
		public string DescrizioneArticolo { get; set; }
		public string Fase { get; set; }
		public string DescrizioneFase { get; set; }
		public int QuantitaOrdine { get; set; }
		public int QuantitaProdotta { get; set; }
		public int QuantitaProdottaNonContabilizzata { get; set; }
		public int QuantitaProdottaContabilizzata { get; set; }
		public int QuantitaScartata { get; set; }
		public int QuantitaScartataNonContabilizzata { get; set; }
		public int QuantitaScartataContabilizzata { get; set; }
		public int QuantitaResidua { get; set; }
		public string SaldoAcconto { get; set; }
		public double? CodiceJMes {  get; set; }
		public Macchina Macchina { get; set; }
		public DateTime DataSchedulata { get; set; }
		public DateTime InizioAttivita { get; set; }
		public DateTime? FineAttivita { get; set; }
	}
}
