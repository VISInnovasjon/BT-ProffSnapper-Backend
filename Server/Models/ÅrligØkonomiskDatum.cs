

namespace Server.Models;

public partial class ÅrligØkonomiskDatum
{
    public int BedriftId { get; set; }

    public int Rapportår { get; set; }

    public decimal? ØkoVerdi { get; set; }

    public string ØkoKode { get; set; } = null!;

    public decimal? Delta { get; set; }

    public virtual BedriftInfo Bedrift { get; set; } = null!;
}
