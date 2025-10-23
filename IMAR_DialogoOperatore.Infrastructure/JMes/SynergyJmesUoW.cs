using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using IMAR_DialogoOperatore.Application.Interfaces.UoW;
using IMAR_DialogoOperatore.Domain.Entities.JMES;

namespace IMAR_DialogoOperatore.Infrastructure.JMes
{
	public class SynergyJmesUoW : ISynergyJmesUoW
	{
		private SynergyJmesContext _context;

		private readonly IGenericRepository<AngBdg> _angBdg;
		private readonly IGenericRepository<AngDay> _angDay;
		private readonly IGenericRepository<AngGrp> _angGrp;
		private readonly IGenericRepository<AngGrpLng> _angGrpLng;
		private readonly IGenericRepository<AngMesBrk> _angMesBrk;
		private readonly IGenericRepository<AngMesBrkLng> _angMesBrkLng;
		private readonly IGenericRepository<AngMesDecTyp> _angMesDecTyp;
		private readonly IGenericRepository<AngMesDecTypLng> _angMesDecTypLng;
		private readonly IGenericRepository<AngMesEvtOpeSts> _angMesEvtOpeSts;
		private readonly IGenericRepository<AngMesEvtOpeStsLng> _angMesEvtOpeStsLng;
        private readonly IGenericRepository<AngMesNotPln> _angMesNotPln;
        private readonly IGenericRepository<AngMesNotPlnLng> _angMesNotPlnLng;
        private readonly IGenericRepository<AngMesSsp> _angMesSsp;
		private readonly IGenericRepository<AngMesSspLng> _angMesSspLng;
		private readonly IGenericRepository<AngMesSts> _angMesSts;
		private readonly IGenericRepository<AngMesStsLng> _angMesStsLng;
		private readonly IGenericRepository<AngRes> _angRes;
		private readonly IGenericRepository<MesBckEvt> _mesBckEvt;
		private readonly IGenericRepository<MesBckEvtDet> _mesBckEvtDet;
		private readonly IGenericRepository<MesBckEvtMacDet> _mesBckEvtMacDet;
		private readonly IGenericRepository<MesBckEvtOpe> _mesBckEvtOpe;
		private readonly IGenericRepository<MesDiaDefMac> _mesDiaDefMac;
		private readonly IGenericRepository<MesDiaOpe> _mesDiaOpe;
		private readonly IGenericRepository<MesDiaOpeDet> _mesDiaOpeDet;
		private readonly IGenericRepository<MesEvt> _mesEvt;
		private readonly IGenericRepository<MesEvtDet> _mesEvtDet;
		private readonly IGenericRepository<MesEvtMacDet> _mesEvtMacDet;
		private readonly IGenericRepository<MesEvtOpe> _mesEvtOpe;
		private readonly IGenericRepository<TblMesClkRes> _tblMesClkRes;
		private readonly IGenericRepository<TblResBrk> _tblResBrk;
		private readonly IGenericRepository<TblResClk> _tblResClk;

		public SynergyJmesUoW(
			SynergyJmesContext context)
		{
			_context = context;
		}

