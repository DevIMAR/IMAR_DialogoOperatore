﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace IMAR_DialogoOperatore.Domain.Entities.JMES;

public partial class AngRes
{
    public decimal Uid { get; set; }

    public decimal? EtyUid { get; set; }

    public decimal ResTypUid { get; set; }

    public string ResCod { get; set; }

    public string ResNam { get; set; }

    public string ResSur { get; set; }

    public string ResDsc { get; set; }

    public string ResShtDsc { get; set; }

    public decimal LogDel { get; set; }

    public string ResIco { get; set; }

    public decimal ResTpl { get; set; }

    public string LgnUsr { get; set; }

    public string EmlAdd { get; set; }

    public string LgnCrpPwd { get; set; }

    public string LgnIpa { get; set; }

    public DateTime? Tsi { get; set; }

    public DateTime? Tsu { get; set; }

    public decimal RecVer { get; set; }

    public DateTime? Tsd { get; set; }

    public decimal? BdgUid { get; set; }

    public decimal? DiaOpeViwUid { get; set; }

    public decimal? BdgSecSty { get; set; }

    public decimal? DefWrkTmDcl { get; set; }

    public decimal? DiaMnoCfgUid { get; set; }

    public decimal? DiaOpeThmUid { get; set; }

    public decimal SchEnb { get; set; }

    public decimal SchFin { get; set; }

    public decimal? CalUid { get; set; }

    public decimal PerNis { get; set; }

    public decimal? ResEff { get; set; }

    public string TmuIco { get; set; }

    public decimal? TmuVal { get; set; }

    public decimal? MaxHouDayCons { get; set; }

    public string PrlGrp { get; set; }

    public decimal? RepGrpUid { get; set; }

    public decimal? PlnQty { get; set; }

    public decimal? GntCfgDefPrjUid { get; set; }

    public decimal? WbsGntCfgDefPrjUid { get; set; }

    public decimal? OrgGntCfgDefPrjUid { get; set; }

    public decimal? GntCfgDefMdlUid { get; set; }

    public decimal? WbsGntCfgDefMdlUid { get; set; }

    public decimal? OrgGntCfgDefMdlUid { get; set; }

    public DateTime? DtsVal { get; set; }

    public DateTime? DteVal { get; set; }

    public decimal? OrgComUid { get; set; }

    public decimal OrgComFlt { get; set; }

    public decimal OrgComShwGlb { get; set; }

    public decimal OrgComEdtGlb { get; set; }

    public decimal? UsrFavLngUid { get; set; }

    public decimal? TimZonUid { get; set; }

    public decimal? StdWrkTim { get; set; }

    public decimal? WrkTimCofTol { get; set; }

    public decimal? SdeBarViw { get; set; }

    public decimal? DiaOpeCfgUid { get; set; }

    public decimal? GntMltPrjCfgDefUid { get; set; }

    public decimal? GntSchPtfCfgDefUid { get; set; }

    public decimal? GntSchPtfCmpCfgDefUid { get; set; }

    public decimal? TeaPlnCfgDefUid { get; set; }

    public decimal PrjBdgSec { get; set; }

    public decimal? GntMltPrjFltDefUid { get; set; }

    public decimal? GntSchPtfFltDefUid { get; set; }

    public decimal? GntSchPtfCmpFltDefUid { get; set; }

    public virtual AngBdg Bd { get; set; }

    public virtual ICollection<MesBckEvtOpe> MesBckEvtOpe { get; set; } = new List<MesBckEvtOpe>();

    public virtual ICollection<MesBckEvt> MesBckEvtResEffEndU { get; set; } = new List<MesBckEvt>();

    public virtual ICollection<MesBckEvt> MesBckEvtResEffStrU { get; set; } = new List<MesBckEvt>();

    public virtual ICollection<MesBckEvt> MesBckEvtResLgnEndU { get; set; } = new List<MesBckEvt>();

    public virtual ICollection<MesBckEvt> MesBckEvtResLgnStrU { get; set; } = new List<MesBckEvt>();

    public virtual ICollection<MesBckEvt> MesBckEvtResOr { get; set; } = new List<MesBckEvt>();

    public virtual ICollection<MesBckEvt> MesBckEvtResPriU { get; set; } = new List<MesBckEvt>();

    public virtual ICollection<MesDiaDefMac> MesDiaDefMacMacU { get; set; } = new List<MesDiaDefMac>();

    public virtual ICollection<MesDiaDefMac> MesDiaDefMacResU { get; set; } = new List<MesDiaDefMac>();

    public virtual ICollection<MesDiaOpe> MesDiaOpeResLgnU { get; set; } = new List<MesDiaOpe>();

    public virtual ICollection<MesDiaOpe> MesDiaOpeResPriU { get; set; } = new List<MesDiaOpe>();

    public virtual ICollection<MesEvtOpe> MesEvtOpe { get; set; } = new List<MesEvtOpe>();

    public virtual ICollection<MesEvt> MesEvtResEffEndU { get; set; } = new List<MesEvt>();

    public virtual ICollection<MesEvt> MesEvtResEffStrU { get; set; } = new List<MesEvt>();

    public virtual ICollection<MesEvt> MesEvtResLgnEndU { get; set; } = new List<MesEvt>();

    public virtual ICollection<MesEvt> MesEvtResLgnStrU { get; set; } = new List<MesEvt>();

    public virtual ICollection<MesEvt> MesEvtResOr { get; set; } = new List<MesEvt>();

    public virtual ICollection<MesEvt> MesEvtResPriU { get; set; } = new List<MesEvt>();

    public virtual AngGrp RepGrpU { get; set; }

    public virtual ICollection<TblMesClkRes> TblMesClkRes { get; set; } = new List<TblMesClkRes>();

    public virtual ICollection<TblResBrk> TblResBrkResLgnEndU { get; set; } = new List<TblResBrk>();

    public virtual ICollection<TblResBrk> TblResBrkResLgnStrU { get; set; } = new List<TblResBrk>();

    public virtual ICollection<TblResBrk> TblResBrkResU { get; set; } = new List<TblResBrk>();

    public virtual ICollection<TblResClk> TblResClk { get; set; } = new List<TblResClk>();
}