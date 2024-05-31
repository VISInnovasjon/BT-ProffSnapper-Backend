

namespace Server.Models;

public partial class BedriftKunngjøringer
{
    public int BedriftId { get; set; }

    public long KunngjøringId { get; set; }

    public DateOnly? Dato { get; set; }

    public string? Kunngjøringstekst { get; set; }

    public string? Kunngjøringstype { get; set; }

    public virtual BedriftInfo Bedrift { get; set; } = null!;
}
