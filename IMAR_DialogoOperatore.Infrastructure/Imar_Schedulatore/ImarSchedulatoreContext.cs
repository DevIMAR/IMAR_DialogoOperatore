﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using IMAR_DialogoOperatore.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace IMAR_DialogoOperatore.Infrastructure.Imar_Schedulatore;

public partial class ImarSchedulatoreContext : DbContext
{
    IConfiguration _configuration;

    public ImarSchedulatoreContext(
        DbContextOptions<ImarSchedulatoreContext> options,
            IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer(_configuration["ConnectionStrings:imarSchedulatore"]);
    }

    public virtual DbSet<DATIMONITOR> DATIMONITOR { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DATIMONITOR>(entity =>
        {
            entity.HasKey(e => new { e.GIORNO, e.ODP, e.FASE }).HasName("PK_DATIMONITOR_1");

            entity.Property(e => e.GIORNO).HasColumnType("datetime");
            entity.Property(e => e.ODP)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ARTICOLO)
                .HasMaxLength(30)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CDOPERAT)
                .HasMaxLength(80)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.DATA_RIFERIMENTO).HasColumnType("datetime");
            entity.Property(e => e.DESCRIZIONE_ARTICOLO)
                .HasMaxLength(100)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.DESCRIZIONE_FASE)
                .HasMaxLength(100)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.DISPONIBILE)
                .HasMaxLength(40)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.FASE_PREC)
                .HasMaxLength(40)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.FLUSSO)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.FLUSSO_PREC)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.FLUSSO_SUC)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.GIORNO_CONSEGNA).HasColumnType("datetime");
            entity.Property(e => e.INFO)
                .HasMaxLength(80)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MATERIALE).IsUnicode(false);
            entity.Property(e => e.NOTE).IsUnicode(false);
            entity.Property(e => e.NOTE_GEN)
                .HasMaxLength(250)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ORDINAMENTO_MANUALE).IsUnicode(false);
            entity.Property(e => e.PRIORITA)
                .HasMaxLength(80)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.STATO)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.TEMPO_STO).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.T_ATT_FASE).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.T_LAV).HasColumnType("decimal(9, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}