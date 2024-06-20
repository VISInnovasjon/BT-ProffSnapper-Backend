using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using Server.Context;
using MiniExcelLibs;
using Microsoft.EntityFrameworkCore;
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
            var now = DateTime.Now.Date;
            var viewList = _context.FullViews.ToList();
            if (viewList == null || viewList.Count == 0)
            {
                return TypedResults.NotFound();
            }
            var memStream = new MemoryStream();
            Console.WriteLine("saving to stream");
            await memStream.SaveAsAsync(viewList);
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