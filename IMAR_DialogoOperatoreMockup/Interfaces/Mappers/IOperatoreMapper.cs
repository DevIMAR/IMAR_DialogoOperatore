using IMAR_DialogoOperatore.Application.Interfaces.ViewModels;
using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Interfaces.Mappers
{
    public interface IOperatoreMapper
    {
        Operatore OperatoreViewModelToOperatore(IOperatoreViewModel operatoreViewModel);
    }
}