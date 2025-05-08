using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using IMAR_DialogoOperatore.Application.Interfaces.UoW;
using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Infrastructure.Imar_Schedulatore
{
    public class ImarSchedulatoreUoW : IImarSchedulatoreUoW
    {
        private ImarSchedulatoreContext _context;

        private readonly IGenericRepository<DATIMONITOR> _datiMonitorRepository;

        public ImarSchedulatoreUoW(
            ImarSchedulatoreContext context)
        {
            _context = context;
        }
        public IGenericRepository<DATIMONITOR> DatiMonitorRepository => _datiMonitorRepository ?? new GenericRepository<DATIMONITOR>(_context);

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
