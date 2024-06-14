
namespace Server.Models;
using Microsoft.EntityFrameworkCore;
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
    public async Task InsertIntoDatabase(BtdbContext context)
    {
        int bedriftId;
        bedriftId = context.BedriftInfos.Single(b => b.Orgnummer == int.Parse(CompanyId)).BedriftId;
        new UpdateNameStructure(
                    Name, PreviousNames?.Count == 0 ? null : PreviousNames
                ).CraftDbValues(context, bedriftId);
        var genInfo = new InsertGenerellInfoStructure(
            ShareholdersLastUpdatedDate, Location, PostalAddress, NumberOfEmployees ?? null
        ).CraftDbValues(bedriftId);
        try
        {
            await context.GenerellÅrligBedriftInfos.AddAsync(genInfo);
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            var existingEntity = context.GenerellÅrligBedriftInfos.SingleOrDefault(b => b.BedriftId == genInfo.BedriftId && b.Rapportår == genInfo.Rapportår);
            if (existingEntity == null) Console.WriteLine($"No entity found, some other error occured {ex.Message}");
            else
            {
                existingEntity.AntallAnsatte = genInfo.AntallAnsatte;
                existingEntity.Fylke = genInfo.Fylke;
                existingEntity.Kommune = genInfo.Kommune;
                existingEntity.Landsdel = genInfo.Landsdel;
                existingEntity.PostAddresse = genInfo.PostAddresse;
                existingEntity.PostKode = genInfo.PostKode;
            }
        }
        List<BedriftKunngjøringer> kunnList = [];
        foreach (var announcement in Announcements)
        {
            var newKunn = new InsertKunngjøringStructure(
                announcement
            ).CraftDbValues(bedriftId);
        }
        try
        {
            await context.BedriftKunngjøringers.AddRangeAsync(kunnList);
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            foreach (var item in kunnList)
            {
                await ConflictHandler.HandleConflicts(context, item, ["BedriftId", "KunngjøringsId"], ["Dato", "KunngjøringsText", "KunngjøringsType"]);
            }
        }
        foreach (var account in CompanyAccounts)
        {
            var økoList = new ØkoDataSqlStructure(
                account
            ).CraftDbValues(bedriftId);
            try
            {
                await context.ÅrligØkonomiskData.AddRangeAsync(økoList);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                foreach (var item in økoList)
                {
                    await ConflictHandler.HandleConflicts(context, item, ["BedriftId", "Rapportår", "ØkoKode"], ["ØkoVerdi"]);
                }
            }
        }
        List<BedriftLederOversikt> ledeList = [];
        foreach (var person in PersonRoles)
        {
            if (person.TitleCode != "DAGL" && person.TitleCode != "LEDE") continue;
            else
            {
                var newLede = new InsertBedriftLederInfoStructure(
                    ShareholdersLastUpdatedDate, person
                ).CraftDbValues(bedriftId);
                ledeList.Add(newLede);
            }
        }
        try
        {
            await context.BedriftLederOversikts.AddRangeAsync(ledeList);
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            foreach (var item in ledeList)
            {
                await ConflictHandler.HandleConflicts(context, item, ["BedriftId", "Rapportår", "Tittelkode"], ["Navn", "Tittel", "Fødselsdag"]);
            }
        }
        List<BedriftShareholderInfo> shareList = [];
        if (Shareholders != null) foreach (var shareholder in Shareholders)
            {
                var newShareholder = new InsertShareholderStructure(
                    ShareholdersLastUpdatedDate, shareholder
                ).CraftDbValues(bedriftId);
                shareList.Add(newShareholder);
            }
        try
        {
            await context.BedriftShareholderInfos.AddRangeAsync(shareList);
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            foreach (var item in shareList)
            {
                await ConflictHandler.HandleConflicts(context, item, ["BedriftId", "Rapportår", "Navn"], ["AntalShares", "ShareholderBedriftId", "ShareholderFornavn", "ShareholderEtternavn", "Sharetype"]);
            }
        }
    }
}

