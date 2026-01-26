using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Interfaces.Mappers;
using IMAR_DialogoOperatore.Interfaces.ViewModels;
using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Mappers
{
    public class NotaMapper : INotaMapper
    {
        public IEnumerable<INotaViewModel> ListNotaToListNotaViewModel(IEnumerable<Nota>? note)
        {
            if (note == null)
                return Enumerable.Empty<INotaViewModel>();

            List<INotaViewModel> listNotaViewModel = new List<INotaViewModel>();
            foreach (Nota nota in note)
                listNotaViewModel.Add(NotaToNotaViewModel(nota));

            return listNotaViewModel;
        }

        public INotaViewModel NotaToNotaViewModel(Nota nota)
        {
            return new NotaViewModel(nota);
        }

        public Nota NotaViewModelToNota(INotaViewModel notaViewModel)
        {
            return new Nota
            {
                DataImmissione = decimal.Parse(notaViewModel.DataImmissione.ToString("yyyyMMdd")),
                Operatore = notaViewModel.Operatore,
                Odp = notaViewModel.Odp,
                Fase = notaViewModel.Fase,
                Riga = notaViewModel.Riga,
                Bolla = notaViewModel.Bolla,
                Testo = notaViewModel.Testo,
            };
        }
    }
}
