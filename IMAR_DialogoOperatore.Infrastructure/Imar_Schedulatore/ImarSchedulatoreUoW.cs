using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using IMAR_DialogoOperatore.Application.Interfaces.UoW;
using IMAR_DialogoOperatore.Domain.Entities.Imar_Schedulazione;

namespace IMAR_DialogoOperatore.Infrastructure.Imar_Schedulatore
{
    public class ImarSchedulatoreUoW : IImarSchedulatoreUoW
    {
        private ImarSchedulatoreContext _context;

        private readonly IGenericRepository<CAL_FL_ODP> _calFlOdpRepository;
        private readonly IGenericRepository<DESCRIZIONE_FASI> _descrizioneFasiRepository;
        private readonly IGenericRepository<FASI> _fasiRepository;
        private readonly IGenericRepository<ODC_ODP> _odcOdpRepository;
        private readonly IGenericRepository<ORDINE_CLIENTE> _ordineClienteRepository;
        private readonly IGenericRepository<ORDINE_PRODUZIONE> _ordineProduzioneRepository;

        public ImarSchedulatoreUoW(
            ImarSchedulatoreContext context)
        {
            _context = context;
        }

        public IGenericRepository<CAL_FL_ODP> CalFlOdpRepository => _calFlOdpRepository ?? new GenericRepository<CAL_FL_ODP>(_context);
        public IGenericRepository<DESCRIZIONE_FASI> DescrizioneFasiRepository => _descrizioneFasiRepository ?? new GenericRepository<DESCRIZIONE_FASI>(_context);
        public IGenericRepository<FASI> FasiRepository => _fasiRepository ?? new GenericRepository<FASI>(_context);
        public IGenericRepository<ODC_ODP> OdcOdpRepository => _odcOdpRepository ?? new GenericRepository<ODC_ODP>(_context);
        public IGenericRepository<ORDINE_CLIENTE> OrdineClienteRepository => _ordineClienteRepository ?? new GenericRepository<ORDINE_CLIENTE>(_context);
        public IGenericRepository<ORDINE_PRODUZIONE> OrdineProduzioneRepository => _ordineProduzioneRepository ?? new GenericRepository<ORDINE_PRODUZIONE>(_context);

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
