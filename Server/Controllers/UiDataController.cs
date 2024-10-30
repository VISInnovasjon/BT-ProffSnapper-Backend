
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
    [HttpGet("accworkyear")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult FetchWorkYear([FromQuery] QueryParamForYear query)
    {

        try
        {
            QueryParamForYear.ExtractedParams queryData;
            try
            {
                queryData = query.ExtractParams();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            var Lang = queryData.Language;
            int EndYear = queryData.EndYear;
            int StartYear = queryData.StartYear;
            decimal? AccumulatedManYears = _context.AvgLaborCostPrYears.Where(e => e.Year <= EndYear && e.Year >= StartYear && e.TotalManYear != null).Sum(e => e.TotalManYear);
            Dictionary<string, object> Count = new(){
                {"text", Lang switch
                {
                    "nor" => $"Akkumulert Årsverk mellom {StartYear} og {EndYear}",
                    "en" => $"Accumulated Man-years between {StartYear} and {EndYear}",
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
    [HttpGet("yearlyworkyear")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult FetchWorkerCount([FromQuery] QueryParamForYear query)
    {
        try
        {
            QueryParamForYear.ExtractedParams queryData;
            try
            {
                queryData = query.ExtractParams();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            var Lang = queryData.Language;
            int EndYear = queryData.EndYear;
            var WorkerCount = _context.AvgLaborCostPrYears.Where(e => e.Year == EndYear).Select(e => e.TotalManYear).ToList();
            Dictionary<string, object> Count = new(){
                {"text", Lang switch
                {
                    "nor" => $"Årsverk totalt {EndYear}",
                    "en" => $"Man-years in total {EndYear}",
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
    [HttpGet("accturnover")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult FetchTotalTurnover([FromQuery] QueryParamForYear query)
    {
        try
        {
            QueryParamForYear.ExtractedParams queryData;
            try
            {
                queryData = query.ExtractParams();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            var Lang = queryData.Language;
            int EndYear = queryData.EndYear;
            int StartYear = queryData.StartYear;
            var TotalTurnover = (int)_context.CompanyEconomicDataPrYears.Where(e => e.EcoCode == "SDI" && e.Year >= StartYear && e.Year <= EndYear).Sum(e => e.EcoValue);
            Dictionary<string, object> Count = new(){
                {"text", Lang switch
                {
                    "nor" => $"Akkumulert Omsetning mellom {StartYear} og {EndYear}",
                    "en" => $"Accumulated Turnover between {StartYear} and {EndYear}",
                    _ => "Missing Language",
                }},
                {"number", TotalTurnover}
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
    [HttpGet("yearlycompanycount")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult FetchCompanyCount([FromQuery] QueryParamForYear query)
    {
        try
        {
            QueryParamForYear.ExtractedParams queryData;
            try
            {
                queryData = query.ExtractParams();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            var Lang = queryData.Language;
            int QueryYear = queryData.EndYear;
            var CompanyCount = _context.CompanyPhaseStatusOverviews.Where(e => e.Year == QueryYear && e.Phase != "Alumni").Select(e => e.CompanyId).Distinct().Count();
            Dictionary<string, object> Count = new(){
                {"text", Lang switch
                {
                    "nor" => $"Antall bedrifter i inkubasjon {query.EndYear}",
                    "en" => $"Number of companies in incubation {query.EndYear}",
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
    [HttpGet("acccompanycount")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult FetchAccumulatedCompanyCount([FromQuery] QueryParamForYear query)
    {

        try
        {
            QueryParamForYear.ExtractedParams queryData;
            try
            {
                queryData = query.ExtractParams();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            var Lang = queryData.Language;
            int StartYear = queryData.StartYear;
            int EndYear = queryData.EndYear;
            var CompanyCount = _context.CompanyInfos.Where(e => _context.CompanyPhaseStatusOverviews.Any(cps => cps.CompanyId == e.CompanyId && cps.Year <= EndYear && cps.Year >= StartYear)).Select(e => e.CompanyId).Count();
            Dictionary<string, object> Count = new(){
                {"text", Lang switch
                {
                    "nor" => $"Antall bedrifter totalt mellom {StartYear} og {EndYear}",
                    "en" => $"Number of companies in total between {StartYear} and {EndYear}",
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