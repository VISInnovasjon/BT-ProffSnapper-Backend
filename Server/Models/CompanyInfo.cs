

namespace Server.Models;

public partial class CompanyInfo
{
    public int CompanyId { get; set; }

    public int Orgnumber { get; set; }

    public string? CompanyName { get; set; }

    public string? Branch { get; set; }

    public string? Description { get; set; }
    public bool? FemaleEntrepreneur { get; set; }
    public bool? Liquidated { get; set; }

    public List<string>? PrevNames { get; set; }

    public virtual ICollection<CompanyAnnouncement> CompanyAnnouncements { get; set; } = new List<CompanyAnnouncement>();

    public virtual ICollection<CompanyLeaderOverview> CompanyLeaderOverviews { get; set; } = new List<CompanyLeaderOverview>();

    public virtual ICollection<CompanyShareholderInfo> CompanyShareholderInfos { get; set; } = new List<CompanyShareholderInfo>();

    public virtual ICollection<GeneralYearlyUpdatedCompanyInfo> GeneralYearlyUpdatedCompanyInfos { get; set; } = new List<GeneralYearlyUpdatedCompanyInfo>();

    public virtual ICollection<CompanyPhaseStatusOverview> CompanyPhaseStatusOverviews { get; set; } = new List<CompanyPhaseStatusOverview>();

    public virtual ICollection<CompanyEconomicDataPrYear> CompanyEconomicDataPrYears { get; set; } = new List<CompanyEconomicDataPrYear>();

}
