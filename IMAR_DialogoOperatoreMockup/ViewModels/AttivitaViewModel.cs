using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Interfaces.ViewModels;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class AttivitaViewModel : ViewModelBase, IAttivitaViewModel
	{
		private readonly Attivita? _attivita;

		private string _causale;
		public string? Bolla => _attivita?.Bolla;
		public string? Odp => _attivita?.Odp;
		public string? Articolo => _attivita?.Articolo;
		public string? DescrizioneArticolo => _attivita?.DescrizioneArticolo;
		public string? Fase => _attivita?.Fase;
		public string? DescrizioneFase => _attivita?.DescrizioneFase;
		public int QuantitaOrdine => _attivita != null ? _attivita.QuantitaOrdine : 0;
		public int QuantitaProdotta => QuantitaProdottaNonContabilizzata + QuantitaProdottaContabilizzata;
        public int QuantitaProdottaNonContabilizzata { get; set; }
        public int QuantitaProdottaContabilizzata { get; set; }
		public int QuantitaScartata => QuantitaScartataNonContabilizzata + QuantitaScartataContabilizzata;
        public int QuantitaScartataNonContabilizzata { get; set; }
        public int QuantitaScartataContabilizzata { get; set; }
		public int QuantitaResidua => QuantitaOrdine - QuantitaProdotta;
		public string SaldoAcconto { get; set; }
		public double? CodiceJMes => _attivita?.CodiceJMes;
		public Macchina? Macchina => _attivita?.Macchina;
		public string? CausaleEstesa => _attivita?.CausaleEstesa;
        public DateTime? InizioAttivita => _attivita?.InizioAttivita;
		public DateTime? FineAttivita => _attivita?.FineAttivita;


        public string Causale
		{
			get { return _causale; }
			set
			{
				_causale = value;
				OnNotifyStateChanged();
			}
		}


        public AttivitaViewModel(Attivita? attivita)
		{
			_attivita = attivita;
			if (_attivita == null)
				return;

			Causale = _attivita.Causale == null ? string.Empty : _attivita.Causale;
			QuantitaProdottaNonContabilizzata = _attivita.QuantitaProdottaNonContabilizzata;
			QuantitaProdottaContabilizzata = _attivita.QuantitaProdottaContabilizzata;
			QuantitaScartataNonContabilizzata = _attivita.QuantitaScartataNonContabilizzata;
            QuantitaScartataContabilizzata = _attivita.QuantitaScartataContabilizzata;
			SaldoAcconto = _attivita.SaldoAcconto;
		}
	}
}
