using Microsoft.AspNetCore.Mvc;
using Server.Context;
using Server.Util;
using System.Security.Cryptography;
using System.Text.Json;
namespace Server.Controllers;

public static class GlobalLanguage
{
    private static string _language = "nor";
    public static string Language
    {
        get { return _language; }
        set { _language = value; }
    }
}
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
        if (!QueryParamsForLang.CheckLangParamOnly(query)) return BadRequest(new
        {
            error = ErrorText(400)
        });
        GlobalLanguage.Language = query.Language;
        Dictionary<string, string> ReturnLang = [];
        var databaseLang = _context.SiteTexts.ToList();
        string langstr;
        switch (GlobalLanguage.Language)
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
        return StatusCode(500, new
        {
            error = ErrorText(500)
        });
    }
    private static string ErrorText(int code)
    {
        switch (code)
        {
            case 500:
                return GlobalLanguage.Language switch
                {
                    "nor" => "Noe gikk galt med bygging av språkobject.",
                    "en" => "Something went wrong building language object",
                    _ => "Server error",
                };
            case 400:
                return GlobalLanguage.Language switch
                {
                    "nor" => "Mangler språk-kode i query parameter.",
                    "en" => "Missing Language Code In Query Param.",
                    _ => "Server error",
                };
        }
        return "Server error";
    }
}
