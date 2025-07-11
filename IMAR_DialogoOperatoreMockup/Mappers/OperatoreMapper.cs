using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Interfaces.Mappers;
using IMAR_DialogoOperatore.Interfaces.ViewModels;

namespace IMAR_DialogoOperatore.Mappers
{
    public class OperatoreMapper : IOperatoreMapper
	{
		private readonly IOperatoreService _operatoriService;

		public OperatoreMapper(
            IOperatoreService operatoriService)
		{
            _operatoriService = operatoriService;
		}

		public Operatore OperatoreViewModelToOperatore(IOperatoreViewModel operatoreViewModel)
		{
			_operatoriService.Operatore.MacchinaAssegnata = operatoreViewModel.MacchinaAssegnata;
			return _operatoriService.Operatore;
		}
	}
}
