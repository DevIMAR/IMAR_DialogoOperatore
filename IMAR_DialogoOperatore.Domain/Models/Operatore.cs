namespace IMAR_DialogoOperatore.Domain.Models
{
    public class Operatore
	{
		public string Badge { get; set; }
		public string Nome { get; set; }
		public string Cognome { get; set; }
		public string Stato { get; set; }
		public DateTime Ingresso { get; set; }
		public DateTime Uscita { get; set; }
		public DateTime InizioPausa { get; set; }
		public DateTime FinePausa { get; set; }
		public IList<Attivita> AttivitaAperte { get; set; }
		public Macchina? MacchinaAssegnata { get; set; }
		public int IdJMes { get; set; }
    }
}
