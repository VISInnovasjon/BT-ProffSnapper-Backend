namespace Server.Models;

public class GjennomsnittVerdier
{
    public int RapportÅr { get; set; }
    public string? ØkoKode { get; set; }
    public string? KodeBeskrivelse { get; set; }
    public decimal AvgØkoVerdi { get; set; }
    public decimal AvgDelta { get; set; }
}