using Server.Models;

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
