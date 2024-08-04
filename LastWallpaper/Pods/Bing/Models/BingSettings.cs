using System.Text.Json.Serialization;

namespace LastWallpaper.Pods.Bing.Models;

public class BingSettings
{
    [JsonPropertyName( "resolution" )]
    public ImageResolution Resolution { get; init; } =
        ImageResolution.UltraHD;
}
