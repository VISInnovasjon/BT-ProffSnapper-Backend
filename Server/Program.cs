using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Npgsql;
using Util.DB;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapGet("/databaseTest", () =>
{
    List<string> name = new List<string>();
    List<int> id = new List<int>();
    Action<NpgsqlDataReader> processRow = reader =>
    {
        for (int i = 0; i < reader.FieldCount; i++)
        {
            if (reader.GetFieldType(i) == typeof(string))
            {
                string? value = reader.IsDBNull(i) ? null : reader.GetString(i);
                if (value != null)
                {
                    name.Add(value);
                }
            }
            else if (reader.GetFieldType(i) == typeof(int))
            {
                int value = reader.GetInt32(i);
                id.Add(value);
            }
            else continue;
        }
    };
    Database.Query("SELECT * FROM testing_database", processRow);
    string jsonedNames = JsonSerializer.Serialize(name);
    string jsonedIds = JsonSerializer.Serialize(id);
    return jsonedIds;
})
.WithName("DatabaseTest")
.WithOpenApi();



app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

