using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Util.InitVisData;
using Util.ProffApiClasses;
using Util.ProffFetch;

namespace Server.Controllers;

[ApiController]
[Route("/ExcelUpload")]
public class ExcelTestController : ControllerBase
{
    [HttpPost("UpdateNewData")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded");

        }
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extension != ".xlsx")
        {
            return BadRequest("Invalid Fileformat. Please upload an Excel file");
        }
        try
        {
            string jsonData;
            using (var stream = file.OpenReadStream())
            {
                List<RawVisBedriftData> RawData;
                List<int> orgNrArray;
                try
                {
                    RawData = RawVisBedriftData.ListFromVisExcelSheet(stream, "Bedrifter");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                var compactData = CompactedVisBedriftData.ListOfCompactedVisExcelSheet(RawData);
                CompactedVisBedriftData.AddListToDb(compactData);
                orgNrArray = CompactedVisBedriftData.GetOrgNrArray(compactData);
                List<SqlParamStructure> paramStructures = await FetchProffData.GetDatabaseValues(orgNrArray);
                foreach (var param in paramStructures)
                {
                    Console.WriteLine($"Adding {param.Name} to DB");
                    param.AddParamToDb();
                    await Task.Delay(100);
                }
                jsonData = JsonSerializer.Serialize(compactData);
            }
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }

    }
}