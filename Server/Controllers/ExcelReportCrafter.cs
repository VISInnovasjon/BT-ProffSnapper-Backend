
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using MiniExcelLibs;
using Server.Views;
using Server.Context;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
namespace Server.Controllers;

[ApiController]
[Route("yearlyreport")]
public class GenÅrsRapport(BtdbContext context) : ControllerBase
{
    private readonly BtdbContext _context = context;
    ///<summary>
    ///Takes in an excel file of orgnumbers and generates reports on each.
    ///</summary>
    ///<param name="file">Excel http FileStream</param>
    ///<returns> Excel filestream with reports, or 404 not found</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<Results<FileStreamHttpResult, NotFound>> ExportExcel([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new BadHttpRequestException("Missing xlsx file");
        }
        var extention = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extention != ".xlsx")
        {
            throw new BadHttpRequestException("Only xslx files are supported currently");
        }
        List<int> orgNrs = [];
        using (var stream = file.OpenReadStream())
        {
            var rows = await stream.QueryAsync<ExcelOrgNrOnly>();
            orgNrs = rows.Select(row => row.Orgnummer).ToList();
        }
        var now = DateOnly.FromDateTime(DateTime.Now);
        var companyIds = _context.CompanyInfos.Where(p => orgNrs.Contains(p.Orgnumber)).Select(p => p.CompanyId).ToList();
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
        var ExcelSheets = new Dictionary<string, object>()
        {
            ["Annonseringer"] = tableList,
            ["Bedrift Info"] = viewList
        };
        if (viewList == null || viewList.Count == 0 || tableList == null || tableList.Count == 0)
        {
            return TypedResults.NotFound();
        }
        var memStream = new MemoryStream();
        Console.WriteLine("saving to stream");
        await memStream.SaveAsAsync(ExcelSheets);
        Console.WriteLine("save completed");
        memStream.Seek(0, SeekOrigin.Begin);
        return TypedResults.File(memStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"YearlyReport{now}.xlsx");
    }
}