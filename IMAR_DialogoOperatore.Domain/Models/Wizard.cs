namespace IMAR_DialogoOperatore.Domain.Models
{
	public class Wizard
	{
		public string CodiceJGalielo { get; set; }
		public bool qck { get { return true; } }
		public string clkBdgCod { get; set; }
		public string notCod { get; set; }
		public string indCod { get; set; }
		public string prjTskUid { get; set; }
		public int clkMacUid { get; set; }
		public bool isWrk { get; set; }
		public DateTime tssStr { get; set; }
		public DateTime? tssEnd { get; set; }
		public int defDecAdv { get; set; }
		public double producedQuantity { get; set; }
		public double notCompliantQuantity { get; set; }
		public double rejectedQuantity { get; set; }
	}
}
