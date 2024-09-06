
using Microsoft.AspNetCore.Mvc;
namespace Server.Controllers;
public static class DateUpdatedController
{
    private static DateTime _LastUpdated = DateTime.Now;
    public static DateTime LastUpdated
    {
        get { return _LastUpdated; }
        set { _LastUpdated = value; }
    }

}
[ApiController]
[Route("api")]
public class LastUpdated : ControllerBase
{
    [HttpGet("lastupdated")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult FetchDate()
    {
        try
        {
            return GlobalLanguage.Language switch
            {
                "nor" => Ok(new { text = $"Sist oppdatert: {DateUpdatedController.LastUpdated.ToShortDateString()}" }),
                "en" => Ok(new { text = $"Last Updated: {DateUpdatedController.LastUpdated.ToShortDateString()}" }),
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