
using Microsoft.EntityFrameworkCore;

namespace Server.Models;

public partial class BtdbContext : DbContext
{
    public BtdbContext()
    {
    }

    public BtdbContext(DbContextOptions<BtdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BedriftInfo> BedriftInfos { get; set; }

    public virtual DbSet<BedriftKunngjøringer> BedriftKunngjøringers { get; set; }

    public virtual DbSet<BedriftLederOversikt> BedriftLederOversikts { get; set; }

    public virtual DbSet<BedriftShareholderInfo> BedriftShareholderInfos { get; set; }

    public virtual DbSet<GenerellÅrligBedriftInfo> GenerellÅrligBedriftInfos { get; set; }

    public virtual DbSet<OversiktBedriftFaseStatus> OversiktBedriftFaseStatuses { get; set; }

    public virtual DbSet<ÅrligØkonomiskDatum> ÅrligØkonomiskData { get; set; }

    public virtual DbSet<ØkoKodeLookup> ØkoKodeLookups { get; set; }
    public virtual DbSet<DataSortertEtterAldersGruppe> DataSortertEtterAldersGruppes { get; set; }
    public virtual DbSet<DataSortertEtterBransje> DataSortertEtterBransjes { get; set; }
    public virtual DbSet<DataSortertEtterFase> DataSortertEtterFases { get; set; }
    public virtual DbSet<GjennomsnittVerdier> GjennomsnittVerdiers { get; set; }
    public virtual DbSet<Årsrapport> Årsrapports { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql($"Host={Environment.GetEnvironmentVariable("DATABASE_HOST")};Username={Environment.GetEnvironmentVariable("DATABASE_USER")};Password={Environment.GetEnvironmentVariable("DATABASE_PASSWORD")};Database={Environment.GetEnvironmentVariable("DATABASE_NAME")}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BedriftInfo>(entity =>
        {
            entity.HasKey(e => e.BedriftId).HasName("bedrift_info_pkey");

            entity.ToTable("bedrift_info");

            entity.HasIndex(e => e.Orgnummer, "bedrift_info_orgnummer_key").IsUnique();

            entity.Property(e => e.BedriftId).HasColumnName("bedrift_id");
            entity.Property(e => e.Beskrivelse)
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("beskrivelse");
            entity.Property(e => e.Bransje)
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("bransje");
            entity.Property(e => e.Målbedrift)
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("målbedrift");
            entity.Property(e => e.Navneliste)
                .HasDefaultValueSql("NULL::character varying[]")
                .HasColumnType("character varying(255)[]")
                .HasColumnName("navneliste");
            entity.Property(e => e.KvinneligGrunder).HasColumnName("kvinnelig_grunder").HasDefaultValue(false);
            entity.Property(e => e.Orgnummer).HasColumnName("orgnummer");
        });

        modelBuilder.Entity<BedriftKunngjøringer>(entity =>
        {
            entity.HasKey(e => new { e.BedriftId, e.KunngjøringId }).HasName("bedrift_kunngjøringer_pkey");

            entity.ToTable("bedrift_kunngjøringer");

            entity.Property(e => e.BedriftId).HasColumnName("bedrift_id");
            entity.Property(e => e.KunngjøringId).HasColumnName("kunngjøring_id");
            entity.Property(e => e.Dato).HasColumnName("dato");
            entity.Property(e => e.Kunngjøringstekst).HasColumnName("kunngjøringstekst");
            entity.Property(e => e.Kunngjøringstype)
                .HasMaxLength(255)
                .HasColumnName("kunngjøringstype");

            entity.HasOne(d => d.Bedrift).WithMany(p => p.BedriftKunngjøringers)
                .HasForeignKey(d => d.BedriftId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("bedrift_kunngjøringer_bedrift_id_fkey");
        });

        modelBuilder.Entity<BedriftLederOversikt>(entity =>
        {
            entity.HasKey(e => new { e.BedriftId, e.Tittelkode, e.Rapportår }).HasName("bedrift_leder_oversikt_pkey");

            entity.ToTable("bedrift_leder_oversikt");

            entity.Property(e => e.BedriftId).HasColumnName("bedrift_id");
            entity.Property(e => e.Tittelkode)
                .HasMaxLength(255)
                .HasColumnName("tittelkode");
            entity.Property(e => e.Rapportår).HasColumnName("rapportår");
            entity.Property(e => e.Fødselsdag).HasColumnName("fødselsdag");
            entity.Property(e => e.Navn)
                .HasMaxLength(255)
                .HasColumnName("navn");
            entity.Property(e => e.Tittel)
                .HasMaxLength(255)
                .HasColumnName("tittel");

            entity.HasOne(d => d.Bedrift).WithMany(p => p.BedriftLederOversikts)
                .HasForeignKey(d => d.BedriftId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("bedrift_leder_oversikt_bedrift_id_fkey");
        });

        modelBuilder.Entity<BedriftShareholderInfo>(entity =>
        {
            entity.HasKey(e => new { e.BedriftId, e.Rapportår, e.Navn }).HasName("bedrift_shareholder_info_pkey");

            entity.ToTable("bedrift_shareholder_info");

            entity.Property(e => e.BedriftId).HasColumnName("bedrift_id");
            entity.Property(e => e.Rapportår).HasColumnName("rapportår");
            entity.Property(e => e.Navn)
                .HasMaxLength(255)
                .HasColumnName("navn");
            entity.Property(e => e.AntalShares)
                .HasPrecision(12, 4)
                .HasColumnName("antal_shares");
            entity.Property(e => e.ShareholderBedriftId)
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("shareholder_bedrift_id");
            entity.Property(e => e.ShareholderEtternavn)
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("shareholder_etternavn");
            entity.Property(e => e.ShareholderFornavn)
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("shareholder_fornavn");
            entity.Property(e => e.Sharetype)
                .HasMaxLength(255)
                .HasColumnName("sharetype");

            entity.HasOne(d => d.Bedrift).WithMany(p => p.BedriftShareholderInfos)
                .HasForeignKey(d => d.BedriftId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("bedrift_shareholder_info_bedrift_id_fkey");
        });

        modelBuilder.Entity<GenerellÅrligBedriftInfo>(entity =>
        {
            entity.HasKey(e => new { e.BedriftId, e.Rapportår }).HasName("generell_årlig_bedrift_info_pkey");

            entity.ToTable("generell_årlig_bedrift_info");

            entity.Property(e => e.BedriftId).HasColumnName("bedrift_id");
            entity.Property(e => e.Rapportår).HasColumnName("rapportår");
            entity.Property(e => e.AntallAnsatte).HasColumnName("antall_ansatte");
            entity.Property(e => e.Fylke)
                .HasMaxLength(255)
                .HasColumnName("fylke");
            entity.Property(e => e.Kommune)
                .HasMaxLength(255)
                .HasColumnName("kommune");
            entity.Property(e => e.Landsdel)
                .HasMaxLength(255)
                .HasColumnName("landsdel");
            entity.Property(e => e.PostAddresse)
                .HasMaxLength(255)
                .HasColumnName("post_addresse");
            entity.Property(e => e.PostKode)
                .HasMaxLength(255)
                .HasColumnName("post_kode");

            entity.HasOne(d => d.Bedrift).WithMany(p => p.GenerellÅrligBedriftInfos)
                .HasForeignKey(d => d.BedriftId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("generell_årlig_bedrift_info_bedrift_id_fkey");
        });

        modelBuilder.Entity<OversiktBedriftFaseStatus>(entity =>
        {
            entity.HasKey(e => new { e.BedriftId, e.Rapportår, e.Fase }).HasName("oversikt_bedrift_fase_lokasjon_pr_år_pkey");

            entity.ToTable("oversikt_bedrift_fase_status");

            entity.Property(e => e.BedriftId).HasColumnName("bedrift_id");
            entity.Property(e => e.Rapportår).HasColumnName("rapportår");
            entity.Property(e => e.Fase)
                .HasMaxLength(255)
                .HasColumnName("fase");

            entity.HasOne(d => d.Bedrift).WithMany(p => p.OversiktBedriftFaseStatuses)
                .HasForeignKey(d => d.BedriftId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("oversikt_bedrift_fase_lokasjon_pr_år_bedrift_id_fkey");
        });

        modelBuilder.Entity<ÅrligØkonomiskDatum>(entity =>
        {
            entity.HasKey(e => new { e.BedriftId, e.Rapportår, e.ØkoKode }).HasName("årlig_økonomisk_data_pkey");

            entity.ToTable("årlig_økonomisk_data");

            entity.Property(e => e.BedriftId).HasColumnName("bedrift_id");
            entity.Property(e => e.Rapportår).HasColumnName("rapportår");
            entity.Property(e => e.ØkoKode)
                .HasMaxLength(255)
                .HasColumnName("øko_kode");
            entity.Property(e => e.Delta)
                .HasDefaultValueSql("(0)::numeric")
                .HasColumnName("delta");
            entity.Property(e => e.ØkoVerdi)
                .HasPrecision(16, 4)
                .HasDefaultValueSql("NULL::numeric")
                .HasColumnName("øko_verdi");

            entity.HasOne(d => d.Bedrift).WithMany(p => p.ÅrligØkonomiskData)
                .HasForeignKey(d => d.BedriftId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("årlig_økonomisk_data_bedrift_id_fkey");
        });

        modelBuilder.Entity<ØkoKodeLookup>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("øko_kode_lookup");

            entity.Property(e => e.KodeBeskrivelse).HasColumnName("kode_beskrivelse");
            entity.Property(e => e.ØkoKode)
                .HasMaxLength(255)
                .HasColumnName("øko_kode");
        });
        modelBuilder.Entity<DataSortertEtterAldersGruppe>(entity =>
            {
                entity
                    .ToView("data_sortert_etter_aldersgruppe")
                    .HasNoKey();
                entity.Property(e => e.AldersGruppe).HasColumnName("alders_gruppe");
                entity.Property(e => e.AvgDelta).HasColumnName("avg_delta");
                entity.Property(e => e.AvgØkoVerdi).HasColumnName("avg_øko_verdi");
                entity.Property(e => e.KodeBeskrivelse).HasColumnName("kode_beskrivelse");
                entity.Property(e => e.RapportÅr).HasColumnName("rapportår");
                entity.Property(e => e.ØkoKode).HasColumnName("øko_kode");

            }
        );
        modelBuilder.Entity<DataSortertEtterBransje>(entity =>
        {
            entity
                .ToView("data_sortert_etter_bransje")
                .HasNoKey();
            entity.Property(e => e.Bransje).HasColumnName("bransje");
            entity.Property(e => e.AvgDelta).HasColumnName("avg_delta");
            entity.Property(e => e.AvgØkoVerdi).HasColumnName("avg_øko_verdi");
            entity.Property(e => e.KodeBeskrivelse).HasColumnName("kode_beskrivelse");
            entity.Property(e => e.RapportÅr).HasColumnName("rapportår");
            entity.Property(e => e.ØkoKode).HasColumnName("øko_kode");
        });
        modelBuilder.Entity<DataSortertEtterFase>(entity =>
        {
            entity
                .ToView("data_sortert_etter_fase")
                .HasNoKey();
            entity.Property(e => e.Fase).HasColumnName("fase");
            entity.Property(e => e.AvgDelta).HasColumnName("avg_delta");
            entity.Property(e => e.AvgØkoVerdi).HasColumnName("avg_øko_verdi");
            entity.Property(e => e.KodeBeskrivelse).HasColumnName("kode_beskrivelse");
            entity.Property(e => e.RapportÅr).HasColumnName("rapportår");
            entity.Property(e => e.ØkoKode).HasColumnName("øko_kode");
        });
        modelBuilder.Entity<GjennomsnittVerdier>(entity =>
        {
            entity
                .ToView("gjennomsnitt_verdier")
                .HasNoKey();
            entity.Property(e => e.AvgDelta).HasColumnName("avg_delta");
            entity.Property(e => e.AvgØkoVerdi).HasColumnName("avg_øko_verdi");
            entity.Property(e => e.KodeBeskrivelse).HasColumnName("kode_beskrivelse");
            entity.Property(e => e.RapportÅr).HasColumnName("rapportår");
            entity.Property(e => e.ØkoKode).HasColumnName("øko_kode");
        });
        modelBuilder.Entity<Årsrapport>(entity =>
        {
            entity
                .ToView("årsrapport")
                .HasNoKey();
            entity.Property(e => e.AntallAnsatte).HasColumnName("antall_ansatte");
            entity.Property(e => e.AntallSharesVis).HasColumnName("antall_shares_vis");
            entity.Property(e => e.DeltaInskuttEgenkapital).HasColumnName("delta_innskutt_egenkapital");
            entity.Property(e => e.DriftsResultat).HasColumnName("driftsresultat");
            entity.Property(e => e.LønnTrygdPensjon).HasColumnName("lønn_trygd_pensjon");
            entity.Property(e => e.SumEgenkapital).HasColumnName("sum_egenkapital");
            entity.Property(e => e.OrdinærtResultat).HasColumnName("ordinært_resultat");
            entity.Property(e => e.Orgnummer).HasColumnName("orgnummer");
            entity.Property(e => e.PostAddresse).HasColumnName("post_addresse");
            entity.Property(e => e.PostKode).HasColumnName("post_kode");
            entity.Property(e => e.SharesProsent).HasColumnName("shares_prosent");
            entity.Property(e => e.SumDriftsIntekter).HasColumnName("sum_drifts_intekter");
            entity.Property(e => e.SumInskuttEgenkapital).HasColumnName("sum_innskutt_egenkapital");
            entity.Property(e => e.Målbedrift).HasColumnName("målbedrift");
            entity.Property(e => e.Rapportår).HasColumnName("rapportår");
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
