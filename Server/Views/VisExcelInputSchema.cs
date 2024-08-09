using MiniExcelLibs;
using Server.Context;
using Server.Controllers;
using Server.Models;
namespace Server.Views;

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
        List<int> WrongOrgNr = [];
        foreach (var org in ensureOrgNr)
        {
            if (org.Orgnummer.ToString().Length < 9 || org.Orgnummer.ToString().Length > 9) WrongOrgNr.Add(org.Orgnummer);
        }
        if (WrongOrgNr.Count > 0)
        {
            string OrgNrs = string.Join(", ", WrongOrgNr);
            throw new ArgumentOutOfRangeException(GlobalLanguage.Language switch
            {
                "nor" => $"De følgende Orgnummere kan være skrevet feil, vennligst rett og prøv igjen: {OrgNrs}",
                "en" => $"The following orgnumbers can be miswritten, please verify and try again {OrgNrs}",
                _ => "Server Error"
            });
        }
        ensureOrgNr.Sort((a, b) => a.Orgnummer.CompareTo(b.Orgnummer));
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
                var companyData = new CompanyInfo
                {
                    Orgnumber = company.Orgnummer,
                    Branch = company.Bransje,
                    FemaleEntrepreneur = company.KvinneligGrunder == 1
                };

                context.CompanyInfos.Add(companyData);
                context.SaveChanges();
                var companyId = companyData.CompanyId;
                /* extracter siste verdien for hvert år. Letter å gjøre her hvor de allerede er paired med en bedrift. */
                var årFaseOversikt = new Dictionary<int, string>();
                for (int i = 0; i < company.RapportÅr.Count; i++)
                {
                    årFaseOversikt[company.RapportÅr[i]] = company.Faser[i];
                }
                foreach (var pair in årFaseOversikt)
                {
                    var faseData = new CompanyPhaseStatusOverview
                    {
                        Phase = pair.Value,
                        Year = pair.Key,
                        CompanyId = companyId
                    };
                    context.CompanyPhaseStatusOverviews.Add(faseData);
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
            var companyId = context.CompanyInfos.Single(b => b.Orgnumber == company.Orgnummer).CompanyId;
            /* extracter siste verdien for hvert år. Letter å gjøre her hvor de allerede er paired med en bedrift. */
            var årFaseOversikt = new Dictionary<int, string>();
            for (int i = 0; i < company.RapportÅr.Count; i++)
            {
                årFaseOversikt[company.RapportÅr[i]] = company.Faser[i];
            }
            foreach (var pair in årFaseOversikt)
            {
                var faseData = new CompanyPhaseStatusOverview
                {
                    Phase = pair.Value,
                    Year = pair.Key,
                    CompanyId = companyId
                };
                if (!context.CompanyPhaseStatusOverviews.Any(b => b.Year == faseData.Year && b.CompanyId == faseData.CompanyId && b.Phase == faseData.Phase)) context.CompanyPhaseStatusOverviews.Add(faseData);
            }
        }
    }
    public static List<int> GetOrgNrArray(List<CompactedVisBedriftData> data)
    {
        List<int> orgNrArray = [];
        foreach (var compactData in data)
        {
            orgNrArray.Add(compactData.Orgnummer);
        }
        return orgNrArray;
    }
}