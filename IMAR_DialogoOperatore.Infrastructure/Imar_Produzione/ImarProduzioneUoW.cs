using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using IMAR_DialogoOperatore.Application.Interfaces.UoW;
using IMAR_DialogoOperatore.Domain.Entities.Imar_Produzione;

namespace IMAR_DialogoOperatore.Infrastructure.Imar_Produzione
{
    internal class ImarProduzioneUoW : IImarProduzioneUoW
    {
        private ImarProduzioneContext _context;

        private readonly IGenericRepository<Forzatura> _forzaturaRepository;
        private readonly IGenericRepository<OrdineProduzioneForzato> _ordineProduzioneForzatoRepository;
        private readonly IGenericRepository<SegnalazioneDifformita> _segnalazioneDifformitaRepository;

        public ImarProduzioneUoW(
            ImarProduzioneContext context)
        {
            _context = context;
        }

        public IGenericRepository<Forzatura> ForzaturaRepository => _forzaturaRepository ?? new GenericRepository<Forzatura>(_context);
        public IGenericRepository<OrdineProduzioneForzato> OrdineProduzioneForzatoRepository => _ordineProduzioneForzatoRepository ?? new GenericRepository<OrdineProduzioneForzato>(_context);
        public IGenericRepository<SegnalazioneDifformita> SegnalazioniDifformitaRepository => _segnalazioneDifformitaRepository ?? new GenericRepository<SegnalazioneDifformita>(_context);

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
