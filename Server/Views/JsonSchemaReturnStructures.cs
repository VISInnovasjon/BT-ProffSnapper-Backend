

namespace Server.Views;

public class Values
{
    public string EcoCode { get; set; }
    public decimal Value { get; set; }
    public decimal Delta { get; set; }
    public Values(
        string code, decimal value, decimal delta
    )
    {
        EcoCode = code;
        Value = value;
        Delta = delta;
    }
}
public class ExtractedEcoCodeValues
{
    public decimal Value { get; set; }
    public decimal Delta { get; set; }
    public decimal Accumulated { get; set; }
    public int UniqueCompanyCount { get; set; }
    public ExtractedEcoCodeValues(
        decimal value, decimal delta, decimal acc, int count
    )
    {
        Value = value;
        Delta = delta;
        Accumulated = acc;
        UniqueCompanyCount = count;
    }
}
public class YearDataGroup
{
    public int Year { get; set; }
    public Dictionary<string, ExtractedEcoCodeValues>? values { get; set; }
}

public class TableData
{
    public string? Name { get; set; }
    public int OrgNumber { get; set; }
    public string? Branch { get; set; }
    public decimal? Accumulated { get; set; }
    public decimal? Value { get; set; }
    public decimal? Delta { get; set; }
    public int ValidYear { get; set; }
    public string? EcoCode { get; set; }
}
