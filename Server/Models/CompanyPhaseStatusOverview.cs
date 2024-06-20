

using Microsoft.EntityFrameworkCore;

namespace Server.Models;
public partial class CompanyPhaseStatusOverview
{
    public int CompanyId { get; set; }

    public int Year { get; set; }

    public string Phase { get; set; } = null!;

    public virtual CompanyInfo Company { get; set; } = null!;
}
