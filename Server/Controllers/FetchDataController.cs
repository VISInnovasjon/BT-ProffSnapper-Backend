
using Microsoft.AspNetCore.Mvc;
using Util.DB;

namespace Server.Controllers;


[ApiController]
[Route("query")]
public class QueryHandler : ControllerBase
{
    [HttpGet("age")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> FetchAge()
    {
        string jsonString = string.Empty;
        try
        {
            await Database.Query("SELECT fetch_json_avg_verdier_ordered_aldergruppe()", reader =>
            {
                jsonString = reader.GetString(0);
            });
            return Ok(jsonString);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpGet("bransje")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> FetchBransje()
    {
        string jsonString = string.Empty;
        try
        {
            await Database.Query("SELECT fetch_json_avg_verdier_ordered_bransje()", reader =>
            {
                jsonString = reader.GetString(0);
            });
            return Ok(jsonString);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpGet("fase")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Fetchfase()
    {
        string jsonString = string.Empty;
        try
        {
            await Database.Query("SELECT fetch_json_avg_verdier_ordered_fase()", reader =>
            {
                jsonString = reader.GetString(0);
            });
            return Ok(jsonString);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpGet("avg")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> FetchAvg()
    {
        string jsonString = string.Empty;
        try
        {
            await Database.Query("SELECT fetch_json_avg_verdier()", reader =>
            {
                jsonString = reader.GetString(0);
            });
            return Ok(jsonString);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}