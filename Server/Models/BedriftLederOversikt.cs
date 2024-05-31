

namespace Server.Models;

public partial class BedriftLederOversikt
{
    public int BedriftId { get; set; }

    public string? Tittel { get; set; }

    public string? Navn { get; set; }

    public DateOnly? Fødselsdag { get; set; }

    public string Tittelkode { get; set; } = null!;

    public int Rapportår { get; set; }

    public virtual BedriftInfo Bedrift { get; set; } = null!;
}
