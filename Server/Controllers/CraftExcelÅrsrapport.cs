
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Models;
using MiniExcelLibs;
using Server.Views;
using Microsoft.AspNetCore.Http.HttpResults;
namespace Server.Controllers;

[ApiController]
[Route("årsrapport")]
public class GenÅrsRapport : ControllerBase
{
    private readonly DbContextOptions<BtdbContext> options = new DbContextOptionsBuilder<BtdbContext>().UseNpgsql($"Host={Environment.GetEnvironmentVariable("DATABASE_HOST")};Username={Environment.GetEnvironmentVariable("DATABASE_USER")};Password={Environment.GetEnvironmentVariable("DATABASE_PASSWORD")};Database={Environment.GetEnvironmentVariable("DATABASE_NAME")}").Options;
    [HttpGet("get")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<Results<FileStreamHttpResult, NotFound>> ExportExcel()
    {
        string now = DateTime.Now.Date.ToShortDateString();
        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        Response.Headers.Add("Content-Disposition", $"attachement; filename=\"aarsrapport{now}.xlsx\"");
        using (var context = new BtdbContext(options))
        {
            var dataList = context.Årsrapports.ToList();
            if (dataList != null)
            {
                List<ExcelÅrsrapport> Årsrapportdata = ExcelÅrsrapport.GetExportValues(dataList);
                var memStream = new MemoryStream();
                await memStream.SaveAsAsync(Årsrapportdata);
                memStream.Seek(0, SeekOrigin.Begin);
                return TypedResults.File(memStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"aarsrapport{now}.xlsx");
            };
            return TypedResults.NotFound();
        }
    }
}