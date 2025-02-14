using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Application.Interfaces.ViewModels
{
	public interface IOperatoreViewModel
	{
		int? Badge { get; }
		string Nome { get; }
		string Cognome { get; }
		string Stato { get; set; }
		IList<Attivita> AttivitaAperte { get; set; }
	}
}