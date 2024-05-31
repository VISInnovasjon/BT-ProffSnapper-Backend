using MiniExcelLibs;
using Server.Models;

namespace Util.InitVisData;

public class RawVisBedriftData
{
    public int RapportÅr { get; set; }
    public int Orgnummer { get; set; }
    public string Fase { get; set; }
    public string? Bransje { get; set; }
    public static async Task<List<RawVisBedriftData>> ListFromVisExcelSheet(Stream stream, string excelSheetName)
    {

        var rows = await stream.QueryAsync<RawVisBedriftData>(sheetName: excelSheetName);

        var ensureOrgNr = rows.Where(row => row.Orgnummer != 0).ToList();
        List<int> WrongOrgNr = new() { };
        foreach (var org in ensureOrgNr)
        {
            if (org.Orgnummer.ToString().Length < 9) WrongOrgNr.Add(org.Orgnummer);
        }
        if (WrongOrgNr.Count > 0)
        {
            string OrgNrs = string.Join(", ", WrongOrgNr);
            throw new ArgumentOutOfRangeException($"De følgende Orgnummere kan være skrevet feil, vennligst rett og prøv igjen: {OrgNrs}");
        }
        ensureOrgNr.Sort((a, b) => a.Orgnummer.CompareTo(b.Orgnummer));
        foreach (var orgnr in ensureOrgNr)
        {
            Console.WriteLine(orgnr.Orgnummer);
        }
        return ensureOrgNr;
    }

}

public class CompactedVisBedriftData
{
    public List<int> RapportÅr { get; set; }
    public int Orgnummer { get; set; }
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
                    RapportÅr = new List<int>(),
                    Faser = new List<string>(),
                };
                cleanExcelData.RapportÅr.Add(data[i].RapportÅr);
                cleanExcelData.Faser.Add(data[i].Fase);
                CleanData.Add(cleanExcelData);
            }
        }
        return CleanData;
    }
    public static void AddListToDb(List<CompactedVisBedriftData> data)
    {
        foreach (var company in data)
        {
            using (var context = new BtdbContext())
            {
                var companyData = new BedriftInfo
                {
                    Orgnummer = company.Orgnummer,
                    Bransje = company.Bransje
                };
                context.BedriftInfos.Add(companyData);
                context.SaveChanges();
                var bedriftId = companyData.BedriftId;
                for (int i = 0; i < company.RapportÅr.Count; i++)
                {
                    var faseData = new OversiktBedriftFaseStatus
                    {
                        Fase = company.Faser[i],
                        Rapportår = company.RapportÅr[i],
                        BedriftId = bedriftId
                    };
                    context.OversiktBedriftFaseStatuses.Add(faseData);
                    context.SaveChanges();
                }

            }
        }
    }
    public static List<int> GetOrgNrArray(List<CompactedVisBedriftData> data)
    {
        List<int> orgNrArray = new();
        foreach (var compactData in data)
        {
            orgNrArray.Add(compactData.Orgnummer);
        }
        return orgNrArray;
    }
}