using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Server.Models;

public class SSBJsonStat
{
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
        //TODO: custom constructor for å initialize readonly fields her. 
    }
    public Dictionary<string, int> GetAvgDataBasedOnDepth()
    {
        int _DepthSize = Size[2] ?? throw new NullReferenceException("Missing size for depth");
        int _GroupSize = Size[0] ?? throw new NullReferenceException("Missing size for group");
        int _WidthSize = Size[1] ?? throw new NullReferenceException("Missing size for group keys");
        List<string> _DepthLabels = Dimension[Id[2]].Category.Label.Values.ToList() ?? throw new NullReferenceException("Missing Labels for Depth");

        List<int> SumData = Enumerable.Repeat(0, _DepthSize).ToList();
        int count = 0;
        for (int i = 0; i < Value.Count; i++)
        {
            SumData[count] += Value[i] ?? 0;
            count++;
            if (count == _DepthSize) count = 0;
        }
        Dictionary<string, int> Data = [];
        for (int i = 0; i < SumData.Count; i++)
        {
            Data.Add(
                _DepthLabels[i], SumData[i] / (_GroupSize * _WidthSize)
            );
        }
        return Data;
    }
    public Dictionary<string, Dictionary<string, List<int>>> GroupedDataByKey()
    {
        // TODO: Legge til labels for år. 
        int _DepthSize = Size[2] ?? throw new NullReferenceException("Missing size for depth");
        int _WithSize = Size[1] ?? throw new NullReferenceException("Missing size for group keys");
        List<string> _GroupLabels = Dimension[Id[0]].Category.Label.Keys.ToList() ?? throw new NullReferenceException("Missing labels for group");
        List<string> _WidthLabels = Dimension[Id[1]].Category.Label.Keys.ToList() ?? throw new NullReferenceException("Missing labels for groupkeys");
        Dictionary<string, Dictionary<string, List<int>>> Data = [];
        foreach (var label in _GroupLabels)
        {
            Data.Add(
                label, []
            );
            foreach (var key in _WidthLabels)
            {
                Data[label].Add(
                    key, Enumerable.Repeat(0, _DepthSize).ToList()
                );
            }
        }
        int DepthCount = 0;
        int KeyCount = 0;
        int GroupCount = 0;
        for (int i = 0; i < Value.Count; i++)
        {
            Data[_GroupLabels[GroupCount]][_WidthLabels[KeyCount]][DepthCount] = Value[i] ?? 0;
            DepthCount++;
            if (DepthCount == _DepthSize)
            {
                DepthCount = 0;
                KeyCount++;
                if (KeyCount == _WithSize)
                {
                    KeyCount = 0;
                    GroupCount++;
                }
            }
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
