using System.Text.Json.Serialization;

namespace LastWallpaper.Pods.Bing.Models;

public class BingImageInfo
{
    [JsonPropertyName("startdate")]
    public required string StartDate { get; init; }

    [JsonPropertyName("urlbase")]
    public required string UrlBase { get; init; }

    [JsonPropertyName("copyright")]
    public string? Copyright { get; set; }
}
