using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Interfaces.ViewModels;

namespace IMAR_DialogoOperatore.ViewModels
{
    public class NotaViewModel : INotaViewModel
    {
        private Nota? _nota;

        public string? Operatore => _nota?.Operatore;
        public DateTime DataImmissione => DateTime.ParseExact(_nota?.DataImmissione.ToString() ?? "00010101", "yyyyMMdd", null);
        public string? Odp => _nota?.Odp;
        public string? Fase => _nota?.Fase;
        public decimal Riga => _nota?.Riga ?? 0;
        public string? Bolla => _nota?.Bolla;
        public string? Testo {  get; set; }

        public NotaViewModel(Nota? nota)
        {
            _nota = nota;
            Testo = _nota?.Testo;
        }
    }
}
