namespace Server.Util;
using Server.Views;
using System.Text.Json;

public class FetchProffData
{

    public static async Task<List<ReturnStructure>> GetDatabaseValues(List<int> orgNrArray, StreamWriter writer)
    {
        string? ApiKey = Environment.GetEnvironmentVariable("PROFF_API_KEY");
        if (string.IsNullOrEmpty(ApiKey))
            throw new ArgumentNullException("Could not fetch token from Environment.");
        string baseUrl = "https://api.proff.no/api/companies/register/NO/";
        string url;
        List<ReturnStructure> ReturnValues = [];
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        using (HttpClient client = new())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Token {ApiKey}");
            foreach (int orgNr in orgNrArray)
            {
                try
                {
                    await Task.Delay(100);
                    url = baseUrl + orgNr.ToString();
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    ReturnStructure? returnValue = JsonSerializer.Deserialize<ReturnStructure>(responseBody, options);
                    if (returnValue != null) ReturnValues.Add(returnValue);
                    writer.WriteLine($"{orgNr} successfully fetched from Proff");
                }
                catch (Exception ex)
                {
                    writer.WriteLine($"Failed to fetch {orgNr} from Proff: {ex.Message}");
                }
            }
        }
        return ReturnValues;
    }
}