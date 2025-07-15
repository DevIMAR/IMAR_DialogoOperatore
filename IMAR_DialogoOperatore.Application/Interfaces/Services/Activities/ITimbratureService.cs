using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Application.Interfaces.Services.Activities
{
    public interface ITimbratureService
    {
        List<Timbratura> GetTimbratureOperatore(string badgeOperatore);
        List<Timbratura> GetTimbratureOperatoriDiIeri();
    }
}
