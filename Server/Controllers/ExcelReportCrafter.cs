
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;
using Server.Views;
using Server.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
namespace Server.Controllers;

[Authorize]
[ApiController]
[Route("api")]
public class GenYearlyReport(BtdbContext context) : ControllerBase
{
    private readonly BtdbContext _context = context;
    ///<summary>
    ///Takes in an excel file of orgnumbers and generates reports on each.
    ///</summary>
    ///<param name="file">Excel http FileStream</param>
    ///<returns> Excel filestream with reports, or 404 not found</returns>
    [HttpPost("yearlyreport")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ExportExcel([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
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
        var extention = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extention != ".xlsx")
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
        List<int> orgNrs = [];
        using (var stream = file.OpenReadStream())
        {
            var rows = await stream.QueryAsync<ExcelOrgNrOnly>();
            orgNrs = rows.Select(row => row.Orgnummer).ToList();
        }
        var now = DateOnly.FromDateTime(DateTime.Now);
        var companyIds = _context.CompanyInfos.Where(p => orgNrs.Contains(p.Orgnumber)).Select(p => p.CompanyId).ToList();
        if (companyIds.Count == 0 || companyIds == null)
        {
            string orgNrString = "";
            orgNrs.ForEach(nr =>
            {
                orgNrString += $"{nr} ";
            });
            return NotFound(new
            {
                error = GlobalLanguage.Language switch
                {
                    "nor" => $"Fant ingen data for {orgNrString}",
                    "en" => $"Found no data for {orgNrString}",
                    _ => "Server Error"
                }
            });
        }
        var viewList = _context.FullViews.Where(p => p.Year == DateTime.Now.Year - 1 && orgNrs.Contains(p.Orgnumber)).ToList();
        var announcementList = await _context.CompanyAnnouncements
                                        .Where(p => p.Date.HasValue && p.Date.Value.Year == DateTime.Now.Year - 1 && companyIds.Contains(p.CompanyId))
                                        .Include(p => p.Company)
                                        .ToListAsync();
        List<AnnouncementTable> tableList = [];
        foreach (var announcement in announcementList)
        {
            tableList.Add(
                new()
                {
                    Orgnumber = announcement.Company.Orgnumber,
                    Name = announcement.Company.CompanyName,
                    Id = announcement.AnnouncementId,
                    Type = announcement.AnnouncementType,
                    Description = announcement.AnnouncementText,
                    Date = announcement.Date
                }
            );
        }
        var shareholderList = await _context.CompanyShareholderInfos.Where(p => p.Year == DateTime.Now.Year - 1 && companyIds.Contains(p.CompanyId) && p.ShareholdeCompanyId == "987753153").Include(p => p.Company).ToListAsync();
        List<ShareHolderTable> shareTable = [];
        foreach (var shareholder in shareholderList)
        {
            shareTable.Add(
                new()
                {
                    Orgnumber = shareholder.Company.Orgnumber,
                    CompanyName = shareholder.Company.CompanyName,
                    Year = shareholder.Year,
                    ShareholderCompanyId = shareholder.ShareholdeCompanyId,
                    ShareholderName = shareholder.Name,
                    NumberOfShares = shareholder.NumberOfShares,
                    PercentageShares = shareholder.PercentageShares
                }
            );
        }
        var ExcelSheets = new Dictionary<string, object>()
        {
            ["Annonseringer"] = tableList,
            ["Bedrift Info"] = viewList,
            ["Shareholder Info"] = shareTable
        };
        var memStream = new MemoryStream();
        Console.WriteLine("saving to stream");
        await memStream.SaveAsAsync(ExcelSheets);
        Console.WriteLine("save completed");
        memStream.Seek(0, SeekOrigin.Begin);
        return File(memStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"YearlyReport{now}.xlsx");
    }
}