using System.Linq.Expressions;

namespace IMAR_DialogoOperatore.Application.Interfaces.Repositories
{
	public interface IGenericRepository<TEntity> where TEntity : class
	{
		Task<IQueryable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null,
		Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
		string includeProperties = null);
		IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
			string includeProperties = null);
		TEntity GetByID(object id);
		void Insert(TEntity entity);
		Task InsertAsync(TEntity entity);
		void InsertRange(IEnumerable<TEntity> entities);
		Task InsertRangeAsync(IEnumerable<TEntity> entities);
		void Delete(object id);
		void Delete(TEntity entityToDelete);
		void Update(TEntity entityToUpdate);
		public IQueryable<TEntity> ExecuteQuery<T>(string query);
	}
}
