

namespace Server.Models;

public partial class CompanyEconomicDataPrYear
{
    public int CompanyId { get; set; }

    public int Year { get; set; }

    public decimal? EcoValue { get; set; }

    public string EcoCode { get; set; } = null!;

    public decimal? Delta { get; set; }
    public decimal? Accumulated { get; set; }

    public virtual CompanyInfo Company { get; set; } = null!;
}
