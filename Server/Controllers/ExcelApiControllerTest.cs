using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Util.InitVisData;
using Util.ProffApiClasses;
using Util.ProffFetch;
using Util.DB;
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
                List<ReturnStructure> paramStructures = new();
                /* List<ReturnStructure> paramStructures = await FetchProffData.GetDatabaseValues(orgNrArray); */
                string contentPath = "./LocalData";
                ReturnStructure? Data = null;
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                foreach (string filename in Directory.GetFiles(contentPath, "*.json"))
                {
                    string jsonContent = System.IO.File.ReadAllText(filename);
                    try
                    {
                        Data = JsonSerializer.Deserialize<ReturnStructure>(jsonContent, options);
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
                        param.InsertToDataBase();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                Console.WriteLine("Insert Complete, updating delta.");
                try
                {
                    Database.Query("SELECT update_delta()", reader => { });
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





/* TEST KODE */
/* ReturnStructure returnStructure = TestJsonStream.ParseJsonStream();
                UpdateNameStructure nameStructure = new(
                    returnStructure.CompanyId, returnStructure.Name, returnStructure.PreviousNames.Count == 0 ? null : returnStructure.PreviousNames
                );
                nameStructure.InsertIntoDatabase();
                InsertGenerellInfoStructure infoStructure = new(
                    returnStructure.CompanyId, returnStructure.ShareholdersLastUpdatedDate, returnStructure.Location, returnStructure.PostalAddress, returnStructure.NumberOfEmployees ?? null
                );
                infoStructure.InsertToDataBase();
                foreach (var announcement in returnStructure.Announcements)
                {
                    InsertKunngjøringStructure kunngjøringStructure = new(
                        returnStructure.CompanyId, announcement
                    );
                    kunngjøringStructure.InsertToDataBase();
                }
                foreach (var account in returnStructure.CompanyAccounts)
                {
                    ØkoDataSqlStructure økoData = new(
                        returnStructure.CompanyId, account
                    );
                    økoData.InsertIntoDatabase();
                }
                foreach (var person in returnStructure.PersonRoles)
                {
                    if (person.TitleCode != "DAGL" && person.TitleCode != "LEDE") continue;
                    else
                    {
                        InsertBedriftLederInfoStructure bedriftLeder = new(
                            returnStructure.CompanyId, returnStructure.ShareholdersLastUpdatedDate, person
                        );
                        bedriftLeder.InsertToDataBase();
                    }
                }
                foreach (var shareholder in returnStructure.Shareholders)
                {
                    InsertShareholderStructure shareholderStructure = new(
                        returnStructure.CompanyId, returnStructure.ShareholdersLastUpdatedDate, shareholder
                    );
                    shareholderStructure.InsertIntoDatabase();
                }
                try
                {
                    Database.Query("SELECT update_delta()", reader => { });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
 */