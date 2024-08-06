
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
        try
        {
            if (string.IsNullOrEmpty(query.Language)) return BadRequest("Missing language");
            Dictionary<string, string> EcoCodes = [];
            if (!QueryParamsForLang.CheckQueryParams(query.Language)) return BadRequest("Could not parse your chosen language, please use en or nor");
            var fetchedCodes = await _context.EcoCodeLookups.ToListAsync();
            if (fetchedCodes == null) return StatusCode(500);
            string? desc;
            foreach (var item in fetchedCodes)
            {
                if (item == null) continue;
                if (item.EcoCode == null) continue;
                if (query.Language == "nor") desc = item.Nor;
                else desc = item.En;
                desc ??= "Missing Description";
                EcoCodes.Add(
                    item.EcoCode, desc
                );
            }
            var json = JsonSerializer.Serialize(EcoCodes);
            return Ok(json);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    [HttpGet("workyear")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult FetchWorkYear([FromQuery] QueryParamsForLang query)
    {
        if (!QueryParamsForLang.CheckQueryParams(query.Language)) return StatusCode(500, "Missing language tag in query. Codes nor or en supported.");
        try
        {
            var WorkYears = _context.CompanyEconomicDataPrYears.GroupBy(e => new { e.CompanyId, e.Year }).Count();
            Dictionary<string, object> Count = new(){
                {"text", string.Equals("nor", query.Language) ? "Årsverk": "Man-years"},
                {"number", WorkYears}
            };
            var JsonString = JsonSerializer.Serialize(Count);
            return Ok(JsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
    [HttpGet("workercount")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult FetchWorkerCount([FromQuery] QueryParamsForLang query)
    {
        if (!QueryParamsForLang.CheckQueryParams(query.Language)) return StatusCode(500, "Missing language tag in query. Codes nor or en supported.");
        try
        {
            var Year = DateTime.Now.Year;
            var WorkerCount = _context.GeneralYearlyUpdatedCompanyInfos.Where(e => e.Year == Year - 1 || e.Year == Year - 2).Sum(e => e.NumberOfEmployees);
            Dictionary<string, object> Count = new(){
                {"text", string.Equals("nor", query.Language) ? "Arbeidsplasser" : "Number of employees"},
                {"number", WorkerCount ?? 0}
            };
            var JsonString = JsonSerializer.Serialize(Count);
            return Ok(JsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
    [HttpGet("totalturnover")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult FetchTotalTurnover([FromQuery] QueryParamsForLang query)
    {
        if (!QueryParamsForLang.CheckQueryParams(query.Language)) return StatusCode(500, "Missing language tag in query. Codes nor or en supported.");
        try
        {
            var Year = DateTime.Now.Year;
            Console.WriteLine(Year);
            var TotalTurnover = _context.AverageValues.Where(e => e.EcoCode == "SDI" && (e.Year == Year - 1 || e.Year == Year - 2)).Select(e => e.TotalAccumulated).ToList();
            TotalTurnover.Reverse();
            Dictionary<string, object> Count = new(){
                {"text", string.Equals("nor", query.Language) ? "Omsetning (i tusen NOK)" : "Total Turnover (1000 NOK)"},
                {"number", TotalTurnover[0]}
            };
            var JsonString = JsonSerializer.Serialize(Count);
            return Ok(JsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
    [HttpGet("companycount")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult FetchCompanyCount([FromQuery] QueryParamsForLang query)
    {
        if (!QueryParamsForLang.CheckQueryParams(query.Language)) return StatusCode(500, "Missing language tag in query. Codes nor or en supported.");
        try
        {
            var CompanyCount = _context.CompanyInfos.Count(e => e.CompanyName != null);
            Dictionary<string, object> Count = new(){
                {"text", string.Equals("nor", query.Language) ? "Antall bedrifter":"Company count"},
                {"number", CompanyCount}
            };
            var JsonString = JsonSerializer.Serialize(Count);
            return Ok(JsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, ex.Message);
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
            var returnstr = JsonSerializer.Serialize(branch);
            return Ok(returnstr);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
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
            var returnstr = JsonSerializer.Serialize(agegroups);
            return Ok(returnstr);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
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
            var returnstr = JsonSerializer.Serialize(sexes);
            return Ok(returnstr);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
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
            var returnstr = JsonSerializer.Serialize(phases);
            return Ok(returnstr);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}