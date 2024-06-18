
namespace Server.Views;

public class Values
{
    public string EcoCode { get; set; }
    public decimal Value { get; set; }
    public decimal Delta { get; set; }
    public string? Description { get; set; }
    public Values(
        string code, decimal value, decimal delta, string? desc
    )
    {
        EcoCode = code;
        Value = value;
        Delta = delta;
        Description = desc;
    }
}
public class ExtractedEcoCodeValues
{
    public decimal Value { get; set; }
    public decimal Delta { get; set; }
    public decimal Accumulated { get; set; }
    public string? Description { get; set; }
    public ExtractedEcoCodeValues(
        decimal value, decimal delta, decimal acc, string? desc
    )
    {
        Value = value;
        Delta = delta;
        Accumulated = acc;
        Description = desc;
    }
}
public class YearDataGroup
{
    public int Year { get; set; }
    public Dictionary<string, ExtractedEcoCodeValues>? values { get; set; }
}
