
using Server.Util;
using Microsoft.AspNetCore.Mvc;
using Server.Context;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
namespace Server.Controllers;


[ApiController]
[Route("api")]
public class UiDataHandler(BtdbContext context) : ControllerBase
{
    private readonly BtdbContext _context = context;
    [HttpGet("ecocode")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> FetchEcoCodes([FromQuery] QueryParamsForLang query)
    {
        if (!QueryParamsForLang.CheckLangParamOnly(query)) return BadRequest(
            new
            {
                Message = "Missing Language"
            }
        );
        try
        {
            var Lang = query.Language;
            Dictionary<string, string> EcoCodes = [];
            var fetchedCodes = await _context.EcoCodeLookups.ToListAsync();
            if (fetchedCodes == null) return StatusCode(500, new
            {
                error = ErrorText(500, Lang)
            });
            string? desc;
            foreach (var item in fetchedCodes)
            {
                if (item == null) continue;
                if (item.EcoCode == null) continue;
                if (string.Equals(Lang, "nor")) desc = item.Nor;
                else if (string.Equals(Lang, "en")) desc = item.En;
                else desc = "Missing Language";
                EcoCodes.Add(
                    item.EcoCode, desc
                );
            }
            var json = JsonSerializer.Serialize(EcoCodes);
            return Ok(json);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = ex.Message
            });
        }
    }
    [HttpGet("workyear")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult FetchWorkYear([FromQuery] QueryParamForYear query)
    {
        if (!QueryParamForYear.CheckYearParam(query)) return BadRequest(new
        {
            error = ErrorText(400, query.Language)
        });
        try
        {
            var Lang = query.Language;
            int QueryYear = int.Parse(query.Year);
            decimal? AccumulatedManYears = _context.AvgLaborCostPrYears.Where(e => e.Year <= QueryYear && e.TotalManYear != null).Sum(e => e.TotalManYear);
            Dictionary<string, object> Count = new(){
                {"text", Lang switch
                {
                    "nor" => "Akkumulert Årsverk",
                    "en" => "Accumulated Man-years",
                    _ => "Missing Language",
                }},
                {"number", (int)AccumulatedManYears}
            };
            var JsonString = JsonSerializer.Serialize(Count);
            return Ok(JsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, new
            {
                error = ex.Message
            });
        }
    }
    [HttpGet("workercount")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult FetchWorkerCount([FromQuery] QueryParamForYear query)
    {
        if (!QueryParamForYear.CheckYearParam(query)) return BadRequest(new
        {
            error = ErrorText(400, query.Language)
        });
        try
        {
            var Lang = query.Language;
            int Year = int.Parse(query.Year);
            var WorkerCount = _context.AvgLaborCostPrYears.Where(e => e.Year == Year).Select(e => e.TotalManYear).ToList();
            Dictionary<string, object> Count = new(){
                {"text", Lang switch
                {
                    "nor" => $"Årsverk totalt {query.Year}",
                    "en" => $"Man-years in total {query.Year}",
                    _ => "Missing Language",
                }},
                {"number", (int)WorkerCount[0]}
            };
            var JsonString = JsonSerializer.Serialize(Count);
            return Ok(JsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, new
            {
                error = ex.Message
            });
        }
    }
    [HttpGet("totalturnover")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult FetchTotalTurnover([FromQuery] QueryParamForYear query)
    {
        if (!QueryParamForYear.CheckYearParam(query)) return BadRequest(new
        {
            error = ErrorText(400, query.Language)
        });
        try
        {
            var Lang = query.Language;
            var Year = int.Parse(query.Year);
            var TotalTurnover = _context.AverageValues.Where(e => e.EcoCode == "SDI" && (e.Year == Year || e.Year == Year - 1)).Select(e => e.TotalAccumulated).ToList();
            TotalTurnover.Reverse();
            Dictionary<string, object> Count = new(){
                {"text", Lang switch
                {
                    "nor" => "Akkumulert Omsetning",
                    "en" => "Accumulated Turnover",
                    _ => "Missing Language",
                }},
                {"number", TotalTurnover[0]}
            };
            var JsonString = JsonSerializer.Serialize(Count);
            return Ok(JsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, new
            {
                error = ex.Message
            });
        }
    }
    [HttpGet("companycount")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult FetchCompanyCount([FromQuery] QueryParamForYear query)
    {
        if (!QueryParamForYear.CheckYearParam(query)) return BadRequest(new
        {
            error = ErrorText(400, query.Language)
        });
        try
        {
            var Lang = query.Language;
            int Year = int.Parse(query.Year);
            var CompanyCount = _context.CompanyPhaseStatusOverviews.Where(e => e.Year == Year && e.Phase != "Alumni").Select(e => e.CompanyId).Distinct().Count();
            Dictionary<string, object> Count = new(){
                {"text", Lang switch
                {
                    "nor" => $"Antall bedrifter i inkubasjon {query.Year}",
                    "en" => $"Number of companies in incubation {query.Year}",
                    _ => "Missing Language",
                }},
                {"number", CompanyCount}
            };
            var JsonString = JsonSerializer.Serialize(Count);
            return Ok(JsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, new
            {
                error = ex.Message
            });
        }
    }
    [HttpGet("accumulatedCompanyCount")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult FetchAccumulatedCompanyCount([FromQuery] QueryParamForYear query)
    {
        var Lang = query.Language;
        try
        {
            var CompanyCount = _context.CompanyInfos.Select(e => e.CompanyId).Count();
            Dictionary<string, object> Count = new(){
                {"text", Lang switch
                {
                    "nor" => $"Antall bedrifter totalt",
                    "en" => $"Number of companies in total",
                    _ => "Missing Language",
                }},
                {"number", CompanyCount}
            };
            var JsonString = JsonSerializer.Serialize(Count);
            return Ok(JsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, new
            {
                error = ex.Message
            });
        }
    }
    [HttpGet("branch")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetBrands()
    {
        try
        {
            var branch = _context.CompanyInfos.Select(e => e.Branch).Where(branch => branch != null).Distinct().ToList();
            branch.Sort();
            var returnstr = JsonSerializer.Serialize(branch);
            return Ok(returnstr);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = ex.Message
            });
        }
    }
    [HttpGet("agegroups")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetAgeGroups()
    {
        try
        {
            var agegroups = _context.DataSortedByLeaderAges.Select(e => e.AgeGroup).Where(AgeGroup => AgeGroup != null).Distinct().ToList();
            agegroups.Sort();
            var returnstr = JsonSerializer.Serialize(agegroups);
            return Ok(returnstr);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = ex.Message
            });
        }
    }
    [HttpGet("sexes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetSexes()
    {
        try
        {
            var sexes = _context.DataSortedByLeaderSexes.Select(e => e.Sex).Distinct().ToList();
            sexes.Sort();
            var returnstr = JsonSerializer.Serialize(sexes);
            return Ok(returnstr);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = ex.Message
            });
        }
    }
    [HttpGet("fases")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetFases()
    {
        try
        {
            var phases = _context.CompanyPhaseStatusOverviews.Select(e => e.Phase).Where(phase => phase != null).Distinct().ToList();
            phases.Sort();
            var returnstr = JsonSerializer.Serialize(phases);
            return Ok(returnstr);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = ex.Message
            });
        }
    }
    private static string ErrorText(int code, string Lang)
    {
        switch (code)
        {
            case 500:
                return Lang switch
                {
                    "nor" => "Noe gikk galt med å fetche fra databasen.",
                    "en" => "Something went wrong fetching from the database",
                    _ => "Server error",
                };
            case 400:
                return Lang switch
                {
                    "nor" => "Mangler år i query parameter.",
                    "en" => "Missing Year In Query Param.",
                    _ => "Server error",
                };
        }
        return "Server error";
    }
}