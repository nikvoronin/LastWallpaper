using System.Text.Json.Serialization;

namespace LastWallpaper.Pods.Wikimedia.Models;

public sealed class WmImageFilename
{
    [JsonPropertyName( "title" )]
    public required string Value { get; init; }
}
