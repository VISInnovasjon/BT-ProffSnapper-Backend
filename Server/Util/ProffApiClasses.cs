
namespace Util.ProffApiClasses;
using Npgsql;
using Util.DB;



public class EcoCodes
{
    public required string Code { get; set; }
    public required int Amount { get; set; }
}
public class AccountsInfo
{
    public string? AccIncompleteCode { get; set; }
    public string? AccIncompleteDesc { get; set; }
    public required int Year { get; set; }
    public required List<EcoCodes> Accounts { get; set; }

    /// <summary>
    /// Converts the AccountsInfo class into a dictionary of npgsql types that can be inserted into a database.
    /// </br>
    /// </br>
    /// codes refer to a sql list of all codes in EcoCodes.</br>
    /// values refer to a sql list of all values in EcoCodes</br>
    /// year refers to the year of these codes.
    /// </summary>
    /// <returns></returns>
    public List<NpgsqlParameter> AccountValues()
    {
        List<NpgsqlParameter> codeDictionary = new() { };
        List<string> codeNames = new() { };
        List<int> values = new() { };
        foreach (var code in Accounts)
        {
            codeNames.Add(code.Code);
            values.Add(code.Amount);
        }
        NpgsqlParameter convertedCodenames = Database.ConvertListToParameter<string>(codeNames, "koder");
        NpgsqlParameter convertedValues = Database.ConvertListToParameter<int>(values, "verdier");
        NpgsqlParameter validYear = new NpgsqlParameter("rapportår", NpgsqlTypes.NpgsqlDbType.Integer) { Value = Year };
        codeDictionary.Add(convertedCodenames);
        codeDictionary.Add(convertedValues);
        codeDictionary.Add(validYear);
        return codeDictionary;
    }
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
    /// <summary>
    /// Funksjon som converterer shareholder object til et dictionary med Npsql parameter.
    /// </summary>
    /// <returns></returns>
    public List<NpgsqlParameter> ShareHolderParam()
    {
        List<NpgsqlParameter> Param = new() { };
        if (CompanyId != null) Param.Add(new NpgsqlParameter("shareholder_bedrift_id", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = CompanyId });
        if (FirstName != null) Param.Add(new NpgsqlParameter("fornavn", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = FirstName });
        if (LastName != null) Param.Add(new NpgsqlParameter("etternavn", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = LastName });
        if (Name != null) Param.Add(new NpgsqlParameter("name", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = Name });
        Param.Add(new NpgsqlParameter("sharecount", NpgsqlTypes.NpgsqlDbType.Integer) { Value = NumberOfShares });
        Param.Add(new NpgsqlParameter("share", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = Share });
        if (Details != null && Details.Href != null) Param.Add(new NpgsqlParameter("website", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = Details.Href });
        return Param;
    }

}

