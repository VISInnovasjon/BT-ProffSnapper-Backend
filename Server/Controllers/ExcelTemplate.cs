using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;
using Server.Views;
namespace Server.Controllers;

[ApiController]
[Route("template")]
public class ExcelTemplateMaker : ControllerBase
{
    ///<summary>
    ///Generates a template on how to set up excel spreadsheet for orgnumbers.
    ///</summary>
    ///<returns>Excel Template File</returns>
    [HttpGet("orgnummer")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<Results<FileStreamHttpResult, NotFound>> SendOrgNrTemplate()
    {
        try
        {
            List<OrgNrTemplate> orgTemplate = OrgNrTemplate.GenTemplate();
            var memStream = new MemoryStream();
            await memStream.SaveAsAsync(orgTemplate);
            memStream.Seek(0, SeekOrigin.Begin);
            return TypedResults.File(memStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"orgnummerTemplate.xlsx");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return TypedResults.NotFound();
        }
    }
    ///<summary>
    ///Generates a template on how to set up excel spreadsheet for updating DB with new entries.
    ///</summary>
    ///<returns>Excel Template File</returns>
    [HttpGet("dbupdate")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<Results<FileStreamHttpResult, NotFound>> SendDbUpdateTemplate()
    {
        try
        {
            List<UpdateDbTemplate> templateList = [
            new()
            {
                RapportÅr = 2020,
                Orgnummer = 12345678,
                Fase = "Alumni",
                Bransje = "IKT",
                KvinneligGrunder = 1,
            }
            ];
            var sheets = new Dictionary<string, object>
            {
                ["Bedrifter"] = templateList
            };
            var memStream = new MemoryStream();
            await memStream.SaveAsAsync(sheets);
            memStream.Seek(0, SeekOrigin.Begin);
            return TypedResults.File(memStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"updateDbTemplate.xlsx");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return TypedResults.NotFound();
        }
    }
}