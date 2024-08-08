
namespace Server.Util;

public class QueryParamsForTable
{
    public string? EcoCode { get; set; }
}
public class QueryParamsForLang
{
    public string? Language { get; set; }
    public static bool CheckLangParamOnly(QueryParamsForLang query)
    {
        return !string.IsNullOrEmpty(query.Language) && string.Equals(query.Language, "en") || string.Equals(query.Language, "nor");
    }
}
public class QueryParamForYear
{
    public string? Year { get; set; }
    public static bool CheckYearParam(QueryParamForYear query)
    {
        return !string.IsNullOrEmpty(query.Year);
    }
}