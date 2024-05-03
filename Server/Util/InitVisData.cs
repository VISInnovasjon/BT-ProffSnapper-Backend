using MiniExcelLibs;

namespace Util.InitVisData;


/* Dette er ganske spesifikt for ett ekselark nå, her er det kanskje bedre å lage noen methods som har med lesing og skriving av excel ark, og så heller pjåte disse typene inn i readeren spesifikt. */
public class RawVisBedriftData
{
    public string Målbedrift { get; set; }
    public string RapportÅr { get; set; }
    public string Orgnummer { get; set; }
    public string Fase { get; set; }
    public string Kommunenr { get; set; }
    public string Kommune { get; set; }
    public string Fylke { get; set; }
    public string? Bransje { get; set; }
    public string? Idekilde { get; set; }
    public string? Etableringsdato { get; set; }
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
    public List<string> MålbedriftNavn { get; set; }
    public List<string> RapportÅr { get; set; }
    public string Orgnummer { get; set; }
    public List<string> Faser { get; set; }
    public List<string> Kommunenr { get; set; }
    public List<string> Kommune { get; set; }
    public List<string> Fylke { get; set; }
    public string? Bransje { get; set; }
    public string? Idekilde { get; set; }
    public string? Etableringsdato { get; set; }

    public static List<CompactedVisBedriftData> ListOfCompactedVisExcelSheet(List<RawVisBedriftData> data)
    {
        List<CompactedVisBedriftData> CleanData = new List<CompactedVisBedriftData>();
        for (int i = 0; i < data.Count; i++)
        {
            if (CleanData.Count > 0 && CleanData.Last().Orgnummer == data[i].Orgnummer)
            {
                CleanData.Last().MålbedriftNavn.Add(data[i].Målbedrift);
                CleanData.Last().Faser.Add(data[i].Fase);
                CleanData.Last().RapportÅr.Add(data[i].RapportÅr);
                CleanData.Last().Fylke.Add(data[i].Fylke);
                CleanData.Last().Kommune.Add(data[i].Kommune);
                CleanData.Last().Kommunenr.Add(data[i].Kommunenr);
            }
            else
            {
                var cleanExcelData = new CompactedVisBedriftData
                {
                    Bransje = data[i].Bransje,
                    Idekilde = data[i].Idekilde,
                    Orgnummer = data[i].Orgnummer,
                    Etableringsdato = data[i].Etableringsdato,
                    RapportÅr = new List<string>(),
                    MålbedriftNavn = new List<string>(),
                    Faser = new List<string>(),
                    Kommune = new List<string>(),
                    Kommunenr = new List<string>(),
                    Fylke = new List<string>()
                };
                cleanExcelData.RapportÅr.Add(data[i].RapportÅr);
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