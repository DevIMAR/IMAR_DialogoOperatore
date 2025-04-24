namespace IMAR_DialogoOperatore.Interfaces.Observers
{
	public interface IAvanzamentoObserver
	{
		uint? QuantitaProdotta { get; set; }
		uint? QuantitaScartata { get; set; }
		string SaldoAcconto { get; set; }

		event Action OnIsFaseSaldataChanged;
		event Action OnQuantitaProdottaChanged;
		event Action OnQuantitaScartataChanged;
	}
}