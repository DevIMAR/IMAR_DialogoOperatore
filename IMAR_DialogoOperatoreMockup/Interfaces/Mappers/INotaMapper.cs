using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Interfaces.ViewModels;

namespace IMAR_DialogoOperatore.Interfaces.Mappers
{
    public interface INotaMapper
    {
        Nota NotaViewModelToNota(INotaViewModel notaViewModel);
        INotaViewModel NotaToNotaViewModel(Nota nota);
        IEnumerable<INotaViewModel> ListNotaToListNotaViewModel(IEnumerable<Nota>? note);
    }
}
