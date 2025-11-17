using IMAR_DialogoOperatore.Domain.Entities.Imar_Schedulazione;

namespace IMAR_DialogoOperatore.Application.Interfaces.Services.Activities
{
    public interface IForzaturaService
    {
        void ForzaRigaOrdineDaIdEvento(int idEvento);
        void ForzaRigaOrdineDaOdp(string odp);
        void ForzaRigheOrdine(IEnumerable<string> righeOrdine);
        void ForzaRigaOrdineDaOrdineCliente(ORDINE_CLIENTE ordineCliente);
    }
}
