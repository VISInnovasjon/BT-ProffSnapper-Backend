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
        var now = DateTime.Now.ToLongDateString();
        string filePath = $"./UpdateReport{now}.txt";
        List<int> orgNrList = context.CompanyInfos.Where(b => b.Liquidated != true).Select(b => b.Orgnumber).ToList();
        List<ReturnStructure> fetchedData = [];
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            if (orgNrList.Count > 0) fetchedData = await FetchProffData.GetDatabaseValues(orgNrList, writer);

            if (fetchedData.Count > 0)
            {
                foreach (var param in fetchedData)
                {
                    writer.WriteLine($"Adding {param.Name} to DB");
                    try
                    {
                        param.InsertIntoDatabase(context);
                    }
                    catch (Exception ex)
                    {
                        writer.WriteLine($"Error passing param to database: {ex.Message}");
                        if (ex.InnerException != null)
                        {
                            writer.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                        }
                    }
                }
                writer.WriteLine("Insert Complete, updating views.");
                try
                {
                    context.Database.ExecuteSqlRaw("CALL update_delta()");
                    context.Database.ExecuteSqlRaw("CALL update_views()");

                }
                catch (Exception ex)
                {
                    writer.WriteLine($"Error updating views: {ex.Message}");
                }
            }
            LaborCostFromSSBController ssbController = new(context);
            await ssbController.UpdateLabourCost(writer);
            try
            {
                _context.LastUpdates.Add(new LastUpdate
                {
                    UpdateDate = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                writer.WriteLine(ex.Message);
            }
            writer.WriteLine($"Scheduler updated.");
            writer.WriteLine("--------------------------------");
            return Ok();
        }
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