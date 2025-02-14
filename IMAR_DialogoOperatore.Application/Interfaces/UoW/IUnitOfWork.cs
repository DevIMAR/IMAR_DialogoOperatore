namespace IMAR_DialogoOperatore.Application.Interfaces.UoW
{
	public interface IUnitOfWork
    {
        Task<int> SaveAsync();
        int Save();
        void Dispose();
    }
}
