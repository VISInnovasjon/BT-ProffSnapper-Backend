using System.Text.Json;
using Util.ProffApiClasses;
namespace Util.TestStructure;
public class TestJsonStream
{
    public static string ParseJsonStream()
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
            var ParsedSqlObject = SqlParamStructure.GetSqlParamStructure(Object);
            return JsonSerializer.Serialize(ParsedSqlObject);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return "Null";
        }

    }
}