public class ØkoDataSqlStructure
{
    public int OrgNr { get; set; }
    public int År { get; set; }
    public List<string> Kodenavn { get; set; }
    public List<decimal> Kodeverdier { get; set; }
    public ØkoDataSqlStructure(AccountsInfo accounts)
    {
        bool ParsingNr;
        ParsingNr = int.TryParse(accounts.Year, out int TempOutput);
        if (!ParsingNr) throw new ArgumentException($"Could not parse {accounts.Year} to an integer.");
        År = TempOutput;
        Kodenavn = [];
        Kodeverdier = [];
        for (int i = 0; i < accounts.Accounts.Count; i++)
        {
            Kodenavn.Add(accounts.Accounts[i].Code);
            Kodeverdier.Add(decimal.Parse(accounts.Accounts[i].Amount));
        }

    }
    public List<ÅrligØkonomiskDatum> CraftDbValues(int bedriftId)
    {
        try
        {
            List<ÅrligØkonomiskDatum> dataList = [];
            for (int i = 0; i < Kodenavn.Count; i++)
            {
                var ØkoData = new ÅrligØkonomiskDatum
                {
                    BedriftId = bedriftId,
                    ØkoKode = Kodenavn[i],
                    ØkoVerdi = Kodeverdier[i],
                    Rapportår = År
                };
                dataList.Add(ØkoData);
            }
            return dataList;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to craft object, {ex.Message}");
        }
    }
}
public class UpdateNameStructure
{
    public string Navn { get; set; }
    public int OrgNr { get; set; }
    public List<string>? TidligereNavn { get; set; }
    public UpdateNameStructure(string Name, List<string>? PreviousNames = null)
    {
        Navn = Name;
        if (PreviousNames != null) TidligereNavn = PreviousNames;
    }
    public void CraftDbValues(BtdbContext context, int bedriftId)
    {
        try
        {

            var bedriftInfo = context.BedriftInfos.Single(b => b.BedriftId == bedriftId);
            bedriftInfo.Målbedrift = Navn;
            if (TidligereNavn != null && TidligereNavn.Count > 0) bedriftInfo.Navneliste = TidligereNavn;
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
    public InsertShareholderStructure(string UpdatedYear, ShareHolderInfo info)
    {
        bool parseSuccess;
        parseSuccess = int.TryParse(UpdatedYear, out int output);
        if (!parseSuccess) throw new ArgumentException($"Couldn't parse {UpdatedYear} to integer");
        År = output;

        AntallShares = info.NumberOfShares;
        InputNavn = info.Name ?? "Ingen navn funnet";
        InputType = info.Share;
        if (!string.IsNullOrEmpty(info.CompanyId)) ShareholderBId = info.CompanyId;
        if (!string.IsNullOrEmpty(info.FirstName)) Fornavn = info.FirstName;
        if (!string.IsNullOrEmpty(info.LastName)) Etternavn = info.LastName;
    }
    public BedriftShareholderInfo CraftDbValues(int bedriftId)
    {
        try
        {
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
            return shareholder;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to craft object, {ex.Message}");
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
    public InsertKunngjøringStructure(Announcement kunngjøring)
    {
        bool parseSuccess;
        parseSuccess = long.TryParse(kunngjøring.Id, out long lOutput);
        if (!parseSuccess) throw new ArgumentException($"Couldn't parse {kunngjøring.Id} to BigInt");
        InputId = lOutput;
        InputDato = string.IsNullOrEmpty(kunngjøring.Date) ? null : DateCorrector.ConvertDate(kunngjøring.Date);
        InputType = kunngjøring.Type;
        Inputdesc = kunngjøring.Text;
    }
    public BedriftKunngjøringer CraftDbValues(int bedriftId)
    {
        try
        {

            var announcement = new BedriftKunngjøringer
            {
                BedriftId = bedriftId,
                Dato = InputDato,
                KunngjøringId = InputId,
                Kunngjøringstekst = Inputdesc,
                Kunngjøringstype = InputType
            };
            return announcement;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to craft Object {ex.Message}");
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
    public InsertGenerellInfoStructure(string UpdateYear, LocationInfo location, PostalInfo post, string? NumberOfEmployees = null)
    {
        bool parseSuccess;
        parseSuccess = int.TryParse(UpdateYear, out int output);
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
    public GenerellÅrligBedriftInfo CraftDbValues(int bedriftId)
    {
        try
        {
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
            return genInfo;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to craft object, {ex.Message}");
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
    public InsertBedriftLederInfoStructure(string UpdateYear, PersonRole person)
    {
        bool parseSuccess;
        parseSuccess = int.TryParse(UpdateYear, out int output);
        if (!parseSuccess) throw new ArgumentException($"Couldn't parse {UpdateYear} into Integer");
        År = output;
        InputNavn = person.Name;
        InputFødselÅr = string.IsNullOrEmpty(person.BirthDate) ? null : DateCorrector.CorrectDate(person.BirthDate);
        InputTittel = person.Title;
        InputTittelKode = person.TitleCode;
    }
    public BedriftLederOversikt CraftDbValues(int bedriftId)
    {
        try
        {
            var leder = new BedriftLederOversikt
            {
                BedriftId = bedriftId,
                Navn = InputNavn,
                Fødselsdag = InputFødselÅr,
                Tittel = InputTittel,
                Tittelkode = InputTittelKode,
                Rapportår = År
            };
            return leder;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to craft Object, {ex.Message}");
        }
    }
}