public class Announcement
{
    public int Id { get; set; }
    public required string Date { get; set; }
    public required string Text { get; set; }
    public required string Type { get; set; }
    public List<NpgsqlParameter> AnnouncementParam()
    {
        List<NpgsqlParameter> Param = new();
        Param.Add(new NpgsqlParameter("kunngjøring_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = Id });
        Param.Add(new NpgsqlParameter("dato", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = Date });
        Param.Add(new NpgsqlParameter("kunngjøringstext", NpgsqlTypes.NpgsqlDbType.Text) { Value = Text });
        Param.Add(new NpgsqlParameter("kunngjøringstype", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = Type });
        return Param;
    }
}
public class LocationInfo
{
    public required string CountryPart { get; set; }
    public required string County { get; set; }
    public required string Municipality { get; set; }
}
public class PostalInfo
{
    public required string AdressLine { get; set; }
    public required string ZipCode { get; set; }
}
public class PersonRole
{
    public required string BirthDate { get; set; }
    public required string Name { get; set; }
    public required string Title { get; set; }
    public required string TitleCode { get; set; }
    public List<NpgsqlParameter> PersonRoleParams()
    {
        List<NpgsqlParameter> Param = new() { };
        if (TitleCode == "DAGL" || TitleCode == "LEDE")
        {
            Param.Add(new NpgsqlParameter("fødselsdag", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = BirthDate });
            Param.Add(new NpgsqlParameter("navn", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = Name });
            Param.Add(new NpgsqlParameter("tittel", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = Title });
            Param.Add(new NpgsqlParameter("tittelkode", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = TitleCode });
        }
        return Param;
    }
}
/* Må lage en class basert på hva data som skal hentes inn fra proff. basert på JSON schema. Se på proffjson schema for referanse. */
public class ReturnStructure
{
    public string? CompanyType { get; set; }
    public string? CompanyTypeName { get; set; }
    public decimal? EstimatedTurnover { get; set; }
    public bool HasSecurity { get; set; }
    public List<string>? PreviousNames { get; set; }
    public string? RegistrationDate { get; set; }
    public string? EstablishedDate { get; set; }
    public int? NumberOfEmployees { get; set; }
    public required int CompanyId { get; set; }
    public required string Name { get; set; }
    /* Rollekoder vi er ute etter fra PROFF er DAGL, eller LEDE hvis DAGL ikke er tilgjengelig. */
    public required List<PersonRole> PersonRoles { get; set; }
    public required List<Announcement> Announcements { get; set; }
    public required List<AccountsInfo> CompanyAccounts { get; set; }
    public required List<ShareHolderInfo> ShareHolders { get; set; }
    public required LocationInfo Location { get; set; }
    public required PostalInfo PostalAddress { get; set; }
}

public class SqlParamStructure
{
    public NpgsqlParameter? CompanyType { get; set; }
    public NpgsqlParameter? CompanyTypeName { get; set; }
    public NpgsqlParameter? EstimatedTurnover { get; set; }
    public required NpgsqlParameter HasSecurity { get; set; }
    public NpgsqlParameter? PreviousNames { get; set; }
    public NpgsqlParameter? RegistrationDate { get; set; }
    public NpgsqlParameter? EstablishedDate { get; set; }
    public NpgsqlParameter? NumberOfEmployees { get; set; }
    public required NpgsqlParameter CompanyId { get; set; }
    public required NpgsqlParameter Name { get; set; }
    public required NpgsqlParameter CurrentYear { get; set; }
    public required List<List<NpgsqlParameter>> PersonRoles { get; set; }
    public required List<List<NpgsqlParameter>> Announcements { get; set; }
    public required List<List<NpgsqlParameter>> CompanyAccounts { get; set; }
    public required List<List<NpgsqlParameter>> ShareHolders { get; set; }
    public required NpgsqlParameter CountryPart { get; set; }
    public required NpgsqlParameter County { get; set; }
    public required NpgsqlParameter Municipality { get; set; }
    public required NpgsqlParameter AdressLine { get; set; }
    public required NpgsqlParameter ZipCode { get; set; }
    public static SqlParamStructure GetSqlParamStructure(ReturnStructure ApiReturn)
    {
        SqlParamStructure paramStruct = new()
        {
            CompanyType = ApiReturn.CompanyType == null ? null : new NpgsqlParameter("companytype", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = ApiReturn.CompanyType },
            CompanyTypeName = ApiReturn.CompanyTypeName == null ? null : new NpgsqlParameter("companytypename", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = ApiReturn.CompanyTypeName },
            EstimatedTurnover = ApiReturn.EstimatedTurnover == null ? null : new NpgsqlParameter("estimatedturnover", NpgsqlTypes.NpgsqlDbType.Numeric) { Value = ApiReturn.EstimatedTurnover },
            HasSecurity = new NpgsqlParameter("hassecurity", NpgsqlTypes.NpgsqlDbType.Boolean) { Value = ApiReturn.HasSecurity },
            PreviousNames = ApiReturn.PreviousNames == null ? null : ApiReturn.PreviousNames.Count == 0 ? null : Database.ConvertListToParameter<string>(ApiReturn.PreviousNames, "previousnames"),
            RegistrationDate = ApiReturn.RegistrationDate == null ? null : new NpgsqlParameter("registrationdate", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = ApiReturn.RegistrationDate },
            EstablishedDate = ApiReturn.EstablishedDate == null ? null : new NpgsqlParameter("establisheddate", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = ApiReturn.EstablishedDate },
            NumberOfEmployees = ApiReturn.NumberOfEmployees == null ? null : new NpgsqlParameter("numberofemployees", NpgsqlTypes.NpgsqlDbType.Integer) { Value = ApiReturn.NumberOfEmployees },
            Name = new NpgsqlParameter("name", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = ApiReturn.Name },
            CompanyId = new NpgsqlParameter("orgnr", NpgsqlTypes.NpgsqlDbType.Integer) { Value = ApiReturn.CompanyId },
            Announcements = new() { },
            CompanyAccounts = new() { },
            ShareHolders = new() { },
            PersonRoles = new() { },
            CurrentYear = new NpgsqlParameter("gjeldende_år", NpgsqlTypes.NpgsqlDbType.Integer) { Value = DateTime.Now.Year },
            CountryPart = new NpgsqlParameter("countrypart", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = ApiReturn.Location.CountryPart },
            County = new NpgsqlParameter("county", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = ApiReturn.Location.County },
            Municipality = new NpgsqlParameter("municipality", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = ApiReturn.Location.Municipality },
            AdressLine = new NpgsqlParameter("adressline", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = ApiReturn.PostalAddress.AdressLine },
            ZipCode = new NpgsqlParameter("zipkode", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = ApiReturn.PostalAddress.ZipCode }
        };
        foreach (var person in ApiReturn.PersonRoles)
        {
            List<NpgsqlParameter> PersonRole = person.PersonRoleParams();
            paramStruct.PersonRoles.Add(PersonRole);
        }
        foreach (var Announcement in ApiReturn.Announcements)
        {
            List<NpgsqlParameter> AnnouncementInfo = Announcement.AnnouncementParam();
            paramStruct.Announcements.Add(AnnouncementInfo);
        }
        foreach (var account in ApiReturn.CompanyAccounts)
        {
            List<NpgsqlParameter> AccountInfo = account.AccountValues();
            paramStruct.CompanyAccounts.Add(AccountInfo);
        }
        foreach (var shareholder in ApiReturn.ShareHolders)
        {
            List<NpgsqlParameter> ShareholderParameter = shareholder.ShareHolderParam();
            paramStruct.ShareHolders.Add(ShareholderParameter);
        }
        return paramStruct;
    }
    public void AddParamToDb(SqlParamStructure param)
    {
        Console.WriteLine($"Working on {param.CompanyId}");
        Database.Query("SELECT update_bedrift_info_with_name (@orgnr, @name, @previousnames)", reader =>
        {
            Console.WriteLine($"Updating Name to {param.Name}");
        }, new List<NpgsqlParameter> { param.CompanyId, param.Name, param.PreviousNames });
        Console.WriteLine("Inserting general params");
        Database.Query("SELECT insert_generell_årlig_bedrift_info(@orgnr, @gjeldende_år, @countrypart, @county, @municipality, @adressline, @zipkode, @numberofemployees)", reader => { }, new List<NpgsqlParameter>{
            param.CompanyId, param.CurrentYear, param.CountryPart, param.County, param.Municipality, param.AdressLine, param.ZipCode, param.NumberOfEmployees
        });
        Console.WriteLine("Inserting Øko Data");
        foreach (var account in param.CompanyAccounts)
        {
            account.Add(param.CompanyId);
            Database.Query("SELECT insert_øko_data(@orgnr, @rapportår, @koder, @verdier)", reader => { }, account);
        }
        Console.WriteLine("Inserting Announcement data");
        foreach (var announcement in param.Announcements)
        {
            announcement.Add(param.CompanyId);
            Database.Query("SELECT insert_kunngjøringer(@orgnr, @kunngjørings_id, @dato, @kunngjøringstekst, @kunngjøringstype)", reader => { }, announcement);
        }
        Console.WriteLine("Inserting leader data");
        foreach (var person in param.PersonRoles)
        {
            person.Add(param.CompanyId);
            person.Add(param.CurrentYear);
            Database.Query("SELECT insert_bedrift_leder_info(@orgnr, @navn, @tittel, @tittelkode, @fødselsdag, @gjeldende_år)", reader => { }, person);
        }
        Console.WriteLine("Inserting Shareholder Info");
        foreach (var shareholder in param.ShareHolders)
        {
            shareholder.Add(param.CompanyId);
            shareholder.Add(param.CurrentYear);
            Database.Query("SELECT insert_shareholder_info(@orgnr, @gjeldende_år, @sharecount, @name, @share, @shareholder_bedrift_id, @fornavn, @etternavn)", reader => { }, shareholder);
        }
    }

}
