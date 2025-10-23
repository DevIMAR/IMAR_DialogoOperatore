using IMAR_DialogoOperatore.Application.Interfaces.Repositories;
using IMAR_DialogoOperatore.Domain.Entities.JMES;

namespace IMAR_DialogoOperatore.Application.Interfaces.UoW
{
	public interface ISynergyJmesUoW : IUnitOfWork
    {
		public IGenericRepository<AngBdg> AngBdg { get; }
		public IGenericRepository<AngDay> AngDay { get; }
		public IGenericRepository<AngGrp> AngGrp { get; }
		public IGenericRepository<AngGrpLng> AngGrpLng { get; }
		public IGenericRepository<AngMesBrk> AngMesBrk { get; }
		public IGenericRepository<AngMesBrkLng> AngMesBrkLng { get; }
		public IGenericRepository<AngMesDecTyp> AngMesDecTyp { get; }
		public IGenericRepository<AngMesDecTypLng> AngMesDecTypLng { get; }
		public IGenericRepository<AngMesEvtOpeSts> AngMesEvtOpeSts { get; }
		public IGenericRepository<AngMesEvtOpeStsLng> AngMesEvtOpeStsLng { get; }
		public IGenericRepository<AngMesNotPln> AngMesNotPln { get; }
		public IGenericRepository<AngMesNotPlnLng> AngMesNotPlnLng { get; }
		public IGenericRepository<AngMesSsp> AngMesSsp { get; }
		public IGenericRepository<AngMesSspLng> AngMesSspLng { get; }
		public IGenericRepository<AngMesSts> AngMesSts { get; }
		public IGenericRepository<AngMesStsLng> AngMesStsLng { get; }
		public IGenericRepository<AngRes> AngRes { get; }
		public IGenericRepository<MesBckEvt> MesBckEvt { get; }
		public IGenericRepository<MesBckEvtDet> MesBckEvtDet { get; }
		public IGenericRepository<MesBckEvtMacDet> MesBckEvtMacDet { get; }
		public IGenericRepository<MesBckEvtOpe> MesBckEvtOpe { get; }
		public IGenericRepository<MesDiaDefMac> MesDiaDefMac { get; }
		public IGenericRepository<MesDiaOpe> MesDiaOpe { get; }
		public IGenericRepository<MesDiaOpeDet> MesDiaOpeDet { get; }
		public IGenericRepository<MesEvt> MesEvt { get; }
		public IGenericRepository<MesEvtDet> MesEvtDet { get; }
		public IGenericRepository<MesEvtMacDet> MesEvtMacDet { get; }
		public IGenericRepository<MesEvtOpe> MesEvtOpe { get; }
		public IGenericRepository<TblMesClkRes> TblMesClkRes { get; }
		public IGenericRepository<TblResBrk> TblResBrk { get; }
		public IGenericRepository<TblResClk> TblResClk { get; }
	}
}
