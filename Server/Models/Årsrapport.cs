namespace Server.Models;

public class Årsrapport
{
    public int Orgnummer { get; set; }
    public string? Målbedrift { get; set; }
    public int? AntallAnsatte { get; set; }
    public int? Rapportår { get; set; }
    public decimal? DriftsResultat { get; set; }
    public decimal? SumDriftsIntekter { get; set; }
    public decimal? SumEgenkapital { get; set; }
    public decimal? SumInskuttEgenkapital { get; set; }
    public decimal? DeltaInskuttEgenkapital { get; set; }
    public decimal? OrdinærtResultat { get; set; }
    public decimal? LønnTrygdPensjon { get; set; }
    public string? PostAddresse { get; set; }
    public string? PostKode { get; set; }
    public decimal? AntallSharesVis { get; set; }
    public string? SharesProsent { get; set; }
}