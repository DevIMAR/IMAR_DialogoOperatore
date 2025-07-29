using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Interfaces.Mappers;
using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Mappers
{
    public class TimbraturaMapper : ITimbraturaMapper
    {
        public TimbraturaAttivitaViewModel TimbraturaToTimbraturaAttivitaViewModel(Timbratura timbratura)
        {
            return new TimbraturaAttivitaViewModel
            {
                CausaleEstesa = timbratura.Causale,
                Timestamp = timbratura.Timestamp
            };
        }

        public IList<TimbraturaAttivitaViewModel> ListTimbratureToListTimbraturaAttivitaViewModel(IList<Timbratura>? timbratureList)
        {
            IList<TimbraturaAttivitaViewModel> TimbratureAttivitaViewModelList = new List<TimbraturaAttivitaViewModel>();

            foreach (Timbratura timbratura in timbratureList)
                TimbratureAttivitaViewModelList.Add(TimbraturaToTimbraturaAttivitaViewModel(timbratura));

            return TimbratureAttivitaViewModelList;
        }
    }
}
