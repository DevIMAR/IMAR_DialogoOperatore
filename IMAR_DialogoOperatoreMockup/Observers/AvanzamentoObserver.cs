using IMAR_DialogoOperatore.Interfaces.Observers;

namespace IMAR_DialogoOperatore.Observers
{
	public class AvanzamentoObserver : ObserverBase, IAvanzamentoObserver
	{
		private uint? _quantitaProdotta;
		private uint? _quantitaScartata;
		private string _saldoAcconto;

		public uint? QuantitaProdotta
		{
			get { return _quantitaProdotta; }
			set
			{
				_quantitaProdotta = value;
				CallAction(OnQuantitaProdottaChanged);
			}
		}
		public uint? QuantitaScartata
		{
			get { return _quantitaScartata; }
			set
			{
				_quantitaScartata = value;
				CallAction(OnQuantitaScartataChanged);
			}
		}
		public string SaldoAcconto
		{
			get { return _saldoAcconto; }
			set
			{
				_saldoAcconto = value;
				CallAction(OnIsFaseSaldataChanged);
			}
		}

		public event Action OnQuantitaProdottaChanged;
		public event Action OnQuantitaScartataChanged;
		public event Action OnIsFaseSaldataChanged;
	}
}
