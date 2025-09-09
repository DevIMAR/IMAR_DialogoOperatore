using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using IMAR_DialogoOperatore.Application.Interfaces.UoW;
using IMAR_DialogoOperatore.Domain.Entities.Imar_Connect;

namespace IMAR_DialogoOperatore.Infrastructure.Imar_Connect
{
    internal class ImarConnectUoW : IImarConnectUoW
    {
        private ImarConnectContext _context;

        private readonly IGenericRepository<Interfaccia> _interfacciaRepository;

        public ImarConnectUoW(
            ImarConnectContext context)
        {
            _context = context;
        }
        public IGenericRepository<Interfaccia> InterfacciaRepository => _interfacciaRepository ?? new GenericRepository<Interfaccia>(_context);

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
