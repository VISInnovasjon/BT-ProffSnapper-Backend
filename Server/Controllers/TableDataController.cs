
using Server.Context;
using Server.Util;
using Server.Views;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Server.Controllers;

[ApiController]
[Route("tabledata")]
public class TableDataController : ControllerBase
{
    private readonly BtdbContext _context;
    public TableDataController(BtdbContext context)
    {
        _context = context;
    }
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SendTableData([FromQuery] QueryParams query)
    {
        Console.WriteLine(query.EcoCode);
        var topPerformers = await _context.ÅrligØkonomiskData
                                .Where(p => p.ØkoKode == query.EcoCode && p.Rapportår == DateTime.Now.Year - 2)
                                .OrderByDescending(p => p.Akkumulert)
                                .Take(20)
                                .Include(p => p.Bedrift)
                                .ToListAsync();
        if (topPerformers == null) return NotFound();
        else
        {
            List<TableData> dataList = [];
            foreach (var performer in topPerformers)
            {
                dataList.Add(new()
                {
                    Name = performer.Bedrift.Målbedrift,
                    OrgNumber = performer.Bedrift.Orgnummer,
                    Branch = performer.Bedrift.Bransje,
                    Value = performer.ØkoVerdi,
                    Delta = performer.Delta,
                    Accumulated = performer.Akkumulert,
                    ValidYear = performer.Rapportår,
                    EcoCode = performer.ØkoKode
                });
            }
            try
            {
                /*   var options = new JsonSerializerOptions
                  {
                      ReferenceHandler = ReferenceHandler.Preserve,
                      DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                  }; */
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
}