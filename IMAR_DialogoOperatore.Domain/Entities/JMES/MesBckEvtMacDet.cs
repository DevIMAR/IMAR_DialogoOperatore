﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace IMAR_DialogoOperatore.Domain.Entities.JMES;

public partial class MesBckEvtMacDet
{
    public decimal Uid { get; set; }

    public decimal EvtUid { get; set; }

    public decimal EvtDetUid { get; set; }

    public decimal QtyPrd { get; set; }

    public decimal QtyRej { get; set; }

    public decimal QtyNco { get; set; }

    public decimal Qt2Prd { get; set; }

    public decimal Qt2Rej { get; set; }

    public decimal Qt2Nco { get; set; }

    public decimal DivQtyPrd { get; set; }

    public decimal DivQtyRej { get; set; }

    public decimal DivQtyNco { get; set; }

    public decimal DivQt2Prd { get; set; }

    public decimal DivQt2Rej { get; set; }

    public decimal DivQt2Nco { get; set; }

    public decimal? DecAdv { get; set; }

    public decimal DivTim { get; set; }

    public DateTime? Tsi { get; set; }

    public DateTime? Tsu { get; set; }

    public decimal RecVer { get; set; }

    public decimal NmvQtyPrd { get; set; }

    public decimal NmvQtyRej { get; set; }

    public decimal NmvQtyNco { get; set; }

    public decimal NmvQt2Prd { get; set; }

    public decimal NmvQt2Rej { get; set; }

    public decimal NmvQt2Nco { get; set; }

    public decimal? MovPrv { get; set; }

    public decimal? EvtMovUid { get; set; }

    public decimal? PerCom { get; set; }

    public decimal QtyPhsPrv { get; set; }

    public virtual MesBckEvtDet EvtDetU { get; set; }

    public virtual MesBckEvt EvtU { get; set; }
}