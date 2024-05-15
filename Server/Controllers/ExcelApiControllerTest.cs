using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Util.InitVisData;
using Util.DB;
using Npgsql;
namespace Server.Controllers;

[ApiController]
[Route("/ExcelUpload")]
public class ExcelTestController : ControllerBase
{
    [HttpPost("UpdateNewData")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromForm] IFormFile file)
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
                jsonData = JsonSerializer.Serialize(compactData);
            }
            return Ok($"Stream successfully parsed: \n {jsonData}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }

    }
}