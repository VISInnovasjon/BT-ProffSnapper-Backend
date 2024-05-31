

namespace Server.Models;

public partial class BedriftShareholderInfo
{
    public int BedriftId { get; set; }

    public int Rapportår { get; set; }

    public decimal? AntalShares { get; set; }

    public string? ShareholderBedriftId { get; set; }

    public string? ShareholderFornavn { get; set; }

    public string? Sharetype { get; set; }

    public string Navn { get; set; } = null!;

    public string? ShareholderEtternavn { get; set; }

    public virtual BedriftInfo Bedrift { get; set; } = null!;
}
