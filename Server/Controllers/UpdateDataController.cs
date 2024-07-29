using Server.Views;
using Server.Util;
using Microsoft.AspNetCore.Mvc;
using Server.Context;
using Microsoft.EntityFrameworkCore;
namespace Server.Controllers;


[ApiController]
[Route("api")]
public class UpdateHandler(BtdbContext context) : ControllerBase
{
    private readonly BtdbContext _context = context;

    [HttpGet("schedulteupdate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update()
    {
        List<int> orgNrList = _context.CompanyInfos.Where(b => b.Liquidated != true).Select(b => b.Orgnumber).ToList();
        List<ReturnStructure> fetchedData = [];
        if (orgNrList.Count > 0) fetchedData = await FetchProffData.GetDatabaseValues(orgNrList);
        if (fetchedData.Count > 0)
        {
            try
            {
                foreach (var param in fetchedData)
                    await param.InsertIntoDatabase(_context);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500);
            }
            Console.WriteLine("Insert Complete, updating delta.");
            try
            {
                await _context.Database.ExecuteSqlRawAsync("SELECT update_delta()");
                await _context.Database.ExecuteSqlRawAsync("SELECT update_views()");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500);
            }
        }
        return Ok();
    }
}