		public IGenericRepository<AngBdg> AngBdg => _angBdg ?? new GenericRepository<AngBdg>(_context);
		public IGenericRepository<AngDay> AngDay => _angDay ?? new GenericRepository<AngDay>(_context);
		public IGenericRepository<AngGrp> AngGrp => _angGrp ?? new GenericRepository<AngGrp>(_context);
		public IGenericRepository<AngGrpLng> AngGrpLng => _angGrpLng ?? new GenericRepository<AngGrpLng>(_context);
		public IGenericRepository<AngMesBrk> AngMesBrk => _angMesBrk ?? new GenericRepository<AngMesBrk>(_context);
		public IGenericRepository<AngMesBrkLng> AngMesBrkLng => _angMesBrkLng ?? new GenericRepository<AngMesBrkLng>(_context);
		public IGenericRepository<AngMesDecTyp> AngMesDecTyp => _angMesDecTyp ?? new GenericRepository<AngMesDecTyp>(_context);
		public IGenericRepository<AngMesDecTypLng> AngMesDecTypLng => _angMesDecTypLng ?? new GenericRepository<AngMesDecTypLng>(_context);
		public IGenericRepository<AngMesEvtOpeSts> AngMesEvtOpeSts => _angMesEvtOpeSts ?? new GenericRepository<AngMesEvtOpeSts>(_context);
		public IGenericRepository<AngMesEvtOpeStsLng> AngMesEvtOpeStsLng => _angMesEvtOpeStsLng ?? new GenericRepository<AngMesEvtOpeStsLng>(_context);
        public IGenericRepository<AngMesNotPln> AngMesNotPln => _angMesNotPln ?? new GenericRepository<AngMesNotPln>(_context);
        public IGenericRepository<AngMesNotPlnLng> AngMesNotPlnLng => _angMesNotPlnLng ?? new GenericRepository<AngMesNotPlnLng>(_context);
        public IGenericRepository<AngMesSsp> AngMesSsp => _angMesSsp ?? new GenericRepository<AngMesSsp>(_context);
		public IGenericRepository<AngMesSspLng> AngMesSspLng => _angMesSspLng ?? new GenericRepository<AngMesSspLng>(_context);
		public IGenericRepository<AngMesSts> AngMesSts => _angMesSts ?? new GenericRepository<AngMesSts>(_context);
		public IGenericRepository<AngMesStsLng> AngMesStsLng => _angMesStsLng ?? new GenericRepository<AngMesStsLng>(_context);
		public IGenericRepository<AngRes> AngRes => _angRes ?? new GenericRepository<AngRes>(_context);
		public IGenericRepository<MesBckEvt> MesBckEvt => _mesBckEvt ?? new GenericRepository<MesBckEvt>(_context);
		public IGenericRepository<MesBckEvtDet> MesBckEvtDet => _mesBckEvtDet ?? new GenericRepository<MesBckEvtDet>(_context);
		public IGenericRepository<MesBckEvtMacDet> MesBckEvtMacDet => _mesBckEvtMacDet ?? new GenericRepository<MesBckEvtMacDet>(_context);
		public IGenericRepository<MesBckEvtOpe> MesBckEvtOpe => _mesBckEvtOpe ?? new GenericRepository<MesBckEvtOpe>(_context);
		public IGenericRepository<MesDiaDefMac> MesDiaDefMac => _mesDiaDefMac ?? new GenericRepository<MesDiaDefMac>(_context);
		public IGenericRepository<MesDiaOpe> MesDiaOpe => _mesDiaOpe ?? new GenericRepository<MesDiaOpe>(_context);
		public IGenericRepository<MesDiaOpeDet> MesDiaOpeDet => _mesDiaOpeDet ?? new GenericRepository<MesDiaOpeDet>(_context);
		public IGenericRepository<MesEvt> MesEvt => _mesEvt ?? new GenericRepository<MesEvt>(_context);
		public IGenericRepository<MesEvtDet> MesEvtDet => _mesEvtDet ?? new GenericRepository<MesEvtDet>(_context);
		public IGenericRepository<MesEvtMacDet> MesEvtMacDet => _mesEvtMacDet ?? new GenericRepository<MesEvtMacDet>(_context);
		public IGenericRepository<MesEvtOpe> MesEvtOpe => _mesEvtOpe ?? new GenericRepository<MesEvtOpe>(_context);
		public IGenericRepository<TblMesClkRes> TblMesClkRes => _tblMesClkRes ?? new GenericRepository<TblMesClkRes>(_context);
		public IGenericRepository<TblResBrk> TblResBrk => _tblResBrk ?? new GenericRepository<TblResBrk>(_context);
		public IGenericRepository<TblResClk> TblResClk => _tblResClk ?? new GenericRepository<TblResClk>(_context);

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
