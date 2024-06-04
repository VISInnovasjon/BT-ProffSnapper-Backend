
using MiniExcelLibs.Attributes;
using Server.Models;


namespace Server.Views;

public class ExcelÅrsrapport
{
    [ExcelColumn(Name = "Orgnummer", Index = 0, Width = 15)]
    public required string Orgnummer { get; set; }
    [ExcelColumn(Name = "Antall Ansatte", Index = 1, Width = 30)]
    public required string AntallAnsatte { get; set; }
    [ExcelColumn(Name = "Driftsresultat", Index = 2, Width = 30)]
    public required string DriftsResultat { get; set; }
    [ExcelColumn(Name = "Sum Driftsintekter", Index = 3, Width = 30)]
    public required string SumDriftsIntekter { get; set; }
    [ExcelColumn(Name = "Sum Innskutt Egenkapital", Index = 4, Width = 30)]
    public required string SumInnskuttEgenkapital { get; set; }
    [ExcelColumn(Name = "Delta Innskutt Egenkapital", Index = 5, Width = 30)]
    public required string DeltaInskuttEgenkapital { get; set; }
    [ExcelColumn(Name = "Ordinært Resultat", Index = 6, Width = 30)]
    public required string OrdinærtResultat { get; set; }
    [ExcelColumn(Name = "Post Addresse", Index = 7, Width = 35)]
    public required string PostAddresse { get; set; }
    [ExcelColumn(Name = "Post Kode", Index = 8, Width = 15)]
    public required string PostKode { get; set; }
    [ExcelColumn(Name = "Antall Shares Vis", Index = 9, Width = 30)]
    public required string AntallSharesVis { get; set; }
    [ExcelColumn(Name = "Prosent Andel Shares Vis", Index = 10, Width = 30)]
    public required string ProsentAndelSharesVis { get; set; }
    public static List<ExcelÅrsrapport> GetExportValues(List<Årsrapport> Data)
    {
        List<ExcelÅrsrapport> exportValues = new List<ExcelÅrsrapport>();
        foreach (var value in Data)
        {
            ExcelÅrsrapport convertedValue = new ExcelÅrsrapport()
            {
                Orgnummer = value.Orgnummer.ToString(),
                AntallAnsatte = value.AntallAnsatte.ToString() ?? "Data Mangler",
                DriftsResultat = value.DriftsResultat.ToString() ?? "Data Mangler",
                SumDriftsIntekter = value.SumDriftsIntekter.ToString() ?? "Data Mangler",
                SumInnskuttEgenkapital = value.SumInskuttEgenkapital.ToString() ?? "Data Mangler",
                DeltaInskuttEgenkapital = value.DeltaInskuttEgenkapital.ToString() ?? "Data Mangler",
                OrdinærtResultat = value.OrdinærtResultat.ToString() ?? "Data Mangler",
                PostAddresse = !string.IsNullOrEmpty(value.PostAddresse) ? value.PostAddresse : "Data Mangler",
                PostKode = !string.IsNullOrEmpty(value.PostKode) ? value.PostKode : "Data Mangler",
                AntallSharesVis = value.AntallSharesVis.ToString() ?? "Data Mangler",
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