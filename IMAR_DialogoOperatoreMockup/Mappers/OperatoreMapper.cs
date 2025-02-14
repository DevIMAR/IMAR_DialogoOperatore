using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Application.Interfaces.ViewModels;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Interfaces.Mappers;

namespace IMAR_DialogoOperatore.Mappers
{
    public class OperatoreMapper : IOperatoreMapper
	{
		private readonly IOperatoriService _operatoriService;

		public OperatoreMapper(
            IOperatoriService operatoriService)
		{
            _operatoriService = operatoriService;
		}

		public Operatore OperatoreViewModelToOperatore(IOperatoreViewModel operatoreViewModel)
		{
			return _operatoriService.Operatore;
		}
	}
}
