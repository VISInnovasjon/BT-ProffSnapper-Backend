
namespace Server.Util;

public class QueryParamsForTable
{
    public string? EcoCode { get; set; }
    public string? Year { get; set; }
    public bool Valid()
    {
        return !string.IsNullOrEmpty(EcoCode) && !string.IsNullOrEmpty(Year);
    }
}

public class QueryParamsForLang
{
    public string? Language { get; set; }
    public static bool CheckLangParamOnly(QueryParamsForLang query)
    {
        return !string.IsNullOrEmpty(query.Language) && (string.Equals(query.Language, "en") || string.Equals(query.Language, "nor"));
    }
}
public class QueryParamForYear
{
    private readonly int _currentYear = DateTime.Now.Year;
    public string? EndYear { get; set; }
    public string? StartYear { get; set; }
    public string? Language { get; set; }
    public ExtractedParams ExtractParams()
    {
        return new ExtractedParams
        {
            EndYear = string.IsNullOrEmpty(EndYear) ? _currentYear : int.Parse(EndYear),
            StartYear = string.IsNullOrEmpty(StartYear) ? 2014 : int.Parse(StartYear),
            Language = string.IsNullOrEmpty(Language) ? "nor" : Language.ToString()
        };
    }
    public class ExtractedParams
    {
        public int EndYear { get; set; }
        public int StartYear { get; set; }
        public required string Language { get; set; }
    }
}