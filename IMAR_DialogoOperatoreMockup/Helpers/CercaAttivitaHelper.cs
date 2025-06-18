using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Interfaces.Mappers;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Helpers
{
    public class CercaAttivitaHelper : ICercaAttivitaHelper
	{
		private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
		private readonly ICercaAttivitaObserver _cercaAttivitaObserver;
		private readonly IAttivitaIndirettaObserver _attivitaIndirettaObserver;
		private readonly IAttivitaService _attivitaService;
		private readonly IAttivitaMapper _attivitaMapper;

		public CercaAttivitaHelper(
			IDialogoOperatoreObserver dialogoOperatoreObserver,
			ICercaAttivitaObserver cercaAttivitaObserver,
			IAttivitaIndirettaObserver attivitaIndirettaObserver,
			IAttivitaService attivitaService,
			IAttivitaMapper attivitaMapper)
		{
			_dialogoOperatoreObserver = dialogoOperatoreObserver;
			_cercaAttivitaObserver = cercaAttivitaObserver;
			_attivitaIndirettaObserver = attivitaIndirettaObserver;
			_attivitaService = attivitaService;
			_attivitaMapper = attivitaMapper;
		}

		public void CercaAttivita(string? bolla = null, string? odp = null)
		{
            if (bolla != null)
				CercaAttivitaDaBolla(bolla);

			if (odp != null)
				CercaAttivitaDaOdp(odp);
		}

		public void CercaAttivitaDaBolla(string bolla)
		{
			_cercaAttivitaObserver.IsAttivitaCercata = true;

            Attivita attivita = _attivitaService.CercaAttivitaDaBolla(bolla);
			if (attivita == null)
			{
				_dialogoOperatoreObserver.AttivitaSelezionata = null;
				return;
            }

			if (string.IsNullOrEmpty(attivita.Odp))
			{
				_dialogoOperatoreObserver.AttivitaSelezionata = null;
				return;
			}

            CercaAttivitaDaOdp(attivita.Odp);

			_dialogoOperatoreObserver.AttivitaSelezionata = _cercaAttivitaObserver.AttivitaTrovate.SingleOrDefault(x => x.Bolla == bolla);


			_cercaAttivitaObserver.FaseCercata = _dialogoOperatoreObserver.AttivitaSelezionata.Fase;
		}

		public void CercaAttivitaDaOdp(string odp)
		{
			_cercaAttivitaObserver.IsAttivitaCercata = true;
            _attivitaIndirettaObserver.IsAttivitaIndiretta = false;

            _cercaAttivitaObserver.AttivitaTrovate = _attivitaMapper.ListaAttivitaToListaAttivitaViewModel(_attivitaService.GetAttivitaPerOdp(odp));

			_dialogoOperatoreObserver.AttivitaSelezionata = _cercaAttivitaObserver.AttivitaTrovate.FirstOrDefault();

			if (_dialogoOperatoreObserver.AttivitaSelezionata != null)
				_cercaAttivitaObserver.FaseCercata = _dialogoOperatoreObserver.AttivitaSelezionata.Fase;
		}

		public void CercaAttivitaDaFase(string fase)
		{
			_cercaAttivitaObserver.IsAttivitaCercata = true;

			_dialogoOperatoreObserver.AttivitaSelezionata = _cercaAttivitaObserver.AttivitaTrovate.Single(x => x.Fase == fase);
		}
	}
}
