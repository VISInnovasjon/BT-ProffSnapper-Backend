
using Server.Context;
using Server.Util;
using Server.Views;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Server.Controllers;

[ApiController]
[Route("api")]
public class TableDataController : ControllerBase
{
    private readonly BtdbContext _context;
    public TableDataController(BtdbContext context)
    {
        _context = context;
    }
    [HttpGet("tabledata")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SendTableData([FromQuery] QueryParams query)
    {
        if (string.IsNullOrEmpty(query.EcoCode)) return BadRequest("Missing EcoCode");
        List<Models.CompanyEconomicDataPrYear>? topPerformers = null;
        try
        {
            topPerformers = await _context.CompanyEconomicDataPrYears
                                    .Where(p => p.EcoCode == query.EcoCode && p.Year == DateTime.Now.Year - 2)
                                    .OrderByDescending(p => p.Accumulated)
                                    .Take(20)
                                    .Include(p => p.Company)
                                    .ToListAsync();
            if (topPerformers == null) return NotFound();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500);
        }
        List<TableData> dataList = [];
        foreach (var performer in topPerformers)
        {
            dataList.Add(new()
            {
                Name = performer.Company.CompanyName,
                OrgNumber = performer.Company.Orgnumber,
                Branch = performer.Company.Branch,
                Value = performer.EcoValue / 1000,
                Delta = performer.Delta / 1000,
                Accumulated = performer.Accumulated / 1000,
                ValidYear = performer.Year,
                EcoCode = performer.EcoCode
            });
        }
        try
        {
            string JsonReturn = JsonSerializer.Serialize(dataList);
            return Ok(JsonReturn);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500);
        }

    }
}