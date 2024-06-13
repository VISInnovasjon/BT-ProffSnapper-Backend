

using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Server.Models;
using Server.Views;

namespace Server.Controllers;


[ApiController]
[Route("query")]
public class QueryHandler(BtdbContext context) : ControllerBase
{
    private readonly BtdbContext _context = context;

    [HttpGet("getall")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult FetchAll()
    {
        try
        {
            var FinalDict = new Dictionary<string, List<YearDataGroup>>{
                {"Total Gjennomsnitt", FetchYearlyData(_context)}
            };
            /* For å legge til flere views må det bare legges til en ny AddGroupedData for hver view her. */
            AddGroupedData(FinalDict, _context.DataSortertEtterFases.ToList(), b => b.Fase);
            AddGroupedData(FinalDict, _context.DataSortertEtterBransjes.ToList(), b => b.Bransje);
            AddGroupedData(FinalDict, _context.DataSortertEtterAldersGruppes.ToList(), b => b.AldersGruppe);
            var JsonString = JsonSerializer.Serialize(FinalDict);
            return Ok(JsonString);

        }
        catch (Exception ex)
        {
            var error = new
            {
                Message = "An Error Occured.",
                Details = ex.Message + " " + ex.Data.ToString()

            };
            return BadRequest(error);
        }
    }
    private List<YearDataGroup> FetchYearlyData(BtdbContext _context)
    {
        List<YearDataGroup>? groupedData;
        var data = _context.GjennomsnittVerdiers.ToList();
        groupedData = data
            .GroupBy(b => b.RapportÅr)
            .Select(g => new YearDataGroup
            {
                Year = g.Key,
                values = g.ToDictionary(
                    b => b.ØkoKode ?? "Code Missing",
                    b => new ExtractedEcoCodeValues(
                        b.AvgØkoVerdi, b.AvgDelta, b.KodeBeskrivelse
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
                .GroupBy(b => (int)GetPropertyValue(b ?? throw new NullReferenceException($"Missing Object Reference {b}"), "RapportÅr"))
                .Select(g => new YearDataGroup
                {
                    Year = g.Key,
                    values = g.ToDictionary(
                        b => (string)GetPropertyValue(b ?? throw new NullReferenceException($"Missing Object Reference {b}"), "ØkoKode") ?? "Kode Mangler",
                          b => new ExtractedEcoCodeValues(
                            (decimal)GetPropertyValue(b ?? throw new NullReferenceException($"Missing Object Reference {b}"), "AvgØkoVerdi"),
                            (decimal)GetPropertyValue(b, "AvgDelta"),
                            (string)GetPropertyValue(b, "KodeBeskrivelse")
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



