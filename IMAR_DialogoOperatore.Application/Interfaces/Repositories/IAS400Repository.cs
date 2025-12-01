namespace IMAR_DialogoOperatore.Application.Interfaces.Repositories
{
    public interface IAs400Repository
    {
        IEnumerable<T> GetAll<T>();
        IEnumerable<T> ExecuteQuery<T>(string query);
        int ExecuteCommand(string sql, object? param = null);
    }
}
