﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace IMAR_DialogoOperatore.Domain.Entities.JMES;

public partial class TblMesClkRes
{
    public decimal Uid { get; set; }

    public decimal ResUid { get; set; }

    public decimal EvtUid { get; set; }

    public decimal? BasTeaUid { get; set; }

    public decimal? SftTeaUid { get; set; }

    public decimal? MacSftTeaUid { get; set; }

    public decimal LogDel { get; set; }

    public DateTime? Tsi { get; set; }

    public DateTime? Tsu { get; set; }

    public decimal RecVer { get; set; }

    public DateTime? Tsd { get; set; }

    public virtual AngRes ResU { get; set; }
}