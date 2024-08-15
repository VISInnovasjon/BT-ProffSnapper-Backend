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
        Dictionary<string, int> newData;
        try
        {
            newData = await FetchSSBData.GetSSBData();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = ex.Message
            });
        }
        foreach (var data in newData)
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
        return Ok(new
        {
            success = "Update Complete"
        });


    }
}