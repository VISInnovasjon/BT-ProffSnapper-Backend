using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using Server.Context;
using MiniExcelLibs;
using Microsoft.EntityFrameworkCore;
using Server.Views;
namespace Server.Controllers;

[ApiController]
[Route("api")]
public class GetFullModelExcel : ControllerBase
{
    private readonly BtdbContext _context;
    public GetFullModelExcel(BtdbContext context)
    {
        _context = context;
    }
    [HttpGet("excelfullview")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFullModel()
    {
        try
        {
            var now = DateOnly.FromDateTime(DateTime.Now);
            var viewList = _context.FullViews.ToList();
            var announcementList = await _context.CompanyAnnouncements
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
            var shareholderList = await _context.CompanyShareholderInfos.Include(p => p.Company).ToListAsync();
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
            if (viewList == null || viewList.Count == 0)
            {
                return NotFound();
            }
            var memStream = new MemoryStream();
            Console.WriteLine("saving to stream");
            await memStream.SaveAsAsync(ExcelSheets);
            Console.WriteLine("save completed");
            memStream.Seek(0, SeekOrigin.Begin);
            return File(memStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"FullView{now}.xlsx");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, new
            {
                error = GlobalLanguage.Language switch
                {
                    "nor" => "Noe gikk galt.",
                    "en" => "Something went wrong",
                    _ => "Server Error"
                }
            });
        }
    }
}