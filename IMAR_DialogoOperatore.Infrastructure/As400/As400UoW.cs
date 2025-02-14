using IMAR_DialogoOperatore.Application.Interfaces.UoW;
using IMAR_DialogoOperatore.Infrastructure.As400;

namespace ImarConnect.Infrastructure.As400
{
	internal class As400UoW : IAs400UoW
	{
		private As400Context _context;

		public As400UoW(As400Context context)
        {
            _context = context;
        }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private bool disposed = false;
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
					_context.Dispose();
			}
			this.disposed = true;
		}

		public int Save()
		{
			return _context.SaveChanges();
		}

		public Task<int> SaveAsync()
		{
			return _context.SaveChangesAsync();
		}
	}
}
