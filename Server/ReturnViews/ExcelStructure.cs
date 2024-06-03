
using MiniExcelLibs.Attributes;
using Server.Models;


namespace Server.Views;

public class ExcelÅrsrapport
{
    [ExcelColumn(Name = "Orgnummer", Index = 0, Width = 15)]
    public string Orgnummer { get; set; }
    [ExcelColumn(Name = "Antall Ansatte", Index = 1, Width = 30)]
    public string AntallAnsatte { get; set; }
    [ExcelColumn(Name = "Driftsresultat", Index = 2, Width = 30)]
    public string DriftsResultat { get; set; }
    [ExcelColumn(Name = "Sum Driftsintekter", Index = 3, Width = 30)]
    public string SumDriftsIntekter { get; set; }
    [ExcelColumn(Name = "Sum Innskutt Egenkapital", Index = 4, Width = 30)]
    public string SumInnskuttEgenkapital { get; set; }
    [ExcelColumn(Name = "Delta Innskutt Egenkapital", Index = 5, Width = 30)]
    public string DeltaInskuttEgenkapital { get; set; }
    [ExcelColumn(Name = "Ordinært Resultat", Index = 6, Width = 30)]
    public string OrdinærtResultat { get; set; }
    [ExcelColumn(Name = "Post Addresse", Index = 7, Width = 35)]
    public string PostAddresse { get; set; }
    [ExcelColumn(Name = "Post Kode", Index = 8, Width = 15)]
    public string PostKode { get; set; }
    [ExcelColumn(Name = "Antall Shares Vis", Index = 9, Width = 30)]
    public string AntallSharesVis { get; set; }
    [ExcelColumn(Name = "Prosent Andel Shares Vis", Index = 10, Width = 30)]
    public string ProsentAndelSharesVis { get; set; }
    public static List<ExcelÅrsrapport> GetExportValues(List<Årsrapport> Data)
    {
        List<ExcelÅrsrapport> exportValues = new List<ExcelÅrsrapport>();
        foreach (var value in Data)
        {
            ExcelÅrsrapport convertedValue = new ExcelÅrsrapport()
            {
                Orgnummer = value.Orgnummer.ToString(),
                AntallAnsatte = value.AntallAnsatte != null ? value.AntallAnsatte.ToString() : "Data Mangler",
                DriftsResultat = value.DriftsResultat != null ? value.DriftsResultat.ToString() : "Data Mangler",
                SumDriftsIntekter = value.SumDriftsIntekter != null ? value.SumDriftsIntekter.ToString() : "Data Mangler",
                SumInnskuttEgenkapital = value.SumInskuttEgenkapital != null ? value.SumInskuttEgenkapital.ToString() : "Data Mangler",
                DeltaInskuttEgenkapital = value.DeltaInskuttEgenkapital != null ? value.DeltaInskuttEgenkapital.ToString() : "Data Mangler",
                OrdinærtResultat = value.OrdinærtResultat != null ? value.OrdinærtResultat.ToString() : "Data Mangler",
                PostAddresse = !string.IsNullOrEmpty(value.PostAddresse) ? value.PostAddresse : "Data Mangler",
                PostKode = !string.IsNullOrEmpty(value.PostKode) ? value.PostKode : "Data Mangler",
                AntallSharesVis = value.AntallSharesVis != null ? value.AntallSharesVis.ToString() : "Data Mangler",
                ProsentAndelSharesVis = !string.IsNullOrEmpty(value.SharesProsent) ? value.SharesProsent : "Data mangler"
            };
            exportValues.Add(convertedValue);
        }
        return exportValues;
    }
}