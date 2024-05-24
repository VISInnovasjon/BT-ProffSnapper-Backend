
namespace Util.ProffApiClasses;

using Npgsql;
using Util.DB;



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
    public int NumberOfShares { get; set; }
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
    public required List<ShareHolderInfo> Shareholders { get; set; }
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
        foreach (var shareholder in Shareholders)
        {
            InsertShareholderStructure shareholderStructure = new(
                CompanyId, ShareholdersLastUpdatedDate, shareholder
            );
            shareholderStructure.InsertIntoDatabase();
        }
        try
        {
            Database.Query("SELECT update_delta()", reader => { });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

public class ØkoDataSqlStructure
{
    public NpgsqlParameter OrgNr { get; set; }
    public NpgsqlParameter Rapportår { get; set; }
    public NpgsqlParameter Kodenavn { get; set; }
    public NpgsqlParameter Kodeverdier { get; set; }
    public ØkoDataSqlStructure(string CompanyId, AccountsInfo accounts)
    {
        List<string> Codes = new();
        List<decimal> Values = new();
        OrgNr = new NpgsqlParameter("orgnr", NpgsqlTypes.NpgsqlDbType.Integer) { Value = int.Parse(CompanyId) };
        Rapportår = new NpgsqlParameter("rapportår", NpgsqlTypes.NpgsqlDbType.Integer) { Value = int.Parse(accounts.Year) };
        for (int i = 0; i < accounts.Accounts.Count; i++)
        {
            Codes.Add(accounts.Accounts[i].Code);
            Values.Add(decimal.Parse(accounts.Accounts[i].Amount));
        }
        Kodeverdier = Database.ConvertListToParameter<decimal>(Values, "verdier");
        Kodenavn = Database.ConvertListToParameter<string>(Codes, "koder");
    }
    public void InsertIntoDatabase()
    {
        try
        {
            List<NpgsqlParameter> parameters = [
                OrgNr, Rapportår, Kodenavn, Kodeverdier
            ];
            Database.Query("SELECT insert_øko_data(orgnr => @orgnr, år => @rapportår, kodenavn => @koder, kodeverdier => @verdier)", reader => { }, parameters);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
public class UpdateNameStructure
{
    public NpgsqlParameter Navn { get; set; }
    public NpgsqlParameter OrgNr { get; set; }
    public NpgsqlParameter? TidligereNavn { get; set; }
    public UpdateNameStructure(string CompanyId, string Name, List<string>? PreviousNames = null)
    {
        Navn = new NpgsqlParameter("navn", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = Name };
        if (PreviousNames != null) this.TidligereNavn = Database.ConvertListToParameter<string>(PreviousNames, "tidligerenavn");
        OrgNr = new NpgsqlParameter("orgnr", NpgsqlTypes.NpgsqlDbType.Integer) { Value = int.Parse(CompanyId) };
    }
    public void InsertIntoDatabase()
    {
        try
        {
            List<NpgsqlParameter> parameters = [
                Navn, OrgNr
            ];
            if (TidligereNavn != null) parameters.Add(TidligereNavn);
            Database.Query($"SELECT update_bedrift_info_with_name(orgnr => @orgnr, navn => @navn{(TidligereNavn != null ? ", tidligere_navn => @tidligerenavn" : "")})", reader => { }, parameters);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
public class InsertShareholderStructure
{
    public NpgsqlParameter OrgNr { get; set; }
    public NpgsqlParameter Rapportår { get; set; }
    public NpgsqlParameter AntallShares { get; set; }
    public NpgsqlParameter InputNavn { get; set; }
    public NpgsqlParameter InputType { get; set; }
    public NpgsqlParameter? BedriftId { get; set; }
    public NpgsqlParameter? Fornavn { get; set; }
    public NpgsqlParameter? Etternavn { get; set; }
    public InsertShareholderStructure(string CompanyId, string UpdatedYear, ShareHolderInfo info)
    {
        OrgNr = new NpgsqlParameter("orgnr", NpgsqlTypes.NpgsqlDbType.Integer) { Value = int.Parse(CompanyId) };
        Rapportår = new NpgsqlParameter("rapportår", NpgsqlTypes.NpgsqlDbType.Integer) { Value = int.Parse(UpdatedYear) };
        AntallShares = new NpgsqlParameter("antall", NpgsqlTypes.NpgsqlDbType.Integer) { Value = info.NumberOfShares };
        InputNavn = new NpgsqlParameter("navn", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = info.Name };
        InputType = new NpgsqlParameter("type", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = info.Share };
        if (!string.IsNullOrEmpty(info.CompanyId)) BedriftId = new NpgsqlParameter("bedriftid", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = info.CompanyId };
        if (!string.IsNullOrEmpty(info.FirstName)) Fornavn = new NpgsqlParameter("fornavn", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = info.FirstName };
        if (!string.IsNullOrEmpty(info.LastName)) Etternavn = new NpgsqlParameter("etternavn", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = info.LastName };
    }
    public void InsertIntoDatabase()
    {
        try
        {
            List<NpgsqlParameter> parameters = [
                        OrgNr, Rapportår, AntallShares,  InputNavn, InputType
                    ];
            if (BedriftId != null) parameters.Add(BedriftId);
            if (Fornavn != null) parameters.Add(Fornavn);
            if (Etternavn != null) parameters.Add(Etternavn);
            Database.Query($"SELECT insert_shareholder_info(orgnr => @orgnr, år => @rapportår, antall => @antall, input_navn => @navn, input_type => @type{(BedriftId != null ? ", bedrift_navn => @bedriftid" : "")}{(Fornavn != null ? ", fornavn => @fornavn" : "")}{(Etternavn != null ? ", etternavn => @etternavn" : "")})", reader => { }, parameters);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
public class InsertKunngjøringStructure
{
    public NpgsqlParameter OrgNr { get; set; }
    public NpgsqlParameter InputId { get; set; }
    public NpgsqlParameter InputDato { get; set; }
    public NpgsqlParameter Inputdesc { get; set; }
    public NpgsqlParameter InputType { get; set; }
    public InsertKunngjøringStructure(string CompanyId, Announcement kunngjøring)
    {
        OrgNr = new NpgsqlParameter("orgnr", NpgsqlTypes.NpgsqlDbType.Integer) { Value = int.Parse(CompanyId) };
        InputId = new NpgsqlParameter("inputid", NpgsqlTypes.NpgsqlDbType.Bigint) { Value = long.Parse(kunngjøring.Id) };
        InputDato = new NpgsqlParameter("inputdato", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = kunngjøring.Date };
        InputType = new NpgsqlParameter("inputtype", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = kunngjøring.Type };
        Inputdesc = new NpgsqlParameter("inputdesc", NpgsqlTypes.NpgsqlDbType.Text) { Value = kunngjøring.Text };
    }
    public void InsertToDataBase()
    {
        try
        {
            List<NpgsqlParameter> parameters = [
                OrgNr, InputId, InputDato, InputType, Inputdesc
            ];
            Database.Query("SELECT insert_kunngjøringer(orgnr => @orgnr, input_id => @inputid, input_dato => @inputdato, input_type => @inputtype, input_desc => @inputdesc)", reader => { }, parameters);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
public class InsertGenerellInfoStructure
{
    public NpgsqlParameter OrgNr { get; set; }
    public NpgsqlParameter Rapportår { get; set; }
    public NpgsqlParameter InputLandsdel { get; set; }
    public NpgsqlParameter InputFylke { get; set; }
    public NpgsqlParameter InputKommune { get; set; }
    public NpgsqlParameter InputPostKode { get; set; }
    public NpgsqlParameter InputPostAddresse { get; set; }
    public NpgsqlParameter? InputAntallAnsatte { get; set; }
    public InsertGenerellInfoStructure(string CompanyId, string UpdateYear, LocationInfo location, PostalInfo post, string? NumberOfEmployees = null)
    {
        OrgNr = new NpgsqlParameter("orgnr", NpgsqlTypes.NpgsqlDbType.Integer) { Value = int.Parse(CompanyId) };
        Rapportår = new NpgsqlParameter("rapportår", NpgsqlTypes.NpgsqlDbType.Integer) { Value = int.Parse(UpdateYear) };
        InputLandsdel = new NpgsqlParameter("inputlandsdel", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = location.CountryPart };
        InputFylke = new NpgsqlParameter("inputfylke", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = location.County };
        InputKommune = new NpgsqlParameter("inputkommune", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = location.Municipality };
        InputPostKode = new NpgsqlParameter("inputpostkode", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = post.ZipCode };
        InputPostAddresse = new NpgsqlParameter("inputpostaddresse", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = post.AddressLine };
        if (!string.IsNullOrEmpty(NumberOfEmployees)) InputAntallAnsatte = new NpgsqlParameter("inputantallansatte", NpgsqlTypes.NpgsqlDbType.Integer) { Value = int.Parse(NumberOfEmployees) };
    }
    public void InsertToDataBase()
    {
        try
        {
            List<NpgsqlParameter> parameters = [
                OrgNr, Rapportår, InputLandsdel, InputFylke, InputKommune, InputPostKode, InputPostAddresse
            ];
            if (InputAntallAnsatte != null) parameters.Add(InputAntallAnsatte);
            Database.Query($"SELECT insert_generell_årlig_bedrift_info(orgnr => @orgnr, år => @rapportår, input_landsdel => @inputlandsdel, input_fylke => @inputfylke, input_kommune => @inputkommune, input_post_kode => @inputpostkode, input_post_addresse => @inputpostaddresse{(InputAntallAnsatte != null ? ", input_antall_ansatte => @inputantallansatte" : "")})", reader => { }, parameters);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
public class InsertBedriftLederInfoStructure
{
    public NpgsqlParameter OrgNr { get; set; }
    public NpgsqlParameter Rapportår { get; set; }
    public NpgsqlParameter InputNavn { get; set; }
    public NpgsqlParameter InputTittel { get; set; }
    public NpgsqlParameter InputTittelKode { get; set; }
    public NpgsqlParameter InputFødselÅr { get; set; }
    public InsertBedriftLederInfoStructure(string CompanyId, string UpdateYear, PersonRole person)
    {
        OrgNr = new NpgsqlParameter("orgnr", NpgsqlTypes.NpgsqlDbType.Integer) { Value = int.Parse(CompanyId) };
        Rapportår = new NpgsqlParameter("rapportår", NpgsqlTypes.NpgsqlDbType.Integer) { Value = int.Parse(UpdateYear) };
        InputNavn = new NpgsqlParameter("inputnavn", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = person.Name };
        InputFødselÅr = new NpgsqlParameter("inputfødselsår", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = person.BirthDate };
        InputTittel = new NpgsqlParameter("inputtittel", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = person.Title };
        InputTittelKode = new NpgsqlParameter("inputtittelkode", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = person.TitleCode };
    }
    public void InsertToDataBase()
    {
        try
        {
            List<NpgsqlParameter> parameters = [
                OrgNr, Rapportår, InputNavn, InputFødselÅr, InputTittel, InputTittelKode
            ];
            Database.Query("Select insert_bedrift_leder_info(orgnr => @orgnr, input_år => @rapportår, input_navn => @inputnavn, input_fødselsår => @inputfødselsår, input_tittel => @inputtittel, input_tittelkode => @inputtittelkode)", reader => { }, parameters);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}