using Microsoft.AspNetCore.Mvc;
using Server.Context;
using Server.Util;
using System.Text.Json;
namespace Server.Controllers;

[ApiController]
[Route("api")]
public class LanguageController(BtdbContext context) : ControllerBase
{
    private readonly BtdbContext _context = context;
    [HttpGet("languagepack")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult FetchLanguage([FromQuery] QueryParamsForLang query)
    {
        if (!QueryParamsForLang.CheckLangParamOnly(query)) return StatusCode(400, "Missing Language Code In Query Param");
        Dictionary<string, string> ReturnLang = [];
        var databaseLang = _context.SiteTexts.ToList();
        string langstr;
        switch (query.Language)
        {
            case "nor":
                databaseLang.ForEach(entry =>
                {
                    if (!string.IsNullOrEmpty(entry.Def) && !string.IsNullOrEmpty(entry.Nor))
                        ReturnLang.Add(
                            entry.Def, entry.Nor
                        );
                });
                langstr = JsonSerializer.Serialize(ReturnLang);
                return Ok(langstr);
            case "en":
                databaseLang.ForEach(entry =>
                {
                    if (!string.IsNullOrEmpty(entry.Def) && !string.IsNullOrEmpty(entry.En))
                        ReturnLang.Add(
                            entry.Def, entry.En
                        );
                });
                langstr = JsonSerializer.Serialize(ReturnLang);
                return Ok(langstr);
        }
        return StatusCode(500, "Something went wrong building language object");
    }
}
