using IMAR_DialogoOperatore.Domain.Entities.Imar_Schedulazione;

namespace IMAR_DialogoOperatore.Application.Interfaces.Services.Activities
{
    public interface IForzaturaService
    {
        Task ForzaRigaOrdineDaIdEvento(int idEvento);
        Task ForzaRigaOrdineDaOdp(string odp);
        Task ForzaRigheOrdine(IEnumerable<string> righeOrdine);
        Task ForzaRigaOrdineDaOrdineCliente(ORDINE_CLIENTE ordineCliente);
    }
}
