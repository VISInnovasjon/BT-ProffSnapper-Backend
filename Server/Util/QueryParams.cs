
namespace Server.Util;

public class QueryParamsForTable
{
    public string? EcoCode { get; set; }
}
public class QueryParamsForLang
{
    public string? Language { get; set; }
    public static bool CheckQueryParams(string? param)
    {
        return string.Equals(param, "en") || string.Equals(param, "nor");
    }
}