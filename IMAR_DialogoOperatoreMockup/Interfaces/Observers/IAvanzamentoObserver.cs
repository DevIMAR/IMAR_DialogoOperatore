namespace IMAR_DialogoOperatore.Interfaces.Observers
{
	public interface IAvanzamentoObserver
	{
		int QuantitaProdotta { get; set; }
		int QuantitaScartata { get; set; }
		string SaldoAcconto { get; set; }

		event Action OnIsFaseSaldataChanged;
		event Action OnQuantitaProdottaChanged;
		event Action OnQuantitaScartataChanged;
	}
}