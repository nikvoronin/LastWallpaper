using System.Text.Json.Serialization;

namespace LastWallpaper.Pods.Wikimedia.Models;

public sealed class WmResponse
{
    [JsonPropertyName("query")]
    public required WmQuery Query { get; init; }
}
