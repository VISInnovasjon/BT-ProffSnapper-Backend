namespace Server.Models;

public partial class AvgLaborCostPrYear
{
    public int Year { get; set; }
    public int Value { get; set; }
    public decimal? TotalManYear { get; set; }
}