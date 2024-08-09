

namespace Server.Models;

public partial class GeneralYearlyUpdatedCompanyInfo
{
    public int CompanyId { get; set; }

    public int Year { get; set; }

    public int? NumberOfEmployees { get; set; }

    public string? CountryPart { get; set; }

    public string? County { get; set; }

    public string? Municipality { get; set; }

    public string? ZipCode { get; set; }

    public string? AdressLine { get; set; }

    public virtual CompanyInfo Company { get; set; } = null!;
}
