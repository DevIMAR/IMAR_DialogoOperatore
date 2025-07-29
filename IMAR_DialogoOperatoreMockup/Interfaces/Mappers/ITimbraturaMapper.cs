using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Interfaces.Mappers
{
    public interface ITimbraturaMapper
    {
        TimbraturaAttivitaViewModel TimbraturaToTimbraturaAttivitaViewModel(Timbratura attivitaList);
        IList<TimbraturaAttivitaViewModel> ListTimbratureToListTimbraturaAttivitaViewModel(IList<Timbratura>? attivitaList);
    }
}
