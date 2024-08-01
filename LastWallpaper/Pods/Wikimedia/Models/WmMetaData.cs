using System.Text.Json;
using System.Text.Json.Serialization;

namespace LastWallpaper.Pods.Wikimedia.Models;

public sealed class WmMetaData
{
    [JsonPropertyName("value")]
    public JsonElement Value { get; init; }
}
