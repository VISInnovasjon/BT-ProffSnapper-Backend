using System.Text;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Util.DB;
using MiniExcelLibs;
namespace Server.Controllers;

[ApiController]
[Route("genårsrapport")]
public class GenÅrsRapport : ControllerBase
{
    [HttpGet("new")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ExportCsv()
    {
        DateTime now = DateTime.Now.Date;
        Response.ContentType = "text/csv";
        Response.Headers.Add("Content-Disposition", $"attachement; filename=\"aarsrapport{now}.csv\"");
        var csvBuilder = new StringBuilder();
        await Database.Query("SELECT * FROM generer_årsrapport()", async reader =>
        {
            CsvConverter.ConvertStreamToCsv(reader, csvBuilder);
        });
        return File(Encoding.Unicode.GetBytes(csvBuilder.ToString()), "text/csv", $"aarsrapport{now}.csv");
    }
}
public class CsvConverter
{
    public static void ConvertStreamToCsv(NpgsqlDataReader reader, StringBuilder csvBuilder)
    {
        for (int i = 0; i < reader.FieldCount; i++)
        {
            if (i > 0) csvBuilder.Append(',');
            csvBuilder.Append($"{reader.GetName(i).ToString()}");
        }
        csvBuilder.AppendLine();
        while (reader.Read())
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (i > 0) csvBuilder.Append(',');
                string input = reader.GetValue(i).ToString().Replace("\"", "\"\"");
                if (string.IsNullOrEmpty(input))
                    input = "Data Mangler";
                csvBuilder.Append($"{input}");
            }
            csvBuilder.AppendLine();
        }
    }
}

