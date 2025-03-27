using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Interfaces.Mappers;
using IMAR_DialogoOperatore.Interfaces.ViewModels;
using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Mappers
{
    public class AttivitaMapper : IAttivitaMapper
	{
        public Attivita AttivitaViewModelToAttivita(IAttivitaViewModel attivitaViewModel)
		{
			return new Attivita
			{
                Causale = attivitaViewModel.Causale,
                Bolla = attivitaViewModel.Bolla,
                Odp = attivitaViewModel.Odp,
                Articolo = attivitaViewModel.Articolo,
                DescrizioneArticolo = attivitaViewModel.DescrizioneArticolo,
                Fase = attivitaViewModel.Fase,
                DescrizioneFase = attivitaViewModel.DescrizioneFase,
                QuantitaOrdine = attivitaViewModel.QuantitaOrdine,
                QuantitaProdotta = attivitaViewModel.QuantitaProdotta,
                QuantitaScartata = attivitaViewModel.QuantitaScartata,
                QuantitaResidua = attivitaViewModel.QuantitaResidua,
                SaldoAcconto = attivitaViewModel.SaldoAcconto,
                CodiceJMes = attivitaViewModel.CodiceJMes,
                Macchina = attivitaViewModel.Macchina
            };
		}

		public IEnumerable<IAttivitaViewModel> ListaAttivitaToListaAttivitaViewModel(IEnumerable<Attivita> attivitaList)
        {
            IList<IAttivitaViewModel> attivitaViewModelList = new List<IAttivitaViewModel>();

			foreach (Attivita attivita in attivitaList)
			{
				attivitaViewModelList.Add(new AttivitaViewModel(attivita));
			}

			return attivitaViewModelList;
		}
    }
}
