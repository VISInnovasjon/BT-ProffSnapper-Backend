using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Context;
using Server.Models;
using Server.Util;
namespace Server.Controllers;

public class LaborCostFromSSBController(BtdbContext context)
{
    private readonly BtdbContext _context = context;
    public async Task UpdateLabourCost()
    {
        SSBJsonStat? newData = null;
        try
        {
            newData = await FetchSSBData.GetSSBDataAvgBasedOnDepth();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to fetch data from SSB: {ex.Message}");
        }
        if (newData == null) return;
        Dictionary<string, int> laborCostData = newData.GetAvgDataBasedOnDepth();
        List<string> yearLabels = laborCostData.Keys.ToList();
        int convFirstYear = int.Parse(yearLabels.First());
        int rangeLength = int.Parse(yearLabels.Last()) + 2 - convFirstYear;
        List<int> yearKeys = Enumerable.Range(convFirstYear, rangeLength).ToList();
        int ValidDataPoint = 0;

        for (int i = 0; i < yearKeys.Count; i++)
        {
            string key = yearKeys[i].ToString();
            if (laborCostData.TryGetValue(key, out int value))
            {
                ValidDataPoint = value;
            }
            else
            {
                ValidDataPoint += ValidDataPoint * 3 / 100;
            }
            var newValue = new AvgLaborCostPrYear()
            {
                Year = yearKeys[i],
                Value = ValidDataPoint,
            };
            try
            {
                UpsertHandler.UpsertEntity(context, newValue);
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Caught update exception: {ex.Message}");
            }
        }
        try
        {
            await _context.Database.ExecuteSqlRawAsync("CALL update_total_man_year()");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}