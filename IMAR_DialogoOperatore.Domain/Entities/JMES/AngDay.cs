﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace IMAR_DialogoOperatore.Domain.Entities.JMES;

public partial class AngDay
{
    public decimal Uid { get; set; }

    public DateTime DayCod { get; set; }

    public decimal DayWee { get; set; }

    public decimal Mon { get; set; }

    public decimal Wee { get; set; }

    public decimal YeaMon { get; set; }

    public decimal YeaWee { get; set; }

    public decimal? EtyUid { get; set; }

    public DateTime? Tsi { get; set; }

    public DateTime? Tsu { get; set; }

    public decimal RecVer { get; set; }

    public virtual ICollection<MesBckEvt> MesBckEvt { get; set; } = new List<MesBckEvt>();

    public virtual ICollection<MesEvt> MesEvt { get; set; } = new List<MesEvt>();

    public virtual ICollection<TblResBrk> TblResBrk { get; set; } = new List<TblResBrk>();

    public virtual ICollection<TblResClk> TblResClk { get; set; } = new List<TblResClk>();
}