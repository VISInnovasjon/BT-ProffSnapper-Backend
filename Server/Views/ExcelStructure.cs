
using MiniExcelLibs.Attributes;
using Server.Models;


namespace Server.Views;

public class ExcelÅrsrapport
{
    [ExcelColumn(Name = "Orgnummer", Index = 0, Width = 15)]
    public int Orgnummer { get; set; }
    [ExcelColumn(Name = "Målbedrift", Index = 1, Width = 30)]
    public string? Målbedrift { get; set; }
    [ExcelColumn(Name = "Antall Ansatte", Index = 2, Width = 30)]
    public int? AntallAnsatte { get; set; }
    [ExcelColumn(Name = "Rapportår", Index = 3, Width = 15)]
    public int? Rapportår { get; set; }
    [ExcelColumn(Name = "Driftsresultat", Index = 4, Width = 30)]
    public decimal? DriftsResultat { get; set; }
    [ExcelColumn(Name = "Sum Driftsintekter", Index = 5, Width = 30)]
    public decimal? SumDriftsIntekter { get; set; }
    [ExcelColumn(Name = "Sum Innskutt Egenkapital", Index = 6, Width = 30)]
    public decimal? SumInnskuttEgenkapital { get; set; }
    [ExcelColumn(Name = "Delta Innskutt Egenkapital", Index = 7, Width = 30)]
    public decimal? DeltaInskuttEgenkapital { get; set; }
    [ExcelColumn(Name = "Ordinært Resultat", Index = 8, Width = 30)]
    public decimal? OrdinærtResultat { get; set; }
    [ExcelColumn(Name = "Post Addresse", Index = 9, Width = 35)]
    public required string PostAddresse { get; set; }
    [ExcelColumn(Name = "Post Kode", Index = 10, Width = 15)]
    public required string PostKode { get; set; }
    [ExcelColumn(Name = "Antall Shares Vis", Index = 11, Width = 30)]
    public decimal? AntallSharesVis { get; set; }
    [ExcelColumn(Name = "Prosent Andel Shares Vis", Index = 12, Width = 30)]
    public required string ProsentAndelSharesVis { get; set; }
    public static List<ExcelÅrsrapport> GetExportValues(List<Årsrapport> Data)
    {
        List<ExcelÅrsrapport> exportValues = new List<ExcelÅrsrapport>();
        foreach (var value in Data)
        {
            ExcelÅrsrapport convertedValue = new ExcelÅrsrapport()
            {
                Orgnummer = value.Orgnummer,
                Målbedrift = value.Målbedrift,
                AntallAnsatte = value.AntallAnsatte,
                Rapportår = value.Rapportår,
                DriftsResultat = value.DriftsResultat,
                SumDriftsIntekter = value.SumDriftsIntekter,
                SumInnskuttEgenkapital = value.SumInskuttEgenkapital,
                DeltaInskuttEgenkapital = value.DeltaInskuttEgenkapital,
                OrdinærtResultat = value.OrdinærtResultat,
                PostAddresse = !string.IsNullOrEmpty(value.PostAddresse) ? value.PostAddresse : "Data Mangler",
                PostKode = !string.IsNullOrEmpty(value.PostKode) ? value.PostKode : "Data Mangler",
                AntallSharesVis = value.AntallSharesVis,
                ProsentAndelSharesVis = !string.IsNullOrEmpty(value.SharesProsent) ? value.SharesProsent : "Data mangler"
            };
            exportValues.Add(convertedValue);
        }
        return exportValues;
    }
}

public class ExcelOrgNrOnly
{
    public int Orgnummer { get; set; }
}