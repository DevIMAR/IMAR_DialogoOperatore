﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace IMAR_DialogoOperatore.Domain.Entities.JMES;

public partial class MesBckEvt
{
    public decimal Uid { get; set; }

    public decimal ResPriUid { get; set; }

    public decimal EvtTypUid { get; set; }

    public decimal ResLgnStrUid { get; set; }

    public decimal? ResLgnEndUid { get; set; }

    public decimal ResEffStrUid { get; set; }

    public decimal? ResEffEndUid { get; set; }

    public DateTime TssStr { get; set; }

    public DateTime? TssEnd { get; set; }

    public decimal? EtyUid { get; set; }

    public decimal? SspUid { get; set; }

    public decimal? DtcDayUid { get; set; }

    public decimal? EvtRefUid { get; set; }

    public decimal? ResOrgUid { get; set; }

    public decimal EvtChk { get; set; }

    public DateTime? TssCnsQty { get; set; }

    public DateTime? TssCnsTim { get; set; }

    public decimal? OvrLapNum { get; set; }

    public decimal MatDecLck { get; set; }

    public decimal? EvtAnmUid { get; set; }

    public decimal? EvtPerEff { get; set; }

    public decimal EvtUac { get; set; }

    public decimal? OrgLvlUid { get; set; }

    public DateTime? Tsi { get; set; }

    public DateTime? Tsu { get; set; }

    public decimal RecVer { get; set; }

    public decimal? CnlUid { get; set; }

    public string MatFlg { get; set; }

    public virtual AngDay DtcDayU { get; set; }

    public virtual ICollection<MesBckEvtDet> MesBckEvtDet { get; set; } = new List<MesBckEvtDet>();

    public virtual ICollection<MesBckEvtMacDet> MesBckEvtMacDet { get; set; } = new List<MesBckEvtMacDet>();

    public virtual ICollection<MesBckEvtOpe> MesBckEvtOpe { get; set; } = new List<MesBckEvtOpe>();

    public virtual AngRes ResEffEndU { get; set; }

    public virtual AngRes ResEffStrU { get; set; }

    public virtual AngRes ResLgnEndU { get; set; }

    public virtual AngRes ResLgnStrU { get; set; }

    public virtual AngRes ResOr { get; set; }

    public virtual AngRes ResPriU { get; set; }

    public virtual AngMesSsp SspU { get; set; }
}