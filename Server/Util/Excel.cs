using MiniExcelLibs;

namespace Util.Excel;


public class ExcelInfo
{
    public string Målbedrift { get; set; }
    public string Rapportår { get; set; }
    public string Orgnummer { get; set; }
    public string Fase { get; set; }
    public string Kommunenr { get; set; }
    public string Kommune { get; set; }
    public string Fylke { get; set; }
    public string? Bransje { get; set; }
    public string? Idekilde { get; set; }
    public string? Etableringsdato { get; set; }
    public List<ExcelInfo> ListFromExcelSheet(string path, string excelSheetName)
    {
        using (var stream = File.OpenRead(path))
        {
            var rows = stream.Query<ExcelInfo>(sheetName: excelSheetName).ToList();

            var ensureOrgNr = rows.Where(row => row.Orgnummer != null).ToList();
            ensureOrgNr.Sort((a, b) =>
            {
                return string.Compare(a.Orgnummer, b.Orgnummer);
            });
            return ensureOrgNr;
        }
    }

}

public class CompactedExcelSheet
{
    public List<string> MålbedriftNavn { get; set; }
    public List<string> Rapportår { get; set; }
    public string Orgnummer { get; set; }
    public List<string> Faser { get; set; }
    public List<string> Kommunenr { get; set; }
    public List<string> Kommune { get; set; }
    public List<string> Fylke { get; set; }
    public string? Bransje { get; set; }
    public string? Idekilde { get; set; }
    public string? Etableringsdato { get; set; }

    public static List<CompactedExcelSheet> ListOfCompactedExcelSheet(List<ExcelInfo> data)
    {
        List<CompactedExcelSheet> CleanData = new List<CompactedExcelSheet>();
        for (int i = 0; i < data.Count; i++)
        {
            if (CleanData.Count > 0 && CleanData.Last().Orgnummer == data[i].Orgnummer)
            {
                CleanData.Last().MålbedriftNavn.Add(data[i].Målbedrift);
                CleanData.Last().Faser.Add(data[i].Fase);
                CleanData.Last().Rapportår.Add(data[i].Rapportår);
                CleanData.Last().Fylke.Add(data[i].Fylke);
                CleanData.Last().Kommune.Add(data[i].Kommune);
                CleanData.Last().Kommunenr.Add(data[i].Kommunenr);
            }
            else
            {
                var cleanExcelData = new CompactedExcelSheet
                {
                    Bransje = data[i].Bransje,
                    Idekilde = data[i].Idekilde,
                    Orgnummer = data[i].Orgnummer,
                    Etableringsdato = data[i].Etableringsdato,
                    Rapportår = new List<string>(),
                    MålbedriftNavn = new List<string>(),
                    Faser = new List<string>(),
                    Kommune = new List<string>(),
                    Kommunenr = new List<string>(),
                    Fylke = new List<string>()
                };
                cleanExcelData.Rapportår.Add(data[i].Rapportår);
                cleanExcelData.MålbedriftNavn.Add(data[i].Målbedrift);
                cleanExcelData.Faser.Add(data[i].Fase);
                cleanExcelData.Kommune.Add(data[i].Kommune);
                cleanExcelData.Kommunenr.Add(data[i].Kommunenr);
                cleanExcelData.Fylke.Add(data[i].Fylke);
                CleanData.Add(cleanExcelData);
            }
        }
        return CleanData;
    }

}