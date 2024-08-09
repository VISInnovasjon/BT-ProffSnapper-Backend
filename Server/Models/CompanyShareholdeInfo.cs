

namespace Server.Models;

public partial class CompanyShareholderInfo
{
    public int CompanyId { get; set; }

    public int Year { get; set; }

    public decimal? NumberOfShares { get; set; }

    public string? ShareholdeCompanyId { get; set; }

    public string? ShareholderFirstName { get; set; }

    public string? PercentageShares { get; set; }

    public string Name { get; set; } = null!;

    public string? ShareholderLastName { get; set; }

    public virtual CompanyInfo Company { get; set; } = null!;
}
