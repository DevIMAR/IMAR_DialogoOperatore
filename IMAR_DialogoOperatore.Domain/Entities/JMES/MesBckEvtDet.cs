﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace IMAR_DialogoOperatore.Domain.Entities.JMES;

public partial class MesBckEvtDet
{
    public decimal Uid { get; set; }

    public decimal EvtUid { get; set; }

    public decimal DecTypUid { get; set; }

    public string DocCod { get; set; }

    public decimal? EvtDetRefUid { get; set; }

    public decimal DocPri { get; set; }

    public string PrdOrdCod { get; set; }

    public string PrdCycCod { get; set; }

    public string PrdPhsCod { get; set; }

    public string PrdPhsDsc { get; set; }

    public string CusCod { get; set; }

    public string ItmCod { get; set; }

    public string ItmDsc { get; set; }

    public string RefCod { get; set; }

    public string ExtCod { get; set; }

    public string JobOrdUid { get; set; }

    public string JobOrdCod { get; set; }

    public string SubJobOrdUid { get; set; }

    public string SubJobOrdCod { get; set; }

    public string LotCod { get; set; }

    public string DptCod { get; set; }

    public string CauCod { get; set; }

    public string WrcCod { get; set; }

    public string WrcDsc { get; set; }

    public decimal? IndUid { get; set; }

    public decimal QtyPrd { get; set; }

    public decimal QtyRej { get; set; }

    public decimal QtyNco { get; set; }

    public decimal QtyRes { get; set; }

    public decimal QtyOrd { get; set; }

    public decimal Qt2Prd { get; set; }

    public decimal Qt2Rej { get; set; }

    public decimal Qt2Nco { get; set; }

    public decimal Qt2Res { get; set; }

    public decimal Qt2Ord { get; set; }

    public decimal? QtyMin { get; set; }

    public decimal? WrcUid { get; set; }

    public DateTime? Tsi { get; set; }

    public DateTime? Tsu { get; set; }

    public decimal RecVer { get; set; }

    public decimal? PrjUid { get; set; }

    public decimal? PrjTskUid { get; set; }

    public string CusDsc { get; set; }

    public string PrsFld001 { get; set; }

    public string PrsFld002 { get; set; }

    public string PrsFld003 { get; set; }

    public string PrsFld004 { get; set; }

    public string PrsFld005 { get; set; }

    public string PrsFld006 { get; set; }

    public string PrsFld007 { get; set; }

    public string PrsFld008 { get; set; }

    public string PrsFld009 { get; set; }

    public string PrsFld010 { get; set; }

    public decimal PrtNum { get; set; }

    public string PrsFldIco001 { get; set; }

    public string PrsFldIco002 { get; set; }

    public string PrsFldIco003 { get; set; }

    public string PrsFldIco004 { get; set; }

    public string PrsFldIco005 { get; set; }

    public string PrsFldClr001 { get; set; }

    public string PrsFldClr002 { get; set; }

    public string PrsFldClr003 { get; set; }

    public string PrsFldClr004 { get; set; }

    public string PrsFldClr005 { get; set; }

    public string PrsFldClr006 { get; set; }

    public string PrsFldClr007 { get; set; }

    public string PrsFldClr008 { get; set; }

    public string PrsFldClr009 { get; set; }

    public string PrsFldClr010 { get; set; }

    public string EqpCod { get; set; }

    public string ResCod { get; set; }

    public string DrwCod { get; set; }

    public string PhaCod { get; set; }

    public string ProCod { get; set; }

    public string PrjCod { get; set; }

    public string PrjDsc { get; set; }

    public string JobOrdDsc { get; set; }

    public string SubJobOrdDsc { get; set; }

    public string UniMeaBasCod { get; set; }

    public decimal WrkTimOpe { get; set; }

    public decimal TotWrkTim { get; set; }

    public string PhaPhsCod { get; set; }

    public string PhaCycCod { get; set; }

    public decimal QtyPhsPrv { get; set; }

    public virtual AngMesDecTyp DecTypU { get; set; }

    public virtual MesBckEvtDet EvtDetRefU { get; set; }

    public virtual MesBckEvt EvtU { get; set; }

    public virtual ICollection<MesBckEvtDet> InverseEvtDetRefU { get; set; } = new List<MesBckEvtDet>();

    public virtual ICollection<MesBckEvtMacDet> MesBckEvtMacDet { get; set; } = new List<MesBckEvtMacDet>();
}