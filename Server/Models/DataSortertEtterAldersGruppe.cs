namespace Server.Models;

public class DataSortertEtterAldersGruppe
{
    public int RapportÅr { get; set; }
    public string? AldersGruppe { get; set; }
    public string? ØkoKode { get; set; }
    public string? KodeBeskrivelse { get; set; }
    public decimal AvgØkoVerdi { get; set; }
    public decimal AvgDelta { get; set; }
    public decimal AvgAkkumulert { get; set; }
}