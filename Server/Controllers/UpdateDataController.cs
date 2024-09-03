using Server.Views;
using Server.Util;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Server.Context;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
namespace Server.Controllers;


[ApiController]
[Route("api")]
public class UpdateHandler(BtdbContext context) : ControllerBase
{
    private readonly BtdbContext _context = context;

    [HttpGet("scheduleupdate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateSchedule()
    {
        List<int> orgNrList = _context.CompanyInfos.Where(b => b.Liquidated != true).Select(b => b.Orgnumber).ToList();
        List<ReturnStructure> fetchedData = [];
        if (orgNrList.Count > 0) fetchedData = await FetchProffData.GetDatabaseValues(orgNrList);
        if (fetchedData.Count > 0)
        {
            try
            {
                foreach (var param in fetchedData)
                    param.InsertIntoDatabase(_context);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = ex.Message
                });
            }
            Console.WriteLine("Insert Complete, updating delta.");
            try
            {
                await _context.Database.ExecuteSqlRawAsync("CALL update_delta()");
                await _context.Database.ExecuteSqlRawAsync("CALL update_views()");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500);
            }
        }
        return Ok();
    }
    [HttpPost("updateecocode")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateEcoCodes([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Missing File");
        }
        var extention = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extention != "json") return BadRequest("Please submit a JSON file.");
        Dictionary<string, string>? ecoCodes;
        using (var stream = file.OpenReadStream())
        {
            ecoCodes = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(stream);
        }
        if (ecoCodes == null || ecoCodes.Count == 0)
        {
            return BadRequest("Please include the eco codes you want to add");
        }
        try
        {
            foreach (var pair in ecoCodes)
            {

                _context.EcoCodeLookups.Add(
                    new EcoKodeLookup()
                    {
                        EcoCode = pair.Key,
                        Nor = pair.Value
                    }
                );
            }
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500);
        }
        return Ok();
    }
}