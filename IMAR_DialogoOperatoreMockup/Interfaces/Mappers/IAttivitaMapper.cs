using IMAR_DialogoOperatore.Application.Interfaces.ViewModels;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Interfaces.Mappers
{
    public interface IAttivitaMapper
    {
        Attivita AttivitaViewModelToAttivita(IAttivitaViewModel attivitaViewModel);
        IEnumerable<IAttivitaViewModel> ListaAttivitaToListaAttivitaViewModel(IEnumerable<Attivita> attivitaList);
    }
}