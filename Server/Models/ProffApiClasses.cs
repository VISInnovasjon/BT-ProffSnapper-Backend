
namespace Server.Models;
using Util;



public class EcoCodes
{
    public required string Code { get; set; }
    public required string Amount { get; set; }
}
public class AccountsInfo
{
    public string? AccIncompleteCode { get; set; }
    public string? AccIncompleteDesc { get; set; }
    public required string Year { get; set; }
    public required List<EcoCodes> Accounts { get; set; }

}
public class WebInfo
{
    public string? Href { get; set; }
    public string? Rel { get; set; }
}
public class ShareHolderInfo
{
    public string? CompanyId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Name { get; set; }
    public decimal NumberOfShares { get; set; }
    public required string Share { get; set; }
    public WebInfo? Details { get; set; }
}

public class Announcement
{
    public required string Id { get; set; }
    public required string Date { get; set; }
    public required string Text { get; set; }
    public required string Type { get; set; }
}
public class LocationInfo
{
    public required string CountryPart { get; set; }
    public required string County { get; set; }
    public required string Municipality { get; set; }
}
public class PostalInfo
{
    public required string AddressLine { get; set; }
    public required string ZipCode { get; set; }
}
public class PersonRole
{
    public required string BirthDate { get; set; }
    public required string Name { get; set; }
    public required string Title { get; set; }
    public required string TitleCode { get; set; }
}
/* Må lage en class basert på hva data som skal hentes inn fra proff. basert på JSON schema. Se på proffjson schema for referanse. */
public class ReturnStructure
{
    public string? CompanyType { get; set; }
    public string? CompanyTypeName { get; set; }
    public List<string>? PreviousNames { get; set; }
    public string? RegistrationDate { get; set; }
    public string? EstablishedDate { get; set; }
    public required string ShareholdersLastUpdatedDate { get; set; }
    public string? NumberOfEmployees { get; set; }
    public required string CompanyId { get; set; }
    public required string Name { get; set; }
    /* Rollekoder vi er ute etter fra PROFF er DAGL, eller LEDE hvis DAGL ikke er tilgjengelig. */
    public required List<PersonRole> PersonRoles { get; set; }
    public required List<Announcement> Announcements { get; set; }
    public required List<AccountsInfo> CompanyAccounts { get; set; }
    public List<ShareHolderInfo>? Shareholders { get; set; }
    public required LocationInfo Location { get; set; }
    public required PostalInfo PostalAddress { get; set; }
    public void InsertToDataBase()
    {
        UpdateNameStructure nameStructure = new(
                    CompanyId, Name, PreviousNames.Count == 0 ? null : PreviousNames
                );
        nameStructure.InsertIntoDatabase();
        InsertGenerellInfoStructure infoStructure = new(
            CompanyId, ShareholdersLastUpdatedDate, Location, PostalAddress, NumberOfEmployees ?? null
        );
        infoStructure.InsertToDataBase();
        foreach (var announcement in Announcements)
        {
            InsertKunngjøringStructure kunngjøringStructure = new(
                CompanyId, announcement
            );
            kunngjøringStructure.InsertToDataBase();
        }
        foreach (var account in CompanyAccounts)
        {
            ØkoDataSqlStructure økoData = new(
                CompanyId, account
            );
            økoData.InsertIntoDatabase();
        }
        foreach (var person in PersonRoles)
        {
            if (person.TitleCode != "DAGL" && person.TitleCode != "LEDE") continue;
            else
            {
                InsertBedriftLederInfoStructure bedriftLeder = new(
                    CompanyId, ShareholdersLastUpdatedDate, person
                );
                bedriftLeder.InsertToDataBase();
            }
        }

        if (Shareholders != null) foreach (var shareholder in Shareholders)
            {
                InsertShareholderStructure shareholderStructure = new(
                    CompanyId, ShareholdersLastUpdatedDate, shareholder
                );
                shareholderStructure.InsertIntoDatabase();
            }
    }
}

