using System.Reflection;
using System.Text.Json.Serialization;

namespace Server.Models;

public class SSBJsonStat
{
    private readonly int _DepthSize;
    private readonly int _GroupSize;
    private readonly int _GroupKeysSize;
    [JsonPropertyName("version")]
    public string? Version { get; set; }
    [JsonPropertyName("class")]
    public string? Class { get; set; }
    [JsonPropertyName("label")]
    public string? Label { get; set; }
    [JsonPropertyName("source")]
    public string? Source { get; set; }
    [JsonPropertyName("updated")]
    public string? Updated { get; set; }
    [JsonPropertyName("note")]
    public List<string>? Note { get; set; }
    [JsonPropertyName("role")]
    public Dictionary<string, List<string>>? Role { get; set; }
    [JsonPropertyName("id")]
    public List<string>? Id { get; set; }
    [JsonPropertyName("size")]
    public List<int?>? Size { get; set; }
    [JsonPropertyName("dimension")]
    public Dictionary<string, Dimentions>? Dimension { get; set; }
    [JsonPropertyName("extension")]
    public StatExtentions? Extension { get; set; }
    [JsonPropertyName("value")]
    public List<int?>? Value { get; set; }
    public SSBJsonStat()
    {
        _DepthSize = Size[2] ?? throw new NullReferenceException("Missing size for depth");
        _GroupSize = Size[0] ?? throw new NullReferenceException("Missing size for group");
        _GroupKeysSize = Size[1] ?? throw new NullReferenceException("Missing size for group keys");
    }
    public Dictionary<string, int> GetAvgDataBasedOnDepth()
    {
        List<int> SumData = Enumerable.Repeat(0, _DepthSize).ToList();
        int count = 0;
        for (int i = 0; i < Value.Count; i++)
        {
            SumData[count] += Value[i] ?? 0;
            count++;
            if (count == _DepthSize) count = 0;
        }
        string DepthKey = Id[2] ?? throw new NullReferenceException("Missing key for depth values.");
        if (!Dimension.ContainsKey(DepthKey)) throw new NullReferenceException($"missing labels for key {DepthKey}");
        List<string> DepthLabels = [];
        try
        {
            foreach (var value in Dimension[DepthKey].Category.Label.Values)
            {
                DepthLabels.Add(value);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Missing value: {ex.Message}");
        }
        Dictionary<string, int> Data = [];
        for (int i = 0; i < SumData.Count; i++)
        {
            Data.Add(
                DepthLabels[i], SumData[i]
            );
        }
        return Data;
    }

}
public class Dimentions
{
    [JsonPropertyName("label")]
    public string? Label { get; set; }
    [JsonPropertyName("category")]
    public DimentionCategories? Category { get; set; }
    [JsonPropertyName("extension")]
    public DimmentionExtentions? Extention { get; set; }
    [JsonPropertyName("link")]
    public Links? Link { get; set; }

}
public class DimentionCategories
{
    [JsonPropertyName("index")]
    public Dictionary<string, int>? Index { get; set; }
    [JsonPropertyName("label")]
    public Dictionary<string, string>? Label { get; set; }
    [JsonPropertyName("unit")]
    public Dictionary<string, Units>? Unit { get; set; }

}
public class Units
{
    [JsonPropertyName("base")]
    public string? Base { get; set; }
    [JsonPropertyName("decimals")]
    public int Decimals { get; set; }
}
public class DimmentionExtentions
{
    [JsonPropertyName("elimination")]
    public bool Elimination { get; set; }
    [JsonPropertyName("refperiod")]
    public Dictionary<string, string>? Refperiod { get; set; }
    [JsonPropertyName("show")]
    public string? Show { get; set; }
}
public class Links
{
    [JsonPropertyName("describedby")]
    public List<Dictionary<string, Dictionary<string, string>>>? DescribedBy { get; set; }
}

public class StatExtentions
{
    [JsonPropertyName("px")]
    public PxInfo? Px { get; set; }
    [JsonPropertyName("contact")]
    public List<Contacts>? Contact { get; set; }
}
public class PxInfo
{
    [JsonPropertyName("infofile")]
    public string? Infofile { get; set; }
    [JsonPropertyName("tableid")]
    public string? Tableid { get; set; }
    [JsonPropertyName("decimals")]
    public int Decimals { get; set; }
    [JsonPropertyName("official-statistics")]
    public bool OfficialStatistics { get; set; }
    [JsonPropertyName("aggregallowed")]
    public bool Aggregallowed { get; set; }
    [JsonPropertyName("language")]
    public string? Language { get; set; }
    [JsonPropertyName("matrix")]
    public string? Matrix { get; set; }
    [JsonPropertyName("subject-code")]
    public string? SubjectCode { get; set; }
}
public class Contacts
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }
    [JsonPropertyName("mail")]
    public string? Mail { get; set; }
    [JsonPropertyName("raw")]
    public string? Raw { get; set; }
}
