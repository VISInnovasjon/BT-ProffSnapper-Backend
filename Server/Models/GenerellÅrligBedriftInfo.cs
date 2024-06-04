

namespace Server.Models;

public partial class GenerellÅrligBedriftInfo
{
    public int BedriftId { get; set; }

    public int Rapportår { get; set; }

    public int? AntallAnsatte { get; set; }

    public string? Landsdel { get; set; }

    public string? Fylke { get; set; }

    public string? Kommune { get; set; }

    public string? PostKode { get; set; }

    public string? PostAddresse { get; set; }

    public virtual BedriftInfo Bedrift { get; set; } = null!;
}
