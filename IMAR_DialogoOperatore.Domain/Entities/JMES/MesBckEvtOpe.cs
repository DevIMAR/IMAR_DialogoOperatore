﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace IMAR_DialogoOperatore.Domain.Entities.JMES;

public partial class MesBckEvtOpe
{
    public decimal Uid { get; set; }

    public decimal EvtUid { get; set; }

    public decimal ResUid { get; set; }

    public decimal? BasTeaUid { get; set; }

    public decimal? SftTeaUid { get; set; }

    public DateTime TssStr { get; set; }

    public DateTime? TssEnd { get; set; }

    public decimal StsUid { get; set; }

    public decimal? BrkUid { get; set; }

    public decimal? SspUid { get; set; }

    public decimal? MacSftTeaUid { get; set; }

    public DateTime? Tsi { get; set; }

    public DateTime? Tsu { get; set; }

    public decimal RecVer { get; set; }

    public virtual AngMesBrk BrkU { get; set; }

    public virtual MesBckEvt EvtU { get; set; }

    public virtual AngRes ResU { get; set; }

    public virtual AngMesSsp SspU { get; set; }

    public virtual AngMesEvtOpeSts StsU { get; set; }
}