

using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Server.Context;
using Server.Views;
using Server.Util;

namespace Server.Controllers;


[ApiController]
[Route("api")]
public class QueryHandler(BtdbContext context) : ControllerBase
{
    private readonly BtdbContext _context = context;
    ///<summary>
    ///Accumulates all grouped data used by graph from database.
    ///</summary>
    ///<returns>Json object of all accumulated data grouped by keys</returns>
    [HttpGet("graphdata")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult FetchAll()
    {
        try
        {
            var FinalDict = new Dictionary<string, List<YearDataGroup>>{
                {"Total", FetchYearlyData(_context)}
            };
            /* For å legge til flere views må det bare legges til en ny AddGroupedData for hver view her. Add grouped data fungerer foreløbig kun hvis group er en string.*/
            AddGroupedData(FinalDict, _context.DataSortedByPhases.ToList(), b => b.Phase);
            AddGroupedData(FinalDict, _context.DataSortedByCompanyBranches.ToList(), b => b.Branch);
            AddGroupedData(FinalDict, _context.DataSortedByLeaderAges.ToList(), b => b.AgeGroup);
            AddGroupedData(FinalDict, _context.DataSortedByLeaderSexes.ToList(), b => b.Sex);
            var JsonString = JsonSerializer.Serialize(FinalDict);
            return Ok(JsonString);

        }
        catch (Exception ex)
        {
            var error = new
            {
                Message = "An Error Occured.",
                Details = ex.Message

            };
            return StatusCode(500, error);
        }
    }

    private List<YearDataGroup> FetchYearlyData(BtdbContext _context)
    {
        List<YearDataGroup>? groupedData;
        var data = _context.AverageValues.ToList();
        groupedData = data
            .GroupBy(b => b.Year)
            .Select(g => new YearDataGroup
            {
                Year = g.Key,
                values = g.ToDictionary(
                    b => b.EcoCode ?? "Code Missing",
                    b => new ExtractedEcoCodeValues(
                        b.AvgEcoValue, b.AvgDelta, b.TotalAccumulated, b.UniqueCompanyCount
                    )
                )
            }).ToList();
        return groupedData;
    }
    private void AddGroupedData<T>(Dictionary<string, List<YearDataGroup>> finalDict, List<T> dataList, Func<T, string?> groupSelector) where T : class
    {
        var groups = dataList.Select(groupSelector).Distinct().ToList();
        foreach (var group in groups)
        {
            if (!string.IsNullOrEmpty(group))
            {
                var data = dataList.Where(b => groupSelector(b) == group).ToList();
                finalDict[group] = FetchGroupData(data);
            }
        }
    }

    private List<YearDataGroup> FetchGroupData<T>(List<T> dataList)
    {
        var groupedData = dataList
                .GroupBy(b => (int)GetPropertyValue(b ?? throw new NullReferenceException($"Missing Object Reference {b}"), "Year"))
                .Select(g => new YearDataGroup
                {
                    Year = g.Key,
                    values = g.ToDictionary(
                        b => (string)GetPropertyValue(b ?? throw new NullReferenceException($"Missing Object Reference {b}"), "EcoCode") ?? "Code Missing",
                          b => new ExtractedEcoCodeValues(
                            (decimal)GetPropertyValue(b ?? throw new NullReferenceException($"Missing Object Reference {b}"), "AvgEcoValue"),
                            (decimal)GetPropertyValue(b, "AvgDelta"),
                            (decimal)GetPropertyValue(b, "TotalAccumulated"),
                            (int)GetPropertyValue(b, "UniqueCompanyCount")
                        )
                    )
                })
                .ToList();
        return groupedData;
    }
    private object GetPropertyValue(object obj, string propertyName)
    {
        var property = obj.GetType().GetProperty(propertyName) ?? throw new NullReferenceException("Missing Property");
        return property.GetValue(obj) ?? throw new NullReferenceException("PropertyValue is null");
    }
}



