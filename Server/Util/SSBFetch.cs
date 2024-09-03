
using System.Text.Json;
using Server.Models;


namespace Server.Util;

public class FetchSSBData
{
    public static async Task<SSBJsonStat> GetSSBDataAvgBasedOnDepth()
    {

        string url = "https://data.ssb.no/api/v0/no/table/07685/";
        var queryFileName = Directory.GetFiles("./SSBQuery", "*.json");
        var jsonContent = File.ReadAllText(queryFileName[0]);
        var query = JsonSerializer.Deserialize<PostQuery>(jsonContent) ?? throw new NullReferenceException("Missing query");
        SSBJsonStat returnValues = new();
        using (HttpClient client = new())
        {
            var response = await client.PostAsJsonAsync(url, query);
            response.EnsureSuccessStatusCode();
            returnValues = await response.Content.ReadFromJsonAsync<SSBJsonStat>() ?? throw new NullReferenceException("Something went wrong parsing content.");
        };
        return returnValues;
    }
}