public class ØkoDataSqlStructure
{
    public int OrgNr { get; set; }
    public int År { get; set; }
    public List<string> Kodenavn { get; set; }
    public List<decimal> Kodeverdier { get; set; }
    public ØkoDataSqlStructure(string CompanyId, AccountsInfo accounts)
    {
        List<string> Codes = new();
        List<decimal> Values = new();
        int TempOutput;
        bool ParsingNr = int.TryParse(CompanyId, out TempOutput);
        if (!ParsingNr) throw new ArgumentException($"Could not parse {CompanyId} to an integer.");
        OrgNr = TempOutput;
        ParsingNr = int.TryParse(accounts.Year, out TempOutput);
        if (!ParsingNr) throw new ArgumentException($"Could not parse {accounts.Year} to an integer.");
        År = TempOutput;
        for (int i = 0; i < accounts.Accounts.Count; i++)
        {
            Codes.Add(accounts.Accounts[i].Code);
            Values.Add(decimal.Parse(accounts.Accounts[i].Amount));
        }
        Kodeverdier = accounts.Accounts.Select(a => decimal.Parse(a.Amount)).ToList();
        Kodenavn = accounts.Accounts.Select(a => a.Code).ToList();
    }
    public void InsertIntoDatabase()
    {
        try
        {
            using (var context = new BtdbContext())
            {
                var bedriftId = context.BedriftInfos.Single(b => b.Orgnummer == OrgNr).BedriftId;
                for (int i = 0; i < Kodenavn.Count; i++)
                {
                    var ØkoData = new ÅrligØkonomiskDatum
                    {
                        BedriftId = bedriftId,
                        ØkoKode = Kodenavn[i],
                        ØkoVerdi = Kodeverdier[i],
                        Rapportår = År
                    };
                    context.SaveChanges();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
public class UpdateNameStructure
{
    public string Navn { get; set; }
    public int OrgNr { get; set; }
    public List<string>? TidligereNavn { get; set; }
    public UpdateNameStructure(string CompanyId, string Name, List<string>? PreviousNames = null)
    {
        Navn = Name;
        if (PreviousNames != null) this.TidligereNavn = PreviousNames;
        int output;
        bool parseSuccess = int.TryParse(CompanyId, out output);
        if (!parseSuccess) throw new ArgumentException($"Failed to parse {CompanyId} to integer");
        OrgNr = output;
    }
    public void InsertIntoDatabase()
    {
        try
        {
            using (var context = new BtdbContext())
            {
                var bedriftInfo = context.BedriftInfos.Single(b => b.Orgnummer == OrgNr);
                bedriftInfo.Målbedrift = Navn;
                if (TidligereNavn != null && TidligereNavn.Count > 0) bedriftInfo.Navneliste = TidligereNavn;
                context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
public class InsertShareholderStructure
{
    public int OrgNr { get; set; }
    public int År { get; set; }
    public decimal AntallShares { get; set; }
    public string InputNavn { get; set; }
    public string InputType { get; set; }
    public string? ShareholderBId { get; set; }
    public string? Fornavn { get; set; }
    public string? Etternavn { get; set; }
    public InsertShareholderStructure(string CompanyId, string UpdatedYear, ShareHolderInfo info)
    {
        int output;

        bool parseSuccess;
        parseSuccess = int.TryParse(CompanyId, out output);
        if (!parseSuccess) throw new ArgumentException($"Couldn't parse {CompanyId} to integer");
        OrgNr = output;
        parseSuccess = int.TryParse(UpdatedYear, out output);
        if (!parseSuccess) throw new ArgumentException($"Couldn't parse {UpdatedYear} to integer");
        År = output;

        AntallShares = info.NumberOfShares;
        InputNavn = info.Name ?? "Ingen navn funnet";
        InputType = info.Share;
        if (!string.IsNullOrEmpty(info.CompanyId)) ShareholderBId = info.CompanyId;
        if (!string.IsNullOrEmpty(info.FirstName)) Fornavn = info.FirstName;
        if (!string.IsNullOrEmpty(info.LastName)) Etternavn = info.LastName;
    }
    public void InsertIntoDatabase()
    {
        try
        {
            using (var context = new BtdbContext())
            {
                var bedriftId = context.BedriftInfos.Single(b => b.Orgnummer == OrgNr).BedriftId;
                var shareholder = new BedriftShareholderInfo
                {
                    BedriftId = bedriftId,
                    Rapportår = År,
                    ShareholderBedriftId = ShareholderBId,
                    ShareholderFornavn = Fornavn,
                    ShareholderEtternavn = Etternavn,
                    Sharetype = InputType,
                    Navn = InputNavn,
                    AntalShares = AntallShares
                };
                context.BedriftShareholderInfos.Add(shareholder);
                context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
public class InsertKunngjøringStructure
{
    public int OrgNr { get; set; }
    public long InputId { get; set; }
    public DateOnly? InputDato { get; set; }
    public string Inputdesc { get; set; }
    public string InputType { get; set; }
    public InsertKunngjøringStructure(string CompanyId, Announcement kunngjøring)
    {
        int output;
        long lOutput;
        bool parseSuccess;
        parseSuccess = int.TryParse(CompanyId, out output);
        if (!parseSuccess) throw new ArgumentException($"Couldn't parse {CompanyId} to integer");
        OrgNr = output;
        parseSuccess = long.TryParse(kunngjøring.Id, out lOutput);
        if (!parseSuccess) throw new ArgumentException($"Couldn't parse {kunngjøring.Id} to BigInt");
        InputId = lOutput;
        InputDato = string.IsNullOrEmpty(kunngjøring.Date) ? null : DateCorrector.ConvertDate(kunngjøring.Date);
        InputType = kunngjøring.Type;
        Inputdesc = kunngjøring.Text;
    }
    public void InsertToDataBase()
    {
        try
        {
            using (var context = new BtdbContext())
            {
                var bedriftId = context.BedriftInfos.Single(b => b.Orgnummer == OrgNr).BedriftId;
                var announcement = new BedriftKunngjøringer
                {
                    BedriftId = bedriftId,
                    Dato = InputDato,
                    KunngjøringId = InputId,
                    Kunngjøringstekst = Inputdesc,
                    Kunngjøringstype = InputType
                };
                context.BedriftKunngjøringers.Add(announcement);
                context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
public class InsertGenerellInfoStructure
{
    public int OrgNr { get; set; }
    public int År { get; set; }
    public string InputLandsdel { get; set; }
    public string InputFylke { get; set; }
    public string InputKommune { get; set; }
    public string InputPostKode { get; set; }
    public string InputPostAddresse { get; set; }
    public int? InputAntallAnsatte { get; set; }
    public InsertGenerellInfoStructure(string CompanyId, string UpdateYear, LocationInfo location, PostalInfo post, string? NumberOfEmployees = null)
    {
        int output;
        bool parseSuccess;
        parseSuccess = int.TryParse(CompanyId, out output);
        if (!parseSuccess) throw new ArgumentException($"Couldn't parse {CompanyId} to Integer");
        OrgNr = output;
        parseSuccess = int.TryParse(UpdateYear, out output);
        if (!parseSuccess) throw new ArgumentException($"Couldn't parse {UpdateYear} to Integer");
        År = output;
        parseSuccess = int.TryParse(NumberOfEmployees, out output);
        if (!parseSuccess) InputAntallAnsatte = 1;
        else InputAntallAnsatte = output;
        InputLandsdel = location.CountryPart;
        InputFylke = location.County;
        InputKommune = location.Municipality;
        InputPostKode = string.IsNullOrEmpty(post.ZipCode) ? "Mangler PostKode" : post.ZipCode;
        InputPostAddresse = post.AddressLine;
    }
    public void InsertToDataBase()
    {
        try
        {
            using (var context = new BtdbContext())
            {
                var bedriftId = context.BedriftInfos.Single(b => b.Orgnummer == OrgNr).BedriftId;
                var genInfo = new GenerellÅrligBedriftInfo
                {
                    BedriftId = bedriftId,
                    Landsdel = InputLandsdel,
                    Fylke = InputFylke,
                    Kommune = InputKommune,
                    PostAddresse = InputPostAddresse,
                    PostKode = InputPostKode,
                    Rapportår = År,
                    AntallAnsatte = InputAntallAnsatte
                };
                context.GenerellÅrligBedriftInfos.Add(genInfo);
                context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
public class InsertBedriftLederInfoStructure
{
    public int OrgNr { get; set; }
    public int År { get; set; }
    public string InputNavn { get; set; }
    public string InputTittel { get; set; }
    public string InputTittelKode { get; set; }
    public DateOnly? InputFødselÅr { get; set; }
    public InsertBedriftLederInfoStructure(string CompanyId, string UpdateYear, PersonRole person)
    {
        int output;
        bool parseSuccess;
        parseSuccess = int.TryParse(CompanyId, out output);
        if (!parseSuccess) throw new ArgumentException($"Couldn't parse {CompanyId} into Integer");
        OrgNr = output;
        parseSuccess = int.TryParse(CompanyId, out output);
        if (!parseSuccess) throw new ArgumentException($"Couldn't parse {UpdateYear} into Integer");
        År = output;
        InputNavn = person.Name;
        InputFødselÅr = string.IsNullOrEmpty(person.BirthDate) ? null : DateCorrector.CorrectDate(person.BirthDate);
        InputTittel = person.Title;
        InputTittelKode = person.TitleCode;
    }
    public void InsertToDataBase()
    {
        try
        {
            using (var context = new BtdbContext())
            {
                var bedriftId = context.BedriftInfos.Single(b => b.Orgnummer == OrgNr).BedriftId;
                var leder = new BedriftLederOversikt
                {
                    BedriftId = bedriftId,
                    Navn = InputNavn,
                    Fødselsdag = InputFødselÅr,
                    Tittel = InputTittel,
                    Tittelkode = InputTittelKode,
                    Rapportår = År
                };
                context.BedriftLederOversikts.Add(leder);
                context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}