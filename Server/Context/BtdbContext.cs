
using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Context;

public partial class BtdbContext : DbContext
{
    public BtdbContext()
    {
    }

    public BtdbContext(DbContextOptions<BtdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CompanyInfo> CompanyInfos { get; set; }

    public virtual DbSet<CompanyAnnouncement> CompanyAnnouncements { get; set; }

    public virtual DbSet<CompanyLeaderOverview> CompanyLeaderOverviews { get; set; }

    public virtual DbSet<CompanyShareholderInfo> CompanyShareholderInfos { get; set; }

    public virtual DbSet<GeneralYearlyUpdatedCompanyInfo> GeneralYearlyUpdatedCompanyInfos { get; set; }

    public virtual DbSet<CompanyPhaseStatusOverview> CompanyPhaseStatusOverviews { get; set; }

    public virtual DbSet<CompanyEconomicDataPrYear> CompanyEconomicDataPrYears { get; set; }

    public virtual DbSet<EcoKodeLookup> EcoCodeLookups { get; set; }
    public virtual DbSet<DataSortedByLeaderAge> DataSortedByLeaderAges { get; set; }
    public virtual DbSet<DataSortedByCompanyBranch> DataSortedByCompanyBranches { get; set; }
    public virtual DbSet<DataSortedByPhase> DataSortedByPhases { get; set; }
    public virtual DbSet<AverageValues> AverageValues { get; set; }
    public virtual DbSet<FullView> FullViews { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql($"Host={Environment.GetEnvironmentVariable("DATABASE_HOST")};Username={Environment.GetEnvironmentVariable("DATABASE_USER")};Password={Environment.GetEnvironmentVariable("DATABASE_PASSWORD")};Database={Environment.GetEnvironmentVariable("DATABASE_NAME")}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CompanyInfo>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("company_info_pkey");

            entity.ToTable("company_info");

            entity.HasIndex(e => e.Orgnumber, "company_info_orgnumber_key").IsUnique();

            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("description");
            entity.Property(e => e.Branch)
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("branch");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("company_name");
            entity.Property(e => e.PrevNames)
                .HasDefaultValueSql("NULL::character varying[]")
                .HasColumnType("character varying(255)[]")
                .HasColumnName("prev_names");
            entity.Property(e => e.Liquidated).HasColumnName("liquidated").HasDefaultValue(false);
            entity.Property(e => e.FemaleEntrepreneur).HasColumnName("female_entrepreneur").HasDefaultValue(false);
            entity.Property(e => e.Orgnumber).HasColumnName("orgnumber");
        });

        modelBuilder.Entity<CompanyAnnouncement>(entity =>
        {
            entity.HasKey(e => new { e.CompanyId, e.AnnouncementId }).HasName("company_announcements_pkey");

            entity.ToTable("company_announcement");

            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.AnnouncementId).HasColumnName("announcement_id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.AnnouncementText).HasColumnName("announcement_text");
            entity.Property(e => e.AnnouncementType)
                .HasMaxLength(255)
                .HasColumnName("announcement_type");

            entity.HasOne(d => d.Company).WithMany(p => p.CompanyAnnouncements)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("company_announcement_company_id_fkey");
        });

        modelBuilder.Entity<CompanyLeaderOverview>(entity =>
        {
            entity.HasKey(e => new { e.CompanyId, e.TitleCode, e.Year }).HasName("company_leader_overview_pkey");

            entity.ToTable("company_leader_overview");

            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.TitleCode)
                .HasMaxLength(255)
                .HasColumnName("title_code");
            entity.Property(e => e.Year).HasColumnName("year");
            entity.Property(e => e.DayOfBirth).HasColumnName("day_of_birth");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Company).WithMany(p => p.CompanyLeaderOverviews)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("company_leader_overview_company_id_fkey");
        });

        modelBuilder.Entity<CompanyShareholderInfo>(entity =>
        {
            entity.HasKey(e => new { e.CompanyId, e.Year, e.Name }).HasName("company_shareholder_info_pkey");

            entity.ToTable("company_shareholder_info");

            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.Year).HasColumnName("year");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.NumberOfShares)
                .HasPrecision(12, 4)
                .HasColumnName("number_of_shares");
            entity.Property(e => e.ShareholdeCompanyId)
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("shareholder_company_id");
            entity.Property(e => e.ShareholderLastName)
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("shareholder_last_name");
            entity.Property(e => e.ShareholderFirstName)
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("shareholder_first_name");
            entity.Property(e => e.PercentageShares)
                .HasMaxLength(255)
                .HasColumnName("percentage_shares");

            entity.HasOne(d => d.Company).WithMany(p => p.CompanyShareholderInfos)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("company_shareholder_info_company_id_fkey");
        });

        modelBuilder.Entity<GeneralYearlyUpdatedCompanyInfo>(entity =>
        {
            entity.HasKey(e => new { e.CompanyId, e.Year }).HasName("general_yearly_updated_company_info_pkey");

            entity.ToTable("general_yearly_updated_company_info");

            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.Year).HasColumnName("year");
            entity.Property(e => e.NumberOfEmployees).HasColumnName("number_of_employees");
            entity.Property(e => e.County)
                .HasMaxLength(255)
                .HasColumnName("county");
            entity.Property(e => e.Municipality)
                .HasMaxLength(255)
                .HasColumnName("municipality");
            entity.Property(e => e.CountryPart)
                .HasMaxLength(255)
                .HasColumnName("country_part");
            entity.Property(e => e.AdressLine)
                .HasMaxLength(255)
                .HasColumnName("adress_line");
            entity.Property(e => e.ZipCode)
                .HasMaxLength(255)
                .HasColumnName("zip_code");

            entity.HasOne(d => d.Company).WithMany(p => p.GeneralYearlyUpdatedCompanyInfos)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("general_yearly_updated_company_info_company_id_fkey");
        });

        modelBuilder.Entity<CompanyPhaseStatusOverview>(entity =>
        {
            entity.ToTable("company_phase_status_overview");
            entity.HasKey(e => new { e.CompanyId, e.Year, e.Phase }).HasName("company_phase_status_overview_pkey");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.Year).HasColumnName("year");
            entity.Property(e => e.Phase)
                .HasMaxLength(255)
                .HasColumnName("phase");

            entity.HasOne(d => d.Company).WithMany(p => p.CompanyPhaseStatusOverviews)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("company_phase_status_overview_company_id_fkey");
        });

        modelBuilder.Entity<CompanyEconomicDataPrYear>(entity =>
        {
            entity.HasKey(e => new { e.CompanyId, e.Year, e.EcoCode }).HasName("company_economic_data_pr_year_pkey");

            entity.ToTable("company_economic_data_pr_year");

            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.Year).HasColumnName("year");
            entity.Property(e => e.EcoCode)
                .HasMaxLength(255)
                .HasColumnName("eco_code");
            entity.Property(e => e.Delta)
                .HasDefaultValueSql("(0)::numeric")
                .HasColumnName("delta");
            entity.Property(e => e.Accumulated)
                .HasDefaultValueSql("(0)::numeric")
                .HasColumnName("accumulated");
            entity.Property(e => e.EcoValue)
                .HasPrecision(16, 4)
                .HasDefaultValueSql("NULL::numeric")
                .HasColumnName("eco_value");

            entity.HasOne(d => d.Company).WithMany(p => p.CompanyEconomicDataPrYears)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("company_economic_data_pr_year_company_id_fkey");
        });

        modelBuilder.Entity<EcoKodeLookup>(entity =>
        {
            entity
                .ToTable("eco_kode_lookup")
                .HasNoKey();

            entity.Property(e => e.Nor).HasColumnName("nor");
            entity.Property(e => e.En).HasColumnName("en");
            entity.Property(e => e.EcoCode)
                .HasMaxLength(255)
                .HasColumnName("eco_code");
        });
        modelBuilder.Entity<DataSortedByLeaderAge>(entity =>
            {
                entity
                    .ToView("data_sorted_by_leader_age")
                    .HasNoKey();
                entity.Property(e => e.AgeGroup).HasColumnName("age_group");
                entity.Property(e => e.AvgDelta).HasColumnName("avg_delta");
                entity.Property(e => e.AvgEcoValue).HasColumnName("avg_eco_value");
                entity.Property(e => e.Year).HasColumnName("year");
                entity.Property(e => e.EcoCode).HasColumnName("eco_code");
                entity.Property(e => e.TotalAccumulated).HasColumnName("total_accumulated");
                entity.Property(e => e.UniqueCompanyCount).HasColumnName("unique_company_count");

            }
        );
        modelBuilder.Entity<DataSortedByCompanyBranch>(entity =>
        {
            entity
                .ToView("data_sorted_by_company_branch")
                .HasNoKey();
            entity.Property(e => e.Branch).HasColumnName("branch");
            entity.Property(e => e.AvgDelta).HasColumnName("avg_delta");
            entity.Property(e => e.AvgEcoValue).HasColumnName("avg_eco_value");
            entity.Property(e => e.Year).HasColumnName("year");
            entity.Property(e => e.EcoCode).HasColumnName("eco_code");
            entity.Property(e => e.TotalAccumulated).HasColumnName("total_accumulated");
            entity.Property(e => e.UniqueCompanyCount).HasColumnName("unique_company_count");
        });
        modelBuilder.Entity<DataSortedByPhase>(entity =>
        {
            entity
                .ToView("data_sorted_by_phase")
                .HasNoKey();
            entity.Property(e => e.Phase).HasColumnName("phase");
            entity.Property(e => e.AvgDelta).HasColumnName("avg_delta");
            entity.Property(e => e.AvgEcoValue).HasColumnName("avg_eco_value");
            entity.Property(e => e.Year).HasColumnName("year");
            entity.Property(e => e.EcoCode).HasColumnName("eco_code");
            entity.Property(e => e.TotalAccumulated).HasColumnName("total_accumulated");
            entity.Property(e => e.UniqueCompanyCount).HasColumnName("unique_company_count");
        });
        modelBuilder.Entity<AverageValues>(entity =>
        {
            entity
                .ToView("average_values")
                .HasNoKey();
            entity.Property(e => e.AvgDelta).HasColumnName("avg_delta");
            entity.Property(e => e.AvgEcoValue).HasColumnName("avg_eco_value");
            entity.Property(e => e.Year).HasColumnName("year");
            entity.Property(e => e.EcoCode).HasColumnName("eco_code");
            entity.Property(e => e.TotalAccumulated).HasColumnName("total_accumulated");
            entity.Property(e => e.UniqueCompanyCount).HasColumnName("unique_company_count");
        });
        modelBuilder.Entity<FullView>(entity =>
        {
            entity
                .ToView("full_view")
                .HasNoKey();
            entity.Property(e => e.Accumulated).HasColumnName("accumulated");
            entity.Property(e => e.AdressLine).HasColumnName("adress_line");
            entity.Property(e => e.Branch).HasColumnName("branch");
            entity.Property(e => e.CodeDescription).HasColumnName("nor");
            entity.Property(e => e.CompanyName).HasColumnName("company_name");
            entity.Property(e => e.CountryPart).HasColumnName("country_part");
            entity.Property(e => e.County).HasColumnName("county");
            entity.Property(e => e.Delta).HasColumnName("delta");
            entity.Property(e => e.EcoCode).HasColumnName("eco_code");
            entity.Property(e => e.EcoValue).HasColumnName("eco_value");
            entity.Property(e => e.Liquidated).HasColumnName("liquidated");
            entity.Property(e => e.Municipality).HasColumnName("municipality");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.NumberOfEmployees).HasColumnName("number_of_employees");
            entity.Property(e => e.Orgnumber).HasColumnName("orgnumber");
            entity.Property(e => e.Phase).HasColumnName("phase");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.TitleCode).HasColumnName("title_code");
            entity.Property(e => e.Year).HasColumnName("year");
            entity.Property(e => e.ZipCode).HasColumnName("zip_code");
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
