using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Models;
namespace Server.Controllers;
/* List<ReturnStructure> paramStructures = await FetchProffData.GetDatabaseValues(orgNrArray); */

[ApiController]
[Route("/updatedb")]
public class ExcelTestController : ControllerBase
{
    private readonly BtdbContext _context;
    public ExcelTestController(BtdbContext context)
    {
        _context = context;
    }
    private readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    [HttpPost("newdata")]
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
                    RawData = await RawVisBedriftData.ListFromVisExcelSheet(stream, "Bedrifter");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

                var compactData = CompactedVisBedriftData.ListOfCompactedVisExcelSheet(RawData);
                CompactedVisBedriftData.AddListToDb(compactData, _context);
                orgNrArray = CompactedVisBedriftData.GetOrgNrArray(compactData);
                List<ReturnStructure> paramStructures = [];
                string contentPath = "./LocalData";
                ReturnStructure? Data = null;
                foreach (string filename in Directory.GetFiles(contentPath, "*.json"))
                {
                    string jsonContent = System.IO.File.ReadAllText(filename);
                    try
                    {
                        Data = JsonSerializer.Deserialize<ReturnStructure>(jsonContent, jsonOptions);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    if (Data != null)
                    {
                        Console.WriteLine($"Adding {Data.Name} To List");
                        paramStructures.Add(Data);
                    };
                }
                foreach (var param in paramStructures)
                {
                    Console.WriteLine($"Adding {param.Name} to DB");
                    try
                    {
                        param.InsertToDataBase(_context);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                try
                {
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("Insert Complete, updating delta.");
                try
                {
                    _context.Database.ExecuteSqlRaw("SELECT update_delta()");

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                jsonData = JsonSerializer.Serialize(paramStructures);
            }
            return Ok(jsonData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }

    }
}

