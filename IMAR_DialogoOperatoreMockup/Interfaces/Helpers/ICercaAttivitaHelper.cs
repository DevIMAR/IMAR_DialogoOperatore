namespace IMAR_DialogoOperatore.Interfaces.Helpers
{
    public interface ICercaAttivitaHelper
	{
		void CercaAttivita(string? bolla = null, string? odp = null);
		void CercaAttivitaDaBolla(string bolla);
		void CercaAttivitaDaOdp(string odp);
		void CercaAttivitaDaFase(string fase);
	}
}