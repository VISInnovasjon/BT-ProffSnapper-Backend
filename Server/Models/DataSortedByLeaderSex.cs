namespace Server.Models;

public class DataSortedByLeaderSex
{
    public string? Sex { get; set; }
    public int Year { get; set; }
    public string? EcoCode { get; set; }
    public decimal AvgEcoValue { get; set; }
    public decimal AvgDelta { get; set; }
    public decimal TotalAccumulated { get; set; }
    public int UniqueCompanyCount { get; set; }
}