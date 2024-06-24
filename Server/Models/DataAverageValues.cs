namespace Server.Models;

public class AverageValues
{
    public int Year { get; set; }
    public string? EcoCode { get; set; }
    public string? CodeDescription { get; set; }
    public decimal AvgEcoValue { get; set; }
    public decimal AvgDelta { get; set; }
    public decimal TotalAccumulated { get; set; }
    public int UniqueCompanyCount { get; set; }
}