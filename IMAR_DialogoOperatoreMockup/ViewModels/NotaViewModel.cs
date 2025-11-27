using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Interfaces.ViewModels;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class NotaViewModel : INotaViewModel
    {
        private Nota _nota;

        public string? Operatore => _nota.Operatore;
        public DateTime DataImmissione => _nota.DataImmissione;
        public string Odp => _nota.Odp;
        public string Fase => _nota.Fase;
        public string Bolla => _nota.Bolla;
        public string Testo => _nota.Testo;

        public NotaViewModel(Nota nota)
        {
            _nota = nota;
        }
    }
}
