namespace Server.Models;

public class Årsrapport
{
    public int Orgnummer { get; set; }
    public int? AntallAnsatte { get; set; }
    public decimal? DriftsResultat { get; set; }
    public decimal? SumDriftsIntekter { get; set; }
    public decimal? SumInskuttEgenkapital { get; set; }
    public decimal? DeltaInskuttEgenkapital { get; set; }
    public decimal? OrdinærtResultat { get; set; }
    public string? PostAddresse { get; set; }
    public string? PostKode { get; set; }
    public decimal? AntallSharesVis { get; set; }
    public string? SharesProsent { get; set; }
}