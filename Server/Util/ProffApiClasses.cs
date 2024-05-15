
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
    
}
public class LocationInfo
{
    public string? CountryPart { get; set; }
    public string? County { get; set; }
    public string? Municipality { get; set; }
}
/* Må lage en class basert på hva data som skal hentes inn fra proff. basert på JSON schema. Se på proffjson schema for referanse. */
public class ProffCompanyReturn
{
    public string? CompanyType { get; set; }
    public string? CompanyTypeName { get; set; }
    public string? EstimatedTurnover { get; set; }
    public bool HasSecurity { get; set; }
    public List<string>? PreviousNames { get; set; }
    public string? RegistrationDate { get; set; }
    public string? EstablishedDate { get; set; }
    public string? NumberOfEmployees { get; set; }
    public required string Name { get; set; }
    public required List<AccountsInfo> AnnualAccounts { get; set; }
    public required List<ShareHolderInfo> ShareHolders { get; set; }
    public required LocationInfo Location { get; set; }

}
