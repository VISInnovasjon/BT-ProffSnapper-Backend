
using Microsoft.AspNetCore.Mvc;
using Server.Context;
namespace Server.Controllers;
[ApiController]
[Route("api")]
public class LastUpdated(BtdbContext context) : ControllerBase
{
    private readonly BtdbContext _context = context;

    [HttpGet("lastupdated")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult FetchDate()
    {
        try
        {
            var LastUpdated = _context.LastUpdates.OrderBy(e => e.Id).Last().UpdateDate;
            return GlobalLanguage.Language switch
            {
                "nor" => Ok(new { text = $"Sist oppdatert: {LastUpdated.ToLongDateString()}" }),
                "en" => Ok(new { text = $"Last Updated: {LastUpdated.ToLongDateString()}" }),
                _ => StatusCode(500, new
                {
                    message = "Missing languagedata for selected language"
                })
            };
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = ex.Message
            });
        }
    }
}