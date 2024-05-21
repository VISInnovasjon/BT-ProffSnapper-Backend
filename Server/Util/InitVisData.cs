using MiniExcelLibs;
using Util.DB;
using Npgsql;
using Server.Models;

namespace Util.InitVisData;

public class RawVisBedriftData
{
    public int RapportÅr { get; set; }
    public int Orgnummer { get; set; }
    public string Fase { get; set; }
    public string? Bransje { get; set; }
    public static List<RawVisBedriftData> ListFromVisExcelSheet(Stream stream, string excelSheetName)
    {

        var rows = stream.Query<RawVisBedriftData>(sheetName: excelSheetName).ToList();

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
            NpgsqlParameter? years = null;
            NpgsqlParameter? phases = null;
            List<NpgsqlParameter> paramList = new() { };
            try
            {
                years = Database.ConvertListToParameter<int>(company.RapportÅr, "years");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (years != null) paramList.Add(years);
            try
            {
                phases = Database.ConvertListToParameter<string>(company.Faser, "phases");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (phases != null) paramList.Add(phases);
            NpgsqlParameter OrgNr = new("orgnr", NpgsqlTypes.NpgsqlDbType.Integer) { Value = company.Orgnummer };
            if (OrgNr != null) paramList.Add(OrgNr);
            NpgsqlParameter bransje = new("bransje", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = company.Bransje };
            if (bransje != null) paramList.Add(bransje);
            Database.Query($"SELECT Insert_Bedrift_Data_Vis(@orgnr,@bransje,@years, @phases)", reader =>
            {
                Console.WriteLine($"Storing {company.Orgnummer}");
            }, paramList);
        }
    }

}