
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Models;
using Server.Context;
using MiniExcelLibs;
using Server.Views;
using Server.Util;
using Microsoft.AspNetCore.Authorization;
namespace Server.Controllers;


[ApiController]
[Authorize]
[Route("api")]
public class InsertDataBasedOnExcel(BtdbContext context) : ControllerBase
{
    private readonly BtdbContext _context = context;

    ///<summary>
    ///Posts new data to database based on Excel spreadsheet.
    ///Then updates the DB with data from other apis based on new entries from excel spreadsheet. 
    ///</summary>
    ///<param name="file">Excel spreadsheed following dbupdateTemplate</param>
    ///<returns> OK or Bad Request on missing, or error prone file</returns>
    [HttpPost("updatewithnewdata")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromForm] IFormFile file)

    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new
            {
                error = GlobalLanguage.Language switch
                {
                    "nor" => "Feil filformat. Vennligst sjekk filen eller prøv igjen med en .xlsx fil. Hvis du mangler fil, eller er usikker på formatering, bruk 'Hent mal' knappen under.",
                    "en" => "Invalid format or file type. Please check the file or try again with a .xlsx file. If missing file or unsure how to format, click on the button 'Get Template'.",
                    _ => "Server Error"
                }
            });

        }
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extension != ".xlsx")
        {
            return BadRequest(new
            {
                error = GlobalLanguage.Language switch
                {
                    "nor" => "Feil filformat. Vennligst skjekk filen eller prøv igjen med en .xlsx fil. Hvis du mangler fil, eller er usikker på formatering, bruk 'Hent mal' knappen under.",
                    "en" => "Invalid format or file type. Please check the file or try again with a .xlsx file. If missing file or unsure how to format, click on the button 'Get Template'.",
                    _ => "Server Error"
                }
            });
        }
        List<CompanyInfo> refreshList = _context.CompanyInfos.AsNoTracking().ToList();
        foreach (var item in refreshList)
        {
            _context.Entry(item).Reload();
        }
        try
        {
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
                    return BadRequest(new
                    {
                        error = ex.Message
                    });
                }

                var compactData = CompactedVisBedriftData.ListOfCompactedVisExcelSheet(RawData);

                orgNrArray = CompactedVisBedriftData.GetOrgNrArray(compactData);

                List<int> DbOrgNrArrays = _context.CompanyInfos.Select(b => b.Orgnumber).ToList();

                List<int> NonDuplicateOrgNrs = orgNrArray.Except(DbOrgNrArrays).ToList();

                List<int> DuplicateOrgNrs = orgNrArray.Intersect(DbOrgNrArrays).ToList();

                List<CompactedVisBedriftData> NonDuplicateData = compactData.Where(data => NonDuplicateOrgNrs.Contains(data.Orgnummer)).ToList();

                List<CompactedVisBedriftData> DuplicateData = compactData.Where(data => DuplicateOrgNrs.Contains(data.Orgnummer)).ToList();

                if (NonDuplicateData.Count > 0) CompactedVisBedriftData.AddListToDb(NonDuplicateData, _context);

                if (DuplicateData.Count > 0) CompactedVisBedriftData.UpdateFaseStatus(DuplicateData, _context);
                var now = DateTime.Now.ToLongDateString();
                string filePath = $"./UpdateReport{now}.txt";
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    List<ReturnStructure> paramStructures = await FetchProffData.GetDatabaseValues(NonDuplicateOrgNrs, writer);
                    foreach (var param in paramStructures)
                    {
                        writer.WriteLine($"Adding {param.Name} to DB");
                        try
                        {
                            param.InsertIntoDatabase(_context);
                        }
                        catch (Exception ex)
                        {
                            writer.WriteLine($"Failed to insert param: {ex.Message}");
                        }
                    }
                    writer.WriteLine("Insert Complete, updating delta and views.");
                    try
                    {
                        await _context.Database.ExecuteSqlRawAsync("CALL update_delta()");
                        await _context.Database.ExecuteSqlRawAsync("CALL update_views()");

                    }
                    catch (Exception ex)
                    {
                        writer.WriteLine($"Failed to update delta and views: {ex.Message}");
                    }
                    try
                    {
                        LaborCostFromSSBController ssbFetcher = new(_context);
                        await ssbFetcher.UpdateLabourCost(writer);
                    }
                    catch (Exception ex)
                    {
                        writer.WriteLine($"Failed to update data from SSB: {ex.Message}");
                    }
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
                    writer.WriteLine($"Update complete.");
                    writer.WriteLine("-----------------------");
                }
            }
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = ex.Message
            });
        }

    }
    [HttpPost("deletedata")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new
            {
                error = GlobalLanguage.Language switch
                {
                    "nor" => "Mangler fil.",
                    "en" => "Missing file.",
                    _ => "Server Error",
                }
            });

        }
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extension != ".xlsx")
        {
            return BadRequest(new
            {
                error = GlobalLanguage.Language switch
                {
                    "nor" => "Feil filformat. Vennligst sjekk filen eller prøv igjen med en .xlsx fil. Hvis du mangler fil, eller er usikker på formatering, bruk 'Hent mal' knappen under.",
                    "en" => "Invalid format or file type. Please check the file or try again with a .xlsx file. If missing file or unsure how to format, click on the button 'Get Template'.",
                    _ => "Server Error"
                }
            });
        }
        List<int> orgNrs = [];
        using (var stream = file.OpenReadStream())
        {
            try
            {
                {
                    var rows = await stream.QueryAsync<ExcelOrgNrOnly>();
                    orgNrs = rows.Select(row => row.Orgnummer).ToList();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = GlobalLanguage.Language switch
                    {
                        "nor" => $"Noe gikk galt med å lese filen {ex.Message}",
                        "en" => $"Something went wrong reading the file {ex.Message}",
                        _ => "Server Error"
                    }
                });
            }
        }
        var entriesToDelete = _context.CompanyInfos.Where(b => orgNrs.Contains(b.Orgnumber)).ToList();
        if (entriesToDelete.Count == 0)
        {
            return NotFound(new
            {
                error = GlobalLanguage.Language switch
                {
                    "nor" => "Kunne ikke finne noen organisasjoner som matchet de listede organisasjonsnummerene.",
                    "en" => "Could not find any organisations with the listet organisation numbers",
                    _ => "Server Error"
                }
            });
        }
        _context.CompanyInfos.RemoveRange(entriesToDelete);
        try
        {
            _context.SaveChanges();
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = GlobalLanguage.Language switch
                {
                    "nor" => $"Noe gikk galt ved sletting av data: {ex.Message}",
                    "en" => $"Something went wrong deleting data: {ex.Message}",
                    _ => "Server Error"
                }
            });
        }
    }
}


