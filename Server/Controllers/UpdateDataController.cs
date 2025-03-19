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
    [HttpGet("scheduleupdate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ScheduleUpdate()
    {
        var now = DateTime.Now.ToLongDateString();
        var filePath = $"./UpdateReport{now}.txt";
        var orgNrList = context.CompanyInfos.Where(b => b.Liquidated != true).Select(b => b.Orgnumber).ToList();
        List<ReturnStructure> fetchedData = [];
        await using var writer = new StreamWriter(filePath, true);
        if (orgNrList.Count > 0) fetchedData = await FetchProffData.GetDatabaseValues(orgNrList, writer);
        var rawData = fetchedData.Select(data => data.ConvertToRawData());
        await context.RawDataFromProff.AddRangeAsync(rawData);

        if (fetchedData.Count > 0)
        {
            foreach (var param in fetchedData)
            {
                await writer.WriteLineAsync($"Adding {param.Name} to DB");
                try
                {
                    await param.InsertIntoDatabase(context);
                }
                catch (Exception ex)
                {
                    await writer.WriteLineAsync($"Error passing param to database: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        await writer.WriteLineAsync($"Inner Exception: {ex.InnerException.Message}");
                    }

                }
            }
            await writer.WriteLineAsync("Insert Complete, updating views.");
            try
            {
                await context.Database.ExecuteSqlRawAsync("CALL update_delta()");
                await context.Database.ExecuteSqlRawAsync("CALL update_views()");

            }
            catch (Exception ex)
            {
                await writer.WriteLineAsync($"Error updating views: {ex.Message}");
            }
        }
        LaborCostFromSSBController ssbController = new(context);
        await ssbController.UpdateLabourCost(writer);
        try
        {
            context.LastUpdates.Add(new LastUpdate
            {
                UpdateDate = DateTime.Now
            });
        }
        catch (Exception ex)
        {
            await writer.WriteLineAsync(ex.Message);
        }
        await writer.WriteLineAsync($"Scheduler updated.");
        await writer.WriteLineAsync("--------------------------------");
        return Ok();
    }
    [HttpPost("updateecocode")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateEcoCode([FromForm] IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Missing File");
        }
        var extention = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extention != "json") return BadRequest("Please submit a JSON file.");
        Dictionary<string, string>? ecoCodes;
        await using (var stream = file.OpenReadStream())
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

                context.EcoCodeLookups.Add(
                    new EcoKodeLookup()
                    {
                        EcoCode = pair.Key,
                        Nor = pair.Value
                    }
                );
            }
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500);
        }
        return Ok();
    }
}