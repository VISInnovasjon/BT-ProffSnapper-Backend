
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using MiniExcelLibs;
using Server.Views;
using Microsoft.AspNetCore.Http.HttpResults;
namespace Server.Controllers;

[ApiController]
[Route("årsrapport")]
public class GenÅrsRapport : ControllerBase
{
    private BtdbContext _context;
    public GenÅrsRapport(BtdbContext context)
    {
        _context = context;
    }
    [HttpPost("get")]
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
        string now = DateTime.Now.Date.ToShortDateString();
        List<Årsrapport> dataList;
        dataList = _context.Årsrapports.Where(b => orgNrs.Contains(b.Orgnummer)).ToList();
        List<ExcelÅrsrapport> Årsrapportdata = ExcelÅrsrapport.GetExportValues(dataList);
        if (dataList == null || dataList.Count == 0)
        {
            return TypedResults.NotFound();
        }
        var memStream = new MemoryStream();
        await memStream.SaveAsAsync(Årsrapportdata);
        memStream.Seek(0, SeekOrigin.Begin);
        return TypedResults.File(memStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"aarsrapport{now}.xlsx");
    }
}