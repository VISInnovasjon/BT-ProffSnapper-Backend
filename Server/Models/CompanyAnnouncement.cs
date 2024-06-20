

namespace Server.Models;

public partial class CompanyAnnouncement
{
    public int CompanyId { get; set; }

    public long AnnouncementId { get; set; }

    public DateOnly? Date { get; set; }

    public string? AnnouncementText { get; set; }

    public string? AnnouncementType { get; set; }

    public virtual CompanyInfo Company { get; set; } = null!;
}
