namespace Util.ProffFetch;
using ProffApiClasses;
using System.Text.Json;

public class FetchProffData
{

    public static async Task<List<SqlParamStructure>> GetDatabaseValues(List<int> orgNrArray)
    {
        string? ApiKey = Environment.GetEnvironmentVariable("PROFF_API_KEY");
        if (string.IsNullOrEmpty(ApiKey))
            throw new ArgumentNullException("Could not fetch token from Environment.");
        List<SqlParamStructure> ReturnValues = new();
        string baseUrl = "https://api.proff.no/api/companies/register/NO/";
        string url;
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
                    ReturnStructure returnValue = JsonSerializer.Deserialize<ReturnStructure>(responseBody);
                    SqlParamStructure parameters = SqlParamStructure.GetSqlParamStructure(returnValue);
                    ReturnValues.Add(parameters);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        return ReturnValues;
    }
}