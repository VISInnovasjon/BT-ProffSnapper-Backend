/* using System.Text.Json;
using Util.ProffApiClasses;
namespace Util.TestStructure;
public class TestJsonStream
{
    public static ReturnStructure ParseJsonStream()
    {
        string jsonFilePath = "./Util/ProffReturnExamle.json";
        string JsonString = File.ReadAllText(jsonFilePath);
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        try
        {
            var Object = JsonSerializer.Deserialize<ReturnStructure>(JsonString, options);
            return Object;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }

    }
} */