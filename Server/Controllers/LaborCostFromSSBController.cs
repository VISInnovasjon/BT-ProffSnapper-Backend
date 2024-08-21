using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Context;
using Server.Models;
using Server.Util;
namespace Server.Controllers;

[ApiController]
[Route("api")]
public class LaborCostFromSSBController(BtdbContext context) : ControllerBase
{
    private readonly BtdbContext _context = context;
    [HttpGet("updatelaborcost")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateLabourCost()
    {
        SSBJsonStat newData;
        try
        {
            newData = await FetchSSBData.GetSSBDataAvgBasedOnDepth();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = ex.Message
            });
        }
        Dictionary<string, int> laborCostData = newData.GetAvgDataBasedOnDepth();
        List<string> yearLabels = laborCostData.Keys.ToList();
        int convFirstYear = int.Parse(yearLabels.First());
        int rangeLength = int.Parse(yearLabels.Last()) + 2 - convFirstYear;
        List<int> yearKeys = Enumerable.Range(convFirstYear, rangeLength).ToList();
        int ValidDataPoint = 0;

        for (int i = 0; i < yearKeys.Count; i++)
        {
            string key = yearKeys[i].ToString();
            if (laborCostData.TryGetValue(key, out int value))
            {
                ValidDataPoint = value;
            }
            else
            {
                ValidDataPoint += ValidDataPoint * 3 / 100;
            }
            var newValue = new AvgLaborCostPrYear()
            {
                Year = yearKeys[i],
                Value = ValidDataPoint,
            };
            try
            {
                await _context.AvgLaborCostPrYears.AddAsync(
                    newValue
            );
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                Console.WriteLine("Caught update exception");
                await ConflictHandler.HandleConflicts(_context, newValue);
            }
        }
        try
        {
            await _context.Database.ExecuteSqlRawAsync("CALL update_total_man_year()");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        var jsonTestData = newData.GroupedDataByKey();
        return Ok(new
        {
            success = "Update Complete",
            data = JsonSerializer.Serialize(jsonTestData)
        });


    }
}