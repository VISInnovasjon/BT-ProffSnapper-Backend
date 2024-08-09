

namespace Server.Models;

public partial class CompanyLeaderOverview
{
    public int CompanyId { get; set; }

    public string? Title { get; set; }

    public string? Name { get; set; }

    public DateOnly? DayOfBirth { get; set; }

    public string TitleCode { get; set; } = null!;

    public int Year { get; set; }

    public virtual CompanyInfo Company { get; set; } = null!;
}
