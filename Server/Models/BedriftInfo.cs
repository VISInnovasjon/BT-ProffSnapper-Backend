

namespace Server.Models;

public partial class BedriftInfo
{
    public int BedriftId { get; set; }

    public int Orgnummer { get; set; }

    public string? Målbedrift { get; set; }

    public string? Bransje { get; set; }

    public string? Beskrivelse { get; set; }
    public bool? KvinneligGrunder { get; set; }
    public bool? Likvidert { get; set; }

    public List<string>? Navneliste { get; set; }

    public virtual ICollection<BedriftKunngjøringer> BedriftKunngjøringers { get; set; } = new List<BedriftKunngjøringer>();

    public virtual ICollection<BedriftLederOversikt> BedriftLederOversikts { get; set; } = new List<BedriftLederOversikt>();

    public virtual ICollection<BedriftShareholderInfo> BedriftShareholderInfos { get; set; } = new List<BedriftShareholderInfo>();

    public virtual ICollection<GenerellÅrligBedriftInfo> GenerellÅrligBedriftInfos { get; set; } = new List<GenerellÅrligBedriftInfo>();

    public virtual ICollection<OversiktBedriftFaseStatus> OversiktBedriftFaseStatuses { get; set; } = new List<OversiktBedriftFaseStatus>();

    public virtual ICollection<ÅrligØkonomiskDatum> ÅrligØkonomiskData { get; set; } = new List<ÅrligØkonomiskDatum>();
}
