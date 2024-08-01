using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LastWallpaper.Pods.Wikimedia.Models;

public sealed class WmImageInfo
{
    [JsonPropertyName( "url" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string? Url { get; init; }

    [JsonPropertyName( "extmetadata" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public IReadOnlyDictionary<string, WmMetaData>? ExtMetaData { get; init; }
}
