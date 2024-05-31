

namespace Server.Models;

public partial class OversiktBedriftFaseStatus
{
    public int BedriftId { get; set; }

    public int Rapportår { get; set; }

    public string Fase { get; set; } = null!;

    public virtual BedriftInfo Bedrift { get; set; } = null!;
}
