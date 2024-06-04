

using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Server.Models;
using Server.Views;

namespace Server.Controllers;


[ApiController]
[Route("query")]
public class QueryHandler : ControllerBase
{
    private readonly DbContextOptions<BtdbContext> options = new DbContextOptionsBuilder<BtdbContext>().UseNpgsql($"Host={Environment.GetEnvironmentVariable("DATABASE_HOST")};Username={Environment.GetEnvironmentVariable("DATABASE_USER")};Password={Environment.GetEnvironmentVariable("DATABASE_PASSWORD")};Database={Environment.GetEnvironmentVariable("DATABASE_NAME")}").Options;

    [HttpGet("age")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult FetchAge()
    {
        string jsonString;
        try
        {
            Dictionary<string, Dictionary<string, Dictionary<string, ExtractedEcoCodeValues>>> AlderData = new Dictionary<string, Dictionary<string, Dictionary<string, ExtractedEcoCodeValues>>>() { };

            using (var context = new BtdbContext(options))
            {
                List<string?> alderGrupper = context.DataSortertEtterAldersGruppes.Select(b => b.AldersGruppe).Distinct().ToList();
                foreach (string? gruppe in alderGrupper)
                {
                    if (string.IsNullOrEmpty(gruppe)) continue;
                    Dictionary<string, Dictionary<string, ExtractedEcoCodeValues>> yearlyData = new() { };
                    List<int> years = context.DataSortertEtterAldersGruppes.Where(b => b.AldersGruppe == gruppe).Select(b => b.RapportÅr).Distinct().ToList();
                    foreach (int year in years)
                    {
                        Dictionary<string, ExtractedEcoCodeValues> CodeValuePairing = new() { };
                        List<Values> DataList = context.DataSortertEtterAldersGruppes.Where(b => b.RapportÅr == year && b.AldersGruppe == gruppe).Select(b => new Values(b.ØkoKode ?? "Mangler Kode", b.AvgØkoVerdi, b.AvgDelta, b.KodeBeskrivelse)).ToList();
                        foreach (var item in DataList)
                        {
                            ExtractedEcoCodeValues exVal = new(
                                item.Value, item.Delta, item.Description
                            );
                            CodeValuePairing.Add(item.EcoCode, exVal);
                        }
                        yearlyData.Add(year.ToString(), CodeValuePairing);

                    }
                    AlderData.Add(gruppe, yearlyData);
                }

            }
            jsonString = JsonSerializer.Serialize(AlderData);

            return Ok(jsonString);
        }
        catch (Exception ex)
        {
            var error = new
            {
                Message = "An Error Occured.",
                Details = ex.Message + " " + ex.Data.ToString() + " " + ex.StackTrace

            };
            return BadRequest(error);
        }
    }
    [HttpGet("branch")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult FetchBransje()
    {
        string jsonString;
        try
        {
            Dictionary<string, Dictionary<string, Dictionary<string, ExtractedEcoCodeValues>>> BransjeData = new Dictionary<string, Dictionary<string, Dictionary<string, ExtractedEcoCodeValues>>>() { };

            using (var context = new BtdbContext(options))
            {
                List<string?> bransjer = context.DataSortertEtterBransjes.Select(b => b.Bransje).Distinct().ToList();
                foreach (string? bransje in bransjer)
                {
                    if (string.IsNullOrEmpty(bransje)) continue;
                    Dictionary<string, Dictionary<string, ExtractedEcoCodeValues>> yearlyData = new() { };
                    List<int> years = context.DataSortertEtterBransjes.Where(b => b.Bransje == bransje).Select(b => b.RapportÅr).Distinct().ToList();
                    foreach (int year in years)
                    {
                        Dictionary<string, ExtractedEcoCodeValues> CodeValuePairing = new() { };
                        List<Values> DataList = context.DataSortertEtterBransjes.Where(b => b.RapportÅr == year && b.Bransje == bransje).Select(b => new Values(b.ØkoKode ?? "Kode Mangler", b.AvgØkoVerdi, b.AvgDelta, b.KodeBeskrivelse)).ToList();
                        foreach (var item in DataList)
                        {
                            ExtractedEcoCodeValues exVal = new(
                                item.Value, item.Delta, item.Description
                            );
                            CodeValuePairing.Add(item.EcoCode, exVal);
                        }
                        yearlyData.Add(year.ToString(), CodeValuePairing);

                    }
                    BransjeData.Add(bransje, yearlyData);
                }

            }
            jsonString = JsonSerializer.Serialize(BransjeData);

            return Ok(jsonString);
        }
        catch (Exception ex)
        {
            var error = new
            {
                Message = "An Error Occured.",
                Details = ex.Message + " " + ex.Data.ToString() + " " + ex.StackTrace

            };
            return BadRequest(error);
        }
    }
    [HttpGet("phase")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Fetchfase()
    {
        string jsonString;
        try
        {
            Dictionary<string, Dictionary<string, Dictionary<string, ExtractedEcoCodeValues>>> FaseData = new Dictionary<string, Dictionary<string, Dictionary<string, ExtractedEcoCodeValues>>>() { };

            using (var context = new BtdbContext(options))
            {
                List<string?> faser = context.DataSortertEtterFases.Select(b => b.Fase).Distinct().ToList();
                foreach (string? fase in faser)
                {
                    if (string.IsNullOrEmpty(fase)) continue;
                    Dictionary<string, Dictionary<string, ExtractedEcoCodeValues>> yearlyData = new() { };
                    List<int> years = context.DataSortertEtterFases.Where(b => b.Fase == fase).Select(b => b.RapportÅr).Distinct().ToList();
                    foreach (int year in years)
                    {
                        Dictionary<string, ExtractedEcoCodeValues> CodeValuePairing = new() { };
                        List<Values> DataList = context.DataSortertEtterFases.Where(b => b.RapportÅr == year && b.Fase == fase).Select(b => new Values(b.ØkoKode ?? "Mangler Kode", b.AvgØkoVerdi, b.AvgDelta, b.KodeBeskrivelse)).ToList();
                        foreach (var item in DataList)
                        {
                            ExtractedEcoCodeValues exVal = new(
                                item.Value, item.Delta, item.Description
                            );
                            CodeValuePairing.Add(item.EcoCode, exVal);
                        }
                        yearlyData.Add(year.ToString(), CodeValuePairing);

                    }
                    FaseData.Add(fase, yearlyData);
                }

            }
            jsonString = JsonSerializer.Serialize(FaseData);

            return Ok(jsonString);
        }
        catch (Exception ex)
        {
            var error = new
            {
                Message = "An Error Occured.",
                Details = ex.Message + " " + ex.Data.ToString() + " " + ex.StackTrace

            };
            return BadRequest(error);
        }
    }
    [HttpGet("avg")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult FetchAvg()
    {
        string jsonString;
        try
        {
            Dictionary<string, Dictionary<string, ExtractedEcoCodeValues>> yearlyData = new() { };
            using (var context = new BtdbContext(options))
            {
                List<int> years = context.GjennomsnittVerdiers.Select(b => b.RapportÅr).Distinct().ToList();
                foreach (int year in years)
                {
                    Dictionary<string, ExtractedEcoCodeValues> CodeValuePairing = new() { };
                    List<Values> DataList = context.GjennomsnittVerdiers.Where(b => b.RapportÅr == year).Select(b => new Values(b.ØkoKode ?? "Kode Mangler", b.AvgØkoVerdi, b.AvgDelta, b.KodeBeskrivelse)).ToList();
                    foreach (var item in DataList)
                    {
                        ExtractedEcoCodeValues exVal = new(
                            item.Value, item.Delta, item.Description
                        );
                        CodeValuePairing.Add(item.EcoCode, exVal);
                    }
                    yearlyData.Add(year.ToString(), CodeValuePairing);

                }

            }
            jsonString = JsonSerializer.Serialize(yearlyData);

            return Ok(jsonString);
        }
        catch (Exception ex)
        {
            var error = new
            {
                Message = "An Error Occured.",
                Details = ex.Message + " " + ex.Data.ToString() + " " + ex.StackTrace

            };
            return BadRequest(error);
        }
    }
}