using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using Server.Context;
using MiniExcelLibs;
using Microsoft.EntityFrameworkCore;
using Server.Views;
namespace Server.Controllers;

[ApiController]
[Route("fullmodel")]
public class GetFullModelExcel : ControllerBase
{
    private readonly BtdbContext _context;
    public GetFullModelExcel(BtdbContext context)
    {
        _context = context;
    }
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<Results<FileStreamHttpResult, NotFound, StatusCodeHttpResult>> GetFullModel()
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
            var ExcelSheets = new Dictionary<string, object>()
            {
                ["Annonseringer"] = tableList,
                ["Bedrift Info"] = viewList
            };
            if (viewList == null || viewList.Count == 0)
            {
                return TypedResults.NotFound();
            }
            var memStream = new MemoryStream();
            Console.WriteLine("saving to stream");
            await memStream.SaveAsAsync(ExcelSheets);
            Console.WriteLine("save completed");
            memStream.Seek(0, SeekOrigin.Begin);
            return TypedResults.File(memStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"FullView{now}.xlsx");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return TypedResults.StatusCode(500);
        }
    }
}