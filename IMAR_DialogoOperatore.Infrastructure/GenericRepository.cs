using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IMAR_DialogoOperatore.Infrastructure
{
	public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
	{
		internal DbContext context;
		internal DbSet<TEntity> dbSet;

		public GenericRepository(DbContext context)
		{
			this.context = context;
			this.context.ChangeTracker.LazyLoadingEnabled = false;
			dbSet = context.Set<TEntity>();
		}

		public async Task<IQueryable<TEntity>> GetAsync(
			Expression<Func<TEntity, bool>> filter = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
			string includeProperties = null)
		{
			return await Task.Run(() =>
			{
				return Get(filter, orderBy, includeProperties);
			});
		}

		public virtual IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
			string includeProperties = null)
		{
			IQueryable<TEntity> query = dbSet;

			if (filter != null)
				query = query.Where(filter);

			if (includeProperties != null)
			{
				foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(includeProperty);
				}
			}

			if (orderBy != null)
				return orderBy(query);
			else
				return query;
		}

		public virtual TEntity GetByID(object id)
		{
			return dbSet.Find(id);
		}

		public virtual void Insert(TEntity entity)
		{
			dbSet.Add(entity);
		}

		public async Task InsertAsync(TEntity entity)
		{
			await dbSet.AddAsync(entity);
		}

		public void InsertRange(IEnumerable<TEntity> entities)
		{
			dbSet.AddRange(entities);
		}

		public async Task InsertRangeAsync(IEnumerable<TEntity> entities)
		{
			await dbSet.AddRangeAsync(entities);
		}

		public virtual void Delete(object id)
		{
			TEntity entityToDelete = dbSet.Find(id);
			Delete(entityToDelete);
		}

		public virtual void Delete(TEntity entityToDelete)
		{
			if (context.Entry(entityToDelete).State == EntityState.Detached)
				dbSet.Attach(entityToDelete);

			dbSet.Remove(entityToDelete);
		}

		public virtual void Update(TEntity entityToUpdate)
		{
			dbSet.Attach(entityToUpdate);
			context.Entry(entityToUpdate).State = EntityState.Modified;
		}

		public IEnumerable<TEntity> GetAll()
		{
			throw new NotImplementedException();
		}

        public IQueryable<TEntity> ExecuteQuery<T>(string sql)
        {
            return dbSet.FromSqlRaw(sql);
        }
    }
}
