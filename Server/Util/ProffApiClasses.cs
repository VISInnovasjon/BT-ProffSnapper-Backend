
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
    public string? AccOIncompleteDesc { get; set; }
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
    public Dictionary<string, NpgsqlParameter> AccountValues()
    {
        Dictionary<string, NpgsqlParameter> codeDictionary = new() { };
        List<string> codeNames = new() { };
        List<int> values = new() { };
        foreach (var code in Accounts)
        {
            codeNames.Add(code.Code);
            values.Add(code.Amount);
        }
        NpgsqlParameter convertedCodenames = Database.ConvertListToParameter<string>(codeNames, "codes");
        NpgsqlParameter convertedValues = Database.ConvertListToParameter<int>(values, "values");
        NpgsqlParameter validYear = new NpgsqlParameter("year", NpgsqlTypes.NpgsqlDbType.Integer) { Value = Year };
        codeDictionary.Add("codes", convertedCodenames);
        codeDictionary.Add("values", convertedValues);
        codeDictionary.Add("year", validYear);
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
    public string? Name { get; set; }
    public int NumberOfShares { get; set; }
    public required string Share { get; set; }
    public WebInfo? Details { get; set; }
    /// <summary>
    /// Funksjon som converterer shareholder object til et dictionary med Npsql parameter.
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, NpgsqlParameter> ShareHolderParam()
    {
        Dictionary<string, NpgsqlParameter> Param = new() { };
        if (CompanyId != null) Param.Add("companyid", new NpgsqlParameter("companyid", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = CompanyId });
        if (FirstName != null) Param.Add("firstname", new NpgsqlParameter("firstname", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = FirstName });
        if (Name != null) Param.Add("name", new NpgsqlParameter("name", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = Name });
        Param.Add("sharecount", new NpgsqlParameter("sharecount", NpgsqlTypes.NpgsqlDbType.Integer) { Value = NumberOfShares });
        Param.Add("share", new NpgsqlParameter("share", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = Share });
        if (Details != null && Details.Href != null) Param.Add("website", new NpgsqlParameter("website", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = Details.Href });
        return Param;
    }

}
public class LocationInfo
{
    public string? CountryPart { get; set; }
    public string? County { get; set; }
    public string? Municipality { get; set; }
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
    public required string Name { get; set; }
    public required List<AccountsInfo> AnnualAccounts { get; set; }
    public required List<ShareHolderInfo> ShareHolders { get; set; }
    public required LocationInfo Location { get; set; }
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
    public required NpgsqlParameter Name { get; set; }
    public required List<Dictionary<string, NpgsqlParameter>> AnnualAccounts { get; set; }
    public required List<Dictionary<string, NpgsqlParameter>> ShareHolders { get; set; }
    public required NpgsqlParameter CountryPart { get; set; }
    public required NpgsqlParameter County { get; set; }
    public required NpgsqlParameter Municipality { get; set; }
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
            AnnualAccounts = new() { },
            ShareHolders = new() { },
            CountryPart = new NpgsqlParameter("countrypart", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = ApiReturn.Location.CountryPart },
            County = new NpgsqlParameter("county", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = ApiReturn.Location.County },
            Municipality = new NpgsqlParameter("municipality", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = ApiReturn.Location.Municipality }
        };
        foreach (var account in ApiReturn.AnnualAccounts)
        {
            Dictionary<string, NpgsqlParameter> AccountInfo = account.AccountValues();
            paramStruct.AnnualAccounts.Add(AccountInfo);
        }
        foreach (var shareholder in ApiReturn.ShareHolders)
        {
            Dictionary<string, NpgsqlParameter> ShareholderParameter = shareholder.ShareHolderParam();
            paramStruct.ShareHolders.Add(ShareholderParameter);
        }
        return paramStruct;
    }

}
