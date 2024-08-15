using System.Text.Json.Serialization;

namespace Server.Models;


public class PostQuery
{
    [JsonPropertyName("query")]
    public List<QueryBody> Query { get; set; }
    [JsonPropertyName("response")]
    public ResponseBody Response { get; set; }
}
public class QueryBody
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }
    [JsonPropertyName("selection")]
    public SelectionBody Selection { get; set; }
}
public class SelectionBody
{
    [JsonPropertyName("filter")]
    public string? Filter { get; set; }
    [JsonPropertyName("values")]
    public List<string>? Values { get; set; }
}
public class ResponseBody
{
    [JsonPropertyName("format")]
    public string? Format { get; set; }
}