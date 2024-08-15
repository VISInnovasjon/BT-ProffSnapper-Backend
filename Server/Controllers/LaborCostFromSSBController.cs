using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
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
        foreach (var data in laborCostData)
        {
            var newValue = new AvgLaborCostPrYear()
            {
                Year = data.Key,
                Value = data.Value
            };
            try
            {
                await _context.AvgLaborCostPrYears.AddAsync(
                    newValue
            );
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                await ConflictHandler.HandleConflicts(_context, newValue);
            }
        }
        var jsonTestData = newData.GroupedDataByKey();
        return Ok(new
        {
            success = "Update Complete",
            data = JsonSerializer.Serialize(jsonTestData)
        });


    }
}