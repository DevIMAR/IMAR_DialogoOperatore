﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace IMAR_DialogoOperatore.Domain.Entities.JMES;

public partial class MesDiaOpe
{
    public decimal Uid { get; set; }

    public decimal ResPriUid { get; set; }

    public decimal ActPri { get; set; }

    public decimal? EvtUid { get; set; }

    public decimal? ResLgnUid { get; set; }

    public decimal StsUid { get; set; }

    public decimal? SspUid { get; set; }

    public decimal? OrgLvlUid { get; set; }

    public DateTime? Tsi { get; set; }

    public DateTime? Tsu { get; set; }

    public decimal RecVer { get; set; }

    public decimal? SevUid { get; set; }

    public decimal? CnlUid { get; set; }

    public string ResNot { get; set; }

    public virtual MesEvt EvtU { get; set; }

    public virtual ICollection<MesDiaOpeDet> MesDiaOpeDet { get; set; } = new List<MesDiaOpeDet>();

    public virtual AngRes ResLgnU { get; set; }

    public virtual AngRes ResPriU { get; set; }

    public virtual AngMesSsp SspU { get; set; }

    public virtual AngMesSts StsU { get; set; }
}