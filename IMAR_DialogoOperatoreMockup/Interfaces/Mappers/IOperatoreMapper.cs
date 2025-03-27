using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Interfaces.ViewModels;

namespace IMAR_DialogoOperatore.Interfaces.Mappers
{
    public interface IOperatoreMapper
    {
        Operatore OperatoreViewModelToOperatore(IOperatoreViewModel operatoreViewModel);
    }
}