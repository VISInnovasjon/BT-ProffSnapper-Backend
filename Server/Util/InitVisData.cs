using MiniExcelLibs;

namespace Util.InitVisData;


/* Dette er ganske spesifikt for ett ekselark nå, her er det kanskje bedre å lage noen methods som har med lesing og skriving av excel ark, og så heller pjåte disse typene inn i readeren spesifikt. */
public class RawVisBedriftData
{
    public string RapportÅr { get; set; }
    public string Orgnummer { get; set; }
    public string Fase { get; set; }
    public string? Bransje { get; set; }
    public static List<RawVisBedriftData> ListFromVisExcelSheet(Stream stream, string excelSheetName)
    {

        var rows = stream.Query<RawVisBedriftData>(sheetName: excelSheetName).ToList();

        var ensureOrgNr = rows.Where(row => row.Orgnummer != null).ToList();
        ensureOrgNr.Sort((a, b) =>
        {
            return string.Compare(a.Orgnummer, b.Orgnummer);
        });
        return ensureOrgNr;
    }

}

public class CompactedVisBedriftData
{
    public List<string> RapportÅr { get; set; }
    public string Orgnummer { get; set; }
    public List<string> Faser { get; set; }
    public string? Bransje { get; set; }

    public static List<CompactedVisBedriftData> ListOfCompactedVisExcelSheet(List<RawVisBedriftData> data)
    {
        List<CompactedVisBedriftData> CleanData = new List<CompactedVisBedriftData>();
        for (int i = 0; i < data.Count; i++)
        {
            if (CleanData.Count > 0 && CleanData.Last().Orgnummer == data[i].Orgnummer)
            {
                CleanData.Last().Faser.Add(data[i].Fase);
                CleanData.Last().RapportÅr.Add(data[i].RapportÅr);
            }
            else
            {
                var cleanExcelData = new CompactedVisBedriftData
                {
                    Bransje = data[i].Bransje,
                    Orgnummer = data[i].Orgnummer,
                    RapportÅr = new List<string>(),
                    Faser = new List<string>(),
                };
                cleanExcelData.RapportÅr.Add(data[i].RapportÅr);
                cleanExcelData.Faser.Add(data[i].Fase);
                CleanData.Add(cleanExcelData);
            }
        }
        return CleanData;
    }

}