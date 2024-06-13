using MiniExcelLibs;
using Microsoft.EntityFrameworkCore;
namespace Server.Models;

public class RawVisBedriftData
{
    public int RapportÅr { get; set; }
    public int Orgnummer { get; set; }
    public string? Fase { get; set; }
    public string? Bransje { get; set; }
    public int KvinneligGrunder { get; set; }
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
    public required List<int> RapportÅr { get; set; }
    public int Orgnummer { get; set; }
    public required List<string> Faser { get; set; }
    public string? Bransje { get; set; }
    public int KvinneligGrunder { get; set; }

    public static List<CompactedVisBedriftData> ListOfCompactedVisExcelSheet(List<RawVisBedriftData> data)
    {
        List<CompactedVisBedriftData> CleanData = [];
        for (int i = 0; i < data.Count; i++)
        {
            if (CleanData.Count > 0 && CleanData.Last().Orgnummer == data[i].Orgnummer)
            {
                CleanData.Last().Faser.Add(data[i].Fase ?? "Fase Missing");
                CleanData.Last().RapportÅr.Add(data[i].RapportÅr);
            }
            else
            {
                var cleanExcelData = new CompactedVisBedriftData
                {
                    Bransje = data[i].Bransje,
                    Orgnummer = data[i].Orgnummer,
                    KvinneligGrunder = data[i].KvinneligGrunder,
                    RapportÅr = [data[i].RapportÅr],
                    Faser = [data[i].Fase ?? "Fase Missing"],
                };
                CleanData.Add(cleanExcelData);
            }
        }
        return CleanData;
    }
    public static void AddListToDb(List<CompactedVisBedriftData> data, BtdbContext context)
    {
        foreach (var company in data)
        {
            try
            {
                var companyData = new BedriftInfo
                {
                    Orgnummer = company.Orgnummer,
                    Bransje = company.Bransje,
                    KvinneligGrunder = company.KvinneligGrunder == 1
                };

                context.BedriftInfos.Add(companyData);
                context.SaveChanges();
                var bedriftId = companyData.BedriftId;
                /* extracter siste verdien for hvert år. Letter å gjøre her hvor de allerede er paired med en bedrift. */
                var årFaseOversikt = new Dictionary<int, string>();
                for (int i = 0; i < company.RapportÅr.Count; i++)
                {
                    årFaseOversikt[company.RapportÅr[i]] = company.Faser[i];
                }
                foreach (var pair in årFaseOversikt)
                {
                    var faseData = new OversiktBedriftFaseStatus
                    {
                        Fase = pair.Value,
                        Rapportår = pair.Key,
                        BedriftId = bedriftId
                    };
                    context.OversiktBedriftFaseStatuses.Add(faseData);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
    public static void UpdateFaseStatus(List<CompactedVisBedriftData> data, BtdbContext context)
    {
        foreach (var company in data)
        {
            var bedriftId = context.BedriftInfos.Single(b => b.Orgnummer == company.Orgnummer).BedriftId;
            /* extracter siste verdien for hvert år. Letter å gjøre her hvor de allerede er paired med en bedrift. */
            var årFaseOversikt = new Dictionary<int, string>();
            for (int i = 0; i < company.RapportÅr.Count; i++)
            {
                årFaseOversikt[company.RapportÅr[i]] = company.Faser[i];
            }
            foreach (var pair in årFaseOversikt)
            {
                var faseData = new OversiktBedriftFaseStatus
                {
                    Fase = pair.Value,
                    Rapportår = pair.Key,
                    BedriftId = bedriftId
                };
                if (!context.OversiktBedriftFaseStatuses.Any(b => b.Rapportår == faseData.Rapportår && b.BedriftId == faseData.BedriftId && b.Fase == faseData.Fase)) context.OversiktBedriftFaseStatuses.Add(faseData);
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